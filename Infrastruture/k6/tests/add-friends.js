import http from 'k6/http';
import { check } from 'k6';
import { SharedArray } from 'k6/data';
import { Trend } from 'k6/metrics';


const unexpectedErrors = [];

// --- CONFIGURAÇÕES ---
const BASE_URL = 'http://riders_API:8080'; // Nome do serviço da API na rede Docker
const LOGIN_ENDPOINT = '/api/Users/LoginUser';
const GET_ALL_USERS_ENDPOINT = '/api/Users/GetAllUsers';
const ADD_FRIEND_ENDPOINT = '/api/Friendship/AddFriendship';


// Métrica customizada para medir o tempo de cada pedido de amizade
const addFriendTrend = new Trend('add_friend_duration');



// Carrega credenciais do admin e dos usuários de teste
const adminCredentials = JSON.parse(open('../json/adminLogin.json'));
const testUsers = new SharedArray('testUsers', function () {
    return JSON.parse(open('../json/users.json'));
});





// --- OPÇÕES DE EXECUÇÃO ---
export const options = {
    scenarios: {
        add_friends: {
            executor: 'per-vu-iterations',
            vus: 100,
            iterations: 1,
            maxDuration: '10m',
            exec: 'add_friends'
        },
    },
    thresholds: {
        'http_req_failed': ['rate<0.05'], // Permite uma taxa de falha um pouco maior devido ao volume
        'checks': ['rate>0.95'],
        'add_friend_duration': ['p(95)<1000'], // 95% dos pedidos de amizade devem levar menos de 1s
    },
};


// --- FASE DE SETUP (Executa 1 vez antes do teste) ---
export function setup() {

    const adminLoginRes = http.post(`${BASE_URL}${LOGIN_ENDPOINT}`, JSON.stringify(adminCredentials), { headers: { 'Content-Type': 'application/json' } });
    const adminToken = adminLoginRes.json('data.token');
    if (!adminToken) throw new Error('Falha no login do admin.');

    const adminHeaders = { 'Authorization': `Bearer ${adminToken}` };
    const allUsersRes = http.get(`${BASE_URL}${GET_ALL_USERS_ENDPOINT}`, { headers: adminHeaders });
    // Ordena a lista de usuários por ID para garantir consistência
    const allUsers = allUsersRes.json('data').sort((a, b) => a.userName.localeCompare(b.userName));

    if (!allUsers || allUsers.length === 0) throw new Error('Não foi possível buscar a lista de usuários.');

    console.log(`Setup completo: ${allUsers.length} usuários carregados e ordenados.`);
    return { allUsers: allUsers };
}


export function add_friends(data) {
    // O __VU vai de 1 a 100. O índice do array vai de 0 a 99.
    const currentUserIndex = __VU - 1;
    const currentUserCredentials = testUsers[currentUserIndex];
    if (!currentUserCredentials) return;

    // 1. Login do usuário atual
    const userLoginPayload = JSON.stringify({ email: currentUserCredentials.Email, password: currentUserCredentials.Password });
    const userLoginRes = http.post(`${BASE_URL}${LOGIN_ENDPOINT}`, userLoginPayload, { headers: { 'Content-Type': 'application/json' } });
    const userToken = userLoginRes.json('data.token');
    if (!check(userLoginRes, { 'Login de usuário bem-sucedido': (r) => r.status === 200 && userToken })) {
        return;
    }
    const userAuthHeaders = { 'Authorization': `Bearer ${userToken}`, 'Content-Type': 'application/json' };

    // 2. Criar uma lista de amigos para adicionar, começando do próximo usuário em diante
    // Ex: VU 1 (índice 0) adiciona todos a partir do índice 1.
    // Ex: VU 2 (índice 1) adiciona todos a partir do índice 2.
    const friendsToAdd = data.allUsers.slice(currentUserIndex + 1);

    // 3. Loop para enviar pedido de amizade para a lista progressiva
    for (const friend of friendsToAdd) {
        const params = {
            headers: userAuthHeaders,
            expectedStatuses: [200, 201, 400]
        };
        const addFriendRes = http.post(`${BASE_URL}${ADD_FRIEND_ENDPOINT}`, JSON.stringify({ AddresseeId: friend.id }), params);
        addFriendTrend.add(addFriendRes.timings.duration);

        // *** LÓGICA DE VERIFICAÇÃO INTELIGENTE ***
        const friendshipCheckPassed = check(addFriendRes, {
            [`${currentUserCredentials.UserName} -> ${friend.userName}`]: (r) =>
                (r.status === 200 || r.status === 201) ||
                (r.status === 400 && r.body.includes('Friendship request already exists')),
        });

        if (!friendshipCheckPassed) {
            unexpectedErrors.push({ event: 'Add Friendship Failed', from: currentUserCredentials.UserName, to: friend.userName, status: addFriendRes.status, body: addFriendRes.body });
        }
    }

}



// --- FASE DE RESUMO ---
export function handleSummary(data) {
    console.log(`\nTeste concluído. Total de erros inesperados: ${unexpectedErrors.length}`);

    const summary = {
        'stdout': textSummary(data, { indent: ' ', enableColors: true }),
    };

    if (unexpectedErrors.length > 0) {
        summary['/app/results/erros_add-friends.json'] = JSON.stringify(unexpectedErrors, null, 2);
    }

    return summary;
}
