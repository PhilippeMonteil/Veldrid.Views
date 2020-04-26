
copy ..\..\Veldrid.Views.CommandListFactory\bin\Release\netstandard2.0\Veldrid.Views.CommandListFactory.dll .\lib\netstandard2.0
copy ..\..\Veldrid.Views.CommandListFactory\bin\Release\netstandard2.0\Veldrid.Views.CommandListFactory.pdb .\lib\netstandard2.0
pause

del Veldrid.Views.CommandListFactory.1.0.*.nupkg

nuget pack Veldrid.Views.CommandListFactory.nuspec

del ..\Veldrid.Views.CommandListFactory.1.0.*.nupkg

copy Veldrid.Views.CommandListFactory.1.0.*.nupkg ..

pause
