using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapshot
{
    // marker only interface
    public interface IAutoSnapshot : INotifyPropertyChanged, ISnapshot
    {
    }
}
