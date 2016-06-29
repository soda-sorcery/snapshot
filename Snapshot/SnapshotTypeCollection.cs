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

    // var foo1 = new Foo().TakeTypeInstanceSnapshot<Foo>();
    // var foo2 = new Foo().TakePrivateSnapshot<Foo>();

    // even though foo2 didn't request a type collection, because it is of the same type as foo1
    // foo2 will automatically be in foo1's type collection, but not foo2's.

    internal class SnapshotTypeCollection
    {
        internal int Id { get; private set; }
        internal List<ISnapshot> snapshots { get; set; } = new List<ISnapshot>();
        

        internal void Add(ISnapshot snapshot)
        {
            //var t = Task.Run(() => snapshots.Add(snapshot));

            //snapshotPromises.Add(t);  
            AddToSnapshots(snapshot);
        }

        private void AddToSnapshots(ISnapshot snapshot)
        {
            snapshots.Add(snapshot);
        }

        //private async Task AddToSnapshots(ISnapshot snapshot)
        //{
        //    await Task.Run(() => snapshots.Add(snapshot));
        //}
    }
}
