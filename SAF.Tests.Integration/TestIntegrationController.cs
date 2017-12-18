namespace SAF.Tests.Integration
{
    using System;

    using SAF.Data.Integration;

    public class TestIntegrationController : IntegrationController<long, DateTime>
    {
        public TestIntegrationController()
            : base("Test Controller")
        {
            
        }
    }
}
