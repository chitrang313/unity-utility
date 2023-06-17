using System.IO.Compression;
using System.IO;
using System.Text;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

namespace ViitorCloud.Soreal
{
    public class StringUtility : MonoBehaviour
    {
        public static byte[] CompressString(string inputString) {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);

            using (MemoryStream outputStream = new MemoryStream()) {
                using (GZipStream gzipStream = new GZipStream(outputStream, CompressionMode.Compress)) {
                    gzipStream.Write(inputBytes, 0, inputBytes.Length);
                }
                return outputStream.ToArray();
            }
        }

        public static string DecompressString(byte[] compressedBytes) {
            using (MemoryStream inputStream = new MemoryStream(compressedBytes)) {
                using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress)) {
                    using (StreamReader streamReader = new StreamReader(gzipStream, Encoding.UTF8)) {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
        public static bool IsValidFileNameForDate(string value) {
            DateTime dateTime;
            bool isValidFileName = false;
            try {
                if (value.Length > 15) {
                    return isValidFileName;
                }
                isValidFileName = DateTime.TryParseExact(value.Substring(0, 15), "ddMMyyyy_HHmmss", null, System.Globalization.DateTimeStyles.None, out dateTime);
            } catch (FormatException ex) {
                Debug.LogError("ERROR: parsing date: " + ex.Message);
            }
            return isValidFileName;
        }
        public static string GetReadableFileName(string value) {
            DateTime dateTime = DateTime.ParseExact(value, "ddMMyyyy_HHmmss", null);
            string readableDate = dateTime.ToString("MMMM-dd-yyyy h-mm-ss tt");
            return readableDate;
        }
    }//StringCompression class end.
}
