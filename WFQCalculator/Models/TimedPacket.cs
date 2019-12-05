using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFQCalculator.Models
{
    public class TimedPacket
    {
        public string Number { get; set; }
        public int Size { get; set; }
        public int ArrivalTime { get; set; }
        public float FinishedTime { get; set; }
        public Flow Flow { get; set; }

        public int SortByFinishedTime(float time1, float time2)
        {
            return time1.CompareTo(time2);
        }

        public override string ToString()
        {
            return Number + " " + Size + " " + ArrivalTime + " " + FinishedTime;
        }
    }
}
