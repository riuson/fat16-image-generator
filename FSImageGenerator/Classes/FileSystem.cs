using FSImageGenerator.Classes.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FSImageGenerator.Classes {
    class FileSystem : IPart {
        public FileSystem(UInt16 bytesPerSector,
            Byte sectorsPerCluster,
            UInt16 reservedSectors,
            Byte numberOfFATs,
            Byte sectorsPerRootDirectory,
            UInt32 totalSectors,
            UInt16 sectorsPerFat,
            UInt32 hiddenSectors,
            UInt32 volumeSerialNumber,
            String volumeLabel) {
            this.SystemArea = new SystemArea(
                bytesPerSector: bytesPerSector,
                sectorsPerCluster: sectorsPerCluster,
                reservedSectors: reservedSectors,
                numberOfFATs: numberOfFATs,
                sectorsPerRootDirectory: sectorsPerRootDirectory,
                totalSectors: totalSectors,
                sectorsPerFat: sectorsPerFat,
                hiddenSectors: hiddenSectors,
                volumeSerialNumber: volumeSerialNumber,
                volumeLabel: volumeLabel);

            Int64 dataClustersCount = totalSectors;
            dataClustersCount -= reservedSectors;
            dataClustersCount -= numberOfFATs * sectorsPerFat;
            dataClustersCount -= (this.SystemArea.BootSector.RootEntries * DirectoryRecord.RecordSize / bytesPerSector);
            dataClustersCount = dataClustersCount / sectorsPerCluster;

            this.DataArea = new Data.DataArea(
                clustersCount: Convert.ToUInt16(dataClustersCount),
                sectorsPerCluster: sectorsPerCluster,
                bytesperSector: bytesPerSector);
        }

        public IEnumerable<Byte> GetBytes() {
            return this.SystemArea.GetBytes()
                .Concat(this.DataArea.GetBytes());
        }

        public System.SystemArea SystemArea { get; private set; }

        public Data.DataArea DataArea { get; private set; }

        public void AddFile(String filepath) {
            if (File.Exists(filepath)) {
                var directoryRecord = this.SystemArea.RootDirectory.FirstOrDefault(x => x.IsEmpty);

                if (directoryRecord == null) {
                    return;
                }

                var filename = Path.GetFileName(filepath);
                var content = File.ReadAllBytes(filepath);
                var bytesPerCluster = this.SystemArea.BootSector.BytesPerSector * this.SystemArea.BootSector.SectorsPerCluster;
                var clustersCount = Math.DivRem(content.Length, bytesPerCluster, out Int32 rem);

                if (rem > 0) {
                    clustersCount++;
                }

                var contentOfClusters = Enumerable.Range(0, clustersCount)
                    .Select(i => i * bytesPerCluster)
                    .Select(start => content.Skip(start).Take(bytesPerCluster));

                var clusterIndex = this.SystemArea.Fat.FindEmptyCluster();

                directoryRecord.Access = DateTime.Now;
                directoryRecord.Creation = DateTime.Now;
                directoryRecord.Write = DateTime.Now;
                directoryRecord.EntryAttributes = DirectoryRecord.Attributes.Archive;
                directoryRecord.FileSize = Convert.ToUInt32(content.Length);
                directoryRecord.Name = filename;
                directoryRecord.StartingCluster = clusterIndex;

                foreach (var contentOfCluster in contentOfClusters) {
                    this.SystemArea.Fat.SetCluster(clusterIndex, FAT.ClusterState.LastInChain);
                    this.DataArea[clusterIndex - 2].SetBytes(contentOfCluster);

                    if (contentOfCluster == contentOfClusters.Last()) {
                        break;
                    }

                    var nextClusterIndex = this.SystemArea.Fat.FindEmptyCluster();

                    this.SystemArea.Fat.SetCluster(clusterIndex, nextClusterIndex);
                    clusterIndex = nextClusterIndex;
                }
            }
        }
    }
}
