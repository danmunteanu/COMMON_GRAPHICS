set DOTNET_CLI_TELEMETRY_OPTOUT=1
dotnet build COMMON_GRAPHICS.csproj /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary /p:Configuration=Release /p:Platform="AnyCPU"

pause
