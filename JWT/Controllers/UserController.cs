using JWT.Data.Models;
using JWT.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<Users> _userManager;

        public UserController(IConfiguration configuration, UserManager<Users> userManager)
        {
            _config = configuration;
            _userManager = userManager;
        }

        #region Login
        [HttpPost]
        [Route("/login")]
        public async Task<ActionResult<TokenDTO>> Login(LoginDTO credentials)
        {
            #region Get user Data
            Users? user = await _userManager.FindByNameAsync(credentials.UserName);
                if (user == null)
                {
                    return BadRequest();
                }
            #endregion

            #region Check user password
                bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, credentials.Password);
                if (!isPasswordCorrect)
                {
                    return BadRequest();
                }
            #endregion

            return GenerateToken(await _userManager.GetClaimsAsync(user));
        }
        #endregion

        #region User Register
        [HttpPost]
        [Route("UserRegister")]

        public async Task<ActionResult<TokenDTO>> UserRegister(RegisterDTO registerDto)
        {
            #region Create User Object
            var newUser = new Users
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                DeptId = registerDto.DeptId,
            };
            #endregion

            #region Generate hash password using user manager feature
            var creationResult = await _userManager.CreateAsync(newUser,
                registerDto.Password);
            if (!creationResult.Succeeded)
            {
                return BadRequest(creationResult.Errors);
            }
            #endregion

            #region generate user claim list
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, newUser.Id),
            new Claim(ClaimTypes.Role, "User"),
            new Claim("Nationality", "Egyptian")
            };
            #endregion

            await _userManager.AddClaimsAsync(newUser, claims);

            return GenerateToken(await _userManager.GetClaimsAsync(newUser));
        }

        #endregion

        #region Admin Register
        [HttpPost]
        [Route("AdminRegister")]
        public async Task<ActionResult<TokenDTO>> AdminRegister(RegisterDTO registerDto)
        {
            #region Create Admin Object
            var newAdmin = new Users
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                DeptId = registerDto.DeptId,
            };
            #endregion

            #region Generate hash password using user manager feature
            var creationResult = await _userManager.CreateAsync(newAdmin,
                registerDto.Password);
            if (!creationResult.Succeeded)
            {
                return BadRequest(creationResult.Errors);
            }
            #endregion

            #region generate user claim list
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, newAdmin.Id),
            new Claim(ClaimTypes.Role, "admin"),
            new Claim("Nationality", "Egyptian")
            };
            #endregion

            await _userManager.AddClaimsAsync(newAdmin, claims);
            return GenerateToken(await _userManager.GetClaimsAsync(newAdmin));
        }

        #endregion
        
        #region Functions
        private TokenDTO GenerateToken(IList<Claim> claimList)
        {
            #region Prepare secretKey
            //1- get secretkey string from secrets file
            var stringKey = _config.GetValue<string>("secretkey") ?? string.Empty;
            //2- convert string to array of bytes
            var bytesKey = Encoding.ASCII.GetBytes(stringKey);
            //3- generate secret key object
            var key = new SymmetricSecurityKey(bytesKey);
            #endregion

            #region Generate JWT Token object 
            //combination between secret key and hashing algorithm 
            var signincredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            //token expire date
            var expireDate = DateTime.Now.AddMinutes(15);
            //generate jwt secret key 
            var jwt = new JwtSecurityToken(
                expires: expireDate,
                claims: claimList,
                signingCredentials: signincredentials
                );
            #endregion

            #region Convert token to string
            var handler = new JwtSecurityTokenHandler();
            var jwtString = handler.WriteToken(jwt);
            #endregion

            return new TokenDTO { Token = jwtString, Expiry = expireDate };
        }
        #endregion


    }
}
