using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>Body dang nhap cho POST api/Users/login.</summary>
    public class AuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
