using System;
using System.Collections.Generic;

namespace PRN231Project.Models
{
    public partial class Account
    {
        public Account()
        {
            Cvs = new HashSet<Cv>();
            Jobs = new HashSet<Job>();
        }

        public int AccountId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? Status { get; set; }
        public string? Role { get; set; }

        public virtual ICollection<Cv> Cvs { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
