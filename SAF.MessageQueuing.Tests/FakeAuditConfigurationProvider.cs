namespace SAF.MessageQueuing.Tests
{
    using SAF.ActionTracking;

    public class FakeAuditConfigurationProvider : IAuditConfigurationProvider
    {
        public AuditConfigurationSection Configuration { get; private set; }
    }
}