using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using MyUtils;

namespace Lab2
{
    public class Window : GameWindow
    {
        private readonly float[] _cubeVertexes =
        {
            // Position         Texture coordinates
             1f,  1f, 1f, 1.0f, 1.0f, // top right
             1f, -1f, 1f, 1.0f, 0.0f, // bottom right
            -1f, -1f, 1f, 0.0f, 0.0f, // bottom left
            -1f,  1f, 1f, 0.0f, 1.0f,  // top left

             1f,  1f, -1f, 0.0f, 1.0f, // back top right
             1f, -1f, -1f, 0.0f, 0.0f, // back bottom right
            -1f, -1f, -1f, 1.0f, 0.0f, // back bottom left
            -1f,  1f, -1f, 1.0f, 1.0f,  // back top left
            // different tex cords
             1f,  1f, -1f, 1.0f, 0.0f, 
             1f, -1f, -1f, 1.0f, 1.0f, 
            -1f, -1f, -1f, 0.0f, 1.0f, 
            -1f,  1f, -1f, 0.0f, 0.0f,  

        };

        private readonly uint[] _cubeIndexes =
        {
            0, 1, 3,
            1, 2, 3,
            0, 5, 1,
            0, 4, 5,
            1, 9, 10,
            1, 2, 10,
            2, 3, 7,
            2, 7, 6,
            3, 11,0,
            0, 8, 11,
            6, 4, 7,
            6, 4, 5,
        };

        private readonly float[] _planeVertexes =
        {
            // Position
            6f,  -1f, 6f,// top right
            -6f, -1f, 6f, // bottom right
            -6f, -1f, -6f, // bottom left
            6f,  -1f, -6f,// top left
        };
        private readonly uint[] _planeIndexes =
        {
            0,1,2,
            0,2,3,
        };

        private float[] _funkPlaneVertexes;
        private uint[] _funkPlaneIndexes;

        private bool _isPerspective = true;

        private int _elementBufferObjectCube;
        private int _vertexBufferObjectCube;
        private int _vertexArrayCube;

        private int _vertexBufferObjectTorus;
        private int _vertexArrayObjectTorus;

        private int _vertexBufferObjectPlane;
        private int _vertexArrayObjectPlane;
        private int _elementBufferObjectPlane;

        private int _vertexBufferObjectFunk;
        private int _vertexArrayObjectFunk;
        private int _elementBufferObjectFunk;

        private Shader _cubeShader;

        private Shader _simpleColorShader;

        private Texture _texture;

        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        private float[] _torus;

        private double _time;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            PlaneGenerator taskFunk = new PlaneGenerator(((float x, float y) input) => (float)(Math.Sin(input.x) + Math.Cos(input.y)));
            _funkPlaneVertexes = taskFunk.Vertex;
            _funkPlaneIndexes = taskFunk.Indexes;

            _torus = Shapes.GetTorusQuadsVert(30,30);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            //torus
            _vertexArrayObjectTorus = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObjectTorus);

            _vertexBufferObjectTorus = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObjectTorus);
            GL.BufferData(BufferTarget.ArrayBuffer, _torus.Length * sizeof(float), _torus, BufferUsageHint.StaticDraw);

            _simpleColorShader = new Shader("Shaders/SimpleColor.vert", "Shaders/SimpleColor.frag");
            _simpleColorShader.Use();
            var vertexLocationTorus = _simpleColorShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocationTorus);
            GL.VertexAttribPointer(vertexLocationTorus, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            //plane
            _vertexArrayObjectPlane = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObjectPlane);

            _vertexBufferObjectPlane = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObjectPlane);
            GL.BufferData(BufferTarget.ArrayBuffer, _planeVertexes.Length * sizeof(float), _planeVertexes, BufferUsageHint.StaticDraw);

            _elementBufferObjectPlane = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObjectPlane);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _planeIndexes.Length * sizeof(uint), _planeIndexes, BufferUsageHint.StaticDraw);


            var vertexLocationPlane = _simpleColorShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocationPlane);
            GL.VertexAttribPointer(vertexLocationPlane, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            //cube
            _vertexArrayCube = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayCube);

            _vertexBufferObjectCube = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObjectCube);
            GL.BufferData(BufferTarget.ArrayBuffer, _cubeVertexes.Length * sizeof(float), _cubeVertexes, BufferUsageHint.StaticDraw);

            _elementBufferObjectCube = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObjectCube);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _cubeIndexes.Length * sizeof(uint), _cubeIndexes, BufferUsageHint.StaticDraw);

            _cubeShader = new Shader("Shaders/Cube.vert", "Shaders/Cube.frag");
            _cubeShader.Use();
            var vertexLocation = _cubeShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _cubeShader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _texture = Texture.LoadFromFile("Textures/planks.png");
            _texture.Use(TextureUnit.Texture0);

            _cubeShader.SetInt("texture0", 0);


            //funk
            _vertexArrayObjectFunk = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObjectFunk);

            _vertexBufferObjectFunk = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObjectFunk);
            GL.BufferData(BufferTarget.ArrayBuffer, _funkPlaneVertexes.Length * sizeof(float), _funkPlaneVertexes, BufferUsageHint.StaticDraw);

            _elementBufferObjectFunk = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObjectFunk);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _funkPlaneIndexes.Length * sizeof(uint), _funkPlaneIndexes, BufferUsageHint.StaticDraw);

            var vertexLocationFunk = _simpleColorShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocationFunk);
            GL.VertexAttribPointer(vertexLocationFunk, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            
            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _time += 4.0 * e.Time;


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //cube
            GL.BindVertexArray(_vertexArrayCube);

            _texture.Use(TextureUnit.Texture0);
            _cubeShader.Use();
            var cubeModel = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(50*_time));
            _cubeShader.SetMatrix4("model", cubeModel);
            _cubeShader.SetMatrix4("view", _camera.GetViewMatrix());
            _cubeShader.SetMatrix4("projection", _camera.GetProjectionMatrix(_isPerspective));

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawElements(PrimitiveType.Triangles, _cubeIndexes.Length, DrawElementsType.UnsignedInt, 0);

            //torus
            GL.BindVertexArray(_vertexArrayObjectTorus);
            var torusModel = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(-10*_time))* new Matrix4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0),
                 new Vector4(6, 0, 0, 1));
            _simpleColorShader.Use();
            _simpleColorShader.SetVector3("aColor", new Vector3(0.5f, 0.5f, 1f));
            _simpleColorShader.SetMatrix4("model", torusModel);
            _simpleColorShader.SetMatrix4("view", _camera.GetViewMatrix());
            _simpleColorShader.SetMatrix4("projection", _camera.GetProjectionMatrix(_isPerspective));

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, _torus.Length/3);


            //plane
            GL.BindVertexArray(_vertexArrayObjectPlane);
            var planeModel = new Matrix4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0),
                 new Vector4(3, 0, 0, 1));
            _simpleColorShader.Use();
            _simpleColorShader.SetVector3("aColor", new Vector3(0.9f, 0.9f,0.9f));
            _simpleColorShader.SetMatrix4("model", planeModel);
            _simpleColorShader.SetMatrix4("view", _camera.GetViewMatrix());
            _simpleColorShader.SetMatrix4("projection", _camera.GetProjectionMatrix(_isPerspective));

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawElements(PrimitiveType.Triangles, _planeIndexes.Length, DrawElementsType.UnsignedInt, 0);



            //funk
            GL.BindVertexArray(_vertexArrayObjectFunk);
            var funkModel = new Matrix4(
                new Vector4(0.1f, 0, 0, 0),
                new Vector4(0, 0.1f, 0, 0),
                new Vector4(0, 0, 0.1f, 0),
                 new Vector4(1, 0, 6, 1));
            _simpleColorShader.Use();
            _simpleColorShader.SetVector3("aColor", new Vector3(1f, 0.2f, 0.6f));
            _simpleColorShader.SetMatrix4("model", funkModel);
            _simpleColorShader.SetMatrix4("view", _camera.GetViewMatrix());
            _simpleColorShader.SetMatrix4("projection", _camera.GetProjectionMatrix(_isPerspective));

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.DrawElements(PrimitiveType.TriangleStrip, _funkPlaneIndexes.Length, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, _funkPlaneVertexes.Length / 3);


            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 2.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            if (input.IsKeyDown(Keys.P))
            {
                _isPerspective = true;
            }
            if (input.IsKeyDown(Keys.O))
            {
                _isPerspective = false;
            }
            var mouse = MouseState;

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}