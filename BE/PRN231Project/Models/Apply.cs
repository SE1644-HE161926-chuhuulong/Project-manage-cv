using System;
using System.Collections.Generic;

namespace PRN231Project.Models
{
    public partial class Apply
    {
        public int ApplyId { get; set; }
        public int CvId { get; set; }
        public int JobId { get; set; }

        public virtual Cv Cv { get; set; } = null!;
        public virtual Job Job { get; set; } = null!;
    }
}
