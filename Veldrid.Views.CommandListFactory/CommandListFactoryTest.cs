using System;
using System.Collections.Generic;
using System.Text;

using Veldrid;
using Veldrid.Views.Contracts;

namespace Veldrid.Views.CommandListFactory
{

    public class CommandListFactoryTest : ICommandListFactory
    {
        public CommandListFactoryTest()
        {
        }

        public CommandList BuildCommandList(GraphicsDevice graphicsDevice, Framebuffer framebuffer)
        {
            Veldrid.CommandList _commandList = null;
            try
            {
                _commandList = graphicsDevice.ResourceFactory.CreateCommandList();
                if (_commandList == null) return null;

                _commandList.Begin();

                _commandList.SetFramebuffer(framebuffer);
                _commandList.SetFullViewports();

                _commandList.ClearColorTarget(0, RgbaFloat.DarkRed);

                _commandList.End();

                Veldrid.CommandList vret = _commandList;
                _commandList = null;

                return vret;
            }
            finally
            {
                _commandList?.Dispose();
            }
        }
    }

}
