using System;
using System.Collections.Generic;
using System.Linq;

namespace FSImageGenerator.Classes.System {
    class SystemArea : IPart {
        public IEnumerable<Byte> GetBytes() {
            var a = this.BootSector.GetBytes()
                .Concat(Enumerable.Range(0, this.NumberOfFATs).SelectMany(_ => this.Fat.GetBytes()))
                .Concat(this.RootDirectory.GetBytes())
                .ToArray();

            return this.BootSector.GetBytes()
                .Concat(Enumerable.Range(0, this.NumberOfFATs).SelectMany(_ => this.Fat.GetBytes()))
                .Concat(this.RootDirectory.GetBytes());
        }

        public BootSector BootSector { get; private set; } = new BootSector();

        public Byte NumberOfFATs { get; set; }

        public FAT Fat { get; private set; } = new FAT();

        public Directory RootDirectory { get; private set; } = new Directory();
    }
}
