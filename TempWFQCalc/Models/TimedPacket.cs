namespace TempWFQCalc.Models
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
    }
}