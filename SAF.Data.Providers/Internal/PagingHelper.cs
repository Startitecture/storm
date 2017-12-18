// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagingHelper.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   A helper class for SQL paging.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Internal
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// A helper class for SQL paging.
    /// </summary>
    internal static class PagingHelper
    {
        #region Static Fields

        /// <summary>
        /// The rx distinct.
        /// </summary>
        public static readonly Regex DistinctRegex = new Regex(
            @"\ADISTINCT\s",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// The rx order by.
        /// </summary>
        public static readonly Regex OrderByRegex = new Regex(OrderByPattern, OrderByOptions);

        /// <summary>
        /// The order by pattern.
        /// </summary>
        private const string OrderByPattern = @"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*";

        /// <summary>
        /// The order by options.
        /// </summary>
        private const RegexOptions OrderByOptions =
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline
            | RegexOptions.Compiled;

        /// <summary>
        /// The rx columns.
        /// </summary>
        private static readonly Regex ColumnsRegex =
            new Regex(
                @"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b", 
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Attempts to split a SQL statement into a <see cref="SqlPageStatement"/>.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to split.
        /// </param>
        /// <param name="pageStatement">
        /// The page statement.
        /// </param>
        /// <returns>
        /// <c>true</c> if the attempt is successful; otherwise, <c>false</c>.
        /// </returns>
        public static bool TrySplitSql(string sql, out SqlPageStatement pageStatement)
        {
            pageStatement = new SqlPageStatement { Sql = sql, SqlSelectRemoved = null, SqlCount = null, SqlOrderBy = null };

            // Extract the columns from "SELECT <whatever> FROM"
            Match match = ColumnsRegex.Match(sql);

            if (match.Success == false)
            {
                return false;
            }

            // Save column list and replace with COUNT(*)
            Group matchGroups = match.Groups[1];
            pageStatement.SqlSelectRemoved = sql.Substring(matchGroups.Index);

            if (DistinctRegex.IsMatch(pageStatement.SqlSelectRemoved))
            {
                pageStatement.SqlCount = sql.Substring(0, matchGroups.Index) + "COUNT(" + match.Groups[1].ToString().Trim() + ") "
                                         + sql.Substring(matchGroups.Index + matchGroups.Length);
            }
            else
            {
                pageStatement.SqlCount = sql.Substring(0, matchGroups.Index) + "COUNT(*) "
                                         + sql.Substring(matchGroups.Index + matchGroups.Length);
            }

            // Look for the last "ORDER BY <whatever>" clause not part of a ROW_NUMBER expression
            match = OrderByRegex.Match(pageStatement.SqlCount);

            if (match.Success)
            {
                matchGroups = match.Groups[0];
                pageStatement.SqlOrderBy = matchGroups.ToString();
                pageStatement.SqlCount = pageStatement.SqlCount.Substring(0, matchGroups.Index)
                                         + pageStatement.SqlCount.Substring(matchGroups.Index + matchGroups.Length);
            }
            else
            {
                pageStatement.SqlOrderBy = null;
            }

            return true;
        }

        #endregion
    }
}