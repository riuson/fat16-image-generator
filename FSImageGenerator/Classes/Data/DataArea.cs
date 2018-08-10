using System;
using System.Collections.Generic;
using System.Linq;

namespace FSImageGenerator.Classes.Data {
    class DataArea : IPart {
        private List<Cluster> mClusters = new List<Cluster>();

        public IEnumerable<Byte> GetBytes() {
            return this.mClusters.SelectMany(x => x.GetBytes());
        }

        public UInt16 ClustersCount {
            get => Convert.ToUInt16(this.mClusters.Count);
            set {
                if (this.mClusters.Count != value) {
                    this.mClusters.Clear();
                    this.mClusters.AddRange(Enumerable.Range(0, value).Select(_ => new Cluster()));
                }
            }
        }

        public Cluster this[Int32 index] => this.mClusters[index];
    }
}
