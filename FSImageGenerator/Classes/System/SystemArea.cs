using System;
using System.Collections.Generic;
using System.Linq;

namespace FSImageGenerator.Classes.System {
    class SystemArea : IPart {
        public SystemArea(
            UInt16 bytesPerSector,
            Byte sectorsPerCluster,
            UInt16 reservedSectors,
            Byte numberOfFATs,
            Byte sectorsPerRootDirectory,
            UInt32 totalSectors,
            UInt16 sectorsPerFat,
            UInt32 hiddenSectors,
            UInt32 volumeSerialNumber,
            String volumeLabel) {
            this.BootSector = new BootSector() {
                BytesPerSector = bytesPerSector,
                SectorsPerCluster = sectorsPerCluster,
                ReservedSectors = reservedSectors, // Сколько зарезервировано секторов под загрузочный сектор.
                NumberOfFATs = numberOfFATs,
                RootEntries = Convert.ToUInt16(bytesPerSector * sectorsPerRootDirectory / DirectoryRecord.RecordSize), // До 16 каталогов/файлов в корне.
                TotalSectors = totalSectors,
                SectorsPerFat = sectorsPerFat,
                HiddenSectors = hiddenSectors,
                VolumeSerialNumber = volumeSerialNumber,// Convert.ToUInt32(DateTime.Now.Subtract(new DateTime(2000, 1, 1)).TotalSeconds),
                VolumeLabel = volumeLabel
            };

            this.Fat = new FAT(sectorsPerFat, bytesPerSector);
            this.RootDirectory = new Directory(this.BootSector.RootEntries);
        }

        public IEnumerable<Byte> GetBytes() {
            var a = this.BootSector.GetBytes()
                .Concat(Enumerable.Range(0, this.BootSector.NumberOfFATs).SelectMany(_ => this.Fat.GetBytes()))
                .Concat(this.RootDirectory.GetBytes())
                .ToArray();

            return this.BootSector.GetBytes()
                .Concat(Enumerable.Range(0, this.BootSector.NumberOfFATs).SelectMany(_ => this.Fat.GetBytes()))
                .Concat(this.RootDirectory.GetBytes());
        }

        public BootSector BootSector { get; private set; }

        public FAT Fat { get; private set; }

        public Directory RootDirectory { get; private set; }
    }
}
