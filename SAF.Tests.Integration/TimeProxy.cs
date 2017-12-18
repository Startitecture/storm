namespace SAF.Tests.Integration
{
    using SAF.Data;
    using SAF.Data.Persistence;

    public class TimeProxy : DataProxy<LongGenerator, long>
    {
        protected override void EmitItems(LongGenerator dataSource)
        {
            for (int i = 0; i < dataSource.ItemsToGenerate; i++)
            {
                this.EmitItem(dataSource.GenerateLong());
            }
        }
    }
}
