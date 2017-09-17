%NUGET% restore TypeSafe.HTTP.NET.sln -NoCache -NonInteractive -ConfigFile Nuget.config
msbuild TypeSafe.HTTP.NET.sln /p:Configuration=Release