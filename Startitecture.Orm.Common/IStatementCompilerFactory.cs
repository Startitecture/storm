// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatementCompilerFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    /// <summary>
    /// The StatementCompilerFactory interface.
    /// </summary>
    public interface IStatementCompilerFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="databaseContext">
        /// The database context.
        /// </param>
        /// <returns>
        /// The <see cref="IStatementCompiler"/>.
        /// </returns>
        IStatementCompiler Create(IDatabaseContext databaseContext);
    }
}