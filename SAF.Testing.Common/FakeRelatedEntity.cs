namespace SAF.Testing.Common
{
    public class FakeRelatedEntity
    {
        public FakeRelatedEntity(int? relatedId)
        {
            this.RelatedId = relatedId;
        }

        private FakeRelatedEntity()
        {
        }

        public int? RelatedId { get; private set; }

        public string RelatedProperty { get; set; }
    }
}
