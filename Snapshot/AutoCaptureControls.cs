using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapshot
{
    internal class AutoCaptureControls
    {
        internal bool ShouldAutoCapture { get; set; } = true;
        internal object Lock { get; } = new object();
    }
}
