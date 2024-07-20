using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Data;
using ProductApi.Dtos;

namespace ProductApi.Controllers;


public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _config = config;
    }

    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        if (userForRegistration.Password == userForRegistration.PasswordConfirm)
        {
            string sqlCheckUserExists = @"
            SELECT Email FROM MyAppSchema.Auth WHERE Email = '" +
            userForRegistration.Email + "'";

            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
            if (existingUsers.Count() == 0)
            {
                // initialize a byte Array with 16 bytes
                byte[] passwordSalt = new byte[128 / 8];

                //populate passwordSalt byte Array with random bytes with non zero bytes
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetNonZeroBytes(passwordSalt);
                }

                byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

                string sqlAddAuth = @"INSERT INTO MyAppSchema.Auth (
                [Email],
                [PasswordHash],
                [PasswordSalt]) VALUES ('" + userForRegistration.Email +
                "', @PasswordHash, @PasswordSalt)";

                // Parameter Creation
                List<SqlParameter> sqlParameters = new List<SqlParameter>();

                SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                passwordSaltParameter.Value = passwordSalt;

                SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                passwordHashParameter.Value = passwordHash;

                sqlParameters.Add(passwordSaltParameter);
                sqlParameters.Add(passwordHashParameter);

                if(_dapper.ExecuteSqlWithParamenters(sqlAddAuth, sqlParameters))
                {
                    return Ok();
                }
                throw new Exception("Failed to Register User");
            }
            throw new Exception("User with this Email Already Exists");
        }
        throw new Exception("Passwords do not Match");
    }

    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt = @"SELECT
            [PasswordHash],
            [PasswordSalt] FROM MyAppSchema.Auth
            WHERE Email = '" + userForLogin.Email + "'";

        UserForLoginConfirmationDto userForLoginConfirmation = _dapper
            .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

        byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

        for (int index = 0; index < passwordHash.Length; index++)
        {
            if (passwordHash[index] != userForLoginConfirmation.PasswordHash[index])
            {
                return StatusCode(401, "Incorrect Password.");
            }
        }

        string userIdSql = @"SELECT
        UserId
        FROM MyAppSchema.Auth
            WHERE Email = '" + userForLogin.Email + "'";

        int userId = _dapper.LoadDataSingle<int>(userIdSql);

        return Ok(new Dictionary<string, string> {
            {"token", CreateToken(userId)}
        });
    }


    private byte[] GetPasswordHash(string password, byte[] passwordSalt)
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

    private string CreateToken(int userId)
    {
        Claim[] claims = new Claim[] {
            new Claim("userId", userId.ToString())
        };

        string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
        //signature
        SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                tokenKeyString != null ? tokenKeyString: ""
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