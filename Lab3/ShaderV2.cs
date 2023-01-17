using OpenTK.Graphics.OpenGL;
using System;
using System.Reflection.Metadata;

namespace JStudio.OpenGL
{

    public class Shader
    {
        public readonly string Name;
        public int UniformModelMtx { get; private set; }
        public int UniformViewMtx { get; private set; }
        public int UniformProjMtx { get; private set; }

        private int m_vertexAddress = -1;
        private int m_fragmentAddress = -1;
        private int m_programAddress = -1;

        public Shader(string name)
        {
            Name = name;
        }

        public void Bind()
        {
            GL.UseProgram(m_programAddress);
        }

        public bool CompileSource(string code, ShaderType type)
        {
            // Generate a new shader and clean up the old shader with a warning if they forgot to link before trying to compile again.
            int shaderAddress = -1;
            switch (type)
            {
                case ShaderType.FragmentShader:
                    if (m_fragmentAddress >= 0)
                    {
                        Console.WriteLine("Shader \"{0}\" called CompileSource for ShaderType: {1} twice before linking! Disposing old shader.", Name, type);
                        GL.DeleteShader(m_fragmentAddress);
                    }
                    m_fragmentAddress = GL.CreateShader(type);
                    shaderAddress = m_fragmentAddress;
                    break;
                case ShaderType.VertexShader:
                    if (m_vertexAddress >= 0)
                    {
                        Console.WriteLine("Shader \"{0}\" called CompileSource for ShaderType: {1} twice before linking! Disposing old shader.", Name, type);
                        GL.DeleteShader(m_fragmentAddress);
                    }
                    m_vertexAddress = GL.CreateShader(type);
                    shaderAddress = m_vertexAddress;
                    break;
            }

            GL.ShaderSource(shaderAddress, code);
            GL.CompileShader(shaderAddress);

            int compileStatus;
            GL.GetShader(shaderAddress, ShaderParameter.CompileStatus, out compileStatus);

            if (compileStatus != 1)
            {
                Console.WriteLine("Failed to compile {0} shader {1}. Log:\n{2}", type, Name, GL.GetShaderInfoLog(shaderAddress));
                return false;
            }

            return true;
        }

        public bool LinkShader()
        {
            if (m_programAddress >= 0)
            {
                Console.WriteLine("Shader \"{0}\" called LinkShader for already linked shader! Disposing old program.", Name);
                GL.DeleteProgram(m_programAddress);
            }

            if (m_fragmentAddress < 0 || m_vertexAddress < 0)
                throw new Exception("Shader does not have both a Vertex and Fragment shader!");

            // Initialize a program and link the already compiled shaders
            m_programAddress = GL.CreateProgram();
            GL.AttachShader(m_programAddress, m_vertexAddress);
            GL.AttachShader(m_programAddress, m_fragmentAddress);

            GL.LinkProgram(m_programAddress);


            // Now that the program is linked, bind to our uniform locations.
            UniformModelMtx = GL.GetUniformLocation(m_programAddress, "model");
            UniformViewMtx = GL.GetUniformLocation(m_programAddress, "view");
            UniformProjMtx = GL.GetUniformLocation(m_programAddress, "projection");


            // Now that we've (presumably) set both a vertex and a fragment shader and linked them to the program,
            // we're going to clean up the reference to the shaders as the Program now keeps its own reference.
            GL.DeleteShader(m_vertexAddress);
            GL.DeleteShader(m_fragmentAddress);
            m_vertexAddress = -1;
            m_fragmentAddress = -1;
            return true;
        }


        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(m_programAddress, attribName);
        }

        ~Shader()
        {
            if (m_vertexAddress >= 0)
                GL.DeleteShader(m_vertexAddress);
            if (m_fragmentAddress >= 0)
                GL.DeleteShader(m_fragmentAddress);

            if (m_programAddress >= -1)
                GL.DeleteProgram(m_programAddress);

            GC.SuppressFinalize(this);
        }
    }
}