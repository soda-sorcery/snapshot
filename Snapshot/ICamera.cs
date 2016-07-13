using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapshot
{
    public interface ICamera
    {
        Snapshot<T> GetLatestSnapShot<T>(T obj) where T : ISnapshot;
        IList<Snapshot<T>> GetAllSnapshots<T>(T snapshot) where T : ISnapshot;
        Snapshot<T> GetFirstSnapshot<T>(T obj) where T : ISnapshot;
        IList<Snapshot<T>> GetSnapShotTypeCollection<T>() where T : ISnapshot;
        bool DeleteSnapshots<T>(T obj) where T : ISnapshot;
    }
}
