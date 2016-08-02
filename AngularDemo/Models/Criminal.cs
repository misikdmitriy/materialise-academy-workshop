using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularDemo.Models
{
    public class Criminal
    {
        public virtual Guid ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal Reward { get; set; }
    }
}