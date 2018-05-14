DATABASE INITIALIZATION
	Run the 'chat-dbscript.sql' on the target MS SQL server.
	The script will create the 'chat' database along with a login named 'bstoyneff' and the 'chat' user in the 'chat' database.

APPLICATION
	To run the application you need: 
		1. A PC with Visual Studio (2017 Pro was used to develop this app) 
		2. An MS SQL Server (2017 Standard was used during development) with the initialized database
	
	Open the solution file.
		A bunch of warninngs and maybe errors will popup, complaining about missing files from the project. Ignore them.
	Open the web.config file and edit the connection string to target the configured MS SQL server.
		Replace 'ALPHA\MSSQL2017S' with the target instance.
	
	Next, choose Build->Rebuild solution from the menu.
		At this point all the NuGet packages shall restore. 
		In addition NugetContentRestore action will re-install the missing static content files.

	Done.
		You can either start a debug session (hit F5) or deploy the app to an IIS server.
