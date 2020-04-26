using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Veldrid;
using Veldrid.D3D11;
using Veldrid.Views.Contracts;
using System.Diagnostics;

namespace Veldrid.Views.UWP
{

    public class SwapChainPanelPainter : DependencyObject
    {

        #region --- CommandListFactory

        public static ICommandListFactory GetCommandListFactory(DependencyObject obj)
        {
            return (ICommandListFactory)obj.GetValue(CommandListFactoryProperty);
        }

        public static void SetCommandListFactory(DependencyObject obj, ICommandListFactory value)
        {
            obj.SetValue(CommandListFactoryProperty, value);
        }

        public static readonly DependencyProperty CommandListFactoryProperty = DependencyProperty.RegisterAttached("CommandListFactory",
                                                                            typeof(ICommandListFactory),
                                                                            typeof(SwapChainPanel),
                                                                            new PropertyMetadata(null, PropertyChangedCallback));

        static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine($"{nameof(SwapChainPanelPainter)}.{nameof(PropertyChangedCallback)} d={d} e.OldValue={e.OldValue} e.NewValue={e.NewValue}");

            SwapChainPanel swapChainPanel = d as SwapChainPanel;
            if (swapChainPanel == null) return;

            ICommandListFactory commandListFactory = e.NewValue as ICommandListFactory;
            if (commandListFactory == null)
            {
                // 'déconnexion de swapChainPanel'
                bool _ok = RenderData.Dispose(swapChainPanel);
                if (_ok)
                {
                    // events
                    swapChainPanel.Loaded -= SwapChainPanel_Loaded;
                    swapChainPanel.Unloaded -= SwapChainPanel_Unloaded;
                    swapChainPanel.SizeChanged -= SwapChainPanel_SizeChanged;
                    swapChainPanel.CompositionScaleChanged -= SwapChainPanel_CompositionScaleChanged;
                }
            }
            else
            {
                RenderData _renderData = RenderData.Find(swapChainPanel, false);
                if (_renderData == null)
                {
                    _renderData = RenderData.Find(swapChainPanel, true);
                    if (_renderData == null) return;

                    bool _ok = false;
                    try
                    {
                        GraphicsDevice _graphicsDevice = _renderData.graphicsDevice;
                        if (_graphicsDevice == null) return;

                        // events
                        swapChainPanel.Loaded += SwapChainPanel_Loaded;
                        swapChainPanel.Unloaded += SwapChainPanel_Unloaded;
                        swapChainPanel.SizeChanged += SwapChainPanel_SizeChanged;
                        swapChainPanel.CompositionScaleChanged += SwapChainPanel_CompositionScaleChanged;

                        _ok = true;
                    }
                    finally
                    {
                        if (_ok == false)
                        {
                            RenderData.Dispose(swapChainPanel);
                        }
                    }

                }
            }

        }

        static void render(SwapChainPanel swapChainPanel)
        {
            ICommandListFactory commandListFactory = GetCommandListFactory(swapChainPanel);
            if (commandListFactory == null) return;

            RenderData _renderData = RenderData.Find(swapChainPanel, false);
            if (_renderData == null) return;

            Debug.WriteLine($"{nameof(SwapChainPanelPainter)}.{nameof(render)} _renderData={_renderData}");

            GraphicsDevice _graphicsDevice = _renderData.graphicsDevice;
            if (_graphicsDevice == null) return;

            Veldrid.Swapchain _swapchain = _renderData.swapchain;
            if (_swapchain == null) return;

            Framebuffer _framebuffer = _renderData.framebuffer;
            if (_framebuffer == null) return;

            {
                Veldrid.CommandList _commandList = null;
                try
                {
                    _commandList = commandListFactory.BuildCommandList(_graphicsDevice, _framebuffer);
                    if (_commandList == null)
                    {
                        return;
                    }

                    _graphicsDevice.SubmitCommands(_commandList);
                    _graphicsDevice.SwapBuffers(_swapchain);
                }
                catch (Exception E)
                {
                }
                finally
                {
                    _commandList?.Dispose();
                }
            }

        }

        #endregion

        #region --- SwapChainPanel events

        private static void SwapChainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            SwapChainPanel swapChainPanel = sender as SwapChainPanel;
            if (swapChainPanel == null) return;

            RenderData _renderData = RenderData.Find(swapChainPanel, false);
            Debug.WriteLine($"{nameof(SwapChainPanelPainter)}.{nameof(SwapChainPanel_Loaded)} _renderData={_renderData}");
            if (_renderData == null) return;

            _renderData.Loaded = true;

            render(swapChainPanel);
        }

        private static void SwapChainPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            SwapChainPanel swapChainPanel = sender as SwapChainPanel;
            if (swapChainPanel == null) return;

            Debug.WriteLine($"{nameof(SwapChainPanelPainter)}.{nameof(SwapChainPanel_Unloaded)} swapChainPanel={swapChainPanel}");

            swapChainPanel.Loaded -= SwapChainPanel_Loaded;
            swapChainPanel.Unloaded -= SwapChainPanel_Unloaded;
            swapChainPanel.SizeChanged -= SwapChainPanel_SizeChanged;
            swapChainPanel.CompositionScaleChanged -= SwapChainPanel_CompositionScaleChanged;

            RenderData.Dispose(swapChainPanel);
        }

        private static void SwapChainPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SwapChainPanel swapChainPanel = sender as SwapChainPanel;
            if (swapChainPanel == null) return;

            Debug.WriteLine($"{nameof(SwapChainPanelPainter)}.{nameof(SwapChainPanel_SizeChanged)} swapChainPanel={swapChainPanel}");

            RenderData _renderData = RenderData.Find(swapChainPanel, false);
            if (_renderData == null) return;

            // _renderData.Reset_swapchain();

            render(swapChainPanel);
        }

        private static void SwapChainPanel_CompositionScaleChanged(SwapChainPanel sender, object args)
        {
            SwapChainPanel swapChainPanel = sender as SwapChainPanel;
            if (swapChainPanel == null) return;

            Debug.WriteLine($"{nameof(SwapChainPanelPainter)}.{nameof(SwapChainPanel_CompositionScaleChanged)} swapChainPanel={swapChainPanel}");

            render(swapChainPanel);
        }

        #endregion

        #region --- public static

        public static void Render(SwapChainPanel swapChainPanel)
        {
            render(swapChainPanel);
        }

        #endregion

        #region --- class RenderData

        class RenderData : IDisposable
        {
            readonly SwapChainPanel m_swapChainPanel;
            Swapchain m_swapchain;

            RenderData(SwapChainPanel swapChainPanel)
            {
                m_swapChainPanel = swapChainPanel;
            }

            public override string ToString()
            {
                return $"{GetType().Name} Loaded={Loaded} m_swapChainPanel={m_swapChainPanel}";
            }

            #region --- static

            static List<RenderData> s_aRenderData = new List<RenderData>();

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

            #region --- IDisposable

            public void Dispose()
            {
                try
                {
                    m_swapchain?.Dispose();
                }
                catch
                {
                }
                finally
                {
                    m_swapchain = null;
                }
            }

            #endregion

            #region --- private

            double width => m_swapChainPanel?.ActualWidth ?? 0;
            double height => m_swapChainPanel?.ActualHeight ?? 0;

            #endregion

            #region --- public

            public bool Loaded { set; get; }

            public SwapChainPanel swapChainPanel => m_swapChainPanel;

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

            public Swapchain swapchain
            {
                get
                {
                    if (Loaded == false)
                    {
                        return null;
                    }
                    if (width <= 0 || height <= 0)
                    {
                        return null;
                    }

                    // vérification de framebuffer, resize de m_swapchain si nécessaire
                    Framebuffer _framebuffer = m_swapchain?.Framebuffer;
                    if (_framebuffer != null)
                    {
                        if (_framebuffer.Width != (uint)width || _framebuffer.Height != (uint)height)
                        {
                            m_swapchain.Resize((uint)width, (uint)height);
                            _framebuffer = m_swapchain?.Framebuffer;
                        }
                    }
                    if (_framebuffer == null)
                    {
                        m_swapchain?.Dispose();
                        m_swapchain = null;
                    }

                    if (m_swapchain == null)
                    {

                        GraphicsDevice _graphicsDevice = graphicsDevice;
                        if (_graphicsDevice == null)
                        {
                            return null;
                        }

                        float logicalDpi = 96; // TMP
                        SwapchainSource ss = SwapchainSource.CreateUwp(swapChainPanel, logicalDpi);

                        SwapchainDescription scd = new SwapchainDescription(
                            ss,
                            (uint)width,
                            (uint)height,
                            PixelFormat.R32_Float,
                            false);

                        m_swapchain = _graphicsDevice.ResourceFactory.CreateSwapchain(ref scd);
                        if (m_swapchain == null)
                        {
                            return null;
                        }

                    }

                    return m_swapchain;
                }
            }

            public void Reset_swapchain()
            {
                m_swapchain?.Dispose();
                m_swapchain = null;
            }

            public Framebuffer framebuffer => swapchain?.Framebuffer;

            #endregion

            #region --- static

            public static RenderData Find(SwapChainPanel swapChainPanel, bool createIfNotFound = true)
            {
                if (swapChainPanel == null) return null;

                lock (s_aRenderData)
                {
                    RenderData renderData = s_aRenderData.FirstOrDefault(rd => rd.swapChainPanel == swapChainPanel);
                    if (renderData != null) return renderData;

                    if (createIfNotFound == false) return null;

                    s_aRenderData.Add(renderData = new RenderData(swapChainPanel));

                    return renderData;
                }

            }

            public static bool Dispose(SwapChainPanel swapChainPanel)
            {
                if (swapChainPanel == null) return false;

                RenderData renderData = null;

                lock (s_aRenderData)
                {
                    s_aRenderData.FirstOrDefault(rd => rd.swapChainPanel == swapChainPanel);
                    if (renderData == null) return false;
                    s_aRenderData.Remove(renderData);
                }

                try
                {
                    renderData?.Dispose();
                }
                catch
                {
                }

                return true;
            }

            #endregion

        }

        #endregion

    }

}
