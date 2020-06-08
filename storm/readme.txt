-------------------------------------------------------------------------------
Startitecture.Orm (ST/orm)
-------------------------------------------------------------------------------

IMPORTANT: .NET SDK and .NET Core projects do not allow NuGet packages to place
mutable content within project files. Follow the steps below to ensure
everything works (Visual Studio):

(1) Navigate to the NuGet package "Generation" folder by right-clicking one of
the following files in this project's "Generation" folder:

* Database.tt
* storm.Reverse.POCO.Core.ttinclude
* storm.Reverse.POCO.ttinclude

Drag all three files from the Explorer window into your project or a project 
subfolder. Do not run the transform if prompted.

(2) Review the settings in Database.tt and ensure that you have a valid 
connection name (if using Web.config or app.config) or connection string 
(.NET Core, .NET SDK).

(3) Save changes and allow the transform to run.

You should now have POCOs generated and the appropriate packages installed.

Happy ST/orm-ing!