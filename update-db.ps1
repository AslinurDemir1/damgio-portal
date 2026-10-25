$env:PATH = "$env:USERPROFILE\.dotnet;$env:USERPROFILE\.dotnet\tools;$env:PATH"
dotnet ef database update --no-build --project src/Damgio.Infrastructure --startup-project src/Damgio.Web
