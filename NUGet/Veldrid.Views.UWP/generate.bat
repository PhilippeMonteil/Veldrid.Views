
copy ..\..\Veldrid.Views\Veldrid.Views.UWP\bin\Release\Veldrid.Views.UWP.dll ..\..\NUGet\Veldrid.Views.UWP\lib\netstandard2.0
copy ..\..\Veldrid.Views\Veldrid.Views.UWP\bin\Release\Veldrid.Views.UWP.pdb ..\..\NUGet\Veldrid.Views.UWP\lib\netstandard2.0
pause

del Veldrid.Views.UWP.1.0.*.nupkg

nuget pack Veldrid.Views.UWP.nuspec

del ..\Veldrid.Views.UWP.1.0.*.nupkg

copy Veldrid.Views.UWP.1.0.*.nupkg ..

pause
