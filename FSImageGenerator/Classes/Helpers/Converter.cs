using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSImageGenerator.Classes.Helpers {
    static class Converter {
        public static IEnumerable<Byte> GetBytes(UInt16 value, Int32 length = -1) {
            var bytes = BitConverter.GetBytes(value);

            if (length >= 0) {
                return bytes.Take(length);
            }

            return bytes;
        }
        public static IEnumerable<Byte> GetBytes(UInt32 value, Int32 length = -1) {
            var bytes = BitConverter.GetBytes(value);

            if (length >= 0) {
                return bytes.Take(length);
            }

            return bytes;
        }
        public static IEnumerable<Byte> GetBytes(String value, Int32 length) {
            var bytes = Encoding.ASCII.GetBytes(value.Trim().PadRight(100, ' ').Substring(0, length));
            return bytes;
        }
        public static IEnumerable<Byte> GetBytesOfDate(DateTime value) {
            if (value.Year < 1980 || value.Year > 2107) {
                throw new ArgumentOutOfRangeException($"Поддерживаются даты только в диапазоне 1980...2107 гг., но задана: {value}");
            }

            UInt16 result = 0;
            result = Convert.ToUInt16(value.Day);
            result = Convert.ToUInt16(result | (value.Month << 5));
            var years = Convert.ToUInt16(value.Year - 1980);
            result = Convert.ToUInt16(result | (years << 9));
            return GetBytes(result);
        }
        public static IEnumerable<Byte> GetBytesOfTime(DateTime value) {
            UInt16 result = 0;
            result = Convert.ToUInt16(value.Second / 2);
            result = Convert.ToUInt16(result | (value.Minute << 5));
            result = Convert.ToUInt16(result | (value.Hour << 11));
            return GetBytes(result);
        }
    }
}
