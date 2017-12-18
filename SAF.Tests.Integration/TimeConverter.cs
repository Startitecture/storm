namespace SAF.Tests.Integration
{
    using System;

    using SAF.Data;

    public class TimeConverter : IDataConverter<long, DateTime>
    {
        public DateTime Convert(long importedItem)
        {
            return new DateTime(importedItem);
        }
    }
}
