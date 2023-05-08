using JWT.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        public DataController(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }

        #region Data access for all 

        [HttpGet]
        [Authorize]
        [Route("ForAll")]
        public async Task<ActionResult> GetSecuredData()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return Ok(new string[] {
             currentUser!.UserName!,
             currentUser!.Email!,
             currentUser!.DeptId!.ToString()
        });
        }
        #endregion

        #region Data access for admin 

        [HttpGet]
        [Authorize(Policy = "admin")]
        [Route("ForAdmin")]
        public async Task<ActionResult> GetAdminData()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return Ok(new string[] {
             currentUser!.UserName!,
             currentUser!.Email!,
            "This Data From admin Only"
        });
        }
        #endregion

        #region Data access for users

        [HttpGet]
        [Authorize(Policy = "users")]
        [Route("ForUsers")]
        public async Task<ActionResult> GetForUsers()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return Ok(new string[] {
             currentUser!.UserName!,
             currentUser!.Email!,
            "This Data For Users and admin"
        });
        }
        #endregion

    }
}
