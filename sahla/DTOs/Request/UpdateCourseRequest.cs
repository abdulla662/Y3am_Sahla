namespace sahla.DTOs.Request
{
    public class UpdateCourseRequest
    {
        public int CoursId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Level { get; set; }
        public string? Category { get; set; }
    }
}
