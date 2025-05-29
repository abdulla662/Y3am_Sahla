namespace sahla.DTOs.Response
{
    public class RetriveLessonDto
    {
        public int LessonId { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public int LessonOrder { get; set; }
        public int CourseId { get; set; }
        public string ContentUrl { get; set; }

        public string Section { get; set; } // ✅ أضف هذا السطر

    }
}
