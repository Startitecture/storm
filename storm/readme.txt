-------------------------------------------------------------------------------
Startitecture.Orm (ST/orm)
-------------------------------------------------------------------------------

IMPORTANT: .NET SDK and .NET Core projects do not allow NuGet packages to place
mutable content within project files. Follow the steps below to ensure
everything works:

(1) Copy these files from the "Generation" folder:

* Database.tt (recommend renaming to <dbname>.tt, or <dbname>-<schema>.tt)
* storm.Reverse.POCO.Core.ttinclude
* storm.Reverse.POCO.ttinclude

into your project or a project subfolder. Do not run the transform if prompted.

(2) Review the settings in Database.tt (or whatever you renamed it to) and 
ensure that you have a valid connection name (if using Web.config or 
app.config) or connection string (.NET Core, .NET SDK).

(3) Save changes and allow the transform to run.

You should now have POCOs generated and the appropriate packages installed.

Happy ST/orm-ing!