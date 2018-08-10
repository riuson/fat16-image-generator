using System;
using System.Collections.Generic;

namespace FSImageGenerator.Classes.Data {
    class Sector : IPart {
        private Byte[] mBytes = new Byte[0] { };
        public IEnumerable<Byte> GetBytes() {
            return this.mBytes;
        }

        public UInt16 BytesPerSector {
            get { return Convert.ToUInt16(this.mBytes.Length); }
            set {
                if (this.mBytes.Length != value) {
                    this.mBytes = new Byte[value];
                }
            }
        }

        public Byte this[Int32 index] {
            get { return this.mBytes[index]; }
            set { this.mBytes[index] = value; }
        }
    }
}
