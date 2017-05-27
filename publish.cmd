dotnet restore .\SlackLogger
dotnet pack .\SlackLogger\ /p:PackageVersion=%appveyor_build_version% --configuration Release -o ../.deploy
.\nuget.exe push .deploy\SlackLogger.%appveyor_build_version%.nupkg -ApiKey %2 -source %3