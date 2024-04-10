using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN231Project.DTO;
using PRN231Project.Models;

namespace PRN231Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplyController : ControllerBase
    {
        private readonly ProjectPRN231Context _context;

        public ApplyController(ProjectPRN231Context context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetApplies()
        {
            var studentList = await _context.Applies.Select(s => new { s.ApplyId, s.CvId, s.JobId, s.Cv }).ToListAsync();

            if (studentList == null)
            {
                return NotFound();
            }

            return Ok(studentList);
        }
        [HttpGet("GetApplyById/{ApplyId}")]
        public async Task<IActionResult> GetApplyById(int ApplyId)
        {
            var studentList = await _context.Applies.Include(x => x.Cv).Include(x => x.Job).Where(x => x.ApplyId == ApplyId).FirstOrDefaultAsync();

            if (studentList == null)
            {
                return NotFound();
            }
            var result = new ApplyDTO()
            {
                CvId = studentList.CvId,
                JobId = studentList.JobId,
            };

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> PostApply(ApplyDTO applyDTO)
        {
            var selectApplyFromDb = await _context.Applies.Where(x => x.CvId == applyDTO.CvId && x.JobId == applyDTO.JobId).FirstOrDefaultAsync();
            if (selectApplyFromDb == null)
            {
                var apply = new Apply
                {
                    CvId = applyDTO.CvId,
                    JobId = applyDTO.JobId,
                };
                _context.Applies.Add(apply);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetApplyById", new { ApplyId = apply.ApplyId }, apply);
            }
            else
                return Ok("Already exist");

        }
        [HttpDelete("{ApplyId}")]
        public async Task<IActionResult> DeleteApply(int ApplyId)
        {
            var apply = await _context.Applies.FindAsync(ApplyId);

            if (apply == null)
            {
                return NotFound();
            }

            _context.Applies.Remove(apply);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("GetApplyByJobId/{JobId}")]
        public async Task<IActionResult> GetApplyByJobId(int JobId)
        {
            var studentList = await _context.Applies.Where(x => x.JobId == JobId).ToListAsync();

            if (studentList == null)
            {
                return NotFound();
            }

            return Ok(studentList);
        }
    }
}
