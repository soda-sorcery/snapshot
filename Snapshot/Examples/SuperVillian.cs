using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Snapshot.Examples
{
    public class SuperVillian : ISnapshot
    {
        public SuperVillian(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public bool HasEvilPlan { get; set; }
    }
}
