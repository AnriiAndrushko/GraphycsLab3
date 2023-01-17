using OpenTK.Graphics.OpenGL;
using JStudio.OpenGL;
using OpenTK.Mathematics;
using Shader = JStudio.OpenGL.Shader;

namespace Lab3
{
    public class SimpleObjRenderer : IDisposable
    {
        struct UniqueVertex
        {
            public Vector3 Position;
            public Vector2 TexCoord;
            public Vector3 Normal;
        }

        private int m_vertexVBO;
        private int m_indexVBO;
        private int m_texcoordVBO;
        private int m_normalVBO;
        private int m_textureVBO;

        private Shader m_unhighlightedShader;
        private int m_triangleCount;

        private int rawPos;
        private int rawTex;
        private int rawNorm;

        // To detect redundant calls
        private bool m_hasBeenDisposed = false;


        public SimpleObjRenderer(Obj file)
        {
            m_vertexVBO = GL.GenBuffer();
            m_indexVBO = GL.GenBuffer();
            m_texcoordVBO = file.TexCoords.Count > 0 ? GL.GenBuffer() : -1;
            m_normalVBO = file.Normals.Count > 0 ? GL.GenBuffer() : -1;
            m_textureVBO = file.Material.DiffuseTexture != null ? GL.GenTexture() : -1;

            m_unhighlightedShader = new Shader("UnlitTexture");
            m_unhighlightedShader.CompileSource(File.ReadAllText("Shaders/UnlitTexture.vert"), ShaderType.VertexShader);
            m_unhighlightedShader.CompileSource(File.ReadAllText("Shaders/UnlitTexture.frag"), ShaderType.FragmentShader);
            m_unhighlightedShader.LinkShader();

            rawPos = m_unhighlightedShader.GetAttribLocation("RawPosition");
            rawTex = m_unhighlightedShader.GetAttribLocation("RawTex0");
            rawNorm = m_unhighlightedShader.GetAttribLocation("RawNormal");

            // Generate an array of all vertices instead of the compact form OBJ comes as.
            Vector3[] positions = null;
            Vector2[] texcoords = null;
            Vector3[] normals = null;
            int[] triangles = new int[file.Faces.Count * 3];
            m_triangleCount = file.Faces.Count;

            List<UniqueVertex> uniqueVerts = new List<UniqueVertex>();

            for (int i = 0; i < file.Faces.Count; i++)
            {
                Obj.ObjFace face = file.Faces[i];
                for (int k = 0; k < 3; k++)
                {
                    var vertex = new UniqueVertex();
                    vertex.Position = file.Vertices[face.Positions[k]];
                    if (face.TexCoords != null) vertex.TexCoord = file.TexCoords[face.TexCoords[k]];
                    if (face.Normals != null) vertex.Normal = file.Normals[face.Normals[k]];


                    int vertIndex = uniqueVerts.IndexOf(vertex);
                    if (vertIndex < 0)
                    {
                        uniqueVerts.Add(vertex);
                        vertIndex = uniqueVerts.Count - 1;
                    }

                    triangles[(i * 3) + k] = vertIndex;
                }
            }

            // Copy the data out of the interlaced buffers.
            positions = new Vector3[uniqueVerts.Count];
            texcoords = file.TexCoords.Count > 0 ? new Vector2[uniqueVerts.Count] : null;
            normals = file.Normals.Count > 0 ? new Vector3[uniqueVerts.Count] : null;

            for (int i = 0; i < uniqueVerts.Count; i++)
            {
                positions[i] = uniqueVerts[i].Position;

                if (texcoords != null) texcoords[i] = new Vector2(uniqueVerts[i].TexCoord.X, 1 - uniqueVerts[i].TexCoord.Y);
                if (normals != null) normals[i] = uniqueVerts[i].Normal;
            }

            // Positions
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(12 * positions.Length), positions, BufferUsageHint.StaticDraw);

            // Upload Indexes
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_indexVBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(4 * triangles.Length), triangles, BufferUsageHint.StaticDraw);

            // Texcoords
            if (m_texcoordVBO >= 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, m_texcoordVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(8 * texcoords.Length), texcoords, BufferUsageHint.StaticDraw);
            }

            // Normals
            if (m_normalVBO >= 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, m_normalVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(12 * normals.Length), normals, BufferUsageHint.StaticDraw);
            }

            // Texture
            if (m_textureVBO >= 0)
            {
                Obj.ObjMaterial mat = file.Material;

                GL.BindTexture(TextureTarget.Texture2D, m_textureVBO);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                System.Drawing.Imaging.BitmapData bmpData = mat.DiffuseTexture.LockBits(new System.Drawing.Rectangle(0, 0, mat.DiffuseTexture.Width, mat.DiffuseTexture.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, mat.DiffuseTexture.Width, mat.DiffuseTexture.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
                mat.DiffuseTexture.UnlockBits(bmpData);
            }
        }

        public void Render(Matrix4 viewMatrix, Matrix4 projMatrix, Matrix4 modelMatrix, Vector3 lightPos, Vector3 viewPos)
        {
            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.CullFace(CullFaceMode.Front);
            //GL.Enable(EnableCap.CullFace);
            //GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DepthMask(true);

            Shader curShader =  m_unhighlightedShader;
            curShader.Bind();

            GL.UniformMatrix4(curShader.UniformModelMtx, false, ref modelMatrix);
            GL.UniformMatrix4(curShader.UniformViewMtx, false, ref viewMatrix);
            GL.UniformMatrix4(curShader.UniformProjMtx, false, ref projMatrix);

            // Position
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vertexVBO);
            GL.EnableVertexAttribArray(rawPos);
            GL.VertexAttribPointer(rawPos, 3, VertexAttribPointerType.Float, false, 12, 0);

            // Texcoord
            if (m_texcoordVBO >= 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, m_texcoordVBO);
                GL.EnableVertexAttribArray(rawTex);
                GL.VertexAttribPointer(rawTex, 2, VertexAttribPointerType.Float, false, 8, 0);
            }

            // Normals
            if (m_normalVBO >= 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, m_normalVBO);
                GL.EnableVertexAttribArray(rawNorm);
                GL.VertexAttribPointer(rawNorm, 3, VertexAttribPointerType.Float, true, 12, 0);
            }

            // Texture
            if (m_textureVBO >= 0)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, m_textureVBO);
            }

            // EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_indexVBO);

            // Draw!
            GL.DrawElements(BeginMode.Triangles, m_triangleCount * 3, DrawElementsType.UnsignedInt, 0);

            // Disable all of our shit.

            GL.Disable(EnableCap.Blend);
            //GL.Disable(EnableCap.DepthTest);

            GL.DisableVertexAttribArray(rawPos);
            GL.DisableVertexAttribArray(rawTex);
            GL.DisableVertexAttribArray(rawNorm);
            GL.BindTexture(TextureTarget.Texture2D, -1);
        }

        ~SimpleObjRenderer()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool manualDispose)
        {
            if (!m_hasBeenDisposed)
            {
                GL.DeleteBuffer(m_vertexVBO);
                GL.DeleteBuffer(m_indexVBO);
                GL.DeleteBuffer(m_normalVBO);
                GL.DeleteBuffer(m_texcoordVBO);
                GL.DeleteTexture(m_textureVBO);

                m_hasBeenDisposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}