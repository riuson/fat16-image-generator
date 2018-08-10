using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FSImageGenerator.Classes.System {
    class Directory : IPart, IEnumerable<DirectoryRecord> {
        private readonly DirectoryRecord[] mRecords;

        public Directory(UInt16 entriesCount) {
            this.mRecords = Enumerable.Range(0, entriesCount)
                .Select(_ => new DirectoryRecord())
                .ToArray();
        }

        public IEnumerable<Byte> GetBytes() => this.mRecords.SelectMany(x => x.GetBytes());

        public IEnumerator<DirectoryRecord> GetEnumerator() {
            return this.mRecords.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.mRecords.GetEnumerator();

        public DirectoryRecord this[Int32 index] {
            get => this.mRecords[index];
        }
    }
}
