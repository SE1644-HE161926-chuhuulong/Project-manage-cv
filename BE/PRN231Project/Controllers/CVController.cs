using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN231Project.Models;

namespace PRN231Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CVController : ControllerBase
    {
        private readonly ProjectPRN231Context _context;

        public CVController(ProjectPRN231Context context)
        {
            _context = context;
        }

        // GET: api/<AdminController>
        [HttpGet]
        public async Task<IActionResult> GetCVs()
        {
            var studentList = await _context.Cvs.Select(s => new { s.CvId, s.CvLink, s.CvName, s.AccountId, s.Applies }).ToListAsync();

            if (studentList == null)
            {
                return NotFound();
            }

            return Ok(studentList);
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCVById(int id)
        {
            var student = await _context.Cvs.FirstOrDefaultAsync(s => s.CvId == id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // POST api/<AdminController>
        [HttpPost("CreateCv")]
        public async Task<IActionResult> CreateCv(int accountId, IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    // Tạo đường dẫn cho việc lưu trữ file
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                    // Tạo thư mục nếu không tồn tại
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    // Tạo tên file duy nhất
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Đường dẫn đầy đủ của file được lưu
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Lưu file vào đường dẫn
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var CV = new Cv
                    {
                        CvName = file.FileName,
                        CvLink = filePath,
                        AccountId = accountId,
                    };
                    _context.Cvs.Add(CV);
                    await _context.SaveChangesAsync();
                    return Ok(new { filePath });
                }
                else
                {
                    return BadRequest("Không có file được tải lên.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }
        // DELETE api/<AdminController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cv = await _context.Cvs.FindAsync(id);

            if (cv == null)
            {
                return NotFound();
            }
            var applies = await _context.Applies.Where(x => x.CvId == id).ToListAsync();
            if (applies.Any())
            {
                foreach (var apply in applies)
                    _context.Applies.Remove(apply);
            }
            _context.Cvs.Remove(cv);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("GetCvsByAccount/{AccountId}")]
        public async Task<IActionResult> GetCvsByAccount(int AccountId)
        {
            var student = await _context.Cvs.Where(s => s.AccountId == AccountId).ToListAsync();

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }
        [HttpGet("GetCvsByJob/{JobId}")]
        public async Task<IActionResult> GetCvsByJob(int JobId)
        {
            var student = await _context.Cvs.Where(s => s.Applies.Where(x => x.JobId == JobId).FirstOrDefault() != null).Select(s => new { s.CvId, s.CvLink, s.CvName, s.AccountId }).ToListAsync();

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }
    }
}
