using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goopify.Forms.ToolForms
{
    public partial class OpenTKTesting : Form
    {

        private readonly float[] _vertices =
        {
             // Position          Normal
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, // Front face
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, // Back face
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, // Left face
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, // Right face
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, // Bottom face
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, // Top face
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
        };

        // Camera Parameters
        private Matrix4 cameraMatrix;
        private Matrix4 projectionMatrix;
        private Vector3 cameraRotation;
        private Vector3 cameraPosition;
        private Vector3 cameraVelocity;
        // Camera aspect settings
        private float aspectRatio;
        private float conditionalAspect;
        private int aspectAxisConstraint;
        // Camera projection FOV settings
        private const float zNear = 0.001f;
        private const float zFar = 10000f;
        private const float zNearOrtho = 0.001f;
        private const float zFarOrtho = 100000000f;
        private float cameraFOV = (float)((70f * Math.PI) / 180f);
        private bool isOrthographic = true;
        private float orthoZoom = 1f;
        private const float orthoZoomBorder = 0.1f;

        private const float matrixZoomAmount = 0.0001f; // Used for scaling the models down and moving the camera on the same scale

        // Camera state
        private bool isMouseLookMode = false;

        private bool forwardKeyPressed;
        private bool backwardKeyPressed;
        private bool leftKeyPressed;
        private bool rightKeyPressed;
        private bool upKeyPressed;
        private bool downKeyPressed;
        private bool shiftKeyPressed;

        private bool mouseOnePressed;

        // Editable settings
        private bool cameraMoveY = false;
        private float cameraSpeed = 10;

        private const float regionHeightIfOrthographic = 100000;

        private const float regionCornerwidthPercent = 0.2f;

        private System.Timers.Timer UpdateTimer = new System.Timers.Timer();


        private Col mapCol;



        // Gets center of the opengl window
        private Point ViewerCenter
        {
            get
            {
                Point topRightPoint = glControl1.PointToScreen(new Point(0, 0));
                return new Point(topRightPoint.X + glControl1.Width / 2, topRightPoint.Y + glControl1.Height / 2);
            }
        }

        public OpenTKTesting()
        {
            InitializeComponent();

            // Timer event for 
            UpdateTimer.Interval = 16; // 60 fps
            UpdateTimer.Elapsed += UpdateTimer_Triggered;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.CadetBlue);

            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            //float[] lightPosition = { 0, 1000, 0 };
            //GL.Light(LightName.Light0, LightParameter.Position, lightPosition);

            GL.FrontFace(FrontFaceDirection.Cw); // Markes front face as clockwise

            // Setup Camera and display settings
            cameraPosition = new Vector3(0f, 10f, 0f);
            cameraRotation = new Vector3((float)Math.PI * 1.5f, (float)Math.PI * -0.5f + 0.01f, 0f);

            if (isOrthographic) // Starts the orthographic camera looking down
            {
                cameraPosition = new Vector3(0f, 10f, 0f);
                cameraRotation = new Vector3((float)Math.PI * 1.5f, (float)Math.PI * -0.5f + 0.00001f, 0f);
            }

            UpdateViewport();
            UpdateCamera();

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Read the contents of the file into a stream
            var fileStream = File.Open("C:\\Users\\alexh\\Downloads\\casino1.szs_ext\\scene\\map\\map.col", FileMode.Open);

            mapCol = new Col(fileStream); // Create the col
            fileStream.Close();

            //shaderProgram = new ShaderProgram(vertexShaderCode, pixelShaderCode);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            glControl1.MakeCurrent();

            GL.DepthMask(true); // ensures that GL.Clear() will successfully clear the buffers
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Setup for viewing (This updates the GL camera matrix, UpdateViewport updates GL the projection matrix)
            GL.MatrixMode(MatrixMode.Modelview); // Load Camera
            GL.LoadMatrix(ref cameraMatrix);

            GL.Disable(EnableCap.Lighting);
            // Draw Direction Lines
            GL.LineWidth(2);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(100000f, 0f, 0f);
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0f, 100000f, 0f);
            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0f, 0f, 100000f);

            GL.End();

            GL.Enable(EnableCap.Lighting);

            DrawModels();

            glControl1.SwapBuffers();
        }

        public void DrawModels()
        {
            mapCol.RenderCol();

            float size = 1;

            GL.Begin(BeginMode.Quads);

            GL.Color3(1, 1, 1);

            // front
            GL.Normal3(0, 0, 1);
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(-size, size, size);
            // back
            GL.Normal3(0, 0, -1);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(-size, size, -size);
            // top
            GL.Normal3(0, 1.0, 0);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(-size, size, size);
            // bottom
            GL.Normal3(0, -1.0, 0);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, -size, size);
            // right
            GL.Normal3(1, 0, 0);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, size, size);
            // left
            GL.Normal3(-1, 0, 0);
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(-size, size, size);

            GL.End();

        }

        private void UpdateCamera()
        {
            Vector3 cameraUnitVector = new Vector3((float)(Math.Cos(cameraRotation.X) * Math.Cos(cameraRotation.Y)),
                                                   (float)(Math.Sin(cameraRotation.Y)),
                                                   (float)(Math.Sin(cameraRotation.X) * Math.Cos(cameraRotation.Y)));
            cameraUnitVector = cameraUnitVector.Normalized();

            cameraMatrix = Matrix4.LookAt(cameraPosition, cameraPosition + cameraUnitVector, Vector3.UnitY);

            //cameraMatrix = Matrix4.Mult(Matrix4.CreateScale(matrixZoomAmount), cameraMatrix); // Scales the matrix
        }

        private void UpdateViewport()
        {
            GL.Viewport(glControl1.ClientRectangle);

            aspectRatio = (float)glControl1.Width / (float)glControl1.Height;
            conditionalAspect = 16f / 9f;
            aspectAxisConstraint = (int)ViewAxis.X;
            if (aspectRatio < aspectAxisConstraint)
            {
                aspectAxisConstraint = (int)ViewAxis.Y;
            }
            GL.MatrixMode(MatrixMode.Projection);
            if (!isOrthographic)
            {
                projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(2.0f * (float)Math.Atan((float)Math.Tan(cameraFOV / 2.0f)
                    / CalculateYAspect(aspectRatio, conditionalAspect, aspectAxisConstraint)), aspectRatio, zNear, zFar);
            }
            else
            {
                projectionMatrix = Matrix4.CreateOrthographic((float)orthoZoom /
                    CalculateYAspect(aspectRatio, conditionalAspect, aspectAxisConstraint) * aspectRatio, (float)orthoZoom /
                    CalculateYAspect(aspectRatio, conditionalAspect, aspectAxisConstraint), zNearOrtho, zFarOrtho);
            }

            GL.LoadMatrix(ref projectionMatrix); // Load
        }

        private float CalculateYAspect(float AspectRatio, float ConditionalAspect, int AspectAxisConstraint)
        {
            switch (AspectAxisConstraint)
            {
                default:
                    return ConditionalAspect / ConditionalAspect;
                case 1:
                    return AspectRatio / ConditionalAspect;
            }
        }


        /// <summary>
        /// Function called at a rate of 60fps to update the camera position based on input axises
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UpdateTimer_Triggered(object sender, EventArgs e)
        {
            Vector3 trot = cameraRotation;
            if (isOrthographic)
            {
                if (cameraRotation.X < 0)
                    cameraRotation.X += (float)Math.PI * 2;
                if (cameraRotation.X >= Math.PI / 4 && cameraRotation.X < 3 * Math.PI / 4)
                    trot.X = (float)Math.PI / 2;
                else if (cameraRotation.X >= 3 * Math.PI / 4 && cameraRotation.X < 5 * Math.PI / 4)
                    trot.X = (float)Math.PI;
                else if (cameraRotation.X >= 5 * Math.PI / 4 && cameraRotation.X < 7 * Math.PI / 4)
                    trot.X = 3 * (float)Math.PI / 2;
                else if (cameraRotation.X >= 7 * Math.PI / 4 || cameraRotation.X < Math.PI / 4)
                    trot.X = 0;
                trot.Y = 0;
            }
            //Get forward and right vectors
            Vector3 f = new Vector3((float)(Math.Cos(trot.X) * (cameraMoveY ? (float)Math.Cos(trot.Y) : 1.0f)),
                                    cameraMoveY ? (float)Math.Sin(trot.Y) : 0.0f,
                                    (float)(Math.Sin(trot.X) * (cameraMoveY ? (float)Math.Cos(trot.Y) : 1.0f)));

            Vector3 r = new Vector3((float)(Math.Sin(trot.X)),
                                    0.0f,
                                    -(float)(Math.Cos(trot.X)));

            Vector3 u = new Vector3(0.0f, 1.0f, 0.0f);

            //Set camera velocity according to key input
            cameraVelocity = new Vector3(0f, 0f, 0f);
            cameraVelocity += ((forwardKeyPressed ? 1.0f : 0.0f) + (backwardKeyPressed ? -1.0f : 0.0f)) * f;
            cameraVelocity += ((upKeyPressed ? 1.0f : 0.0f) + (downKeyPressed ? -1.0f : 0.0f)) * u;
            cameraVelocity += -((leftKeyPressed ? -1.0f : 0.0f) + (rightKeyPressed ? 1.0f : 0.0f)) * r;

            cameraPosition += cameraVelocity * cameraSpeed / 100f;

            Console.WriteLine("Cam Move Pos: " + cameraPosition);

            //Update view
            UpdateCamera();
            glControl1.Invalidate();
        }

        #region MOUSE

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) // Right Click pressed down
            {
                if (!isMouseLookMode) // Start mouse look mode
                {
                    // Start the camera update loop
                    UpdateTimer.Start();

                    isMouseLookMode = true;
                    Cursor.Position = ViewerCenter;
                    Cursor.Hide();
                    return;
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                glControl1.Refresh();
                mouseOnePressed = true;
                glControl1.Refresh();
            }
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) // Right Click released
            {
                if (isMouseLookMode) // Stop mouse look mode
                {
                    // Stop the camera update loop
                    UpdateTimer.Stop();

                    isMouseLookMode = false;
                    Cursor.Show();

                    // Reset input axis
                    upKeyPressed = false;
                    downKeyPressed = false;
                    forwardKeyPressed = false;
                    backwardKeyPressed = false;
                    leftKeyPressed = false;
                    rightKeyPressed = false;

                    return;
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                mouseOnePressed = false;
                glControl1.Refresh();
            }
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            // Set this as input focus if mouse is on area
            if (!glControl1.Focused)
            {
                glControl1.Focus();
            }

            // Moving the camera for the view
            if (isMouseLookMode)
            {
                // Get cursor move offset and reset it
                Point center = ViewerCenter;
                int cursorXOffset = Cursor.Position.X - center.X;
                int cursorYOffset = Cursor.Position.Y - center.Y;
                Cursor.Position = center;   // Lock mouse to center of screen

                if (!isOrthographic) // Look View Mode
                {
                    //Rotate camera
                    cameraRotation.X += cursorXOffset * 0.01f;
                    cameraRotation.Y -= cursorYOffset * 0.01f;


                    //Keep direction from overflowing
                    float pi2 = 2f * (float)Math.PI - 0.001f;
                    float pii2 = (float)Math.PI / 2f - 0.001f;
                    while (cameraRotation.X > pi2)
                        cameraRotation.X -= pi2;
                    while (cameraRotation.X < -pi2)
                        cameraRotation.X += pi2;

                    //Keep camera from going upside down
                    if (cameraRotation.Y > pii2)
                        cameraRotation.Y = pii2;
                    if (cameraRotation.Y < -pii2)
                        cameraRotation.Y = -pii2;

                }
                else // Pan Mode
                {
                    //Move camera
                    cameraPosition.X -= cursorXOffset * 0.01f;
                    cameraPosition.Z -= cursorYOffset * 0.01f;
                }

                UpdateViewport();
                UpdateCamera();
                glControl1.Refresh();

            }
        }

        #endregion

        #region KEYS

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isMouseLookMode) // Setup input axis based on key press
            {
                if (e.KeyCode == Keys.W)
                {
                    forwardKeyPressed = true;
                }
                else if (e.KeyCode == Keys.S)
                {
                    backwardKeyPressed = true;
                }
                else if (e.KeyCode == Keys.A)
                {
                    leftKeyPressed = true;
                }
                else if (e.KeyCode == Keys.D)
                {
                    rightKeyPressed = true;
                }
                else if (e.KeyCode == Keys.Q)
                {
                    downKeyPressed = true;
                }
                else if (e.KeyCode == Keys.E)
                {
                    upKeyPressed = true;
                }
            }

            if(e.KeyCode == Keys.O)
            {
                isOrthographic = !isOrthographic;
                UpdateViewport();
                glControl1.Refresh();
            }

            if (e.KeyCode == Keys.ShiftKey)
            {
                shiftKeyPressed = true;
            }

        }

        private void glControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (isMouseLookMode) // Setup input axis based on key release
            {
                if (e.KeyCode == Keys.W)
                {
                    forwardKeyPressed = false;
                }
                else if (e.KeyCode == Keys.S)
                {
                    backwardKeyPressed = false;
                }
                else if (e.KeyCode == Keys.A)
                {
                    leftKeyPressed = false;
                }
                else if (e.KeyCode == Keys.D)
                {
                    rightKeyPressed = false;
                }
                else if (e.KeyCode == Keys.Q)
                {
                    downKeyPressed = false;
                }
                else if (e.KeyCode == Keys.E)
                {
                    upKeyPressed = false;
                }
            }

            if (e.KeyCode == Keys.ShiftKey)
            {
                shiftKeyPressed = false;
            }
        }

        #endregion
    }
}
