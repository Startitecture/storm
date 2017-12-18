namespace SAF.Data.Integration
{
    using System;

    using SAF.Data;

    /// <summary>
    /// Contains instructions on converting an item into a model.
    /// </summary>
    /// <typeparam name="TItem">The type of item that was converted.</typeparam>
    /// <typeparam name="TModel">The type of model that the item is converted to.</typeparam>
    public class ItemConversionDirective<TItem, TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemConversionDirective&lt;TItem, TModel&gt;"/> class with 
        /// the specified source item and converter.
        /// </summary>
        /// <param name="importResult">The source item to convert.</param>
        /// <param name="converter">The converter that converts the items into models.</param>
        public ItemConversionDirective(TItem importResult, IDataConverter<TItem, TModel> converter)
        {
            if (importResult == null)
            {
                throw new ArgumentNullException("importResult");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            this.SourceItem = importResult;
            this.Converter = converter;
        }

        /// <summary>
        /// Gets the item to convert.
        /// </summary>
        public TItem SourceItem { get; private set; }

        /// <summary>
        /// Gets the converter for the conversion process.
        /// </summary>
        public IDataConverter<TItem, TModel> Converter { get; private set; }
    }
}
