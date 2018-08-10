using System;
using System.Collections.Generic;
using System.Linq;

namespace FSImageGenerator.Classes.Data {
    class Cluster : IPart {
        private List<Sector> mSectors = new List<Sector>();

        public IEnumerable<Byte> GetBytes() {
            return this.mSectors.SelectMany(x => x.GetBytes());
        }

        public Byte SectorsPerCluster {
            get { return Convert.ToByte(this.mSectors.Count); }
            set {
                if (this.mSectors.Count != value) {
                    this.mSectors.Clear();
                    this.mSectors.AddRange(Enumerable.Range(0, value).Select(_ => new Sector()));
                }
            }
        }

        public Sector this[Int32 index] {
            get => this.mSectors[index];
            set => this.mSectors[index] = value;
        }

        public void SetContent(IEnumerable<Byte> clusterContent) {
            var availableSpace = this.mSectors.Sum(x => x.BytesPerSector);

            if (clusterContent.Count() > availableSpace) {
                throw new ArgumentOutOfRangeException("Размер массива данных превышает размер кластера!");
            }

            using (var enumerator = clusterContent.GetEnumerator()) {
                foreach (var sector in this.mSectors) {
                    for (var i = 0; i < sector.BytesPerSector; i++) {
                        if (!enumerator.MoveNext()) {
                            return;
                        }

                        sector[i] = enumerator.Current;
                    }
                }
            }
        }
    }
}
