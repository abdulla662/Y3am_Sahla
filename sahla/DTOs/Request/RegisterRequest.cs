
namespace sahla.DTOs.Request
{
    public class RegisterRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(password))]
        public string ConfirmPassword { get; set; }

        public string Address { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }
}
