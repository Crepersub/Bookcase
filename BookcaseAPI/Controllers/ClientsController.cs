using BookcaseAPI.Data;
using BookcaseAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookcaseAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var isAdmin = User.IsInRole("Admin");
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (!isAdmin && userId != id)
            {
                return Forbid();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            var applicationIds = await _context.Applications
                .Where(a => a.StudentId == id)
                .Select(a => a.Id)
                .ToListAsync();

            var examIds = await _context.Exams
                .Where(e => e.ClientId == id)
                .Select(e => e.Id)
                .ToListAsync();

            var majorIds = await _context.Majors
                .Where(m => m.ClientId == id)
                .Select(m => m.Id)
                .ToListAsync();

            var applicationExams = await _context.ApplicationExams
                .Where(ae => applicationIds.Contains(ae.ApplicationId) || examIds.Contains(ae.ExamId))
                .ToListAsync();

            if (applicationExams.Count > 0)
            {
                _context.ApplicationExams.RemoveRange(applicationExams);
            }

            var majorExams = await _context.MajorExams
                .Where(me => majorIds.Contains(me.MajorId) || examIds.Contains(me.ExamId))
                .ToListAsync();

            if (majorExams.Count > 0)
            {
                _context.MajorExams.RemoveRange(majorExams);
            }

            await _context.SaveChangesAsync();

            var applications = await _context.Applications
                .Where(a => a.StudentId == id)
                .ToListAsync();

            if (applications.Count > 0)
            {
                _context.Applications.RemoveRange(applications);
            }

            var majors = await _context.Majors
                .Where(m => m.ClientId == id)
                .ToListAsync();

            if (majors.Count > 0)
            {
                _context.Majors.RemoveRange(majors);
            }

            var exams = await _context.Exams
                .Where(e => e.ClientId == id)
                .ToListAsync();

            if (exams.Count > 0)
            {
                _context.Exams.RemoveRange(exams);
            }

            await _context.SaveChangesAsync();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}