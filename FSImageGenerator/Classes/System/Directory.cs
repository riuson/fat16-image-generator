using System;
using System.Collections.Generic;
using System.Linq;

namespace FSImageGenerator.Classes.System {
    class Directory : IPart {
        private List<DirectoryRecord> mRecords;
        public Directory() {
            this.mRecords = new List<DirectoryRecord>();
        }
        public IEnumerable<Byte> GetBytes() {
            return this.mRecords.SelectMany(x => x.GetBytes());
        }

        public UInt16 EntriesCount {
            get => Convert.ToUInt16(this.mRecords.Count);
            set {
                if (this.mRecords.Count != value) {
                    this.mRecords.Clear();
                    this.mRecords.AddRange(Enumerable.Range(0, value).Select(_ => new DirectoryRecord()));
                }
            }
        }

        public DirectoryRecord this[Int32 index] {
            get => this.mRecords[index];
            set => this.mRecords[index] = value;
        }

        public void AddRecord(DirectoryRecord directoryRecord) {
            var index = this.mRecords.FindIndex(x => x.EntryAttributes == DirectoryRecord.Attributes.None);

            if (index >= 0) {
                this[index] = directoryRecord;
                return;
            }

            throw new IndexOutOfRangeException("Не найдено место под новую запись в каталоге!");
        }
    }
}
