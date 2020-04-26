
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

using Xamarin.Forms;

using Veldrid;

using Veldrid.Views.Contracts;

namespace Veldrid.Views.XamarinForms.Core
{

    public class VeldridViewPainter
    {

        public static readonly BindableProperty CommandListFactoryProperty =
            BindableProperty.CreateAttached(
                propertyName: "CommandListFactory",
                returnType: typeof(ICommandListFactory),
                declaringType: typeof(VeldridViewPainter),
                defaultValue: null,
                defaultBindingMode: BindingMode.OneWay,
                validateValue: null,
                propertyChanged: OnCommandListFactoryChanged);

        // NB: le traitement des events SizeChanged est inutile: il est fait en interne

        static void OnCommandListFactoryChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var veldridview = bindable as VeldridView;
            if (veldridview == null) return;

            if (oldValue != null)
            {
                bool _old = ViewData.Dispose(veldridview);
                if (_old)
                {
                    veldridview.OnRendererSet -= Veldridview_OnRendererSet;
                    veldridview.OnUpdate -= Veldridview_OnUpdate;
                }
            }

            ICommandListFactory _newValue = newValue as ICommandListFactory;
            if (_newValue == null)
            {
                return;
            }

            ViewData viewData = ViewData.Find(veldridview);
            if (viewData == null) return;

            viewData.commandListFactory = _newValue;

            veldridview.OnRendererSet += Veldridview_OnRendererSet;
            veldridview.OnUpdate += Veldridview_OnUpdate;
        }

        #region --- VeldridView events

        private static void Veldridview_OnUpdate(object sender, EventArgs e)
        {
            var veldridview = sender as VeldridView;
            paint(veldridview);
        }

        private static void Veldridview_OnRendererSet(object sender, EventArgs e)
        {
            var veldridview = sender as VeldridView;

            ViewData viewData = ViewData.Find(veldridview);
            if (viewData == null) return;

            viewData.Enabled = true; // IMPORTANT

            paint(veldridview);
        }

        #endregion

        #region --- static

        static void paint(VeldridView veldridview)
        {
            CommandList _commandList = null;
            try
            {
                Debug.WriteLine($"{nameof(paint)}(-) veldridview.Width={veldridview.Width} .Height={veldridview.Height}");

                ViewData viewData = ViewData.Find(veldridview);
                if (viewData == null) return;

                if (viewData.Enabled == false) return;

                ICommandListFactory commandListFactory = viewData.commandListFactory;
                if (commandListFactory == null) return;

                GraphicsDevice _graphicsDevice = veldridview?.VeldridView0Renderer?.GraphicsDevice;
                if (_graphicsDevice == null) return;

                Swapchain swapchain = veldridview?.VeldridView0Renderer?.Swapchain;
                if (swapchain == null) return;

                Framebuffer _framebuffer = swapchain.Framebuffer;
                if (_framebuffer == null) return;

                Debug.WriteLine($"{nameof(paint)} veldridview.Width={veldridview.Width} .Height={veldridview.Height}");
                Debug.WriteLine($"{nameof(paint)} _framebuffer.Width={_framebuffer.Width} .Height={_framebuffer.Height}");

                _commandList = commandListFactory.BuildCommandList(_graphicsDevice, _framebuffer);
                if (_commandList == null)
                {
                    return;
                }

                _graphicsDevice.SubmitCommands(_commandList);
                _graphicsDevice.SwapBuffers(swapchain);
            }
            catch (Exception E)
            {
            }
            finally
            {
                _commandList?.Dispose();
                Debug.WriteLine($"{nameof(paint)}(+) veldridview.Width={veldridview.Width} .Height={veldridview.Height}");
            }
        }

        #endregion

        #region --- public static

        public static ICommandListFactory GetCommandListFactory(BindableObject bindable)
        {
            return (ICommandListFactory)bindable.GetValue(CommandListFactoryProperty);
        }

        public static void SetCommandListFactory(BindableObject bindable, ICommandListFactory value)
        {
            bindable.SetValue(CommandListFactoryProperty, value);
        }

        public static void Update(VeldridView veldridview)
        {
            paint(veldridview);
        }

        #endregion

        #region --- class ViewData

        class ViewData
        {
            readonly VeldridView m_veldridView;

            ViewData(VeldridView veldridView)
            {
                m_veldridView = veldridView;
            }

            public override string ToString()
            {
                return $"{GetType().Name} Enabled={Enabled} m_veldridView={m_veldridView}";
            }

            public bool Enabled { set; get; }

            public VeldridView veldridView => m_veldridView;

            public ICommandListFactory commandListFactory { set; get; }

            #region --- public static

            static List<ViewData> s_apViewData = new List<ViewData>();

            public static ViewData Find(VeldridView veldridView, bool createIfNotFound = true)
            {
                if (veldridView == null) return null;
                lock (s_apViewData)
                {
                    ViewData viewData = s_apViewData.FirstOrDefault(vd => vd.veldridView == veldridView);
                    if (viewData != null) return viewData;

                    if (createIfNotFound == false) return null;

                    s_apViewData.Add(viewData = new ViewData(veldridView));

                    return viewData;
                }
            }

            public static bool Dispose(VeldridView veldridView)
            {
                if (veldridView == null) return false;

                lock (s_apViewData)
                {
                    ViewData viewData = s_apViewData.FirstOrDefault(vd => vd.veldridView == veldridView);
                    if (viewData == null) return false;

                    s_apViewData.Remove(viewData);

                    return true;
                }

            }

            #endregion

        }

        #endregion
    }

}
