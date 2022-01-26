using System.ComponentModel.DataAnnotations;

namespace API.Requests
{
    public class SignupRequest
    {
        [Required]
        public string ReCaptchaToken { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
