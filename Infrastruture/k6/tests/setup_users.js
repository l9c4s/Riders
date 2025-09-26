import http from 'k6/http';
import { check } from 'k6';
import { SharedArray } from 'k6/data';

// --- CONFIGURAÇÕES ---
// Use o nome do serviço da API como hostname, pois o k6 rodará na mesma rede Docker
const BASE_URL = 'http://riders_API:8080'; 
// AJUSTE se o seu endpoint de registro for diferente
const REGISTER_ENDPOINT = '/api/Users/addUser'; 

// Carrega o arquivo JSON com os dados dos usuários
// O caminho é relativo à raiz do projeto, que é mapeada para /scripts no container
const users = new SharedArray('users', function () {
    return JSON.parse(open('../json/users.json'));
});
// --- OPÇÕES DE EXECUÇÃO ---
// Configura o k6 para usar até 100 VUs, com cada um executando uma única vez.
export const options = {
    scenarios: {
        create_users: {
            executor: 'per-vu-iterations',
            vus: 100,
            iterations: 1,
            maxDuration: '2m',
        },
    },
};

// --- LÓGICA DO TESTE ---
export default function () {
    // Cada VU pega um usuário diferente do array com base em seu ID
    const user = users[__VU - 1];

    const payload = JSON.stringify(user);
    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    // Faz a requisição para registrar o usuário
    const res = http.post(`${BASE_URL}${REGISTER_ENDPOINT}`, payload, params);

    // Verifica se o usuário foi criado com sucesso (Status 201 ou 200)
    check(res, {
        'usuário criado com sucesso': (r) => r.status === 201 || r.status === 200,
    });
    
}