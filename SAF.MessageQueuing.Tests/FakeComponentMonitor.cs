namespace SAF.MessageQueuing.Tests
{
    using SAF.Core;
    using SAF.Observer;

    public class FakeComponentMonitor : ComponentMonitor
    {
        public FakeComponentMonitor(string qualifiedName, params FakeResourceMonitor[] monitors)
            : base(qualifiedName)
        {
            foreach (var monitor in monitors)
            {
                this.RegisterMonitor(monitor);
            }
        }
    }
}