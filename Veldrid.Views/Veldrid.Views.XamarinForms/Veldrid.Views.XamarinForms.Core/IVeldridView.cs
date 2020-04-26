using System;
using System.Collections.Generic;
using System.Text;

namespace Veldrid.Views.XamarinForms.Core
{

    interface IVeldridView
    {
        void SetRenderer(IVeldridViewRenderer veldridViewRenderer);
        void Update();

    }

}
