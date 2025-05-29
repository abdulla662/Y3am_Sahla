namespace sahla.DTOs.Response
{
    public class UserDto
    {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Adress { get; set; }
            public int Points { get; set; }
            public DateTime CreatedAt { get; set; }
        public bool IsBlocked { get; set; }
        public string Role { get; set; } 


    }
}

