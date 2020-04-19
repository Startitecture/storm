// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Startitecture" file="GlobalSuppressions.cs">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Unnecessary for literal comparisons in unit or integration tests", Scope = "module")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Readability for unit tests", Scope = "module")]
