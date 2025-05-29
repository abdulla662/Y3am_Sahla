using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sahla.DataAcess;
using sahla.DTOs.Response;
using sahla.Models;

namespace sahla.Controllers.Admin.CoursesControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
       private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManger)
        {
           _unitOfWork = unitOfWork;
            _userManager = userManger;
        }
        [HttpPost("CreateCourses")]
        public async Task<IActionResult> CreateCourses([FromForm] CreateCourseRequest createCourse, IFormFile? profilePicture)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = createCourse.Adapt<Course>();
            course.TeacherId = teacherId;

            if (profilePicture != null && profilePicture.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";
                var filePath = Path.Combine("wwwroot", "Course_images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                course.ImageUrl = $"/Course_images/{fileName}";
            }

            _unitOfWork.Courses.Create(course);
            _unitOfWork.Courses.comit();

            return Ok(new { message = "Course created successfully", courseId = course.CoursId });
        }

        [HttpGet("GetMyCourses")]
        public IActionResult GetMyCourses()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = _unitOfWork.Courses.Get(e => e.TeacherId == teacherId).ToList();
            var courseDtos = data.Adapt<List<RetriveCourseDto>>();


            if (data == null || !data.Any())
                return NotFound("No courses found for this teacher.");

            return Ok(new { message = "Courses Found", data = courseDtos });
        }
        [HttpPut("UpdateCourse")]
        public async Task<IActionResult> UpdateCourse([FromForm] UpdateCourseRequest request, IFormFile? profilePicture)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var course = _unitOfWork.Courses.GetOne(e => e.CoursId == request.CoursId);
            if (course == null || course.TeacherId != teacherId)
                return NotFound("Course not found or you're not authorized.");

            course.Title = request.Title;
            course.Description = request.Description;
            course.Level = request.Level;
            course.Category = request.Category;

            if (profilePicture != null && profilePicture.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Course_images");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                course.ImageUrl = $"/Course_images/{fileName}";
            }

            _unitOfWork.Courses.Edit(course);
            _unitOfWork.Courses.comit();

            return Ok(new { message = "Course updated successfully" });
        }
        [HttpGet("{id}")]
        public IActionResult GetCourseById(int id)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var course = _unitOfWork.Courses.GetOne(e => e.CoursId == id && e.TeacherId == teacherId);
            if (course == null)
                return NotFound("Course not found or you're not authorized.");

            var result = course.Adapt<RetriveCourseDto>();
            return Ok(result);
        }
        [HttpDelete("DeleteCourse")]
        public IActionResult DeleteCourse(int id) {
            if (id == 0 || id == null) return NotFound("File Not Found");
            var DeletedCourse = _unitOfWork.Courses.GetOne(e => e.CoursId == id);
            _unitOfWork.Courses.Delete(DeletedCourse);
            _unitOfWork.Courses.comit();
            return Ok("Course Has Been Deleted Succefully");
        }
    }
}
