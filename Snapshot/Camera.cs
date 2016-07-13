using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


namespace Snapshot
{
    // this is the container for all snapshots in the app
    public sealed class Camera : ICamera
    {
        private static readonly Dictionary<int, List<ISnapshot>> _snapShots = new Dictionary<int, List<ISnapshot>>();
        private static readonly Dictionary<int, SnapshotTypeCollection> _typeCollectionHashMap = new Dictionary<int, SnapshotTypeCollection>();       

        // snapshot creation method -- 
        public static Snapshot<T> CreateSnapshot<T>(T obj, bool excludeSnapshotTypeCollection = false) where T : ISnapshot
        {
            // the object's hash code is used as the key because it shows reference equality
            var key = obj.GetHashCode();
            return AddSnapshotToCamera(obj, key, excludeSnapshotTypeCollection);
        }

        // snapshot creation method used when Iautosnapshot is implemented. this method is called from snapshot when a subscription
        // to a object is fired.
        internal static void CreateSnapshot<T>(T obj, int key, bool excludeSnapshotTypeCollection = false) where T : ISnapshot
        {            
            AddSnapshotToCamera(obj, key, excludeSnapshotTypeCollection);
        }

        // add a new snapshot to the camera roll (container)
        private static Snapshot<T> AddSnapshotToCamera<T>(T obj, int key, bool excludeSnapshotTypeCollection = false) where T : ISnapshot
        {
            var snapshot = new Snapshot<T>(obj, excludeSnapshotTypeCollection);
            AddToSnapshotHash(key, snapshot);

            // here a check is made to see if there are related types in the gallery, i.e., 
            // several instances of the same class
            if (!excludeSnapshotTypeCollection)
            {
                TryAddToSnapshotTypeCollection(snapshot);
            }
            return snapshot;
        }

        // returns the lastest snapshot added to the gallery
        public Snapshot<T> GetLatestSnapShot<T>(T obj) where T : ISnapshot
        {
            if (!IsValid(obj))
            {
                return null;
            }

            var snapshots =  GetAllSnapshots<T>(obj);
            var snapshot = snapshots.Last();
            return snapshot;
        }

        // gets all the snapshots associated with a specific reference
        public IList<Snapshot<T>> GetAllSnapshots<T>(T snapshot) where T : ISnapshot
        {           
            if (!IsValid(snapshot))
            {
                return null;
            }

            var key = GetKey(snapshot);

            // get the collection
            var snapshots = _snapShots[key];

            // cast snapshots
            var requestedSnapshots = FinalizeSnapshots<T>(snapshots);
            
            return requestedSnapshots;
        }

        // gets all snapshots of a type
        public IList<Snapshot<T>> GetSnapShotTypeCollection<T>() where T : ISnapshot
        {
            var snapshotTypeCollection = GetSnapshotTypeCollection(typeof(T));
            var snapshots = FinalizeSnapshots<T>(snapshotTypeCollection.GetSnapshots());
            return snapshots;
        }

        // gets the first snapshot taken of a reference
        public Snapshot<T> GetFirstSnapshot<T>(T obj) where T : ISnapshot
        {            
            if (!IsValid(obj))
            {
                return null;
            }

            var snapshots = GetAllSnapshots<T>(obj);
            var snapshot = snapshots.First();
            return snapshot;
        }

        // add a snapshot to the type collection. this is used when we snapshots are being taken for a type
        // and not a reference
        public static void AddToSnapshotTypeCollection<T>(T obj, Snapshot<T> snapshot) where T : ISnapshot
        {
            if (obj == null || snapshot == null)
            {
                return;
            }

            var key = obj.GetType().GetHashCode();

            if (_typeCollectionHashMap.ContainsKey(key))
            {
                _typeCollectionHashMap[key].Add(snapshot);
                return;
            }

            var typeCollection = new SnapshotTypeCollection();
            typeCollection.Add(snapshot);
            _typeCollectionHashMap.Add(key, typeCollection);
        }

        // deletes a collection of snapshots from the camera
        public bool DeleteSnapshots<T>(T obj) where T : ISnapshot
        {
            if (!IsValid(obj))
            {
                return false;
            }

            var key = GetKey(obj);

            _snapShots.Remove(key);
            return true;
        }

        

        private List<Snapshot<T>> ConvertToSnapshot<T>(List<ISnapshot> snapshotsToConvert) where T : ISnapshot
        {
            var snapshots = new List<Snapshot<T>>();
            foreach (var snap in snapshotsToConvert)
            {
                var developedSnapshot = (Snapshot<T>)snap;

                snapshots.Add(developedSnapshot);
            }
            return snapshots;
        }

        private List<Snapshot<T>> RemoveDuplicates<T>(List<Snapshot<T>> snapshots) where T : ISnapshot
        {
            var uniqueList = snapshots.GroupBy(r => r?.Id).Select(g => g.First()).ToList();
            return uniqueList;
        }

        private SnapshotTypeCollection GetSnapshotTypeCollection(Type type)
        {
            var key = type.GetHashCode();
            SnapshotTypeCollection typeCollection = null;
            if (_typeCollectionHashMap.ContainsKey(key))
            {

                typeCollection = _typeCollectionHashMap[key];
            }

            return typeCollection;
        }

        private List<Snapshot<T>> FinalizeSnapshots<T>(List<ISnapshot> snapshots) where T : ISnapshot
        {
            var convertedSnapshots = ConvertToSnapshot<T>(snapshots);

            var uniqueSnapshots = RemoveDuplicates(convertedSnapshots);

            return uniqueSnapshots;
        }

        private static void TryAddToSnapshotTypeCollection<T>(Snapshot<T> obj) where T : ISnapshot
        {
            var key = obj.ObjImage.GetType().GetHashCode();
            if (!_typeCollectionHashMap.ContainsKey(key))
            {
                return;
            }

            AddToSnapshotTypeCollection(obj.ObjImage, obj);
        }

        private bool IsValid(ISnapshot obj)
        {
            if (obj == null)
            {
                return false;
            }
            var key = obj.GetHashCode();
            var result = key > 0 && _snapShots.ContainsKey(key);
            return result;
        }

        private int GetKey(ISnapshot snapshot)
        {
            var key = snapshot.GetHashCode();
            return _snapShots.ContainsKey(key) ? key : 0;
        }


        private static void AddToSnapshotHash<T>(int key, Snapshot<T> snapshot) where T : ISnapshot
        {
            
            if (_snapShots.ContainsKey(key))
            {
                _snapShots[key].Add(snapshot);
            }
            else
            {
                _snapShots.Add(key, new List<ISnapshot> { snapshot });
                snapshot.TryCreateSubscription();
            }
        }   
    }


}
