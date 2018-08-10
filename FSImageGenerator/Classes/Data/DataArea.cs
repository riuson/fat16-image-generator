using System;
using System.Collections.Generic;
using System.Linq;

namespace FSImageGenerator.Classes.Data {
    class DataArea : IPart {
        private readonly Cluster[] mClusters;

        public DataArea(UInt16 clustersCount, Byte sectorsPerCluster, UInt16 bytesperSector) {
            this.mClusters = Enumerable.Range(0, clustersCount)
                .Select(_ => new Cluster(sectorsPerCluster, bytesperSector))
                .ToArray();
        }

        public IEnumerable<Byte> GetBytes() => this.mClusters.SelectMany(x => x.GetBytes());

        public UInt16 ClustersCount => Convert.ToUInt16(this.mClusters.Length);

        public Cluster this[Int32 index] => this.mClusters[index];
    }
}
