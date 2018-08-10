using FSImageGenerator.Classes.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FSImageGenerator.Classes {
    class FileSystem : IPart {
        public IEnumerable<Byte> GetBytes() {
            return this.SystemArea.GetBytes()
                .Concat(this.DataArea.GetBytes());
        }

        public System.SystemArea SystemArea { get; private set; } = new System.SystemArea();

        public Data.DataArea DataArea { get; private set; } = new Data.DataArea();

        public void AddFile(String filepath) {
            if (File.Exists(filepath)) {
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

                var directoryRecord = new DirectoryRecord {
                    Access = DateTime.Now,
                    Creation = DateTime.Now,
                    Write = DateTime.Now,
                    EntryAttributes = DirectoryRecord.Attributes.Archive,
                    FileSize = Convert.ToUInt32(content.Length),
                    Name = filename,
                    StartingCluster = clusterIndex
                };

                this.SystemArea.RootDirectory.AddRecord(directoryRecord);

                foreach (var contentOfCluster in contentOfClusters) {
                    this.SystemArea.Fat.MarkCluster(clusterIndex, FAT.ClusterState.LastInChain);
                    this.DataArea[clusterIndex - 2].SetContent(contentOfCluster);

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
