using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Veldrid;
using Veldrid.Views.Contracts;

namespace Veldrid.Views.CommandListFactory
{

    public class CommandListFactoryTest2D : ICommandListFactory
    {

        #region --- static (Resources)

        static Stream loadResourceStream(Assembly Assembly, string Name)
        {
            if (Assembly == null) return null;
            if (string.IsNullOrEmpty(Name)) return null;

            string[] resources = Assembly.GetManifestResourceNames();
            if (resources == null) return null;

            foreach (string resource in resources)
            {
                if (resource.Contains(Name))
                {
                    return Assembly.GetManifestResourceStream(resource);
                }
            }

            return null;
        }

        static string loadResourceString(Assembly Assembly, string Name)
        {
            StringBuilder _sb = new StringBuilder();

            Stream _stream = null;
            StreamReader _streamReader = null;
            try
            {
                _stream = loadResourceStream(Assembly, Name);
                if (_stream == null) return null;

                _streamReader = new StreamReader(_stream);

                while (_streamReader.EndOfStream == false)
                {
                    _sb.AppendLine(_streamReader.ReadLine());
                }

            }
            finally
            {
                _streamReader?.Dispose();
                _stream?.Close();
            }

            return _sb.ToString();
        }

        #endregion

        #region --- static (Buffers)

        static short convert_float_to_16SNorm(float Value) // Value: [-1, 1] -> [short.MinValue, short.MaxValue]
        {
            float _v = (float)short.MinValue + ((float)short.MaxValue - (float)short.MinValue) * (Value + 1) / 2;
            if (_v < (float)short.MinValue) _v = (float)short.MinValue;
            if (_v > (float)short.MaxValue) _v = (float)short.MaxValue;
            return (short)_v;
        }

        static byte convert_float_to_8UNorm(float Value) // Value: [0, 1] -> [byte.MinValue, byte.MaxValue]
        {
            float _v = (float)byte.MinValue + ((float)byte.MaxValue - (float)byte.MinValue) * Value;
            if (_v < (float)byte.MinValue) _v = (float)byte.MinValue;
            if (_v > (float)byte.MaxValue) _v = (float)byte.MaxValue;
            return (byte)_v;
        }

        static void write(BinaryWriter BW, float X, float Y, float R, float G, float B, float A)
        {
            BW.Write(convert_float_to_16SNorm(X));
            BW.Write(convert_float_to_16SNorm(Y));
            BW.Write(convert_float_to_8UNorm(R));
            BW.Write(convert_float_to_8UNorm(G));
            BW.Write(convert_float_to_8UNorm(B));
            BW.Write(convert_float_to_8UNorm(A));
        }

        static byte[] Build_2DColoredQuads_16SNorm_8UNorm(int HCount, int VCount, out int VerticeCount)
        {
            VerticeCount = 0;

            MemoryStream _ms = null;
            try
            {
                #region --- try
                _ms = new MemoryStream();

                BinaryWriter _bw = new BinaryWriter(_ms);

                int _verticeCount = 0;

                for (int _y = 0; _y < VCount; _y++)
                {
                    float _yi = -1f + (float)(2 * _y) / (float)VCount;
                    float _yf = -1f + (float)(2 * (_y + 1)) / (float)VCount;
                    for (int _x = 0; _x < HCount; _x++)
                    {
                        float _xi = -1f + (float)(2 * _x) / (float)HCount;
                        float _xf = -1f + (float)(2 * (_x + 1)) / (float)HCount;
                        //
                        write(_bw, _xi, _yi, 1, 0, 0, 1);
                        write(_bw, _xf, _yf, 1, 0, 0, 1);
                        write(_bw, _xf, _yi, 0, 1, 0, 1);
                        //
                        write(_bw, _xi, _yi, 1, 0, 0, 1);
                        write(_bw, _xf, _yf, 1, 0, 0, 1);
                        write(_bw, _xi, _yf, 0, 0, 1, 1);
                        //
                        _verticeCount += 6;
                    }
                }

                _bw.Flush();

                VerticeCount = _verticeCount;

                return _ms.ToArray();
                #endregion
            }
            finally
            {
                _ms?.Dispose();
            }

        }

        #endregion

        public CommandListFactoryTest2D()
        {
        }

        unsafe public CommandList BuildCommandList(GraphicsDevice graphicsDevice, Framebuffer framebuffer)
        {
            ResourceFactory factory = graphicsDevice.ResourceFactory;
            Shader _vsShader = null;
            Shader _psShader = null;
            DeviceBuffer _vertexBuffer = null;
            Pipeline _pipeline = null;
            Veldrid.CommandList _commandList = null;
            try
            {
                #region --- try

                #region --- shaders
                {
                    string _vsname = null;
                    string _psname = null;

                    switch (graphicsDevice.BackendType)
                    {
                        case GraphicsBackend.Direct3D11:
                            _vsname = "Test_Colored2DVertices.vs";
                            _psname = "Test_Colored2DVertices.ps";
                            break;

                        case GraphicsBackend.Metal:
                            _vsname = "Test_Colored2DVertices_vs.msl";
                            _psname = "Test_Colored2DVertices_ps.msl";
                            break;

                        case GraphicsBackend.Vulkan:
                            _vsname = "Test_Colored2DVertices_vs.spv";
                            _psname = "Test_Colored2DVertices_ps.spv";
                            break;

                        case GraphicsBackend.OpenGL:
                            _vsname = "Test_Colored2DVertices_vs.glsl";
                            _psname = "Test_Colored2DVertices_ps.glsl";
                            break;

                        case GraphicsBackend.OpenGLES:
                            _vsname = "Test_Colored2DVertices_vs_es.glsl";
                            _psname = "Test_Colored2DVertices_ps_es.glsl";
                            break;
                    }

                    string _vs = loadResourceString(Assembly.GetExecutingAssembly(), _vsname);
                    if (string.IsNullOrEmpty(_vs)) return null;
                    string _ps = loadResourceString(Assembly.GetExecutingAssembly(), _psname);
                    if (string.IsNullOrEmpty(_ps)) return null;

                    ShaderDescription vertexShaderDesc = new ShaderDescription(
                        ShaderStages.Vertex,
                        Encoding.UTF8.GetBytes(_vs),
                        "_VertexShader");
                    ShaderDescription fragmentShaderDesc = new ShaderDescription(
                        ShaderStages.Fragment,
                        Encoding.UTF8.GetBytes(_ps),
                        "_PixelShader");

                    _vsShader = factory.CreateShader(vertexShaderDesc);
                    _psShader = factory.CreateShader(fragmentShaderDesc);
                }
                #endregion

                VertexLayoutDescription _vertexLayout = new VertexLayoutDescription();
                int _vertexCount;

                #region --- _vertexBuffer + _vertexLayout
                {
                    int _count = 10;
                    byte[] _data = Build_2DColoredQuads_16SNorm_8UNorm(_count, _count, out _vertexCount);
                    //
                    Veldrid.BufferDescription vbDescription = new Veldrid.BufferDescription(
                                                                (uint)_data.Length,
                                                                BufferUsage.VertexBuffer);
                    _vertexBuffer = factory.CreateBuffer(vbDescription);

                    fixed (byte* _pdata = _data)
                    {
                        graphicsDevice.UpdateBuffer(_vertexBuffer, bufferOffsetInBytes: 0, (IntPtr)_pdata, (uint)_data.Length);
                    }

                    _vertexLayout = new VertexLayoutDescription(
                        new VertexElementDescription("Position",
                                                        VertexElementSemantic.Position,
                                                        VertexElementFormat.Short2_Norm,
                                                        offset: 0),
                        new VertexElementDescription("Color",
                                                        VertexElementSemantic.Color,
                                                        VertexElementFormat.Byte4_Norm,
                                                        offset: 2 * sizeof(short)));

                }
                #endregion

                #region --- pipeline
                {
                    GraphicsPipelineDescription _pipelineDescription = new GraphicsPipelineDescription();

                    _pipelineDescription.BlendState = Veldrid.BlendStateDescription.SingleOverrideBlend;

                    _pipelineDescription.DepthStencilState = new Veldrid.DepthStencilStateDescription(
                        depthTestEnabled: false,
                        depthWriteEnabled: false,
                        comparisonKind: ComparisonKind.LessEqual);

                    _pipelineDescription.RasterizerState = new Veldrid.RasterizerStateDescription(
                        cullMode: FaceCullMode.None,
                        fillMode: PolygonFillMode.Solid,
                        frontFace: FrontFace.Clockwise,
                        depthClipEnabled: true, // Android ?
                        scissorTestEnabled: false);

                    _pipelineDescription.PrimitiveTopology = Veldrid.PrimitiveTopology.TriangleList;
                    _pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();

                    _pipelineDescription.ShaderSet = new ShaderSetDescription(
                        vertexLayouts: new VertexLayoutDescription[] { _vertexLayout },
                        shaders: new Shader[] { _vsShader, _psShader });

                    _pipelineDescription.Outputs = framebuffer.OutputDescription; // _offscreenFB.OutputDescription;

                    _pipeline = factory.CreateGraphicsPipeline(_pipelineDescription);
                }
                #endregion

                #region --- commandList
                {
                    _commandList = factory.CreateCommandList();

                    // Begin() must be called before commands can be issued.
                    _commandList.Begin();

                    _commandList.SetPipeline(_pipeline);

                    // We want to render directly to the output window.
                    _commandList.SetFramebuffer(framebuffer);

                    _commandList.SetFullViewports();
                    _commandList.ClearColorTarget(0, RgbaFloat.DarkRed);

                    // Set all relevant state to draw our quad.
                    _commandList.SetVertexBuffer(0, _vertexBuffer);

                    _commandList.Draw(vertexCount: (uint)_vertexCount);

                    // End() must be called before commands can be submitted for execution.
                    _commandList.End();
                }
                #endregion

                CommandList vret = _commandList;
                _commandList = null;

                return vret;
                #endregion
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                _vsShader?.Dispose();
                _psShader?.Dispose();
                _vertexBuffer?.Dispose();
                _pipeline?.Dispose();
                _commandList?.Dispose();
            }
        }
    }

}
