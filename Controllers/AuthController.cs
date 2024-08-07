using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProductApi.Data;
using ProductApi.Dtos;
using ProductApi.Helpers;

namespace ProductApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly AuthHelper _authHelper;

    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _authHelper = new AuthHelper(config);
    }

    [AllowAnonymous]
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

                byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

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

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt = @"SELECT
            [PasswordHash],
            [PasswordSalt] FROM MyAppSchema.Auth
            WHERE Email = '" + userForLogin.Email + "'";

        UserForLoginConfirmationDto userForLoginConfirmation = _dapper
            .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

        byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

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
            {"token", _authHelper.CreateToken(userId)}
        });
    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        string userId = User.FindFirstValue("userId") + "";

        string userIdSql = "SELECT userId FROM TutorialAppSchema.Users WHERE UserId = " 
            + userId;
        
        int userIdFromDb = _dapper.LoadDataSingle<int>(userIdSql);

        return Ok(new Dictionary<string, string> {
            {"token", _authHelper.CreateToken(userIdFromDb)}
    });
    }
}