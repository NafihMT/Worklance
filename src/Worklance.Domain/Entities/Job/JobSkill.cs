using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worklance.Domain.Entities.Job
{
    public class JobSkill
    {
        public int JobId { get; set; }
        public Job Job { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}
