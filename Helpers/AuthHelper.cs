using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace ProductApi.Helpers;

public class AuthHelper
{
    private readonly IConfiguration _config;

    public AuthHelper(IConfiguration config)
    {
        _config = config;
    }


    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        // gets PasswordKey from appsettings.json and concatenates it to password salt converted to base 64 string
        string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
            Convert.ToBase64String(passwordSalt);

        // password Derivation
        byte[] passwordHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256, // pseudo-random function
            iterationCount: 100000,
            numBytesRequested: 256 / 8 // 32 bytes
        );
        return passwordHash;
    }

    public string CreateToken(int userId)
    {
        Claim[] claims = new Claim[] {
            new Claim("userId", userId.ToString())
        };

        string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
        //signature
        SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                tokenKeyString != null ? tokenKeyString : ""
            )
        );

        SigningCredentials credentials = new SigningCredentials(
            tokenKey, SecurityAlgorithms.HmacSha256Signature
        );

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
        {
            // obj declaaration
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(1),
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);
    }
}