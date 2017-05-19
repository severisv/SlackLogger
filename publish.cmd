dotnet restore .\SlackLogger
dotnet pack .\SlackLogger\ /p:PackageVersion=1.0.%1 --configuration Release -o ../.deploy
.\nuget.exe push .deploy\SlackLogger.1.0.%1.nupkg -ApiKey %2 -source %3