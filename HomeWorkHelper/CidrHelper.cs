using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HomeWorkHelper
{
    public class CidrHelper
    {
        public static bool CheckCidrIpAddressOverlap(string firstCidrIp, string secondCidrIp)
        {
            if (!ValidateGivenCidrAddressNotation(firstCidrIp) || !ValidateGivenCidrAddressNotation(secondCidrIp))
                return false;

            string[] splitFirst = firstCidrIp.Split('/');
            string[] splitSecond = secondCidrIp.Split('/');

            byte[] firstByteArray = IPAddress.Parse(splitFirst[0]).GetAddressBytes();
            byte[] secondByteArray = IPAddress.Parse(splitSecond[0]).GetAddressBytes();

            int.TryParse(splitFirst[1], out int firstPrefixLength);
            int.TryParse(splitSecond[1], out int secondPrefixLength);

            int minPrefix = (firstPrefixLength > secondPrefixLength) ? secondPrefixLength : firstPrefixLength;

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(firstByteArray);
                Array.Reverse(secondByteArray);
                BitArray firstBitArray = new BitArray(firstByteArray);
                BitArray secondBitArray = new BitArray(secondByteArray);

                for (int i = firstBitArray.Count - 1; i >= firstBitArray.Count - minPrefix; i--)
                {
                    if (firstBitArray.Get(i) != secondBitArray.Get(i))
                        return false;
                }
            }
            else
            {
                BitArray firstBitArray = new BitArray(firstByteArray);
                BitArray secondBitArray = new BitArray(secondByteArray);

                for (int i = 0; i < minPrefix; i++)
                {
                    if (firstBitArray.Get(i) != secondBitArray.Get(i))
                        return false;
                }
            }

            return true;
        }

        public static string GetAddresssRagngeFormCidrNotation(string cidrIpAddress)
        {
            if (!ValidateGivenCidrAddressNotation(cidrIpAddress))
                return "BAD RANGE!";

            var splitAdress = cidrIpAddress.Split('/');
            IPAddress ip = IPAddress.Parse(splitAdress[0]);
            var cidrIpBytes = ip.GetAddressBytes();
            int.TryParse(splitAdress[1], out int prefixLength);



            if (BitConverter.IsLittleEndian)
            {
                byte[] convertedIpAddress = new byte[4];
                Array.Reverse(cidrIpBytes);
                BitArray originalCidrBits = new BitArray(cidrIpBytes);
                BitArray reversedIpBits = new BitArray(cidrIpBytes);
                reversedIpBits.Xor(new BitArray(new byte[] { 255, 255, 255, 255 }));

                for (int i = 0; i < reversedIpBits.Length - prefixLength; i++)
                {
                    originalCidrBits.Set(i, reversedIpBits.Get(i));
                }

                originalCidrBits.CopyTo(convertedIpAddress, 0);
                Array.Reverse(convertedIpAddress);
                return new IPAddress(convertedIpAddress).ToString();
            }
            else
            {
                byte[] convertedIpAddress = new byte[4];
                BitArray originalCidrBits = new BitArray(cidrIpBytes);
                BitArray reversedIpBits = new BitArray(cidrIpBytes);
                reversedIpBits.Xor(new BitArray(new byte[] { 255, 255, 255, 255 }));

                for (int i = prefixLength; i < reversedIpBits.Length; i++)
                {
                    originalCidrBits.Set(i, reversedIpBits.Get(i));
                }

                originalCidrBits.CopyTo(convertedIpAddress, 0);
                return new IPAddress(convertedIpAddress).ToString();
            }

        }

        public static bool ValidateGivenCidrAddressNotation(string cidrIpAddress)
        {
            var splitAddress = cidrIpAddress.Split('/');

            // pattern taken from stackoverflow https://stackoverflow.com/questions/19576622/java-regex-find-characters-in-any-string
            var rx = new Regex("^(([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.){3}([01]?\\d\\d?|2[0-4]\\d|25[0-5])$");

            if (splitAddress.Length != 2)
                return false;

            if (!int.TryParse(splitAddress[1], out int prefixSize))
                return false;

            if (prefixSize < 1 || prefixSize > 32)
                return false;

            MatchCollection matches = rx.Matches(splitAddress[0]);
            if (matches.Count == 0)
                return false;


            byte[] ipAddressBytes = IPAddress.Parse(splitAddress[0].Trim()).GetAddressBytes();

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(ipAddressBytes);
                var ipAddressBitArray = new BitArray(ipAddressBytes);

                for (int i = 0; i < ipAddressBitArray.Length - prefixSize; i++)
                {
                    if (ipAddressBitArray.Get(i))
                    {
                        return false;
                    }
                }
            }
            else
            {
                var ipAddressBitArray = new BitArray(ipAddressBytes);

                for (int i = prefixSize; i < ipAddressBitArray.Length; i++)
                {
                    if (ipAddressBitArray.Get(i))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
