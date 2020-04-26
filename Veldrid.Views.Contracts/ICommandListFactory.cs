using System;
using System.Collections.Generic;
using System.Text;

using Veldrid;

namespace Veldrid.Views.Contracts
{

    public interface ICommandListFactory
    {
        Veldrid.CommandList BuildCommandList(Veldrid.GraphicsDevice graphicsDevice, Veldrid.Framebuffer framebuffer);
    }

}
