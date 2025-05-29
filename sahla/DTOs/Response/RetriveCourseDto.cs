namespace sahla.DTOs.Response
{
    public class RetriveCourseDto
    {
        public int CoursId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }  // ✅ لازم تكون موجودة
        public string Level { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }     // ✅ لازم تكون بنفس الاسم
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.ToLocalTime();

    }
}
