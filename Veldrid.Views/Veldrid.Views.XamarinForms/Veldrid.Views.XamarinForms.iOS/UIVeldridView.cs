using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Veldrid.Views.XamarinForms.Core;

namespace Veldrid.Views.XamarinForms.iOS
{

    public class UIVeldridView : UIView
    {

        public UIVeldridView()
        {
            this.Layer.MasksToBounds = true; // IMPORTANT
        }

    }

}
