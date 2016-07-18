using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Snapshot
{
    internal class Timer
    {
        public Timer(int interval)
        {
            Interval = interval;
        }

        private ISnapshot _firstSnapshot;
        private static bool _shouldTakePhoto = true;

        public Timer Init<T>(Snapshot<T> snapshot) where T : ISnapshot
        {
            _firstSnapshot = snapshot;

            if (Interval > 0)
            {
                Task.Run(() => TakePhoto<T>());
            }

            return this;
        }

        public int Interval { get; private set; }

        private readonly List<Task> _promises = new List<Task>();

        internal async Task GetAllTimedSnapshots()
        {
            _shouldTakePhoto = false;
            await Task.WhenAll(_promises);
        }

        private void TakePhoto<T>() where T : ISnapshot
        {

            while (_shouldTakePhoto)
            {
                Thread.Sleep(Interval);
                var t = Task.Run(() =>
                {
                    var s = (Snapshot<T>)_firstSnapshot;

                    Camera.CreateSnapshot(s.ObjImage, s.Key);
                });
                _promises.Add(t);
            }


        }
    }
}
