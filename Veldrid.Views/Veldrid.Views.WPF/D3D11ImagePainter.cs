using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Wpf.Interop.DirectX;

using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Veldrid;
using Veldrid.D3D11;

using Veldrid.Views.Contracts;

namespace Veldrid.Views.WPF
{

    public class D3D11ImagePainter : DependencyObject
    {

        public static ICommandListFactory GetCommandListFactory(DependencyObject obj)
        {
            return (ICommandListFactory)obj.GetValue(RendererProperty);
        }

        public static void SetCommandListFactory(DependencyObject obj, ICommandListFactory value)
        {
            obj.SetValue(RendererProperty, value);
        }

        public static readonly DependencyProperty RendererProperty = DependencyProperty.RegisterAttached("CommandListFactory",
                                                                            typeof(ICommandListFactory),
                                                                            typeof(D3D11ImagePainter),
                                                                            new PropertyMetadata(null, PropertyChangedCallback));

        static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            D3D11Image d3D11Image = d as D3D11Image;
            if (d3D11Image == null) return;

            d3D11Image.OnRender = null;

            ICommandListFactory commandListFactory = e.NewValue as ICommandListFactory;
            if (commandListFactory == null)
            {
                return;
            }

            RenderData _renderData = RenderData.Find(d3D11Image, true);
            if (_renderData == null) return;

            GraphicsDevice _graphicsDevice = _renderData.graphicsDevice;
            if (_graphicsDevice == null) return;

            SharpDX.Direct3D11.Device _device = _renderData.device;
            if (_device == null) return;

            d3D11Image.OnRender = (IntPtr surface, bool isNewSurface) =>
            {
                #region --- OnRender
                Framebuffer _framebuffer = _renderData.FrameBuffer(surface, isNewSurface);
                if (_framebuffer == null) return;

                Veldrid.CommandList commandList = null;
                try
                {
                    commandList = commandListFactory.BuildCommandList(_graphicsDevice, _framebuffer);
                    if (commandList == null) return;
                    _graphicsDevice.SubmitCommands(commandList);
                }
                finally
                {
                    commandList?.Dispose();
                }

                _device.ImmediateContext.Flush();
                #endregion
            };

            if (d3D11Image.Width > 0 && d3D11Image.Height > 0)
            {
                d3D11Image.AddDirtyRect(new Int32Rect(0, 0, (int)d3D11Image.Width, (int)d3D11Image.Height));
                d3D11Image.RequestRender();
            }
        }

        #region --- class RenderData

        class RenderData
        {
            readonly D3D11Image m_d3D11Image;

            Framebuffer _framebuffer;

            RenderData(D3D11Image d3D11Image)
            {
                m_d3D11Image = d3D11Image;
            }

            #region --- static

            static List<RenderData> s_aRenderData = new List<RenderData>();

            static SharpDX.Direct3D11.Device s_device;
            static GraphicsDevice s_graphicsDevice;

            static Veldrid.PixelFormat ToVeldridFormat(Format format)
            {
                switch (format)
                {
                    case Format.R8_UNorm:
                        return PixelFormat.R8_UNorm;
                    case Format.R8_SNorm:
                        return PixelFormat.R8_SNorm;
                    case Format.R8_UInt:
                        return PixelFormat.R8_UInt;
                    case Format.R8_SInt:
                        return PixelFormat.R8_SInt;

                    case Format.R16_UNorm:
                    case Format.D16_UNorm:
                        return PixelFormat.R16_UNorm;
                    case Format.R16_SNorm:
                        return PixelFormat.R16_SNorm;
                    case Format.R16_UInt:
                        return PixelFormat.R16_UInt;
                    case Format.R16_SInt:
                        return PixelFormat.R16_SInt;
                    case Format.R16_Float:
                        return PixelFormat.R16_Float;

                    case Format.R32_UInt:
                        return PixelFormat.R32_UInt;
                    case Format.R32_SInt:
                        return PixelFormat.R32_SInt;
                    case Format.R32_Float:
                    case Format.D32_Float:
                        return PixelFormat.R32_Float;

                    case Format.R8G8_UNorm:
                        return PixelFormat.R8_G8_UNorm;
                    case Format.R8G8_SNorm:
                        return PixelFormat.R8_G8_SNorm;
                    case Format.R8G8_UInt:
                        return PixelFormat.R8_G8_UInt;
                    case Format.R8G8_SInt:
                        return PixelFormat.R8_G8_SInt;

                    case Format.R16G16_UNorm:
                        return PixelFormat.R16_G16_UNorm;
                    case Format.R16G16_SNorm:
                        return PixelFormat.R16_G16_SNorm;
                    case Format.R16G16_UInt:
                        return PixelFormat.R16_G16_UInt;
                    case Format.R16G16_SInt:
                        return PixelFormat.R16_G16_SInt;
                    case Format.R16G16_Float:
                        return PixelFormat.R16_G16_Float;

                    case Format.R32G32_UInt:
                        return PixelFormat.R32_G32_UInt;
                    case Format.R32G32_SInt:
                        return PixelFormat.R32_G32_SInt;
                    case Format.R32G32_Float:
                        return PixelFormat.R32_G32_Float;

                    case Format.R8G8B8A8_UNorm:
                        return PixelFormat.R8_G8_B8_A8_UNorm;
                    case Format.R8G8B8A8_UNorm_SRgb:
                        return PixelFormat.R8_G8_B8_A8_UNorm_SRgb;

                    case Format.B8G8R8A8_UNorm:
                        return PixelFormat.B8_G8_R8_A8_UNorm;
                    case Format.B8G8R8A8_UNorm_SRgb:
                        return PixelFormat.B8_G8_R8_A8_UNorm_SRgb;
                    case Format.R8G8B8A8_SNorm:
                        return PixelFormat.R8_G8_B8_A8_SNorm;
                    case Format.R8G8B8A8_UInt:
                        return PixelFormat.R8_G8_B8_A8_UInt;
                    case Format.R8G8B8A8_SInt:
                        return PixelFormat.R8_G8_B8_A8_SInt;

                    case Format.R16G16B16A16_UNorm:
                        return PixelFormat.R16_G16_B16_A16_UNorm;
                    case Format.R16G16B16A16_SNorm:
                        return PixelFormat.R16_G16_B16_A16_SNorm;
                    case Format.R16G16B16A16_UInt:
                        return PixelFormat.R16_G16_B16_A16_UInt;
                    case Format.R16G16B16A16_SInt:
                        return PixelFormat.R16_G16_B16_A16_SInt;
                    case Format.R16G16B16A16_Float:
                        return PixelFormat.R16_G16_B16_A16_Float;

                    case Format.R32G32B32A32_UInt:
                        return PixelFormat.R32_G32_B32_A32_UInt;
                    case Format.R32G32B32A32_SInt:
                        return PixelFormat.R32_G32_B32_A32_SInt;
                    case Format.R32G32B32A32_Float:
                        return PixelFormat.R32_G32_B32_A32_Float;

                    case Format.BC1_UNorm:
                    case Format.BC1_Typeless:
                        return PixelFormat.BC1_Rgba_UNorm;
                    case Format.BC2_UNorm:
                        return PixelFormat.BC2_UNorm;
                    case Format.BC3_UNorm:
                        return PixelFormat.BC3_UNorm;
                    case Format.BC4_UNorm:
                        return PixelFormat.BC4_UNorm;
                    case Format.BC4_SNorm:
                        return PixelFormat.BC4_SNorm;
                    case Format.BC5_UNorm:
                        return PixelFormat.BC5_UNorm;
                    case Format.BC5_SNorm:
                        return PixelFormat.BC5_SNorm;
                    case Format.BC7_UNorm:
                        return PixelFormat.BC7_UNorm;

                    case Format.D24_UNorm_S8_UInt:
                        return PixelFormat.D24_UNorm_S8_UInt;
                    case Format.D32_Float_S8X24_UInt:
                        return PixelFormat.D32_Float_S8_UInt;

                    case Format.R10G10B10A2_UInt:
                        return PixelFormat.R10_G10_B10_A2_UInt;
                    case Format.R10G10B10A2_UNorm:
                        return PixelFormat.R10_G10_B10_A2_UNorm;
                    case Format.R11G11B10_Float:
                        return PixelFormat.R11_G11_B10_Float;
                    default:
                        throw new Exception("...");
                }
            }

            static Veldrid.TextureDescription getTexture2DDescription(Texture2D texture2D)
            {
                TextureDescription vret = new TextureDescription();

                if (texture2D == null) return vret;

                vret = TextureDescription.Texture2D((uint)texture2D.Description.Width, (uint)texture2D.Description.Height,
                                                    1, 1,
                                                    ToVeldridFormat(texture2D.Description.Format), TextureUsage.RenderTarget);

                return vret;
            }

            #endregion

            #region --- public

            public D3D11Image D3D11Image => m_d3D11Image;

            public GraphicsDevice graphicsDevice
            {
                get
                {
                    if (s_graphicsDevice == null)
                    {
                        GraphicsDeviceOptions _options = new GraphicsDeviceOptions(false, null, false, ResourceBindingModel.Improved);
                        s_graphicsDevice = GraphicsDeviceFactory.CreateD3D11(_options);
                    }
                    return s_graphicsDevice;
                }
            }

            public SharpDX.Direct3D11.Device device
            {
                get
                {
                    if (s_device == null)
                    {
                        D3D11GraphicsDevice d3D11GraphicsDevice = graphicsDevice as D3D11GraphicsDevice;
                        if (d3D11GraphicsDevice == null) return null;

                        BackendInfoD3D11 _info;
                        bool _ok = d3D11GraphicsDevice.GetD3D11Info(out _info);
                        if (_ok == false || _info == null) return null;

                        s_device = new SharpDX.Direct3D11.Device(_info.Device);
                    }
                    return s_device;
                }
            }

            public Framebuffer FrameBuffer(IntPtr surface, bool isNewSurface)
            {
                if (isNewSurface)
                {
                    if (_framebuffer != null)
                    {
                        _framebuffer.Dispose();
                        _framebuffer = null;
                    }
                }

                if (_framebuffer == null)
                {
                    SharpDX.Direct3D11.Device _deviceD11 = device;
                    if (_deviceD11 == null) return null;

                    using (var _ComObject = new SharpDX.ComObject(surface))
                    {
                        Texture2D _texture2D = null;
                        Texture _texture = null;
                        try
                        {
                            var _dxgiResource = _ComObject.QueryInterface<SharpDX.DXGI.Resource>();
                            if (_dxgiResource == null) return null;

                            _texture2D = _deviceD11.OpenSharedResource<Texture2D>(_dxgiResource.SharedHandle);
                            if (_texture2D == null) return null;

                            _texture = graphicsDevice.ResourceFactory.CreateTexture((ulong)_texture2D.NativePointer, getTexture2DDescription(_texture2D));
                            if (_texture == null) return null;

                            _framebuffer = s_graphicsDevice.ResourceFactory.CreateFramebuffer(new FramebufferDescription(null, _texture));
                        }
                        finally
                        {
                            _texture?.Dispose();
                            _texture2D?.Dispose();
                        }
                    }
                }

                return _framebuffer;
            }

            #endregion

            #region --- static

            public static RenderData Find(D3D11Image d3D11Image, bool createIfNotFound = true)
            {
                if (d3D11Image == null) return null;

                lock (s_aRenderData)
                {
                    RenderData renderData = s_aRenderData.FirstOrDefault(rd => rd.D3D11Image == d3D11Image);
                    if (renderData != null) return renderData;

                    if (createIfNotFound == false) return null;

                    s_aRenderData.Add(renderData = new RenderData(d3D11Image));

                    return renderData;
                }

            }

            #endregion

        }

        #endregion

    }

}
