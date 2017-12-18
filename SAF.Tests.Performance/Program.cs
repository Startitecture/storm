namespace SAF.Tests.Performance
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SAF.Tests.Integration;

    class Program
    {
        static Random rand = new Random();
        static string errorMask = "Memory increased over the span of the test: {0}";

        static void Main(string[] args)
        {
            IntegrationTests tests = new IntegrationTests();
            try
            {
                //tests.DataIntegrationController_1000000Items_MemorySizeDoesNotIncrease();
                //tests.ProcessController_1000000Items_MemorySizeDoesNotIncrease();
                tests.TaskEngine_1000000Items_MemoryDeltaUnder100KB();
                //tests.ItemProducer_1000000Items_MemorySizeDoesNotIncrease();
                //tests.EventProcess_1000000EventsTriggered_MemorySizeDoesNotIncrease();
            }
            catch (AssertFailedException ex)
            {
                System.Diagnostics.Trace.TraceError(errorMask, ex.Message);
            }
        }
    }
}
