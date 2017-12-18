using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAF.Observer
{
    public class ObservationErrorEventArgs : EventArgs
    {
        public ObservationErrorEventArgs(Exception observationError)
            : this(observationError, false)
        {
        }

        public ObservationErrorEventArgs(Exception observationError, bool observationTerminated)
        {
            this.ObservationError = observationError;
            this.ObservationTerminated = observationTerminated;
        }

        public Exception ObservationError { get; private set; }

        public bool ObservationTerminated { get; private set; }
    }
}
