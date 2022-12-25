dotnet restore .\SlackLogger
dotnet pack .\SlackLogger\ /p:PackageVersion=%appveyor_build_version% --configuration Release -o ../.deploy
dotnet nuget push .deploy\SlackLogger.%appveyor_build_version%.nupkg --api-key %2 --source nuget.org