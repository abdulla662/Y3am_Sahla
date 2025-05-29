namespace sahla.DTOs.Request
{
    public class CreateLessonDto
    {
        public string Title { get; set; }
        public string ContentType { get; set; }
        public int LessonOrder { get; set; }
        public int CourseId { get; set; }
        public string Section { get; set; }
        public string ContentUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.ToLocalTime(); 

    }
}
