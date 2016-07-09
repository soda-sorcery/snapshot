using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Snapshot
{
    // this is a type relator. it keeps a snapshot collection of all snapshots taken of the 
    // same type.

    // var foo1 = new Foo().TakeTypeSnapshot<Foo>();
    // var foo2 = new Foo().TakePrivateSnapshot<Foo>();

    // even though foo2 didn't request a type collection, because it is of the same type as foo1
    // foo2 will automatically be in foo1's type collection, but not foo2's.

    internal class SnapshotTypeCollection
    {
        internal int Id { get; private set; }
        
        internal List<ISnapshot> snapshots = new List<ISnapshot>();

        internal void Add(ISnapshot snapshot)
        {
            AddToSnapshots(snapshot);
        }

        private void AddToSnapshots(ISnapshot snapshot)
        {
            snapshots.Add(snapshot);
        }

        internal List<ISnapshot> GetSnapshots()
        {
            
            return snapshots;
        }
    }
}
