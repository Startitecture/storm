namespace SAF.Data.Integration
{
    using System;
    using System.Diagnostics;

    using SAF.Core;
    using SAF.ProcessEngine;

    /// <summary>
    /// Converts imported items into models.
    /// </summary>
    /// <typeparam name="TItem">The type of item to convert.</typeparam>
    /// <typeparam name="TModel">The type of model that is the result of the conversion.</typeparam>
    public sealed class ItemConverter<TItem, TModel> 
        : TaskEngine<ItemConversionDirective<TItem, TModel>, ItemConversionResult<TItem, TModel>>
    {
        /// <summary>
        /// Name mask for this class.
        /// </summary>
        private const string NameMask = "Convert {0} -> {1}";

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemConverter&lt;TItem, TModel&gt;"/> class.
        /// </summary>
        public ItemConverter()
            : base("Item Converter")
        {
        }

        /// <summary>
        /// Converts an imported item into one or more models.
        /// </summary>
        /// <param name="directive">A directive containing conversion instructions.</param>
        public void ConvertItem(ItemConversionDirective<TItem, TModel> directive)
        {
            this.QueueTask(directive);
        }

        /// <summary>
        /// Returns the string representation of this <see cref="ItemConverter&lt;TItem, TModel&gt;"/>.
        /// </summary>
        /// <returns>A string representation of the conversion process.</returns>
        public override string ToString()
        {
            return String.Format(NameMask, typeof(TItem).Name, typeof(TModel).Name);
        }

        /// <summary>
        /// Converts an imported item into a model.
        /// </summary>
        /// <param name="directive">A directive containing instructions for conversion.</param>
        /// <returns>The result of the conversion.</returns>
        protected override ItemConversionResult<TItem, TModel> ConsumeItem(ItemConversionDirective<TItem, TModel> directive)
        {
            if (directive == null)
            {
                throw new ArgumentNullException("directive");
            }

            Exception error = null;
            TModel model = default(TModel);

            try
            {
                model = directive.Converter.Convert(directive.SourceItem);

                if (Evaluate.IsDefaultValue(model))
                {
                    error =
                        new BusinessException(
                            directive.SourceItem, String.Format(ErrorMessages.ConversionReturnedNoModels, directive.SourceItem));
                }
            }
            catch (ArgumentException ex)
            {
                error = ex;
            }
            catch (InvalidOperationException ex)
            {
                error = ex;
            }
            catch (BusinessException ex)
            {
                error = ex;
            }
            catch (OperationException ex)
            {
                error = ex;
            }
            catch (AccessException ex)
            {
                error = ex;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ErrorMessages.UnhandledConversionException, directive.SourceItem, ex);
                throw;
            }

            if (error != null)
            {
                Trace.TraceError(ErrorMessages.ConversionBusinessException, directive.Converter, directive.SourceItem, error);
            }

            return new ItemConversionResult<TItem, TModel>(directive, model, error);
        }
    }
}
