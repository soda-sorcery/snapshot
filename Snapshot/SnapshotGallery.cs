using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Snapshot
{
    internal class SnapshotGallery
    {

        internal SnapshotGallery Init<T>(Snapshot<T> snapshot, int timer) where T : ISnapshot
        {
            snapshots.Add(snapshot);

            Timer = new Timer(timer).Init<T>(snapshot);
            return this;
        }

        internal void AddSnapshot<T>(Snapshot<T> snapshot) where T : ISnapshot
        {
            snapshots.Add(snapshot);
        }

        internal List<ISnapshot> snapshots { get; private set; } = new List<ISnapshot>();

        internal Timer Timer;
    }

    
}
