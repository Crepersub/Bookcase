using BookcaseAPI.Data;
using BookcaseAPI.Models;
using BookcaseAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookcaseAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exam>>> GetExams()
        {
            var isAdmin = User.IsInRole("Admin");
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (isAdmin)
                return await _context.Exams.ToListAsync();

            return await _context.Exams.Where(e => e.ClientId == userId).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Exam>> GetExam(int id)
        {
            var exam = await _context.Exams.FindAsync(id);

            if (exam == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole("Admin");
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (!isAdmin && exam.ClientId != userId)
            {
                return Forbid();
            }

            return exam;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Exam>> CreateExam(CreateExamDto dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            var exam = new Exam
            {
                Date = dto.Date,
                Address = dto.Address,
                TestName = dto.TestName,
                ClientId = userId
            };

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExam), new { id = exam.Id }, exam);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExam(int id, UpdateExamDto dto)
        {
            var existingExam = await _context.Exams.FindAsync(id);

            if (existingExam == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole("Admin");
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (!isAdmin && existingExam.ClientId != userId)
            {
                return Forbid();
            }

            existingExam.Date = dto.Date;
            existingExam.Address = dto.Address;
            existingExam.TestName = dto.TestName;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole("Admin");
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (!isAdmin && exam.ClientId != userId)
            {
                return Forbid();
            }

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}