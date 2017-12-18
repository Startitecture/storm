namespace SAF.Data.Integration
{
    using System;

    using SAF.ProcessEngine;

    /// <summary>
    /// Contains the result of an item conversion.
    /// </summary>
    /// <typeparam name="TItem">The type of item that was converted.</typeparam>
    /// <typeparam name="TModel">The type of model that the item is converted to.</typeparam>
    public class ItemConversionResult<TItem, TModel> 
        : TaskResult<ItemConversionDirective<TItem, TModel>, TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemConversionResult&lt;TItem, TModel&gt;"/> class with the
        /// specified directive, model and associated error (if any).
        /// </summary>
        /// <param name="directive">The conversion directive that produced the models.</param>
        /// <param name="model">The model that was converted from the item.</param>
        /// <param name="error">The error, if any, associated with the conversion process.</param>
        public ItemConversionResult(
            ItemConversionDirective<TItem, TModel> directive, TModel model, Exception error)
            : base(directive, model, error)
        {
        }
    }
}
