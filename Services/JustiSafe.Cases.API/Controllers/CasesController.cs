using JustiSafe.Cases.API.Data;
using JustiSafe.Cases.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JustiSafe.Cases.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CasesController : ControllerBase
    {
        private readonly CasesDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public CasesController(CasesDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Simple logic: return all if admin, filter if judge.
            // Getting user info from Claims
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value; // Assuming Role claim
            var userIdStr = User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            int userId = int.Parse(userIdStr);

            if (role == "Admin")
            {
                var cases = await _context.Cases.OrderByDescending(c => c.CreatedAt).ToListAsync();
                return Ok(cases);
            }
            else
            {
                var cases = await _context.Cases
                    .Where(c => c.JudgeId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
                return Ok(cases);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var caseItem = await _context.Cases.Include(c => c.Verdicts).FirstOrDefaultAsync(c => c.CaseId == id);
            if (caseItem == null) return NotFound();
            return Ok(caseItem);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCaseDto dto)
        {
            // Call Identity to get Judges
            var client = _httpClientFactory.CreateClient("IdentityClient");
            var response = await client.GetAsync("api/auth/judges");
            
            if (!response.IsSuccessStatusCode)
                return StatusCode(500, "Error getting judges for allocation");

            var judgesJson = await response.Content.ReadAsStringAsync();
            var judges = JsonSerializer.Deserialize<List<JudgeDto>>(judgesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (judges == null || !judges.Any())
                return BadRequest("No judges available for lottery.");

            // Lottery
            var random = new Random();
            var selectedJudge = judges[random.Next(judges.Count)];

            string year = DateTime.Now.Year.ToString();
            string randomPart = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();

            var newCase = new Case
            {
                Title = dto.Title,
                Description = dto.Description,
                JudgeId = selectedJudge.UserId,
                Status = "Sorteado",
                AnonCode = $"CASE-{year}-{randomPart}",
                CreatedAt = DateTime.Now
            };

            _context.Cases.Add(newCase);
            await _context.SaveChangesAsync();

            return Ok(newCase);
        }

        [HttpPut("{id}/verdict")]
        public async Task<IActionResult> AddVerdict(int id, [FromBody] VerdictDto dto)
        {
             var caseItem = await _context.Cases.FindAsync(id);
             if (caseItem == null) return NotFound();

             var verdict = new Verdict
             {
                 CaseId = id,
                 Content = dto.Content,
                 IsFinal = dto.IsFinal,
                 DateIssued = DateTime.Now
             };
             
             _context.Verdicts.Add(verdict);
             await _context.SaveChangesAsync();
             return Ok(verdict);
        }
    }

    public class CreateCaseDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class JudgeDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
    }

    public class VerdictDto
    {
        public string Content { get; set; }
        public bool IsFinal { get; set; }
    }
}
