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

        private bool mouseOnePressed;

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
            public float heightRatio = 1;
            public int horizontalResolution = 512;
            public int verticalResolution = 512;

            public GoopRegionBox Clone()
            {
                GoopRegionBox newGoopRegion = new GoopRegionBox();
                newGoopRegion.centerPoint = centerPoint;
                newGoopRegion.size = size;
                newGoopRegion.heightRatio = heightRatio;
                return newGoopRegion;
            }
        }

        private List<GoopRegionBox> goopCutRegions = new List<GoopRegionBox>();
        private List<int> selectedRegions = new List<int>();
        private List<Vector3> selectedRegionOffset = new List<Vector3>();
        private bool goopSelectionByCode;

        private const float selectionRadius = 80f;

        private enum GoopRegionClick { ClickedNothing, ClickedCenter, ClickedCorner, Null }
        private GoopRegionClick currentClickType;

        // Undo/Redo stuff

        private class RegionState
        {
            
            public GoopRegionBox[] currentRegions;
            public int[] selectedRegions;

            /// <summary>
            /// Creates a deep clone of the gotten regions and saves them to this RegionState
            /// </summary>
            /// <param name="allRegions">All goop regions</param>
            /// <param name="currentSelectedRegions">The selected goop regions</param>
            public RegionState(List<GoopRegionBox> allRegions, List<int> currentSelectedRegions)
            {
                int selectedRegionIndex = 0;
                selectedRegions = new List<int>(currentSelectedRegions).ToArray();
                currentRegions = new GoopRegionBox[allRegions.Count];
                for (int i = 0; i < allRegions.Count; i++)
                {
                    GoopRegionBox clonedRegion = allRegions[i].Clone();
                    currentRegions[i] = clonedRegion;
                }
            }

            public RegionState(RegionState existingState)
            {
                List<GoopRegionBox> allRegions = existingState.currentRegions.ToList();

                int selectedRegionIndex = 0;
                selectedRegions = new List<int>(existingState.selectedRegions).ToArray();
                currentRegions = new GoopRegionBox[allRegions.Count];
                for (int i = 0; i < allRegions.Count; i++)
                {
                    GoopRegionBox clonedRegion = allRegions[i].Clone();
                    currentRegions[i] = clonedRegion;
                }
            }

            public RegionState()
            {
                selectedRegions = new int[0];
                currentRegions = new GoopRegionBox[0];
            }

            public bool Equals(RegionState otherRegionState)
            {
                var list1Subset = currentRegions.Select(i => new { i.centerPoint, i.size, i.heightRatio });
                var list2Subset = otherRegionState.currentRegions.Select(i => new { i.centerPoint, i.size, i.heightRatio });

                bool regionsEqual = list1Subset.SequenceEqual(list2Subset);

                bool selectedEqual = selectedRegions.SequenceEqual(otherRegionState.selectedRegions);

                return regionsEqual && selectedEqual;
            }
        }

        private Stack<RegionState> undoStack = new Stack<RegionState>();
        private Stack<RegionState> redoStack = new Stack<RegionState>();

        public bool snapPositions = true;
        public float snapInterval = 100f;

        public float RoundToInterval(float i)
        {
            return ((float)Math.Round(i / snapInterval)) * snapInterval;
        }

        // Main functions
        public MainWindow()
        {
            undoStack.Push(new RegionState()); // Push empty goop region to stack to start

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
            for(int i = 0; i < goopCutRegions.Count; i++)
            {
                // Draw box lines
                GL.LineWidth(1f);
                Color lineColor = Color.DarkGray;
                if(selectedRegions.Contains(i))
                {
                    GL.LineWidth(4f);
                    lineColor = Color.Gray;
                }
                GL.Begin(PrimitiveType.Lines);

                Vector3 boxTopRight = goopCutRegions[i].centerPoint + new Vector3(goopCutRegions[i].size, 0, goopCutRegions[i].size);
                Vector3 boxTopLeft = goopCutRegions[i].centerPoint + new Vector3(goopCutRegions[i].size, 0, -goopCutRegions[i].size);
                Vector3 boxBottomRight = goopCutRegions[i].centerPoint + new Vector3(-goopCutRegions[i].size, 0, goopCutRegions[i].size);
                Vector3 boxBottomLeft = goopCutRegions[i].centerPoint + new Vector3(-goopCutRegions[i].size, 0, -goopCutRegions[i].size);

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
                if (selectedRegions.Contains(i))
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
                
                GL.Vertex3(goopCutRegions[i].centerPoint);

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
                // Select the goop region based on clicked corners or center points
                // Already selected regions take priority, center points over corners
                Vector3 mouseWorldPos = ScreenToWorld(e.X, e.Y);
                currentClickType = ClickGoopZones(mouseWorldPos);
                mouseOnePressed = true;



                // Drawing a line, converting to the boxes atm
                /*if(!drawingLine)
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
                }*/
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

            if(e.Button == MouseButtons.Left)
            {
                mouseOnePressed = false;
                if(currentClickType != GoopRegionClick.Null || currentClickType != GoopRegionClick.ClickedNothing)
                {
                    SaveAction();

                }
                currentClickType = GoopRegionClick.Null;
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

            // Moving mouse around the screen for goop sections
            if(mouseOnePressed)
            {
                Vector3 mouseWorldPos = ScreenToWorld(e.X, e.Y);
                if(snapPositions)
                {
                    mouseWorldPos = new Vector3(RoundToInterval(mouseWorldPos.X), RoundToInterval(mouseWorldPos.Y), RoundToInterval(mouseWorldPos.Z));
                }
                Console.WriteLine(mouseWorldPos);
                    
                // TODO: Allow for movement of multiple areas at once
                if (currentClickType == GoopRegionClick.ClickedCenter)
                {
                    for(int i = 0; i < selectedRegions.Count; i++)
                    {
                        goopCutRegions[selectedRegions[i]].centerPoint = new Vector3(mouseWorldPos.X, 0, mouseWorldPos.Z) + selectedRegionOffset[i];
                    }
                    glControl1.Refresh();
                } else if(currentClickType == GoopRegionClick.ClickedCorner) // TODO: Support different ratios
                {
                    float widthDifference = Math.Abs(goopCutRegions[selectedRegions[0]].centerPoint.X - mouseWorldPos.X);
                    float heightDifference = Math.Abs(goopCutRegions[selectedRegions[0]].centerPoint.Z - mouseWorldPos.Z);
                    goopCutRegions[selectedRegions[0]].size = widthDifference > heightDifference ? widthDifference : heightDifference;
                    glControl1.Refresh();
                }
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

        /// <summary>
        /// Open Collision File Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openCollisionButton_Click(object sender, EventArgs e)
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


        /// <summary>
        /// 
        /// </summary>
        private GoopRegionClick ClickGoopZones(Vector3 mouseWorldPos)
        {
            // Clear selection offsets
            selectedRegionOffset.Clear();
            // Checks each goop region and sees if we clicked near one of their points
            GoopRegionClick currentClick = GoopRegionClick.ClickedNothing;
            int selectedIndex = -1;
            bool foundSelectedCorner = false;
            for (int i = 0; i < goopCutRegions.Count; i++)
            {
                // Checks if clicked near center point
                float distFromClick = GetDistance(mouseWorldPos.X, mouseWorldPos.Z, goopCutRegions[i].centerPoint.X, goopCutRegions[i].centerPoint.Z);
                //Console.WriteLine("Dist from mouse: " + distFromClick);
                if (distFromClick <= selectionRadius) // Clicked near center radius
                {
                    if(shiftKeyPressed && selectedRegions.Contains(i)) // Skip this if we're multiselecting and already have this box
                    {
                        Console.WriteLine("De-selecting");
                        selectedRegions.Remove(i);
                        //SaveAction();
                        UpdateListBoxByCode();
                        glControl1.Refresh();
                        return GoopRegionClick.Null;
                    }
                    currentClick = GoopRegionClick.ClickedCenter;
                    selectedIndex = i;
                    if (selectedRegions.Contains(i)) // Prioritize already selected boxes so this is the one we want
                    {
                        break;
                    }
                }

                if(currentClick == GoopRegionClick.ClickedCenter) // Skip corners if we're multiselecting or have a center point already
                {
                    continue;
                }

                // Checks if clicked near corners
                for (int j = 0; j < 4; j++)
                {
                    int horizontalCorner = 0;
                    int verticalCorner = 0;
                    switch (j)
                    {
                        case 0:
                            horizontalCorner = 1;
                            verticalCorner = 1;
                            break;
                        case 1:
                            horizontalCorner = -1;
                            verticalCorner = 1;
                            break;
                        case 2:
                            horizontalCorner = 1;
                            verticalCorner = -1;
                            break;
                        case 3:
                            horizontalCorner = -1;
                            verticalCorner = -1;
                            break;
                    }
                    float horizontalCornerPos = goopCutRegions[i].centerPoint.X + (goopCutRegions[i].size * horizontalCorner);
                    float verticalCornerPos = goopCutRegions[i].centerPoint.Z + (goopCutRegions[i].size * verticalCorner * goopCutRegions[i].heightRatio);
                    distFromClick = GetDistance(mouseWorldPos.X, mouseWorldPos.Z, horizontalCornerPos, verticalCornerPos);
                    //Console.WriteLine("Corner " + j + " Dist: " + distFromClick);
                    if (distFromClick <= selectionRadius) // Clicked near center radius
                    {
                        // Prioritize last selected
                        if(foundSelectedCorner && !selectedRegions.Contains(i))
                        {
                            continue;
                        }
                        if(selectedRegions.Contains(i))
                        {
                            foundSelectedCorner = true;
                        }
                        currentClick = GoopRegionClick.ClickedCorner;
                        selectedIndex = i;
                    }
                }
            }

            //Console.WriteLine("CLICK TYPE: " + currentClick.ToString());
            
            // Update the selected regions based on keys we were holding down when clicking
            if(selectedIndex == -1 || currentClick == GoopRegionClick.ClickedNothing) // No selections found
            {
                if(selectedRegions.Count > 0)
                {
                    selectedRegions.Clear();
                }
            } else if(shiftKeyPressed && currentClick == GoopRegionClick.ClickedCenter) // Case for multiselecting centers
            {
                selectedRegions.Add(selectedIndex);
            } else // Clicked corner with/without multiselect or a center without multiselect
            {
                bool hasItem = selectedRegions.Contains(selectedIndex);
                if (!hasItem)
                {
                    selectedRegions.Clear();
                    selectedRegions.Add(selectedIndex);
                }
            }

            // Update the offsets of all selected boxes based on mouse click pos
            foreach(int index in selectedRegions)
            {
                if (snapPositions)
                {
                    mouseWorldPos = new Vector3(RoundToInterval(mouseWorldPos.X), RoundToInterval(mouseWorldPos.Y), RoundToInterval(mouseWorldPos.Z));
                }
                selectedRegionOffset.Add(goopCutRegions[index].centerPoint - mouseWorldPos);
            }

            // Update the list box with the new selections
            UpdateListBoxByCode();

            glControl1.Refresh();

            return currentClick;
        }

        /// <summary>
        /// Updates the goop region list box based on our currently selected goop regions
        /// </summary>
        public void UpdateListBoxByCode()
        {
            goopSelectionByCode = true;
            goopRegionListBox.SelectedIndices.Clear();
            for (int i = 0; i < selectedRegions.Count(); i++)
            {
                goopSelectionByCode = true;
                int listIndex = selectedRegions[i];
                goopRegionListBox.SetSelected(listIndex, true);
            }
        }

        public void SyncListBoxToRegions()
        {
            goopSelectionByCode = true;
            goopRegionListBox.Items.Clear();
            for (int i = 0; i < goopCutRegions.Count; i++)
            {
                goopRegionListBox.Items.Add("Goop Cut Region " + i);
            }
            goopRegionListBox.EndUpdate(); // Allow listbox to update
            UpdateListBoxByCode();
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

        private float GetDistance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        /// <summary>
        /// Checkbox to swap between perspective and orthographic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        private void addRegionButton_Click(object sender, EventArgs e)
        {
            GoopRegionBox newGoopBox = new GoopRegionBox();
            Vector3 camPos = cameraPosition / matrixZoomAmount;
            newGoopBox.centerPoint = new Vector3(camPos.X, 10000, camPos.Z);
            newGoopBox.size = 1000;
            goopCutRegions.Add(newGoopBox);
            selectedRegions.Clear();
            selectedRegions.Add(goopCutRegions.Count - 1);
            SaveAction();

            SyncListBoxToRegions();

            glControl1.Invalidate();
        }

        /// <summary>
        /// Highlights the correct box when the corrosponding list item is clicked
        /// If you're changing the selected items by code, make goopSelectionByCode true before any
        /// changing any indexes. It calls this function otherwise
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goopRegionListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(goopSelectionByCode)
            {
                goopSelectionByCode = false;
                return;
            }
            selectedRegions.Clear();
            foreach (int selectionIndex in goopRegionListBox.SelectedIndices)
            {
                selectedRegions.Add(selectionIndex);
                Console.WriteLine("List Selected: " + selectionIndex);
            }

            glControl1.Invalidate();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Z && e.Control && !e.Shift) // Undo
            {
                UndoAction();
            }

            if (e.KeyCode == Keys.Y && e.Control) // Redo
            {
                RedoAction();
            }

            if (e.KeyCode == Keys.Z && e.Control && e.Shift) // Redo
            {
                RedoAction();
            }
        }

        #region UNDO/REDO
        
        private void UndoAction()
        {
            if (undoStack.Count <= 1) // Can't undo more
            {
                return;
            }
            Console.WriteLine("UNDOING ACTION");
            redoStack.Push(undoStack.Pop()); // Add last action we did to the redo
            RegionState stateToUndoTo = new RegionState(undoStack.Peek());

            if(stateToUndoTo.currentRegions.Length > 0)
            {
                Console.WriteLine("UNDID CENTER: " + stateToUndoTo.currentRegions[0].centerPoint);
            }
            Console.WriteLine("NEW SELECTED COUNT: " + stateToUndoTo.selectedRegions.Length);

            goopCutRegions = new List<GoopRegionBox>(stateToUndoTo.currentRegions);
            selectedRegions = new List<int>(stateToUndoTo.selectedRegions);
            SyncListBoxToRegions();
            glControl1.Refresh();
        }

        private void RedoAction()
        {
            if (redoStack.Count < 1) // Can't redo more
            {
                return;
            }
            Console.WriteLine("REDOING ACTION");
            undoStack.Push(redoStack.Pop()); // Add last action we did to the undo
            RegionState stateToUndoTo = new RegionState(undoStack.Peek());
            goopCutRegions = new List<GoopRegionBox>(stateToUndoTo.currentRegions);
            selectedRegions = new List<int>(stateToUndoTo.selectedRegions);
            SyncListBoxToRegions();
            glControl1.Refresh();
        }

        private void SaveAction()
        {
            // Don't save action if we didn't make any changes
            RegionState newState = new RegionState(goopCutRegions, selectedRegions);
            if (newState.Equals(undoStack.Peek()))
            {
                return;
            }
            // Can't redo anymore
            redoStack.Clear();
            Console.WriteLine("SAVING ACTION");
            Console.WriteLine("MOVED CENTER: " + newState.currentRegions[0].centerPoint);
            undoStack.Push(newState);
        }

        #endregion

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
