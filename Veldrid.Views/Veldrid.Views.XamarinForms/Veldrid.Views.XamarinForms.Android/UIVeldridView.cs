
using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;

using Veldrid;
using Veldrid.Views.XamarinForms.Core;

namespace Veldrid.Views.XamarinForms.Android
{

    public class UIVeldridView : SurfaceView, ISurfaceHolderCallback
    {
        private readonly IVeldridViewRenderer _veldridViewRenderer;
        private readonly GraphicsBackend _backend;
        protected GraphicsDeviceOptions DeviceOptions { get; }
        private bool _surfaceDestroyed;
        private bool _paused;
        private bool _enabled;
        private bool _needsResize;
        private bool _surfaceCreated;
        private bool _renderLoop;

        public GraphicsDevice GraphicsDevice { get; protected set; }
        public Swapchain MainSwapchain { get; protected set; }

        public event Action Rendering;
        public event Action DeviceCreated;
        public event Action DeviceDisposed;
        public event Action Resized;

        #region --- constructors

        public UIVeldridView(IVeldridViewRenderer veldridView0Renderer, Context context, GraphicsBackend backend)
            : this(veldridView0Renderer, context, backend, new GraphicsDeviceOptions())
        {
        }

        public UIVeldridView(IVeldridViewRenderer veldridView0Renderer, Context context, GraphicsBackend backend, GraphicsDeviceOptions deviceOptions) : base(context)
        {
            if (!(backend == GraphicsBackend.Vulkan || backend == GraphicsBackend.OpenGLES))
            {
                throw new NotSupportedException($"{backend} is not supported on Android.");
            }

            _veldridViewRenderer = veldridView0Renderer;

            _backend = backend;
            DeviceOptions = deviceOptions;

            Holder.AddCallback(this);
        }

        #endregion

        #region --- ISurfaceHolderCallback

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            bool deviceCreated = false;
            if (_backend == GraphicsBackend.Vulkan)
            {
                if (GraphicsDevice == null)
                {
                    GraphicsDevice = Veldrid.Vk.GraphicsDeviceFactory.CreateVulkan(DeviceOptions);
                    deviceCreated = true;
                }

                Debug.Assert(MainSwapchain == null);
                SwapchainSource ss = SwapchainSource.CreateAndroidSurface(holder.Surface.Handle, JNIEnv.Handle);
                SwapchainDescription sd = new SwapchainDescription(
                    ss,
                    (uint)Width,
                    (uint)Height,
                    DeviceOptions.SwapchainDepthFormat,
                    DeviceOptions.SyncToVerticalBlank);
                MainSwapchain = GraphicsDevice.ResourceFactory.CreateSwapchain(sd);
            }
            else
            {
                Debug.Assert(GraphicsDevice == null && MainSwapchain == null);
                SwapchainSource ss = SwapchainSource.CreateAndroidSurface(holder.Surface.Handle, JNIEnv.Handle);
                SwapchainDescription sd = new SwapchainDescription(
                    ss,
                    (uint)Width,
                    (uint)Height,
                    DeviceOptions.SwapchainDepthFormat,
                    DeviceOptions.SyncToVerticalBlank);
                GraphicsDevice = Veldrid.OpenGL.GraphicsDeviceFactory.CreateOpenGLES(DeviceOptions, sd);
                MainSwapchain = GraphicsDevice.MainSwapchain;
                deviceCreated = true;
            }

            if (deviceCreated)
            {
                DeviceCreated?.Invoke();
            }

            _surfaceCreated = true;
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            _surfaceDestroyed = true;
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            Debug.WriteLine($"{nameof(SurfaceChanged)} _renderLoop={_renderLoop} width={width} height={height}");
            if (_renderLoop)
            {
                _needsResize = true;
            }
            else
            {
                MainSwapchain.Resize((uint)Width, (uint)Height);
                try
                {
                    Resized?.Invoke();
                }
                catch
                {
                }
            }
        }

        #endregion

        #region --- public

        public void Disable()
        {
            _enabled = false;
        }

        public void RunContinuousRenderLoop()
        {
            Task.Factory.StartNew(() => RenderLoop(), TaskCreationOptions.LongRunning);
        }

        private void RenderLoop()
        {
            try
            {
                _renderLoop = true;
                _enabled = true;
                while (_enabled)
                {
                    try
                    {
                        if (_paused || !_surfaceCreated) { continue; }

                        if (_surfaceDestroyed)
                        {
                            HandleSurfaceDestroyed();
                            continue;
                        }

                        if (_needsResize)
                        {
                            _needsResize = false;
                            MainSwapchain.Resize((uint)Width, (uint)Height);
                            Resized?.Invoke();
                        }

                        if (GraphicsDevice != null)
                        {
                            Rendering?.Invoke();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Encountered an error while rendering: " + e);
                        throw;
                    }
                }
            }
            finally
            {
                _renderLoop = false;
            }
        }

        private void HandleSurfaceDestroyed()
        {
            if (_backend == GraphicsBackend.Vulkan)
            {
                MainSwapchain.Dispose();
                MainSwapchain = null;
            }
            else
            {
                GraphicsDevice.Dispose();
                GraphicsDevice = null;
                MainSwapchain = null;
                DeviceDisposed?.Invoke();
            }
        }

        public void OnPause()
        {
            _paused = true;
        }

        public void OnResume()
        {
            _paused = false;
        }

        #endregion

    }

}