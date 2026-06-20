using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Security;

public class JwtTokenProvider : ITokenProvider
{
    private readonly IConfiguration _configuration;

    public JwtTokenProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        // 1. Siapkan data yang akan dimasukkan ke dalam token (Claims)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Username)
        };

        // 2. Ambil Key dari konfigurasi dan ubah menjadi byte array
        var keyString = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

        // 3. Siapkan algoritma untuk signing (tanda tangan) token
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 4. Konfigurasi struktur token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2), // Token berlaku selama 2 jam
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = credentials
        };

        // 5. Generate token
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }
}
