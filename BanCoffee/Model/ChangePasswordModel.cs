#pragma warning disable IDE1006
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>Body doi mat khau cho POST api/Users/change-password.</summary>
    public class ChangePasswordModel {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}



