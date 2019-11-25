using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempWFQCalc.Models
{
    public class Flow
    {
        public string FlowName { get; set; }
        public float FlowWeight { get; set; }

        public override string ToString()
        {
            return FlowName + "," + FlowWeight;
        }
    }
}
