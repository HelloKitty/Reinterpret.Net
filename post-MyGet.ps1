##Looks through the entire src directory and runs nuget pack with dependencies added on each csproj found
##foreach file in src/*
foreach($f in Get-ChildItem ./src/)
{
    ##foreach file in the src/*/ directory that ends with the .csproj format
    foreach($ff in (Get-ChildItem (Join-Path ./src/ $f.Name) | Where-Object { $_.Name.EndsWith(".csproj") }))
    {
        ##Add the project path + the csproj name and add the include referenced projects argument which will
        ##force nuget dependencies
        $projectArgs = "pack " + (Join-Path (Join-Path src/ $f.Name) $ff.Name)## + "-c Release -IncludeReferencedProjects"
        Start-Process dotnet $projectArgs -Wait
    }
}

Start-Process ILRepack.exe "/out:.\src\Reinterpret.Net\bin\Release\net20\Reinterpret.Net.dll .\src\Reinterpret.Net\bin\Release\net20\Reinterpret.Net.dll  .\src\Reinterpret.Net\bin\Release\net20\UnsafeAs.Backport.dll" -Wait
Start-Process ILRepack.exe "/out:.\src\Reinterpret.Net\bin\Release\net30\Reinterpret.Net.dll .\src\Reinterpret.Net\bin\Release\net30\Reinterpret.Net.dll  .\src\Reinterpret.Net\bin\Release\net30\UnsafeAs.Backport.dll" -Wait
Start-Process ILRepack.exe "/out:.\src\Reinterpret.Net\bin\Release\net35\Reinterpret.Net.dll .\src\Reinterpret.Net\bin\Release\net35\Reinterpret.Net.dll  .\src\Reinterpret.Net\bin\Release\net35\UnsafeAs.Backport.dll" -Wait
Start-Process ILRepack.exe "/out:.\src\Reinterpret.Net\bin\Release\net40\Reinterpret.Net.dll .\src\Reinterpret.Net\bin\Release\net40\Reinterpret.Net.dll  .\src\Reinterpret.Net\bin\Release\net40\UnsafeAs.Backport.dll" -Wait
