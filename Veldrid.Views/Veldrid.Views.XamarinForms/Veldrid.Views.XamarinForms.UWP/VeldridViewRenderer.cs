
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

using Veldrid.Views.XamarinForms.Core;
using Veldrid.Views.XamarinForms.UWP;

[assembly: ExportRenderer(typeof(Veldrid.Views.XamarinForms.Core.VeldridView), typeof(VeldridViewRenderer))]
namespace Veldrid.Views.XamarinForms.UWP
{

    public class VeldridViewRenderer : ViewRenderer<VeldridView, UIVeldridView>, IVeldridViewRenderer
    {
        VeldridView _veldridView;
        UIVeldridView _UIVeldridView;

        GraphicsDeviceOptions _options;
        GraphicsDevice _graphicsDevice; // TBD: static
        Swapchain _swapChain;

        public VeldridViewRenderer()
        {
        }

        #region --- private

        void reset_Graphics()
        {
            reset_swapchain();
            _graphicsDevice?.Dispose();
            _graphicsDevice = null;
        }

        void reset_swapchain()
        {
            _swapChain?.Dispose();
            _swapChain = null;
        }

        double width { set; get; } 
        double height { set; get; }

        GraphicsDevice graphicsDevice
        {
            get
            {
                if (_graphicsDevice == null)
                {
                    _options = new GraphicsDeviceOptions(false, null, false, ResourceBindingModel.Improved);
                    _graphicsDevice = Veldrid.D3D11.GraphicsDeviceFactory.CreateD3D11(_options);
                }
                return _graphicsDevice;
            }
        }

        Swapchain swapchain
        {
            get
            {
                int _width = (int)width;
                int _height = (int)height;

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
                    if (_graphicsDevice == null)
                    {
                        return null;
                    }

                    float logicalDpi = 96; // TMP
                    SwapchainSource ss = SwapchainSource.CreateUwp(_UIVeldridView, logicalDpi);

                    SwapchainDescription scd = new SwapchainDescription(
                        ss,
                        (uint)_width,
                        (uint)_height,
                        PixelFormat.R32_Float,
                        false);

                    _swapChain = _graphicsDevice.ResourceFactory.CreateSwapchain(ref scd);
                    if (_swapChain == null)
                    {
                        return null;
                    }

                }

                return _swapChain;
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
                this.width = width;
                this.height = height;
                // reset_swapchain();
            }
            catch (Exception E)
            {
                Debug.WriteLine($"{nameof(OnResize)} width={width} height={height} EXCEPTION={E.Message}");
            }
            finally
            {
                _veldridView?.Update(); // IMPORTANT
            }
        }

        #endregion

        #region --- protected override

        protected override void OnElementChanged(ElementChangedEventArgs<VeldridView> e)
        {
            try
            {
                Debug.WriteLine($"{nameof(OnElementChanged)}(-) 1.001");

                base.OnElementChanged(e);

                if (_veldridView != null)
                {
                    _veldridView.SetRenderer(null);
                    _veldridView = null;
                }

                if (e.OldElement != null)
                {
                    reset_Graphics();
                }

                if (e.NewElement != null)
                {
                    if (Control == null)
                    {
                        if (Control == null)
                        {
                            _UIVeldridView = new UIVeldridView();
                            SetNativeControl(_UIVeldridView);
                            Debug.WriteLine($"{nameof(OnElementChanged)}(-) _UIVeldridView0={_UIVeldridView}");
                        }
                    }

                    _veldridView = e.NewElement;
                    _veldridView.SetRenderer(this);
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
