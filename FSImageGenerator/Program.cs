using FSImageGenerator.Classes;
using FSImageGenerator.Classes.System;
using System;
using System.IO;
using System.Linq;

namespace FSImageGenerator {
    class Program {
        static void Main(System.String[] args) {
            var fs = new FileSystem();

            fs.SystemArea.BootSector.BytesPerSector = 512;
            fs.SystemArea.BootSector.SectorsPerCluster = 1;
            fs.SystemArea.BootSector.ReservedSectors = 1; // Зарезервирован 1 сектор под загрузочный сектор.
            fs.SystemArea.BootSector.NumberOfFATs = 2;
            fs.SystemArea.BootSector.RootEntries = 512 / 32; // До 16 каталогов/файлов в корне.
            fs.SystemArea.BootSector.TotalSectors = 16 * 1024 * 1024 / 512; // 16 МиБ диск по 512-байтным секторам.
            fs.SystemArea.BootSector.SectorsPerFat = 128;
            fs.SystemArea.BootSector.HiddenSectors = 0;
            fs.SystemArea.BootSector.VolumeSerialNumber = Convert.ToUInt32(DateTime.Now.Subtract(new DateTime(2000, 1, 1)).TotalSeconds);
            fs.SystemArea.BootSector.VolumeLabel = "MYDEVICE";

            fs.SystemArea.NumberOfFATs = 2;
            fs.SystemArea.Fat.SectorsPerFat = fs.SystemArea.BootSector.SectorsPerFat;

            for (UInt16 i = 2; i < fs.SystemArea.Fat.ClustersPerFat; i++) {
                fs.SystemArea.Fat.MarkCluster(i, Classes.System.FAT.ClusterState.Free);
            }

            fs.SystemArea.RootDirectory.EntriesCount = fs.SystemArea.BootSector.RootEntries;

            fs.DataArea.ClustersCount = Convert.ToUInt16((fs.SystemArea.BootSector.TotalSectors - fs.SystemArea.BootSector.ReservedSectors - fs.SystemArea.BootSector.NumberOfFATs * fs.SystemArea.BootSector.SectorsPerFat - (fs.SystemArea.BootSector.RootEntries * DirectoryRecord.RecordSize / fs.SystemArea.BootSector.BytesPerSector)) / fs.SystemArea.BootSector.SectorsPerCluster);

            for (var clusterIndex = 0; clusterIndex < fs.DataArea.ClustersCount; clusterIndex++) {
                fs.DataArea[clusterIndex].SectorsPerCluster = 1;

                for (var sectorIndex = 0; sectorIndex < fs.DataArea[clusterIndex].SectorsPerCluster; sectorIndex++) {
                    fs.DataArea[clusterIndex][sectorIndex].BytesPerSector = fs.SystemArea.BootSector.BytesPerSector;
                }
            }

            fs.AddFile(@"r:\DOCUMENT.PDF");

            using (var stream = new FileStream(@"R:\image-fat16.img", FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) {
                var array = fs.GetBytes().ToArray();
                stream.Write(array, 0, array.Length);
            }
        }
    }
}
