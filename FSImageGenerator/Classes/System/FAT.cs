using System;
using System.Collections.Generic;
using System.Linq;

namespace FSImageGenerator.Classes.System {
    class FAT : IPart {
        private UInt16 mSectorsPerFat;
        private UInt16[] mTable = new UInt16[] { };

        public FAT() {
            this.SectorsPerFat = 1;
        }

        public IEnumerable<Byte> GetBytes() {
            return this.mTable.SelectMany(x => BitConverter.GetBytes(x));
        }

        /// <summary>
        /// Число кластеров, которые может обслуживать FAT.
        /// </summary>
        public UInt16 ClustersPerFat => Convert.ToUInt16((this.mSectorsPerFat * 512u) / 2u);

        /// <summary>
        /// Число секторов, используемых для хранения копии FAT, из загрузочного сектора.
        /// </summary>
        public UInt16 SectorsPerFat {
            get => this.mSectorsPerFat;
            set {
                this.mSectorsPerFat = value;

                if (this.mTable.Length != this.ClustersPerFat) {
                    this.mTable = new UInt16[this.ClustersPerFat];
                    this.mTable[0] = 0xfff8;
                    this.mTable[1] = 0xffff;
                }
            }
        }

        public UInt16 FindEmptyCluster() {
            for (UInt16 i = 0; i < this.mTable.Length; i++) {
                if (this.mTable[i] == 0x0000) {
                    return Convert.ToUInt16(i);
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

        public void MarkCluster(UInt16 index, ClusterState state) {
            this.SetCluster(index, Convert.ToUInt16(state));
        }

        public void SetCluster(UInt16 index, UInt16 nextIndexOrState) {
            if (index <= 1 || index >= this.ClustersPerFat) {
                throw new IndexOutOfRangeException($"Указан недопустимый индекс кластера: {index}");
            }

            if (nextIndexOrState == 0x0001) {
                throw new IndexOutOfRangeException($"Указано недопустимое содержимое кластера: {nextIndexOrState}");
            }

            this.mTable[index] = nextIndexOrState;
        }
    }
}
