# Veldrid.Views


        Veldrid.Views: leveraging the power of the Veldrid portable GPU API with Xamarin.Forms, WPF, UWP

1) Presenting Veldrid and Veldrid.Views

Veldrid is a low-level graphics library for .NET. It can be used to create high-performance 2D and 3D games, simulations, tools, and other graphical applications. Unlike most other .NET graphics libraries, Veldrid is designed to be portable, meaning it is not tied to any particular operating system or native graphics API. With Direct3D, Vulkan, Metal, OpenGL, and OpenGL ES backends, applications built with Veldrid can run on all desktop and mobile platforms without modification.
The Veldrid web site: https://veldrid.dev/

Veldrid.Views proposes a simple mechanism for connecting Veldrid to the various UI engines available to .Net developers: WPF, UWP, Xamarin.Forms
in a consistent and portable manner. 

2) Key Concepts

Veldrid.Views proposes a CommandList Factory mechanism in charge of producing the Veldrid CommandLists executed by one of its Views to render their content.

A CommandList Factory is exposed by a .Net Standard Veldrid based component and can be used by all the supported platforms: Xamarin.Forms, WPF, UWP.

Veldrid.Views provides an attached property mechanism for connecting its Views with the CommandList Factories producing the CommandLists defining their aspect.

Veldrid.Views includes a Xamarin.Forms custom control, provided with its renderers for iOS, Android and UWP, in charge of consuming CommandLists.

A CommandFactory component exposes the ICommandListFactory defined in the Veldrid.Views.Contracts component:

using Veldrid;

namespace Veldrid.Views.Contracts
{

    public interface ICommandListFactory
    {
        Veldrid.CommandList BuildCommandList(Veldrid.GraphicsDevice graphicsDevice, Veldrid.Framebuffer framebuffer);
    }

}

A CommandFactory component can then be connected with no code to a Veldrid.View View, the Xamarin.Forms one in this case, through the VeldridViewPainter.CommandListFactory attached property:

    XAML extract:

        xmlns:vvxfcore="clr-namespace:Veldrid.Views.XamarinForms.Core;assembly=Veldrid.Views.XamarinForms.Core"
        xmlns:clf="clr-namespace:Veldrid.Views.CommandListFactory;assembly=Veldrid.Views.CommandListFactory"

        <vvxfcore:VeldridView x:Name="VeldridView0" Grid.Row="1" BackgroundColor="Red" Margin="64" >
            <vvxfcore:VeldridViewPainter.CommandListFactory>
                <clf:CommandListFactoryTest2D />
            </vvxfcore:VeldridViewPainter.CommandListFactory>
        </vvxfcore:VeldridView>

Veldrid.View exposes a different VeldridViewPainter.CommandListFactory Attached Property for each of the supported plaforms: Xamarin.Forms, WPF, UWP.

3) The Veldrid.Views nugets

The following Nuget are temporarily included in the project:

- Veldrid.Platforms.1.0.9.nupkg: providing the Veldrid runtime

- Veldrid.Views.Contracts.1.0.2.nupkg: defining the ICommandListFactory interface

- Veldrid.Views.CommandListFactory.1.0.3.nupkg: providing the CommandListFactory components used by the test applications

- Veldrid.Views.XamarinForms.1.0.2.nupkg
- Veldrid.Views.WPF.1.0.2.nupkg
- Veldrid.Views.UWP.1.0.2.nupkg

They can be found in the NUGet directory which should be referenced as a local source by NUGet.

Veldrid.Views temporarily uses the Veldrid.Platforms.1.0.9.nupkg home made package instead of the 'official' Veldrid one
which requires a few changes to be integrated and be usable by Veldrid.Views.

4) Organisation of the project source code

- Veldrid.Views.Contracts: definition of the ICommandListFactory interface

- Veldrid.Views: source code of the XamarinForms view and of the various VeldridViewPainter.CommandListFactory attached properties
  - Veldrid.Views.XamarinForms
  - Veldrid.Views.UWP
  - Veldrid.Views.WPF

- Veldrid.Views.CommandListFactory: source code of the sample CommandListFactory used by the test samples

- Tests: source code of the XamarinForms/WPF/UWP test samples

  - Test.Veldrid.Views.XamarinForms (iOS / Android / UWP)
  - Test.Veldrid.Views.WPF
  - Test.Veldrid.Views.UWP

- NUGet

  Includes the .nuspec definitions of the Nuget packages produced and used by the Veldrid.Views projects along with tha batch files to produce them.
  Note: those packages are not published yet, they should be stored in a local directory made available to Nuget as a local package source.   

5) The Veldrid.Views.CommandListFactory sample component: 

The CommandListFactoryTest2D CommandListFactory illustrates the generation and use of the HLSL / GLSL / SPIR-V and Metal versions of the shaders
needed by a CommandListFactory component using the DXC and SPIRV-Cross compilers.

A shader is initially written in HSLS and tested under WPF or UWP. It is then compiled into a SPIR-V version using DXC and then back to GLSL / Metal
using SPIR-Cross.

The Shaders.BAT batch file provides the commands needed by the CommandListFactoryTest2D component:

rem .vs/.ps -> .spv
dxc -T vs_6_0 -spirv -E _VertexShader -Fo .\CommandListFactory\Shaders\Test_Colored2DVertices_vs.spv .\CommandListFactory\Shaders\Test_Colored2DVertices.vs
dxc -T ps_6_0 -spirv -E _PixelShader -Fo .\CommandListFactory\Shaders\Test_Colored2DVertices_ps.spv .\CommandListFactory\Shaders\Test_Colored2DVertices.ps

rem .spv -> .glsl
spirv-cross SPIR-V .\CommandListFactory\Shaders\Test_Colored2DVertices_vs.spv --output .\CommandListFactory\Shaders\Test_Colored2DVertices_vs.glsl
spirv-cross SPIR-V .\CommandListFactory\Shaders\Test_Colored2DVertices_ps.spv --output .\CommandListFactory\Shaders\Test_Colored2DVertices_ps.glsl

rem .spv -> .es.glsl
spirv-cross --version 310 --es SPIR-V .\CommandListFactory\Shaders\Test_Colored2DVertices_vs.spv --output .\CommandListFactory\Shaders\Test_Colored2DVertices_vs_es.glsl
spirv-cross --version 310 --es SPIR-V .\CommandListFactory\Shaders\Test_Colored2DVertices_ps.spv --output .\CommandListFactory\Shaders\Test_Colored2DVertices_ps_es.glsl

rem .spv -> .msl
spirv-cross --msl SPIR-V .\CommandListFactory\Shaders\Test_Colored2DVertices_vs.spv --output .\CommandListFactory\Shaders\Test_Colored2DVertices_vs.msl
spirv-cross --msl SPIR-V .\CommandListFactory\Shaders\Test_Colored2DVertices_ps.spv --output .\CommandListFactory\Shaders\Test_Colored2DVertices_ps.msl

7) Additional resources

- The WPF D3D11Image is provided separately in 32 and 64 bit versions found in nuget.org (include prerelease packages)
  microsoft.wpf.interop.directx-x64.0.9.0-beta-22856, microsoft.wpf.interop.directx-x32.0.9.0-beta-22856.
  The 64 bit version is used by the Test.Veldrid.Views.WPF test sample.

- The DXC compiler supporting SPIR-V cross compilation, which is not the case of the DirectX one,
  can be found here:
  https://ci.appveyor.com/project/antiagainst/directxshadercompiler/branch/master/artifacts

- The SPIRV-Cross compiler is part of the Vulkan SDK:
  https://vulkan.lunarg.com/sdk/home#windows

- the Veldrid web site: https://veldrid.dev/

- other Veldrid based projects

  - https://github.com/mellinoe/veldrid-samples
  - https://github.com/gleblebedev/VeldridGlTF



