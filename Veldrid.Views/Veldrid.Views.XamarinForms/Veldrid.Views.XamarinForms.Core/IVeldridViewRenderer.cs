using System;
using System.Collections.Generic;
using System.Text;

using Veldrid;

namespace Veldrid.Views.XamarinForms.Core
{


    public interface IVeldridViewRenderer
    {
        GraphicsDevice GraphicsDevice { get; }
        Swapchain Swapchain { get; }

        void OnResize(double width, double height);
    }

}
