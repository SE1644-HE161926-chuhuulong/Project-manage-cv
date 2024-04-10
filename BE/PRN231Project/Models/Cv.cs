using System;
using System.Collections.Generic;

namespace PRN231Project.Models
{
    public partial class Cv
    {
        public Cv()
        {
            Applies = new HashSet<Apply>();
        }

        public int CvId { get; set; }
        public string CvName { get; set; } = null!;
        public string CvLink { get; set; } = null!;
        public int AccountId { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<Apply> Applies { get; set; }
    }
}
