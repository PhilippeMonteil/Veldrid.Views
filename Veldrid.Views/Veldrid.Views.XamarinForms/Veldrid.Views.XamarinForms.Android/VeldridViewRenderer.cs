using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Veldrid.Views.XamarinForms.Core;
using Veldrid.Views.XamarinForms.Android;

[assembly: ExportRenderer(typeof(Veldrid.Views.XamarinForms.Core.VeldridView), typeof(VeldridViewRenderer))]
namespace Veldrid.Views.XamarinForms.Android
{

    public class VeldridViewRenderer : ViewRenderer<VeldridView, UIVeldridView>, IVeldridViewRenderer
    {
        VeldridView _veldridView;
        UIVeldridView _UIVeldridView;

        public VeldridViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VeldridView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                VeldridView __veldridView0 = e.OldElement as VeldridView;
                __veldridView0?.SetRenderer(null);
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    _UIVeldridView = new UIVeldridView(this, Context, GraphicsBackend.OpenGLES);
                    SetNativeControl(_UIVeldridView);
                    //
                    _UIVeldridView.Resized += _UIVeldridView_Resized;
                }

                _veldridView = e.NewElement;
                _veldridView.SetRenderer(this);
            }
        }

        private void _UIVeldridView_Resized()
        {
            _veldridView?.Dispatcher?.BeginInvokeOnMainThread(() =>
            {
                _veldridView?.Update();
            });
        }

        #region --- IVeldridViewRenderer

        public GraphicsDevice GraphicsDevice => _UIVeldridView?.GraphicsDevice;

        public Swapchain Swapchain => _UIVeldridView?.MainSwapchain;

        public void OnResize(double width, double height)
        {
        }

        #endregion

    }

}