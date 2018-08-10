using System;
using System.Collections.Generic;
using System.Linq;

namespace FSImageGenerator.Classes.System {
    class FAT : IPart {
        private readonly UInt16 mSectorsPerFat;
        private readonly UInt16 mClustersPerFat;
        private readonly UInt16[] mTable = new UInt16[] { };

        public FAT(UInt16 sectorsPerFat, UInt16 bytesPerSector) {
            this.mSectorsPerFat = sectorsPerFat;
            this.mClustersPerFat = Convert.ToUInt16((this.mSectorsPerFat * bytesPerSector) / sizeof(UInt16));
            this.mTable = new UInt16[this.mClustersPerFat];

            //for (var i = 2; i < this.mClustersPerFat; i++) {
            //    this.mTable[i] = Convert.ToUInt16(ClusterState.Free);
            //}
            this.mTable[0] = 0xfff8;
            this.mTable[1] = 0xffff;
        }

        public IEnumerable<Byte> GetBytes() => this.mTable.SelectMany(x => BitConverter.GetBytes(x));

        public UInt16 FindEmptyCluster() {
            for (UInt16 i = 0; i < this.mTable.Length; i++) {
                if (this.mTable[i] == Convert.ToUInt16(ClusterState.Free)) {
                    return i;
                }
            }

            throw new IndexOutOfRangeException("Не найден пустой кластер под данные.");
        }

        public enum ClusterState {
            Free = 0x0000,
            Reserved = 0xfff0,
            Defect = 0xfff7,
            LastInChain = 0xfff8
        }

        public void SetCluster(UInt16 index, ClusterState state) {
            this.SetCluster(index, Convert.ToUInt16(state));
        }

        public void SetCluster(UInt16 index, UInt16 nextIndex) {
            if (index <= 1 || index >= this.mClustersPerFat) {
                throw new IndexOutOfRangeException($"Указан недопустимый индекс кластера: {index}");
            }

            if (nextIndex == 0x0001) {
                throw new IndexOutOfRangeException($"Указано недопустимое содержимое кластера: {nextIndex}");
            }

            this.mTable[index] = nextIndex;
        }
    }
}
