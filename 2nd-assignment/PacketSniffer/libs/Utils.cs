/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/25</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using System;
using System.Collections.Generic;
using System.Linq;

namespace PacketSniffer
{
    /// <summary>
    /// Utils class containing useful convertors and snippets that can be used
    /// throughout the code base.
    /// </summary>
    public static class Utils
    {
        public static string ConvertTimeToRfc3339(DateTime time)
        {
            return time.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ssZ");
        }

        /// <summary>
        /// Convert byte array to string, place a space in between the bytes.
        /// </summary>
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

        /// <summary>
        /// Convert number to 4 character long hex string prepended by '0x'.
        /// </summary>
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

        /// <summary>
        /// Important functoin for parsing the packet byte data.
        /// Splits the bytes into chunks of 16 and prints them sequentially
        /// as hexa code and ascii code.
        ///
        /// Uses other functoins defined in Utils.
        /// </summary>
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

        /// <summary>
        /// Find out if character is printable
        /// </summary>
        public static bool IsPrintable(char character)
        {
            return (Char.IsLetterOrDigit(character) || Char.IsPunctuation(character) ||
                    Char.IsSymbol(character) || (character == ' ')) && character != '?';
        }

        /// <summary>
        /// Function for splitting byte array to chunks of bytes.
        /// </summary>
        /// <see>
        /// https://www.codeproject.com/Questions/684370/i-want-to-read-1-kb-data-each-time-from-byte-array
        /// https://www.codeproject.com/Answers/684747/i-want-to-read-1-kb-data-each-time-from-byte-array#answer2
        /// <author>Matt T Heffron</author>
        /// </see>
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
