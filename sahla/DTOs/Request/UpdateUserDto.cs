namespace sahla.DTOs.Request
{
    public class UpdateUserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; } // إذا كنت بتحتاج تحدثه
        public string? Adress { get; set; }
        public int? Points { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
