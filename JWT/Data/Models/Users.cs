using Microsoft.AspNetCore.Identity;

namespace JWT.Data.Models
{
    public class Users : IdentityUser
    {
        public int DeptId { get; set; }
    }
}
