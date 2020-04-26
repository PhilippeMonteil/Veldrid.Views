
using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using Veldrid;

namespace Veldrid.Views.XamarinForms.Core
{

    public class VeldridView : View, IVeldridView
    {
        IVeldridViewRenderer m_veldridViewRenderer;

        public event EventHandler OnRendererSet;
        public event EventHandler OnUpdate;

        public VeldridView()
        {
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            m_veldridViewRenderer?.OnResize(width, height); // IMPORTANT
        }

        #region --- IVeldridView

        public void SetRenderer(IVeldridViewRenderer veldridViewRenderer)
        {
            m_veldridViewRenderer = veldridViewRenderer;
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    OnRendererSet?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception E)
                {
                }
            });
        }

        public void Update()
        {
            // IMPORTANT: BeginInvokeOnMainThread
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    OnUpdate?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception E)
                {
                }
            });
        }

        #endregion

        #region --- public

        public IVeldridViewRenderer VeldridView0Renderer => m_veldridViewRenderer;

        #endregion

        #region --- public static

        public static CommandList BuildTest(GraphicsDevice graphicsDevice, Framebuffer framebuffer, RgbaFloat color)
        {
            CommandList _commandList = graphicsDevice.ResourceFactory.CreateCommandList();
            if (_commandList == null) return null;

            _commandList.Begin();

            _commandList.SetFramebuffer(framebuffer);

            _commandList.ClearColorTarget(0, color);

            _commandList.End();

            return _commandList;
        }

        #endregion

    }

}
