using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sahla.DTOs.Response;

namespace sahla.Controllers.Admin.LessonsControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public LessonController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        [HttpPost("CreateLesson")]
        public async Task<IActionResult> CreateLesson([FromForm] CreateLessonDto dto, IFormFile? File)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var isDuplicate = _unitOfWork.Lessons.Get(x => x.CourseId == dto.CourseId && x.LessonOrder == dto.LessonOrder).Any();
            if (isDuplicate)
                return BadRequest("There is already a lesson with the same order in this course.");

            string filePath = null;
            string folder = "uploads";

            if (File != null && File.Length > 0)
            {
                var extension = Path.GetExtension(File.FileName).ToLower();

                if (dto.ContentType == "Text")
                    folder = Path.Combine("wwwroot", "text");
                else if (extension == ".pdf" || extension == ".doc" || extension == ".docx")
                    folder = Path.Combine("wwwroot", "documents");
                else if (extension == ".mp4" || extension == ".mov" || extension == ".avi")
                    folder = Path.Combine("wwwroot", "videos");
                else
                    return BadRequest("Unsupported file type");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + extension;
                filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await File.CopyToAsync(stream);
                }

                dto.ContentUrl = $"/{Path.GetFileName(folder)}/{fileName}";
            }

            var lesson = dto.Adapt<Lesson>();
            _unitOfWork.Lessons.Create(lesson);
            _unitOfWork.Lessons.comit();

            return Ok(new { message = "Lesson created successfully", lessonId = lesson.LessonId });
        }
        [HttpGet("GetCoursesLessons")]
        public IActionResult GetCoursesLessons()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = _unitOfWork.Courses.Get(e => e.TeacherId == teacherId).ToList();
            var courseDtos = data.Adapt<List<RetriveCourseDto>>();


            if (data == null || !data.Any())
                return NotFound("No courses found for this teacher.");

            return Ok(new { message = "Courses Found", data = courseDtos });
        }
        [HttpGet("GetLessonsByCourseId/{courseId}")]
        public IActionResult GetLessonsByCourseId(int courseId)
        {
            var lessons = _unitOfWork.Lessons.Get(e => e.CourseId == courseId).ToList();
            if (lessons == null || !lessons.Any())
                return NotFound("No lessons found for this course.");
            var lessonDtos = lessons.Adapt<List<RetriveLessonDto>>();
            return Ok(new { message = "Lessons Found", data = lessonDtos });
        }



        [HttpPut("UpdateLesson/{lessonId}")]
        public async Task<IActionResult> UpdateLesson(int lessonId, [FromForm] UpdateLessonDto dto, IFormFile? File)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lesson = _unitOfWork.Lessons.GetOne(e => e.LessonId == lessonId);
            if (lesson == null)
                return NotFound("Lesson not found.");

            if (File != null && File.Length > 0)
            {
                if (!string.IsNullOrEmpty(lesson.ContentUrl))
                {
                    var oldPath = Path.Combine("wwwroot", lesson.ContentUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                var extension = Path.GetExtension(File.FileName).ToLower();
                string folder = "uploads";

                if (dto.ContentType == "Text")
                    folder = Path.Combine("wwwroot", "text");
                else if (extension == ".pdf" || extension == ".doc" || extension == ".docx")
                    folder = Path.Combine("wwwroot", "documents");
                else if (extension == ".mp4" || extension == ".mov" || extension == ".avi")
                    folder = Path.Combine("wwwroot", "videos");
                else
                    return BadRequest("Unsupported file type");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + extension;
                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await File.CopyToAsync(stream);
                }

                dto.ContentUrl = $"/{Path.GetFileName(folder)}/{fileName}";
            }

            dto.Adapt(lesson);

            _unitOfWork.Lessons.Edit(lesson);
            _unitOfWork.Lessons.comit();

            return Ok(new { message = "Lesson updated successfully", lessonId = lesson.LessonId });
        }
        [HttpGet("GetLessonById/{lessonId}")]
        public IActionResult GetLessonForEdit(int lessonId)
        {
            var lesson = _unitOfWork.Lessons.GetOne(e => e.LessonId == lessonId);
            if (lesson == null)
                return NotFound("Lesson not found.");
            var lessonDto = lesson.Adapt<RetriveLessonDto>();
            return Ok(new { message = "Lesson Found", data = lessonDto });
        }
        [HttpDelete("DeleteLesson/{lessonId}")]
        public IActionResult DeleteLesson(int lessonId) { 
               var deletedlesson=_unitOfWork.Lessons.GetOne(e => e.LessonId == lessonId);
            if (deletedlesson == null)
                return NotFound("Lesson not found.");
             _unitOfWork.Lessons.Delete(deletedlesson);
            _unitOfWork.Lessons.comit();
            return Ok(new { message = "Lesson deleted successfully" });
        }
    }
}


