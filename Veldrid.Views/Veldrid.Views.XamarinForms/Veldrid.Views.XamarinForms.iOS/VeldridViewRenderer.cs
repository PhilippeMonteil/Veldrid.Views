
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using Veldrid;

using System.Diagnostics;
using CoreGraphics;

using Veldrid.Views.XamarinForms.Core;
using Veldrid.Views.XamarinForms.iOS;

[assembly: ExportRenderer(typeof(Veldrid.Views.XamarinForms.Core.VeldridView), typeof(VeldridViewRenderer))]
namespace Veldrid.Views.XamarinForms.iOS
{

    public class VeldridViewRenderer : ViewRenderer<VeldridView, UIVeldridView>, IVeldridViewRenderer
    {
        VeldridView _veldridView0;
        UIVeldridView _UIVeldridView0;

        GraphicsDeviceOptions _options;

        GraphicsDevice _graphicsDevice;
        Swapchain _swapChain;

        public VeldridViewRenderer()
        {
        }

        #region --- private

        void reset_Graphics()
        {
            reset_swapchain(); // reset_Graphics
            _graphicsDevice?.Dispose();
            _graphicsDevice = null;
        }

        void reset_swapchain()
        {
            _swapChain?.Dispose();
            _swapChain = null;
        }

        GraphicsDevice graphicsDevice
        {
            get
            {
                if (_graphicsDevice == null)
                {
                    _options = new GraphicsDeviceOptions(false, null, false, ResourceBindingModel.Improved);
                    _graphicsDevice = Veldrid.MTL.GraphicsDeviceFactory.CreateMetal(_options);
                }
                return _graphicsDevice;
            }
        }

        Swapchain swapchain
        {
            get
            {
                // calage sur la taille du NativeControl
                double _width = _UIVeldridView0.Bounds.Width;
                double _height = _UIVeldridView0.Bounds.Height;

                Debug.WriteLine($"{GetType().Name}.get {nameof(swapchain)}(-) 1.600 _width={_width} _height={_height} _swapChain={_swapChain}");

                if (_width <= 0 || _height <= 0)
                {
                    return null;
                }

                if (_swapChain != null)
                {
                    Framebuffer _framebuffer = _swapChain.Framebuffer;
                    if (_framebuffer != null)
                    {
                        if (_framebuffer.Width != _width || _framebuffer.Height != _height) // TBD: pixels physiques vs logiques
                        {
                            _swapChain.Resize((uint)_width, (uint)_height); // TBC
                        }
                    }
                }

                if (_swapChain == null)
                {
                    GraphicsDevice _graphicsDevice = graphicsDevice;
                    if (_graphicsDevice == null) return null;

                    SwapchainSource ss = SwapchainSource.CreateUIView(Control.Handle);
                    SwapchainDescription scd = new SwapchainDescription(
                        ss,
                        (uint)_width,
                        (uint)_height,
                        PixelFormat.R32_Float,
                        true); // TBC

                    _swapChain = _graphicsDevice.ResourceFactory.CreateSwapchain(ref scd);
                }

                {
                    Framebuffer _framebuffer = _swapChain.Framebuffer;
                    Debug.WriteLine($"{GetType().Name}.get {nameof(swapchain)}(+) ({_width}, {_height}) -> _framebuffer.width={_framebuffer.Width} .height={_framebuffer.Height}");
                }

                return _swapChain;
            }
        }

        void test()
        {
            CommandList _commandList = null;
            try
            {
                Debug.WriteLine($"{nameof(test)}(-) 1.001");

                GraphicsDevice _graphicsDevice = graphicsDevice;
                if (_graphicsDevice == null) return;

                Swapchain _swapchain = swapchain;
                if (_swapchain == null) return;

                _commandList = _graphicsDevice.ResourceFactory.CreateCommandList();
                if (_commandList == null) return;

                _commandList.Begin();

                _commandList.SetFramebuffer(_swapchain.Framebuffer);

                _commandList.ClearColorTarget(0, RgbaFloat.Pink);

                _commandList.End();

                _graphicsDevice.SubmitCommands(_commandList);
                _graphicsDevice.SwapBuffers(_swapchain);
                _graphicsDevice.WaitForIdle();
            }
            catch (Exception E)
            {
                Debug.WriteLine($"{nameof(test)} EXCEPTION={E.Message} {E.StackTrace}");
            }
            finally
            {
                _commandList?.Dispose();
                Debug.WriteLine($"{nameof(test)}(+)");
            }
        }

        #endregion

        #region --- IVeldridViewRenderer

        public GraphicsDevice GraphicsDevice => graphicsDevice;

        public Swapchain Swapchain => swapchain;

        public void OnResize(double width, double height)
        {
            try
            {
                Debug.WriteLine($"{GetType().Name}.{nameof(OnResize)}(-) 1.300 width={width} height={height} _swapChain={_swapChain}");

                // pas de resize systématique
                // reset_swapchain(); // OnResize

                _veldridView0?.Update(); // important
            }
            catch (Exception E)
            {
            }
            finally
            {
                Debug.WriteLine($"{nameof(OnResize)}(+) width={width} height={height}");
            }
        }

        #endregion

        #region --- protected override

        protected override void OnElementChanged(ElementChangedEventArgs<VeldridView> e)
        {
            try
            {
                Debug.WriteLine($"{nameof(OnElementChanged)}(-) 1.300");

                base.OnElementChanged(e);

                if (_veldridView0 != null)
                {
                    _veldridView0.SetRenderer(null);
                    _veldridView0 = null;
                }

                if (e.OldElement != null)
                {
                    reset_Graphics(); // OnElementChanged
                }

                if (e.NewElement != null)
                {
                    if (Control == null)
                    {
                        if (Control == null)
                        {
                            _UIVeldridView0 = new UIVeldridView();
                            SetNativeControl(_UIVeldridView0);
                            Debug.WriteLine($"{nameof(OnElementChanged)}(-) _UIVeldridView0.Frame={_UIVeldridView0.Frame}");
                        }
                    }

                    _veldridView0 = e.NewElement;
                    _veldridView0.SetRenderer(this);
                }
            }
            catch (Exception E)
            {
                Debug.WriteLine($"{nameof(OnElementChanged)} EXCEPTION={E.Message} {E.StackTrace}");
            }
            finally
            {
                Debug.WriteLine($"{nameof(OnElementChanged)}(+)");
            }

        }

        #endregion

    }

}