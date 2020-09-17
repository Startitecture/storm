-------------------------------------------------------------------------------
Startitecture.Orm (ST/ORM)
-------------------------------------------------------------------------------

IMPORTANT: .NET SDK and .NET Core projects do not allow NuGet packages to place
mutable content within project files. Follow the steps below to ensure
everything works (Visual Studio):

(1) Navigate to the NuGet package's "Generation" folder by right-clicking one 
of the following files in this project's "Generation" folder

* Database.tt
* storm.Reverse.POCO.Core.ttinclude
* storm.Reverse.POCO.ttinclude

and clicking "Open Containing Folder".

(2) Drag all three files from the Explorer window into your project or a project 
subfolder. Do not run the transform if prompted.

(3) Review the settings in Database.tt and ensure that you have a valid 
connection name (if using Web.config or app.config) or connection string 
and provider name (.NET Core, .NET SDK).

(4) Save changes and allow the transform to run. You should now have POCOs 
generated.

(5) Install the Startitecture.Orm.* NuGet package appropriate for your 
connection (i.e., Startitecture.Orm.SqlClient, .PostgreSql, etc.).

Happy ST/ORM-ing!