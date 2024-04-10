using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Implementation;
using PRN231Project.DTO;
using PRN231Project.Models;

namespace PRN231Project.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class jobController : ControllerBase
   {
      private readonly ProjectPRN231Context _context;

      public jobController(ProjectPRN231Context context)
      {
         _context = context;
      }

      // GET: api/Jobs
      [HttpGet("GetJobs")]
      public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
      {
         if (_context.Jobs.Include(x=>x.Applies) == null)
         {
            return NotFound();
         }



         return await _context.Jobs.ToListAsync();
      }
      [HttpGet("GetJobsByAccountId/{accountId}")]
      public async Task<ActionResult<IEnumerable<Job>>> GetJobsByAccountId(int accountId)
      {
         var result = _context.Jobs.Where(x => x.AccountId == accountId);
         if (result == null)
         {
            return NotFound();
         }



         return await result.ToListAsync();
      }
      [HttpGet("SearchJobByName/{search}")]
      public async Task<ActionResult<IEnumerable<Job>>> SearchJobByName(string search)
      {
         if (_context.Jobs == null)
         {
            return NotFound();
         }



         return  Ok(_context.Jobs.Where(x => x.JobName.Contains(search)).ToListAsync());
      }

      // GET: api/Jobs/5
      [HttpGet("{id}")]
      public async Task<ActionResult<Job>> GetJob(int id)
      {
         if (_context.Jobs == null)
         {
            return NotFound();
         }
         var job = await _context.Jobs.FindAsync(id);

         if (job == null)
         {
            return NotFound();
         }

         return Ok(job);
      }

      // PUT: api/Jobs/5
      // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      [HttpPut("UpdateJob/{id}")]
      public async Task<IActionResult> PutJob(int id, JobDTO jobDTO)
      {
         if (id != jobDTO.JobId)
         {
            return BadRequest();
         }
         var jobSearch = _context.Jobs.SingleOrDefault(x => x.JobId == id);
         jobSearch.JobName = jobDTO.JobName;
         jobSearch.JobDesc= jobDTO.JobDesc;
         jobSearch.JobRequire = jobDTO.JobRequire;
         jobSearch.Address= jobDTO.Address;
         jobSearch.Salary= jobDTO.Salary;
         jobSearch.StartDate= jobDTO.StartDate;
         jobSearch.EndDate = jobDTO.EndDate;


         _context.Entry(jobSearch).State = EntityState.Modified;

         try
         {
            await _context.SaveChangesAsync();
         }
         catch (DbUpdateConcurrencyException)
         {
            if (!JobExists(id))
            {
               return NotFound();
            }
            else
            {
               throw;
            }
         }

         return NoContent();
      }

      // POST: api/Jobs
      // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      [HttpPost("CreateJob")]
      public async Task<ActionResult<Job>> CreateJobManager(JobDTO JobDTO)
      {
         if (_context.Jobs == null)
         {
            return Problem("Entity set 'ProjectPRN231Context.Jobs'  is null.");
         }
         var Job = new Job
         {
            JobName = JobDTO.JobName,
            JobDesc= JobDTO.JobDesc,
            JobRequire  = JobDTO.JobRequire,
            Salary= JobDTO.Salary,
            StartDate= JobDTO.StartDate,
            EndDate= JobDTO.EndDate,
            Address= JobDTO.Address,
            AccountId = JobDTO.AccountId,
           


         };
         _context.Jobs.Add(Job);
         await _context.SaveChangesAsync();

         return CreatedAtAction("GetJob", new { id = Job.JobId }, Job);
      }
      
      // DELETE: api/Jobs/5
      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteJob(int id)
      {
         
            var job = await _context.Jobs.FindAsync(id);

            if (job == null)
            {
               return NotFound();
            }
            var applies = await _context.Applies.Where(x => x.JobId == id).ToListAsync();
            if (applies.Any())
            {
               foreach (var apply in applies)
                  _context.Applies.Remove(apply);
            }
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
         
      }

      private bool JobExists(int id)
      {
         return (_context.Jobs?.Any(e => e.JobId == id)).GetValueOrDefault();
      }
   }
}

