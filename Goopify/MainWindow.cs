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
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Goopify
{
    public enum ViewAxis : int
    {
        X,
        Y,
    }

    public partial class MainWindow : Form
    {
        // Gets the center of this window
        private Point FormCenter
        {
            get
            {
                return new Point(this.Location.X + this.Width / 2, this.Location.Y + this.Height / 2);
            }
        }

        // Gets center of the opengl window
        private Point ViewerCenter
        {
            get
            {
                Point topRightPoint = glControl1.PointToScreen(new Point(0, 0));
                return new Point(topRightPoint.X + glControl1.Width / 2, topRightPoint.Y + glControl1.Height / 2);
            }
        }

        private bool loaded;    //Whether or not the glControl is loaded

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
        private const float zNear = 0.01f;
        private const float zFar = 1000f;
        private const float zNearOrtho = -1f;
        private const float zFarOrtho = 100f;
        private float cameraFOV = (float)((70f * Math.PI) / 180f);
        private bool isOrthographic = true;
        private float orthoZoom = 1f;
        private const float orthoZoomBorder = 0.1f;

        private const float camYHeight = 10f;

        private const float matrixZoomAmount = 0.0001f; // Used for scaling the models down and moving the camera on the same scale

        // Editable settings
        private bool cameraMoveY = false;
        private float cameraSpeed = 1;

        // Camera state
        private bool isMouseLookMode = false;

        private bool forwardKeyPressed;
        private bool backwardKeyPressed;
        private bool leftKeyPressed;
        private bool rightKeyPressed;
        private bool upKeyPressed;
        private bool downKeyPressed;
        private bool shiftKeyPressed;

        private System.Timers.Timer UpdateTimer = new System.Timers.Timer();


        private Col mapCol;

        private bool drawingLine = false;
        private bool lineDrawn = false;
        private Vector3 lineStart;
        private Vector3 lineEnd;

        //Goop Regions
        private class GoopRegionBox
        {
            public Vector3 centerPoint;
            public float size;
            public bool selected;
        }

        private List<GoopRegionBox> goopCutRegions = new List<GoopRegionBox>();
        private bool movingRegion;
        private int prevBoxSelected = -1;

        // Main functions
        public MainWindow()
        {
            InitializeComponent();

            // Timer event for 
            UpdateTimer.Interval = 16; // 60 fps
            UpdateTimer.Elapsed += UpdateTimer_Triggered;
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

        #region Camera

        /// <summary>
        /// Updates the camera matrix for proper view drawing
        /// REMINDER THAT FOR SUNSHINE, LEFT IS POSITIVE X AND FORWARD IS POSITIVE Z. I DON'T KNOW WHY AND IT MAKES ME SAD
        /// </summary>
        private void UpdateCamera()
        {
            Vector3 cameraUnitVector = new Vector3((float)(Math.Cos(cameraRotation.X) * Math.Cos(cameraRotation.Y)) + (cameraPosition.X),
                                                   (float)(Math.Sin(cameraRotation.Y) + (cameraPosition.Y)),
                                                   (float)(Math.Sin(cameraRotation.X) * Math.Cos(cameraRotation.Y)) + (cameraPosition.Z));


            cameraMatrix = Matrix4.LookAt(cameraPosition, cameraUnitVector, Vector3.UnitY);

            


            cameraMatrix = Matrix4.Mult(Matrix4.CreateScale(matrixZoomAmount), cameraMatrix); // Scales the matrix
        }

        /* Camera and Rendering based off of camera in LevelEditorForm.cs in Whitehole by StapleButter*/
        /* 1/21/15 */
        /// <summary>
        /// Updates the viewport based on the new screen ratio and projection mode (orthogaphic or perspective)
        /// </summary>
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

        #endregion


        #region GL Control Functions

        /// <summary>
        /// Loads the glControl1 window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;

            glControl1.MakeCurrent();

            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1f);

            GL.FrontFace(FrontFaceDirection.Cw); // Unsure what this does

            // Setup Camera and display settings
            cameraPosition = new Vector3(0f, 1f, 0f);
            cameraRotation = new Vector3((float)Math.PI * 0.5f, (float)Math.PI * -0.5f + 0.01f, 0f);

            if (isOrthographic) // Starts the orthographic camera looking down
            {
                cameraPosition = new Vector3(0f, 10f, 0f);
                cameraRotation = new Vector3((float)Math.PI * 0.5f, (float)Math.PI * -0.5f + 0.00001f, 0f);
            }

            UpdateViewport();
            UpdateCamera();


            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            loaded = true;
        }

        /// <summary>
        /// Paints screen again after glControl1 is invalidated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl1_Paint(object sender, EventArgs e)
        {
            if (!loaded)
                return;

            glControl1.MakeCurrent();

            GL.DepthMask(true); // ensures that GL.Clear() will successfully clear the buffers
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            // Setup for viewing (This updates the GL camera matrix, UpdateViewport updates GL the projection matrix)
            GL.MatrixMode(MatrixMode.Modelview); // Load Camera
            GL.LoadMatrix(ref cameraMatrix);

            // Render the collision object
            if (mapCol != null)
            {
                mapCol.RenderCol();
            }

            // For drawing a line onscreen to cut the model
            if(drawingLine || lineDrawn)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.LineWidth(100f);

                GL.Color3(Color.Aquamarine);
                GL.Vertex3(lineStart + new Vector3(0, 100, 0));
                GL.Vertex3(lineEnd + new Vector3(0, 100, 0));

                GL.End();

                GL.PointSize(5f);
                GL.Begin(PrimitiveType.Points);

                GL.Color3(Color.Yellow);
                GL.Vertex3(lineStart);

                GL.End();
            }

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

            // Draw the sections we'll cut into goop models
            foreach(GoopRegionBox goopBox in goopCutRegions)
            {
                // Draw box lines
                GL.LineWidth(1f);
                Color lineColor = Color.DarkGray;
                if(goopBox.selected)
                {
                    GL.LineWidth(4f);
                    lineColor = Color.Gray;
                }
                GL.Begin(PrimitiveType.Lines);

                Vector3 boxTopRight = goopBox.centerPoint + new Vector3(goopBox.size, 0, goopBox.size);
                Vector3 boxTopLeft = goopBox.centerPoint + new Vector3(goopBox.size, 0, -goopBox.size);
                Vector3 boxBottomRight = goopBox.centerPoint + new Vector3(-goopBox.size, 0, goopBox.size);
                Vector3 boxBottomLeft = goopBox.centerPoint + new Vector3(-goopBox.size, 0, -goopBox.size);

                GL.Color4(lineColor);
                GL.Vertex3(boxTopLeft);
                GL.Vertex3(boxTopRight);
                GL.Color4(lineColor);
                GL.Vertex3(boxTopRight);
                GL.Vertex3(boxBottomRight);
                GL.Color4(lineColor);
                GL.Vertex3(boxBottomRight);
                GL.Vertex3(boxBottomLeft);
                GL.Color4(lineColor);
                GL.Vertex3(boxBottomLeft);
                GL.Vertex3(boxTopLeft);

                GL.End();

                // Draw box verts
                GL.PointSize(5);
                Color dotColor = Color.Gray;
                if (goopBox.selected)
                {
                    GL.PointSize(7);
                    dotColor = Color.White;
                }
                GL.Begin(PrimitiveType.Points);

                GL.Color4(dotColor);
                GL.Vertex3(boxTopLeft);
                GL.Vertex3(boxTopRight);
                GL.Vertex3(boxBottomRight);
                GL.Vertex3(boxBottomLeft);
                
                GL.Vertex3(goopBox.centerPoint);

                GL.End();
            }

            glControl1.SwapBuffers(); // Takes from the GL and puts into control
        }

        /// <summary>
        /// Event for glControl1 being resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();

            UpdateViewport();
        }

        /* MOUSE EVENTS */

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) // Right Click pressed down
            {
                if(!isMouseLookMode) // Start mouse look mode
                {
                    // Start the camera update loop
                    UpdateTimer.Start();

                    isMouseLookMode = true;
                    Cursor.Position = ViewerCenter;
                    Cursor.Hide();
                    return;
                }
            }

            if(e.Button == MouseButtons.Left)
            {
                if(!drawingLine)
                {
                    Console.WriteLine("Drawing Line");
                    lineDrawn = false;
                    drawingLine = true;
                    lineStart = ScreenToWorld(e.X, e.Y) + cameraPosition / matrixZoomAmount;
                    lineEnd = lineStart;
                    glControl1.Refresh();
                } else
                {
                    if(mapCol != null)
                    {
                        drawingLine = false;
                        lineDrawn = true;
                        mapCol.SplitModelByLine(lineStart, lineEnd);
                        glControl1.Refresh();
                    }
                }
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
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            // Set this as input focus if mouse is on area
            if (!glControl1.Focused)
            {
                glControl1.Focus();
            }

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

                } else // Pan Mode
                {
                    //Move camera
                    cameraPosition.X += cursorXOffset * 0.01f;
                    cameraPosition.Z += cursorYOffset * 0.01f;
                }

                UpdateViewport();
                UpdateCamera();
                glControl1.Refresh();

            }

            if(drawingLine)
            {
                lineEnd = ScreenToWorld(e.X, e.Y) + cameraPosition / matrixZoomAmount;
                glControl1.Refresh();
            }

            /*Vector2 mousePos = new Vector2(e.X, e.Y);
            Console.WriteLine(mousePos + ":" + lineEnd);*/
        }

        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            if(isOrthographic)
            {
                float zoomSpeed = 0.001f;
                if(shiftKeyPressed)
                {
                    zoomSpeed = 0.01f;
                }

                orthoZoom -= e.Delta * zoomSpeed;

                if (orthoZoom < 0.1f)
                {
                    orthoZoom = 0.1f;
                }

                UpdateViewport();
                UpdateCamera();
                glControl1.Refresh();
            }
            
        }

        /* KEYBOARD EVENTS */

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if(isMouseLookMode) // Setup input axis based on key press
            {
                if (e.KeyCode == Keys.W)
                {
                    forwardKeyPressed = true;
                } else if (e.KeyCode == Keys.S)
                {
                    backwardKeyPressed = true;
                } else if (e.KeyCode == Keys.A)
                {
                    leftKeyPressed = true;
                } else if (e.KeyCode == Keys.D)
                {
                    rightKeyPressed = true;
                } else if (e.KeyCode == Keys.Q)
                {
                    downKeyPressed = true;
                }
                else if (e.KeyCode == Keys.E)
                {
                    upKeyPressed = true;
                }
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

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Collision File (*.col)|*.col";
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == DialogResult.OK) // If the Ok button is hit
            {
                //Get the path of specified file
                string filePath = fileDialog.FileName;

                // Read the contents of the file into a stream
                var fileStream = fileDialog.OpenFile();

                mapCol = new Col(fileStream); // Create the col

                // Set orthoZoom based on model bounds
                Vector2 colTopLeftBound = mapCol.ReturnTopLeft();
                Vector2 colBottomRightBound = mapCol.ReturnBottomRight();
                float modelWidth = colTopLeftBound.X - colBottomRightBound.X;
                float modelHeight = colTopLeftBound.Y - colBottomRightBound.Y;
                float modelAspectRatio = modelWidth / modelHeight;

                Console.WriteLine("Model Bound Corners: " + colTopLeftBound + ":" + colBottomRightBound);

                // Converts screen corners to world points
                Vector3 camTopLeft = ScreenToWorld(0, 0);
                Vector3 camBottomRight = ScreenToWorld(glControl1.Width, glControl1.Height);
                Console.WriteLine("Cam World Corners: " + camTopLeft + ":" + camBottomRight);

                if (modelAspectRatio < aspectRatio) // Model height is focus for zoom
                {
                    float screenWorldHeight = camTopLeft.Z - camBottomRight.Z;
                    orthoZoom *= modelHeight / screenWorldHeight + orthoZoomBorder;

                } else // Model width is focus for zoom
                {
                    float screenWorldWidth =  camTopLeft.X - camBottomRight.X;
                    orthoZoom *= modelWidth / screenWorldWidth + orthoZoomBorder;
                }
                //Center the camera to the model
                cameraPosition = new Vector3((colBottomRightBound.X + modelWidth / 2) * matrixZoomAmount, camYHeight, (colBottomRightBound.Y + modelHeight / 2) * matrixZoomAmount);

                UpdateViewport();
                UpdateCamera();
                glControl1.Invalidate();
            }

        }

        public Vector3 ScreenToWorld(int screenX, int screenY)
        {
            float xPos = (2.0f * screenX) / (float)glControl1.Width - 1f;
            float yPos = 1f - (2f * screenY / (float)glControl1.Height);


            Matrix4 viewProjectionInverse = Matrix4.Invert(projectionMatrix *
             cameraMatrix);

            Vector4 point = new Vector4(xPos, 0, yPos, 0);

            Vector4 transformedPoint = point * viewProjectionInverse;

            return new Vector3(transformedPoint.X, transformedPoint.Z, transformedPoint.Y);
        }

        public Vector2 WorldToScreen(Vector3 worldPos)
        {
            Matrix4 viewProjectionMatrix = projectionMatrix * cameraMatrix;
            Vector4 point = new Vector4(worldPos, 0);

            Vector4 transformedPoint = point * viewProjectionMatrix;



            float xPos = ((transformedPoint.X + 1) / 2) * (float)glControl1.Width;
            float yPos = ((1 - transformedPoint.Y) / 2) * (float)glControl1.Height;

            return new Vector2(xPos, yPos);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            isOrthographic = !isOrthographic;
            if(isOrthographic)
            {
                cameraPosition = new Vector3(0, 10, 0);
                cameraRotation = new Vector3((float)Math.PI * 0.5f, (float)Math.PI * -0.5f + 0.00001f, 0f);
            } else
            {
                cameraPosition = new Vector3(0f, 1f, 0f);
                cameraRotation = new Vector3((float)Math.PI * 0.5f, (float)Math.PI * -0.5f + 0.01f, 0f);
            }

            UpdateViewport();
            UpdateCamera();
            glControl1.Invalidate();
        }

        /// <summary>
        /// Adds new region to the map that will be used to cut out a goop section
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void regionButton_Click(object sender, EventArgs e)
        {
            GoopRegionBox newGoopBox = new GoopRegionBox();
            Vector3 camPos = cameraPosition / matrixZoomAmount;
            newGoopBox.centerPoint = new Vector3(camPos.X, 10000, camPos.Z);
            newGoopBox.size = 1000;
            goopCutRegions.Add(newGoopBox);

            int goopRegionIndex = goopCutRegions.Count - 1;
            listBox1.Items.Add("Goop Cut Region " + goopRegionIndex);
            listBox1.EndUpdate(); // Allow listbox to repaint
            listBox1.SelectedIndex = goopRegionIndex;

            glControl1.Invalidate();
        }

        /// <summary>
        /// Highlights the correct box when the corrosponding list item is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newSelectedIndex = listBox1.SelectedIndex;
            Console.WriteLine("List Selected: " + newSelectedIndex);
            goopCutRegions[listBox1.SelectedIndex].selected = true;
            if (prevBoxSelected != -1 && prevBoxSelected != newSelectedIndex)
            {
                goopCutRegions[prevBoxSelected].selected = false;
            }
            prevBoxSelected = listBox1.SelectedIndex;

            glControl1.Invalidate();
        }
    }
}
