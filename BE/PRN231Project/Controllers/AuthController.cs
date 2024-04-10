using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN231Project.DTO;
using PRN231Project.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PRN231Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ProjectPRN231Context _context;

        private readonly IConfiguration _configuration;


        public AuthController(IConfiguration configuration, ProjectPRN231Context context)
        {
            _context = context;
            _configuration = configuration;
        }



        [HttpPost("register")]
        public async Task<ActionResult<Account>> Register(AccountDTOAuthen accountDTO)
        {

            var account = new Account()
            {
                Username = accountDTO.Username,
                Password = accountDTO.Password,
                Name = accountDTO.Name,
                Address = accountDTO.Address,
                Phone = accountDTO.Phone,
                Email = accountDTO.Email,
                Status = 1,
                Role = accountDTO.Role
            };

            var account1 = _context.Accounts.Where(x => x.Username == accountDTO.Username).FirstOrDefault();
            if (account1 == null)
            {
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();
                return Ok(account);
            }
            else if(account1 != null)
            {
                return BadRequest("User name have exist! Please choose other username");
            }

            return Ok(account);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<Account> Login(AccountDTOLogin request)
        {

            var account1 = _context.Accounts.Where(x => x.Username == request.Username).FirstOrDefault();

            if (account1 == null)
            {
                return BadRequest("User not found.");
            }

            if (request.Password == account1.Password && account1 != null)
            {
                string token = Generate(account1);

                return Ok(token);
            }
            else
            {
                return BadRequest("Wrong Password");
            }

        }



        private string Generate(Account user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Actor, user.AccountId.ToString()),
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet("UserInfo")]
        [Authorize]
        public ActionResult<Account> GetUserCurrentInfo()
        {

            var currentUser = GetCurrentUser();
            return Ok(currentUser);

        }

        private Account GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new Account
                {
                    Username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    Name = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                    Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
                    AccountId = int.TryParse(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Actor)?.Value, out int accountId) ? accountId : 0 
                };
            }
            return null;
        }

    }
}
