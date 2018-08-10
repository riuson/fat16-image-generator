using System;
using System.Collections.Generic;
using System.Linq;

namespace FSImageGenerator.Classes.Data {
    class Cluster : IPart {
        private readonly Sector[] mSectors;

        public Cluster(Byte sectorsPerCluster, UInt16 bytesPerSector)
        {
            this.mSectors = Enumerable.Range(0, sectorsPerCluster)
                .Select(_ => new Sector(bytesPerSector))
                .ToArray();
        }

        public IEnumerable<Byte> GetBytes() => this.mSectors.SelectMany(x => x.GetBytes());

        public Byte SectorsPerCluster => Convert.ToByte(this.mSectors.Length);

        public Sector this[Int32 index] {
            get => this.mSectors[index];
        }

        public void SetBytes(IEnumerable<Byte> value) {
            var availableSpace = this.mSectors.Sum(x => x.BytesPerSector);

            if (value.Count() > availableSpace) {
                throw new ArgumentOutOfRangeException("Размер массива данных превышает размер кластера!");
            }

            using (var enumerator = value.GetEnumerator()) {
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
