
copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.Core\bin\Release\netstandard2.0\Veldrid.Views.XamarinForms.Core.dll .\lib\netstandard2.0
copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.Core\bin\Release\netstandard2.0\Veldrid.Views.XamarinForms.Core.pdb .\lib\netstandard2.0

copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.Android\bin\Release\Veldrid.Views.XamarinForms.Core.dll .\lib\monoandroid
copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.Android\bin\Release\Veldrid.Views.XamarinForms.Core.pdb .\lib\monoandroid
copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.Android\bin\Release\Veldrid.Views.XamarinForms.Android.dll .\lib\monoandroid
copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.Android\bin\Release\Veldrid.Views.XamarinForms.Android.pdb .\lib\monoandroid

copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.UWP\bin\Release\Veldrid.Views.XamarinForms.Core.dll .\lib\uap10.0
copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.UWP\bin\Release\Veldrid.Views.XamarinForms.Core.pdb .\lib\uap10.0
copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.UWP\bin\Release\Veldrid.Views.XamarinForms.UWP.dll .\lib\uap10.0
copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.UWP\bin\Release\Veldrid.Views.XamarinForms.UWP.pdb .\lib\uap10.0

copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.iOS\bin\Release\Veldrid.Views.XamarinForms.Core.dll .\lib\xamarinios
copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.iOS\bin\Release\Veldrid.Views.XamarinForms.Core.pdb .\lib\xamarinios
copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.iOS\bin\Release\Veldrid.Views.XamarinForms.iOS.dll .\lib\xamarinios
copy ..\..\Veldrid.Views\Veldrid.Views.XamarinForms\Veldrid.Views.XamarinForms.iOS\bin\Release\Veldrid.Views.XamarinForms.iOS.pdb .\lib\xamarinios
pause

del Veldrid.Views.XamarinForms.1.0.*.nupkg

nuget pack Veldrid.Views.XamarinForms.nuspec

del ..\Veldrid.Views.XamarinForms.1.0.*.nupkg

copy Veldrid.Views.XamarinForms.1.0.*.nupkg ..

pause
