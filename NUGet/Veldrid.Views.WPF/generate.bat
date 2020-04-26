
copy ..\..\Veldrid.Views\Veldrid.Views.WPF\bin\Release\Veldrid.Views.WPF.dll ..\..\NUGet\Veldrid.Views.WPF\lib\netstandard2.0
copy ..\..\Veldrid.Views\Veldrid.Views.WPF\bin\Release\Veldrid.Views.WPF.pdb ..\..\NUGet\Veldrid.Views.WPF\lib\netstandard2.0
pause

del Veldrid.Views.WPF.1.0.*.nupkg

nuget pack Veldrid.Views.WPF.nuspec

del ..\Veldrid.Views.WPF.1.0.*.nupkg
copy Veldrid.Views.WPF.1.0.*.nupkg ..

pause
