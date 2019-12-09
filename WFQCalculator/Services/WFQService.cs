using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFQCalculator.Models;

namespace WFQCalculator.Services
{
    public class WFQService
    {
        #region Calculation Methods
        public static List<TimedPacket> CalculateWFQWithoutSkew(List<TimedPacket> packets)
        {
            List<TimedPacket> calculatedPackets = new List<TimedPacket>();

            foreach (TimedPacket packet in packets)
            {
                float previousCalculatedTime = 0;
                try
                {
                    if (calculatedPackets.Count != 0)
                        previousCalculatedTime = calculatedPackets.FindLast(previouslyCalculatedPacket => previouslyCalculatedPacket.Flow.FlowName == packet.Flow.FlowName).FinishedTime;
                }
                catch (Exception) { } //if no packet is found FindLast throws an exception

                packet.FinishedTime = previousCalculatedTime + (packet.Size / packet.Flow.FlowWeight);
                calculatedPackets.Add(packet);
            }

            packets.Sort(ComparePacketsByFinishedTime);

            return packets;
        }

        public static List<TimedPacket> CalculatWFQwithSkew(List<Flow> flows, List<TimedPacket> packets)
        {
            List<TimedPacket> sentPackets = new List<TimedPacket>();
            Dictionary<Flow, Queue<TimedPacket>> arrivedPacketsQueues = new Dictionary<Flow, Queue<TimedPacket>>();
            Dictionary<Flow, float> lastCalculatedPackets = new Dictionary<Flow, float>();
            int wallClock = 1, sentPacketsCount = 0, waitForPacketTime = 0;
            float arriavalTime = 1;
            bool packetIsSent = false;

            packets.Sort(ComparePacketsByArrivalTimes);


            foreach (Flow flow in flows)
            {
                arrivedPacketsQueues.Add(flow, new Queue<TimedPacket>());
                lastCalculatedPackets.Add(flow, 0);
            }

            while (sentPacketsCount != packets.Count)
            {
                List<TimedPacket> arrivedPackets = packets.FindAll(timedPackets => timedPackets.ArrivalTime == wallClock);
                List<TimedPacket> packetsInQueues = new List<TimedPacket>();
                TimedPacket minPacketInQueues = null;
                int totalQueueCount = 0;

                if (arrivedPackets.Count > 0)
                {

                    foreach (var packet in arrivedPackets)
                    {
                        CalculatePacketsFinishTime(lastCalculatedPackets, packet, arriavalTime);
                    }

                    foreach (TimedPacket packet in arrivedPackets)
                    {
                        lastCalculatedPackets[packet.Flow] = packet.FinishedTime;
                    }

                }

                foreach (var packet in arrivedPackets)
                {
                    arrivedPacketsQueues[packet.Flow].Enqueue(packet);
                }

                foreach (var flow in arrivedPacketsQueues.Keys)
                {
                    if (arrivedPacketsQueues[flow].Count > 0)
                        packetsInQueues.Add(arrivedPacketsQueues[flow].Peek());
                }

                if (sentPackets.Count != 0 && waitForPacketTime <= wallClock)
                    packetIsSent = false;


                if (packetsInQueues.Count > 0 && !packetIsSent)
                    minPacketInQueues = packetsInQueues.Aggregate((currentMinPacket, packet) => currentMinPacket.FinishedTime <= packet.FinishedTime ? currentMinPacket : packet);

                if (minPacketInQueues != null && !packetIsSent)
                {
                    sentPackets.Add(arrivedPacketsQueues[minPacketInQueues.Flow].Dequeue());
                    sentPacketsCount++;
                    waitForPacketTime = wallClock + minPacketInQueues.Size;
                    packetIsSent = true;
                }


                foreach (var flow in arrivedPacketsQueues.Keys)
                {
                    totalQueueCount += arrivedPacketsQueues[flow].Count;
                }

                arriavalTime += (1 / (float)(1 + totalQueueCount));
                wallClock++;
            }

            return sentPackets;
        }
        #endregion

        #region Create information from File
        public static List<TimedPacket> CreatePacketsFromFileLines(string[] lines, List<Flow> flows)
        {
            List<TimedPacket> packets = new List<TimedPacket>();

            for (int i = 1; i < lines.Length; i++)
            {
                string[] split = lines[i].Split(';');

                if (split.Length != 4)
                    throw new FormatException("Row format is invalid");

                if (string.IsNullOrEmpty(split[0]))
                    throw new FormatException("Packet Number cannot be empty");

                if (!int.TryParse(split[1], out int size))
                    throw new FormatException("packet arrival is not a valid integer number");

                if (size <= 0)
                    throw new FormatException("packet size cannot be zero or negative value");

                Flow flow = flows.Find(f => f.FlowName == split[2]);

                if (!flows.Exists(f => f.FlowName == flow.FlowName))
                    throw new FormatException("packet flow doesn't exsits");

                if (!int.TryParse(split[3], out int arrival))
                    throw new FormatException("packet arrival is not a valid integer number");

                if (arrival <= 0)
                    throw new FormatException("packet arrival cannot be zero or negative value");

                TimedPacket packet = new TimedPacket() { Number = split[0], Size = size, ArrivalTime = arrival, Flow = flow };
                if (packets.Exists(p => p.Number == packet.Number))
                    throw new FormatException("packet number already exsits");

                packets.Add(packet);
            }

            return packets;
        }

        public static List<Flow> CreateFlowsFromFileLines(string[] lines)
        {
            List<Flow> flows = new List<Flow>();

            string[] split = lines[0].Split(';');
            foreach (var flow in split)
            {
                var inners = flow.Split('=');

                if (inners.Length != 2)
                    throw new FormatException("Flow formatting is invalid");

                if (string.IsNullOrEmpty(inners[0]))
                    throw new FormatException("Invalid flow name");

                if (!float.TryParse(inners[1], out float weight))
                    throw new FormatException("Weight is not a number");

                if (weight <= 0)
                    throw new FormatException("Weight cannot be a negative number or zero");

                Flow flowItem = new Flow() { FlowName = inners[0], FlowWeight = weight };

                if (flows.Exists(f => f.FlowName == flowItem.FlowName))
                    throw new FormatException("Flows cannot have the same name");

                flows.Add(flowItem);
            }

            return flows;
        }
        #endregion

        #region Sorting Packets
        private static int ComparePacketsByFinishedTime(TimedPacket first, TimedPacket second)
        {
            return first.FinishedTime.CompareTo(second.FinishedTime);
        }

        private static int ComparePacketsByArrivalTimes(TimedPacket first, TimedPacket second)
        {
            return first.ArrivalTime.CompareTo(second.ArrivalTime);
        }
        #endregion

        #region Calculations Helpers

        private static void CalculatePacketsFinishTime(Dictionary<Flow, float> lastCalculatedPackets, TimedPacket packet, float arrivalTime = 0)
        {
            var previusPacketFinishTime = lastCalculatedPackets[packet.Flow];

            packet.FinishedTime = Math.Max(arrivalTime, previusPacketFinishTime) + ((packet.Size) / (packet.Flow.FlowWeight));
        }



        #endregion
    }
}
