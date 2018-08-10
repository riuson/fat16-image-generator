using System;
using System.Collections.Generic;
using System.Linq;

namespace FSImageGenerator.Classes.Helpers {
    static class IEnumerableExtension {
        public static void CopyTo(this IEnumerable<Byte> source, Byte[] destinationArray, UInt32 destinationOffset) {
            for (var i = 0; i < source.Count(); i++) {
                destinationArray[destinationOffset + i] = source.ElementAt(i);
            }
        }
    }
}
