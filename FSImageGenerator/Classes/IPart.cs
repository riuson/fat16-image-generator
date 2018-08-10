using System;
using System.Collections.Generic;

namespace FSImageGenerator.Classes {
    interface IPart {
        IEnumerable<Byte> GetBytes();
    }
}
