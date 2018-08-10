using FSImageGenerator.Classes.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FSImageGenerator.Classes.System {
    public class DirectoryRecord : IPart {
        public IEnumerable<Byte> GetBytes() {
            var result = new Byte[32];

            if (this.EntryAttributes == Attributes.None) {
                return result;
            }

            var name = this.Name.ToUpper();

            if ((this.EntryAttributes & Attributes.Directory) == Attributes.None) {
                var notallowed = Path.GetInvalidFileNameChars();

                var filename = Path.GetFileNameWithoutExtension(name);
                filename = new String(filename.Where(c => !notallowed.Contains(c)).ToArray());
                Converter.GetBytes(filename, 8).CopyTo(result, 0x00);

                var extension = Path.GetExtension(name);
                extension = new String(extension.Where(c => !notallowed.Contains(c)).ToArray());
                extension = extension.TrimStart('.');
                Converter.GetBytes(extension, 3).CopyTo(result, 0x08);
            } else {
                var notallowed = Path.GetInvalidPathChars();

                var filename = Path.GetFileNameWithoutExtension(name);
                filename = new String(filename.Where(c => !notallowed.Contains(c)).ToArray());
                Converter.GetBytes(filename, 8).CopyTo(result, 0x00);

                var extension = "   ";
                extension = new String(extension.Where(c => !notallowed.Contains(c)).ToArray());
                extension = extension.TrimStart('.');
                Converter.GetBytes(extension, 3).CopyTo(result, 0x08);
            }

            result[0x0b] = Convert.ToByte(this.EntryAttributes);
            result[0x0c] = 0;
            result[0x0d] = Convert.ToByte(this.Creation.Millisecond / 10);
            Converter.GetBytesOfTime(this.Creation).CopyTo(result, 0x0e);
            Converter.GetBytesOfDate(this.Creation).CopyTo(result, 0x10);
            Converter.GetBytesOfDate(this.Access).CopyTo(result, 0x12);
            Converter.GetBytes((UInt16)0).CopyTo(result, 0x14);
            Converter.GetBytesOfTime(this.Write).CopyTo(result, 0x16);
            Converter.GetBytesOfDate(this.Write).CopyTo(result, 0x18);
            Converter.GetBytes(this.StartingCluster).CopyTo(result, 0x1a);
            Converter.GetBytes(this.FileSize).CopyTo(result, 0x1c);

            return result;
        }

        public String Name { get; set; } = String.Empty;

        [Flags]
        public enum Attributes : Byte {
            None = 0x00,
            ReadOnly = 0x01,
            Hidden = 0x02,
            System = 0x04,
            VolumeName = 0x08,
            Directory = 0x10,
            Archive = 0x20
        }

        public Attributes EntryAttributes { get; set; } = Attributes.None;
        public DateTime Creation { get; set; } = DateTime.Now;
        public DateTime Access { get; set; } = DateTime.Now;
        public DateTime Write { get; set; } = DateTime.Now;
        public UInt16 StartingCluster { get; set; } = 0;
        public UInt32 FileSize { get; set; } = 0;

        public static UInt16 RecordSize => 32;
    }
}
