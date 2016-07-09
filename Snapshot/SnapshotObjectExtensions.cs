using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapshot
{
    public static class SnapshotObjectExtensions
    {


        public static T TakeSnapshot<T>(this object obj) where T : ISnapshot
        {
            var s = (T)obj;
            Camera.CreateSnapShot(s);
            return s;
        }

        public static T TakePrivateSnapshot<T>(this object obj) where T : ISnapshot
        {
            var s = (T)obj;
            Camera.CreateSnapShot(s, excludeSnapshotTypeCollection: true);
            return s;
        }

        public static T TakeTypeSnapshot<T>(this object obj) where T : ISnapshot
        {
            var s = (T)obj;
            var snapshot = Camera.CreateSnapShot(s);
            Camera.AddToSnapshotTypeCollection(s, snapshot);
            return s;
        }
    }
}
