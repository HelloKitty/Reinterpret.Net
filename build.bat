dotnet restore Reinterpret.Net.sln
dotnet build Reinterpret.Net.sln -c Release

Start-Process ILRepack.exe "/out:.\src\Reinterpret.Net\bin\Release\net20\Reinterpret.Net.dll .\src\Reinterpret.Net\bin\Release\net20\Reinterpret.Net.dll  .\src\Reinterpret.Net\bin\Release\net20\UnsafeAs.Backport.dll" -Wait
Start-Process ILRepack.exe "/out:.\src\Reinterpret.Net\bin\Release\net30\Reinterpret.Net.dll .\src\Reinterpret.Net\bin\Release\net30\Reinterpret.Net.dll  .\src\Reinterpret.Net\bin\Release\net30\UnsafeAs.Backport.dll" -Wait
Start-Process ILRepack.exe "/out:.\src\Reinterpret.Net\bin\Release\net35\Reinterpret.Net.dll .\src\Reinterpret.Net\bin\Release\net35\Reinterpret.Net.dll  .\src\Reinterpret.Net\bin\Release\net35\UnsafeAs.Backport.dll" -Wait
Start-Process ILRepack.exe "/out:.\src\Reinterpret.Net\bin\Release\net40\Reinterpret.Net.dll .\src\Reinterpret.Net\bin\Release\net40\Reinterpret.Net.dll  .\src\Reinterpret.Net\bin\Release\net40\UnsafeAs.Backport.dll" -Wait