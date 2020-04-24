namespace Startitecture.Orm.Model
{
    public interface IEntityMapperFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <returns>
        /// The <see cref="IEntityMapper"/>.
        /// </returns>
        IEntityMapper Create();
    }
}