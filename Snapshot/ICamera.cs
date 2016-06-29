using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapshot
{
    public interface ICamera
    {
        Task<Snapshot<T>> GetLatestSnapShot<T>(T obj) where T : ISnapshot;
        Task<List<Snapshot<T>>> GetAllSnapshots<T>(T snapshot) where T : ISnapshot;
        Task<Snapshot<T>> GetFirstSnapshot<T>(T obj) where T : ISnapshot;
        Task<List<Snapshot<T>>> GetSnapShotTypeCollection<T>(T obj) where T : ISnapshot;
    }
}
