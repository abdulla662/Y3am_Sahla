namespace sahla.DTOs.Request
{
    public class UpdateLessonDto
    {
        public string Title { get; set; }
        public string ContentType { get; set; }
        public int LessonOrder { get; set; }
        public int CourseId { get; set; }
        public string ContentUrl { get; set; }
    }
}
