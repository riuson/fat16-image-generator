using FSImageGenerator.Classes;
using FSImageGenerator.Classes.System;
using System;
using System.IO;
using System.Linq;

namespace FSImageGenerator {
    class Program {
        static void Main(System.String[] args) {
            var fs = new FileSystem(
                bytesPerSector: 512,
                sectorsPerCluster: 1,
                reservedSectors: 1,
                numberOfFATs: 2,
                sectorsPerRootDirectory: 1,
                totalSectors: 16 * 1024 * 1024 / 512,
                sectorsPerFat: 128,
                hiddenSectors: 0,
                volumeSerialNumber: Convert.ToUInt32(DateTime.Now.Subtract(new DateTime(2000, 1, 1)).TotalSeconds),
                volumeLabel: "MYDEVICE");

            fs.AddFile(@"r:\DOCUMENT.PDF");

            using (var stream = new FileStream(@"R:\image-fat16.img", FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) {
                var array = fs.GetBytes().ToArray();
                stream.Write(array, 0, array.Length);
            }
        }
    }
}
