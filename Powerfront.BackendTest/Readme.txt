DATABASE INITIALIZATION
	Run the 'chat-dbscript.sql' on the target MS SQL server.
	The script will create the 'chat' database along with a login named 'bstoyneff' and the 'chat' user in the 'chat' database.
	Don't forget to set the paths for the MDF and LDF fiels (line 7 and 9).

APPLICATION
	To run the application you need a PC with Visual Studio (2017 Pro was used to develop the project).
	After you deploy the solution to the file system, follow the steps bellow: 

	Open the solution file - Powerfront.sln
		A bunch of warninngs and maybe errors will popup, complaining about missing files from the project. 
		Ignore them and  choose Build->Rebuild solution from the menu.
		At this point all the NuGet packages shall restore. 
		In addition NugetContentRestore action will re-install the missing files.
	
	In the Powerfront.BackendTest project, open the web.config file and edit the connection string to target the configured MS SQL server.
		Replace 'ALPHA\MSSQL2017S' with the target instance.
	Do the same with the app.config in Powerfront.BackendTest.UnitTests project to enable the unit testing. 

	Done.
		You can either start a debug session (hit F5) or deploy the app to an IIS server.
