using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Snapshot
{
    internal class SnapshotHashProvider : HMACSHA1
    {
        public byte[] GetFinalizedHashCode()
        {
            Initialize();
            return HashFinal();
        }
    }
}
