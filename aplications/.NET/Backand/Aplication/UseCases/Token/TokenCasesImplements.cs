using Aplication.Contracts;
using Domain.Dto.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Domain.Contracts;


namespace Aplication.UseCases.Token
{
    public class TokenCasesImplements : ITokenUseCases
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _UsersRepositories;
        private readonly ITokenRepository _tokenRepository;

        public TokenCasesImplements(IConfiguration configuration,
                                    IUserRepository UsersRepositories,
                                    ITokenRepository tokenRepository
                                    )
        {
            _tokenRepository = tokenRepository;
            _UsersRepositories = UsersRepositories;
            _configuration = configuration;
        }

        public async Task<TokenResponse> GenerateToken(string userId, string email)
        {
            var user = await _UsersRepositories.GetByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }



            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, user.AccessLevel.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            // Geração aprimorada de refresh token
            var refreshToken = GenerateSecureRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            RefreshToken RefreshToken = new()
            {
                UserId = user.Id,
                Token = refreshToken,
                Expiration = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                RevokedAt = false
            };
            await _tokenRepository.AddAsync(RefreshToken);
            return new TokenResponse
            {
                Token = tokenString,
                RefreshToken = refreshToken,
                expiration = refreshTokenExpiration
            };

        }

        public async Task<string> RefreshToken(string refreshToken, string TokenJwt)
        {
            // Valida o token JWT
            if (!IsTokenValid(TokenJwt))
                throw new SecurityTokenException("Token JWT inválido.");

            var storedToken = await _tokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.Expiration < DateTime.UtcNow || storedToken.RevokedAt)
                throw new SecurityTokenException("Refresh token inválido ou expirado.");

            // Marca o token como revogado
            storedToken.RevokedAt = true;
            await _tokenRepository.UpdateAsync(storedToken);

            var newRefreshToken = GenerateSecureRefreshToken();

            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = storedToken.UserId,
                Token = newRefreshToken,
                Expiration = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                RevokedAt = false
            };
            await _tokenRepository.AddAsync(newRefreshTokenEntity);

            return newRefreshToken;
        }

        private string GenerateSecureRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private bool IsTokenValid(string token)
        {

            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = false,
                ValidateLifetime = false, // Não valida expiração aqui, pois pode estar expirado
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuerSigningKey = true
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                throw new SecurityTokenException("Token JWT inválido.");
            }
        }
    }

}

