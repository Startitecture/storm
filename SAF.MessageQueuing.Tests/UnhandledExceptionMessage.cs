namespace SAF.MessageQueuing.Tests
{
    using System;

    public class UnhandledExceptionMessage : FakeMessage
    {
        public UnhandledExceptionMessage(DateTimeOffset requestTime, DateTimeOffset deadline, TimeSpan escalationThreshold)
            : base(requestTime, deadline, escalationThreshold)
        {
        }
    }
}
