rem .vs/.ps -> .spv
dxc -T vs_6_0 -spirv -E _VertexShader -Fo C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_vs.spv C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices.vs
dxc -T ps_6_0 -spirv -E _PixelShader -Fo C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_ps.spv C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices.ps
pause

rem .spv -> .glsl
spirv-cross SPIR-V C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_vs.spv --output C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_vs.glsl
spirv-cross SPIR-V C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_ps.spv --output C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_ps.glsl

rem .spv -> .es.glsl
spirv-cross --version 310 --es SPIR-V C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_vs.spv --output C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_vs_es.glsl
spirv-cross --version 310 --es SPIR-V C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_ps.spv --output C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_ps_es.glsl

rem .spv -> .msl
spirv-cross --msl SPIR-V C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_vs.spv --output C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_vs.msl
spirv-cross --msl SPIR-V C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_ps.spv --output C:\Users\philm\source\repos\Veldrid.Xamarin.Forms\CommandListFactory\Shaders\Test_Colored2DVertices_ps.msl
pause
