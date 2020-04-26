
copy ..\..\Veldrid.Views.Contracts\bin\Release\netstandard2.0\Veldrid.Views.Contracts.dll .\lib\netstandard2.0
copy ..\..\Veldrid.Views.Contracts\bin\Release\netstandard2.0\Veldrid.Views.Contracts.pdb .\lib\netstandard2.0
pause

del Veldrid.Views.Contracts.1.0.*.nupkg

nuget pack Veldrid.Views.Contracts.nuspec

copy Veldrid.Views.Contracts.1.0.*.nupkg ..

pause
