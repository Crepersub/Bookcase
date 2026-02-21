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
    public class MajorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MajorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Major>>> GetMajors()
        {
            var isAdmin = User.IsInRole("Admin");
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            var query = _context.Majors
                .Include(m => m.MajorExams)
                    .ThenInclude(me => me.Exam)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(m => m.ClientId == userId);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Major>> GetMajor(int id)
        {
            var major = await _context.Majors
                .Include(m => m.MajorExams)
                    .ThenInclude(me => me.Exam)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (major == null)
            {
                return NotFound();
            }

            return major;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Major>> CreateMajor(CreateMajorDto dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var status = Enum.TryParse<MajorStatus>(dto.Status, out var parsedStatus) ? parsedStatus : MajorStatus.Liked;

            var major = new Major
            {
                Name = dto.Name,
                UniversityName = dto.UniversityName,
                Address = dto.Address,
                Duration = dto.Duration,
                Language = dto.Language,
                GradingSystem = dto.GradingSystem,
                ClientId = userId,
                Status = status,
                Notes = dto.Notes
            };

            _context.Majors.Add(major);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMajor), new { id = major.Id }, major);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMajor(int id, UpdateMajorDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var major = await _context.Majors
                .Include(m => m.MajorExams)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (major == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole("Admin");
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (!isAdmin && major.ClientId != userId)
            {
                return Forbid();
            }

            var status = Enum.TryParse<MajorStatus>(dto.Status, out var parsedStatus) ? parsedStatus : MajorStatus.Liked;

            major.Name = dto.Name;
            major.UniversityName = dto.UniversityName;
            major.Address = dto.Address;
            major.Duration = dto.Duration;
            major.Language = dto.Language;
            major.GradingSystem = dto.GradingSystem;
            major.Status = status;
            major.Notes = dto.Notes;

            var requestedIds = dto.ExamIds.Distinct().ToList();

            var allowedExamIds = await _context.Exams
                .Where(e => requestedIds.Contains(e.Id))
                .Where(e => isAdmin || e.ClientId == userId)
                .Select(e => e.Id)
                .ToListAsync();

            if (allowedExamIds.Count != requestedIds.Count)
            {
                return BadRequest("One or more exams are invalid or not owned by the user.");
            }

            var allowedSet = allowedExamIds.ToHashSet();
            var existingSet = major.MajorExams.Select(me => me.ExamId).ToHashSet();

            foreach (var link in major.MajorExams.Where(me => !allowedSet.Contains(me.ExamId)).ToList())
            {
                _context.MajorExams.Remove(link);
            }

            foreach (var examId in allowedSet)
            {
                if (!existingSet.Contains(examId))
                {
                    major.MajorExams.Add(new MajorExam
                    {
                        MajorId = major.Id,
                        ExamId = examId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMajor(int id)
        {
            var major = await _context.Majors
                .Include(m => m.MajorExams)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (major == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole("Admin");
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (!isAdmin && major.ClientId != userId)
            {
                return Forbid();
            }

            if (major.MajorExams.Count > 0)
            {
                _context.MajorExams.RemoveRange(major.MajorExams);
            }

            _context.Majors.Remove(major);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}