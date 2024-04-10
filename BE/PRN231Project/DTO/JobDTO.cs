namespace PRN231Project.DTO
{
   public class JobDTO
   {
      public int JobId { get; set; }
      public string JobName { get; set; } = null!;
      public string JobDesc { get; set; } = null!;
      public string JobRequire { get; set; } = null!;
      public string Address { get; set; } = null!;
      public string Salary { get; set; } = null!;
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public int AccountId { get; set; }
   }
}
