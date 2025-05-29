using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sahla.DTOs.Response;
using sahla.ViewModel;
using System.Linq.Expressions;

namespace sahla.Controllers.Admin.TestsController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public TestController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        [HttpPost("CreateTest")]
        public IActionResult CreateTest([FromBody] TestCreationViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var test = new Test
            {
                Title = model.Title,
                Description = model.Description,
                Partition = model.Partition,
                CourseId = model.CourseId,
                CreatedAt = DateTime.Now,
                Questions = model.Questions.Select(q => new Question
                {
                    QuestionText = q.QuestionText,
                    Points = q.Points,
                    QuestionType = q.QuestionType,
                    Answers = q.Answers.Select(a => new Answer
                    {
                        AnswerText = a.AnswerText,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                }).ToList()
            };
            _unitOfWork.Tests.Create(test);
            _unitOfWork.Tests.comit();
            return Ok(new { message = "✅ Test created successfully", testId = test.TestId });
        }
        [HttpGet("GetCoursesForTestAdd")]
        public IActionResult GetCoursesForTestAdd()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (teacherId == null)
                return Unauthorized("❌ Token not valid or missing.");

            var data = _unitOfWork.Courses
      .Get(e => e.TeacherId == teacherId, includes: [e => e.Tests])
      .ToList()
      .Where(c => c.Tests != null && c.Tests.Any())
      .ToList();

            if (!data.Any())
                return NotFound("❌ No courses found for this teacher with tests.");

            if (!data.Any())
                return NotFound("❌ No courses found for this teacher.");

            var courseDtos = data.Adapt<List<RetriveCourseDto>>();
            return Ok(new { message = "✅ Courses Found", data = courseDtos });
        }
        [HttpGet("GetCourses")]
        public IActionResult GetCourses()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (teacherId == null)
                return Unauthorized("❌ Token not valid or missing.");

            var data = _unitOfWork.Courses.Get(e => e.TeacherId == teacherId).ToList();


            var courseDtos = data.Adapt<List<RetriveCourseDto>>();
            return Ok(new { message = "✅ Courses Found", data = courseDtos });
        }

        [HttpGet("GetTests")]
        public IActionResult GetTests(int courseId)
        {
            var tests = _unitOfWork.Tests.Get(
                e => e.CourseId == courseId,
                includes: [e => e.Course]
            ).ToList();

            if (!tests.Any())
                return NotFound("❌ No tests found for this course.");

            var testDtos = tests.Select(test => new RetriveTestDto
            {
                TestId = test.TestId,
                Title = test.Title,
                CourseTitle = test.Course?.Title ?? "N/A",
                CreatedAt = test.CreatedAt
            }).ToList();

            return Ok(new { message = "✅ Tests Found", data = testDtos });
        }
        [HttpGet("GetTestDetails")]
        public IActionResult GetTestDetails(int testId)
        {
            // 1. جلب التيست ومعاه الـ Questions
            var test = _unitOfWork.Tests.GetOne(
                t => t.TestId == testId,
                includes: [t => t.Questions, t => t.Course] // لاحظ: نجيب الـ Questions بس هنا
            );

            if (test == null)
                return NotFound("❌ Test not found");

            // 2. جلب الـ Answers لكل Question يدويًا
            foreach (var question in test.Questions)
            {
                question.Answers = _unitOfWork.Answers.Get(a => a.QuestionId == question.QuestionId).ToList();
            }

            // 3. إعداد الداتا للإرسال
            var dto = new TestDetailsDto
            {
                TestId = test.TestId,
                Title = test.Title,
                Description = test.Description,
                Partition = test.Partition,
                CourseId = test.CourseId,
                CourseTitle = test.Course?.Title,
                CreatedAt = test.CreatedAt,
                Questions = test.Questions.Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Points = q.Points,
                    QuestionType = q.QuestionType,
                    Answers = q.Answers.Select(a => new AnswerDto
                    {
                        AnswerId = a.AnswerId,
                        AnswerText = a.AnswerText,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                }).ToList()
            };

            return Ok(new { message = "✅ Test loaded", data = dto });
        }
        [HttpPut("UpdateTest")]
        public IActionResult UpdateTest([FromBody] TestUpdateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingTest = _unitOfWork.Tests.Get(
                    t => t.TestId == model.TestId,
                    includes: new Expression<Func<Test, object>>[]
                    {
                t => t.Questions
                    }
                ).FirstOrDefault();

                if (existingTest == null)
                    return NotFound("❌ Test not found.");

                foreach (var q in existingTest.Questions)
                {
                    q.Answers = _unitOfWork.Answers.Get(a => a.QuestionId == q.QuestionId).ToList();
                }

                // تحديث بيانات التست
                existingTest.Title = model.Title;
                existingTest.Description = model.Description;
                existingTest.Partition = model.Partition;
                existingTest.CourseId = model.CourseId;

                // حذف الأسئلة المحذوفة
                var updatedQuestionIds = model.Questions
                    .Where(q => q.QuestionId.HasValue)
                    .Select(q => q.QuestionId.Value)
                    .ToList();

                var questionsToDelete = existingTest.Questions
                    .Where(q => !updatedQuestionIds.Contains(q.QuestionId))
                    .ToList();

                foreach (var q in questionsToDelete)
                {
                    var answers = _unitOfWork.Answers.Get(a => a.QuestionId == q.QuestionId).ToList();
                    foreach (var a in answers)
                    {
                        _unitOfWork.Answers.Delete(a);
                    }
                    _unitOfWork.Questions.Delete(q);
                }

                // تحديث أو إضافة الأسئلة
                foreach (var updatedQ in model.Questions)
                {
                    if (updatedQ.QuestionId.HasValue)
                    {
                        // تعديل سؤال موجود
                        var existingQ = existingTest.Questions.FirstOrDefault(q => q.QuestionId == updatedQ.QuestionId);
                        if (existingQ != null)
                        {
                            existingQ.QuestionText = updatedQ.QuestionText;
                            existingQ.Points = updatedQ.Points;
                            existingQ.QuestionType = updatedQ.QuestionType;

                            var updatedAnswerIds = updatedQ.Answers
                                .Where(a => a.AnswerId.HasValue)
                                .Select(a => a.AnswerId.Value)
                                .ToList();

                            var existingAnswers = existingQ.Answers.ToList();

                            var answersToDelete = existingAnswers
                                .Where(a => !updatedAnswerIds.Contains(a.AnswerId))
                                .ToList();

                            foreach (var a in answersToDelete)
                            {
                                _unitOfWork.Answers.Delete(a);
                            }

                            foreach (var updatedA in updatedQ.Answers)
                            {
                                if (updatedA.AnswerId.HasValue)
                                {
                                    var existingA = existingAnswers.FirstOrDefault(a => a.AnswerId == updatedA.AnswerId);
                                    if (existingA != null)
                                    {
                                        existingA.AnswerText = updatedA.AnswerText;
                                        existingA.IsCorrect = updatedA.IsCorrect;
                                    }
                                }
                                else
                                {
                                    var newAnswer = new Answer
                                    {
                                        AnswerText = updatedA.AnswerText,
                                        IsCorrect = updatedA.IsCorrect,
                                        QuestionId = existingQ.QuestionId
                                    };
                                    _unitOfWork.Answers.Create(newAnswer);
                                }
                            }
                        }
                    }
                    else
                    {
                        // إضافة سؤال جديد
                        var newQ = new Question
                        {
                            QuestionText = updatedQ.QuestionText,
                            Points = updatedQ.Points,
                            QuestionType = updatedQ.QuestionType,
                            TestId = existingTest.TestId
                        };
                        _unitOfWork.Questions.Create(newQ);

                        // إضافة إجاباته
                        foreach (var a in updatedQ.Answers)
                        {
                            var newA = new Answer
                            {
                                AnswerText = a.AnswerText,
                                IsCorrect = a.IsCorrect,
                                QuestionId = newQ.QuestionId // مهم جدًا
                            };
                            _unitOfWork.Answers.Create(newA);
                        }
                    }
                }

                _unitOfWork.Tests.Edit(existingTest);
                _unitOfWork.Tests.comit();

                return Ok(new { message = "✅ Test updated successfully", testId = existingTest.TestId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Internal error: {ex.Message}");
            }
        }
        [HttpDelete("DeleteTest")]
        public IActionResult DeleteTest(int testId)
        {
            var test = _unitOfWork.Tests.GetOne(t => t.TestId == testId, includes: [t => t.Questions]);
            if (test == null)
                return NotFound("❌ Test not found.");
            _unitOfWork.Tests.Delete(test);
            _unitOfWork.Tests.comit();
            return Ok(new { message = "✅ Test deleted successfully" });
        }



    }
}

