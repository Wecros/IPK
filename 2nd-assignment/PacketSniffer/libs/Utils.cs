using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PacketSniffer
{
    public static class Utils
    {
        public static string ConvertTimeToRfc3339(DateTime time)
        {
            return time.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ssZ");
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            var hex = BitConverter.ToString(bytes);
            hex = hex.Replace("-", " ").ToLower();
            if (hex.Length > 24)
            {
                hex = hex.Insert(23, " ");
            }
            return hex;
        }

        public static string IntToHex(int num)
        {
            string hex = "0x" + num.ToString("X4");
            return hex.ToLower();
        }

        public static char[] ByteArrayToAscii(byte[] bytes)
        {
            string outputString = System.Text.Encoding.ASCII.GetString(bytes);
            if (outputString.Length > 8)
            {
                 outputString = outputString.Insert(8, " ");
            }
            char[] output = outputString.ToCharArray();
            return output;
        }

        public static string ConvertPacketDataToOutputFormat(byte[] packetData)
        {
            const int ChunkSize = 16;
            int dataOffset = 0;
            string output = "";

            //byte[][] dataChunks = SplitBytesToByteChunks(packetData, ChunkSize);

            foreach (var dataChunk in packetData.Chunkify(ChunkSize))
            {
                string offsetString = IntToHex(dataOffset);
                string hexaByteString = ByteArrayToString(dataChunk.ToArray());
                char[] hexaAsciiCharacters = ByteArrayToAscii(dataChunk.ToArray());
                string hexaAsciiString = "";
                foreach (char character in hexaAsciiCharacters)
                {
                    if (IsPrintable(character))
                    {
                        hexaAsciiString += character;
                    }
                    else
                    {
                        hexaAsciiString += '.';
                    }
                }
                output += String.Format("{0,-6}: {1,-48} {2,-16}\n",
                                        offsetString, hexaByteString, hexaAsciiString);

                dataOffset += ChunkSize;
            }

            return output;
        }

        public static bool IsPrintable(char character)
        {
            return (Char.IsLetterOrDigit(character) || Char.IsPunctuation(character) ||
                    Char.IsSymbol(character) || (character == ' ')) && character != '?';
        }

        // https://www.codeproject.com/Questions/684370/i-want-to-read-1-kb-data-each-time-from-byte-array
        public static IEnumerable<IEnumerable<T>> Chunkify<T>(this IEnumerable<T> source, int chunkSize)
        {
            while (source.Any())
            {
                yield return source.Take(chunkSize);
                source = source.Skip(chunkSize);
            }
        }
    }
}
