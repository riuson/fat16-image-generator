using System;
using System.Collections.Generic;

namespace FSImageGenerator.Classes.Data {
    class Sector : IPart {
        private readonly Byte[] mBytes;

        public Sector(UInt16 bytesPerSector) {
            this.mBytes = new Byte[bytesPerSector];
        }

        public IEnumerable<Byte> GetBytes() => this.mBytes;

        public UInt16 BytesPerSector => Convert.ToUInt16(this.mBytes.Length);

        public Byte this[Int32 index] {
            get => this.mBytes[index];
            set => this.mBytes[index] = value;
        }
    }
}
