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
using System.Drawing;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Drawing.Drawing2D;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using System.Diagnostics;
using System.Threading;
using Goopify.Forms.ToolForms;

namespace Goopify
{
    public enum ViewAxis : int
    {
        X,
        Y,
    }

    public partial class EditorWindow : Form
    {

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!hasSaved)
            {
                var result = MessageBox.Show("There are unsaved changes. Are you sure you want to exit?", "Goopify Prompt", MessageBoxButtons.YesNo);
                e.Cancel = result == DialogResult.No;
            }
            if(!e.Cancel)
            {
                if (Program.startingForm != null)
                    Program.startingForm.Close();
                base.OnFormClosing(e);
            }
        }

        private void backToMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool cancelled = false;
            if (!hasSaved)
            {
                var result = MessageBox.Show("There are unsaved changes. Are you sure you want to exit?", "Goopify Prompt", MessageBoxButtons.YesNo);
                cancelled = result == DialogResult.No;
            }
            if (!cancelled)
            {
                if (Program.startingForm != null)
                    Program.startingForm.Show();
                this.Hide();
            }
        }

        private void newMenuItem_Click(object sender, EventArgs e)
        {
            bool cancelled = false;
            if (!hasSaved)
            {
                var result = MessageBox.Show("There are unsaved changes. Are you sure you want to exit?", "Goopify Prompt", MessageBoxButtons.YesNo);
                cancelled = result == DialogResult.No;
            }
            if (!cancelled)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Collision File (*.col)|*.col";
                fileDialog.InitialDirectory = Properties.Settings.Default.loadColDialogueRestore;
                if (fileDialog.ShowDialog() == DialogResult.OK) // If the Ok button is hit
                {
                    // Save the directory for next time
                    Properties.Settings.Default.loadColDialogueRestore = Path.GetDirectoryName(fileDialog.FileName);
                    Properties.Settings.Default.Save();
                    // Open and setup the window
                    this.Hide();
                    EditorWindow editorWindow = new EditorWindow();
                    editorWindow.Show();

                    editorWindow.NewGoopMap(fileDialog.FileName);
                }
            }
        }

        private void openMenuItem_Click(object sender, EventArgs e)
        {
            bool cancelled = false;
            if (!hasSaved)
            {
                var result = MessageBox.Show("There are unsaved changes. Are you sure you want to exit?", "Goopify Prompt", MessageBoxButtons.YesNo);
                cancelled = result == DialogResult.No;
            }
            if (!cancelled)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "GoopMap File (*.goo)|*.goo";
                fileDialog.InitialDirectory = Properties.Settings.Default.loadGoopDialogueRestore;
                if (fileDialog.ShowDialog() == DialogResult.OK) // If the Ok button is hit
                {
                    // Save the directory for next time
                    Properties.Settings.Default.loadGoopDialogueRestore = Path.GetDirectoryName(fileDialog.FileName);
                    Properties.Settings.Default.Save();
                    // Open and setup the window
                    this.Hide();
                    EditorWindow editorWindow = new EditorWindow();
                    editorWindow.Show();

                    editorWindow.LoadGoopMap(fileDialog.FileName);
                }
            }
        }

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

        public static Shader visualShader;
        public static Shader colShader;

        string vertexShaderCode =
                @"
                #version 330 core

                layout (location = 0) in vec3 aPosition;
                layout (location = 1) in vec3 aColor;
                layout (location = 2) in vec2 aTexCoord;
                layout (location = 3) in vec2 aVisualCoord;

                out vec3 vColor;
                out vec2 TexCoord;
                out vec2 VisualCoord;

                uniform mat4 uView;
                uniform mat4 uProjection;

                void main()
                {
                    gl_Position = uProjection * uView * vec4(aPosition, 1.0);
                    vColor = aColor;
                    TexCoord = aTexCoord;
                    VisualCoord = aVisualCoord;
                }
                ";

        string pixelShaderCode =
            @"
                #version 330 core

                in vec3 vColor;
                in vec2 TexCoord;
                in vec2 VisualCoord;

                out vec4 FragColor;

                uniform sampler2D maskTexture;   // Black & White Mask
                uniform sampler2D colorTexture;  // Foreground Texture

                void main()
                {
                    float mask = texture(maskTexture, TexCoord).r; // Sample the red channel of mask
                    vec4 color = texture(colorTexture, VisualCoord);  // Sample the color texture
                    float alpha = 1;
                    if(mask < 0.5) { alpha = 0; }
                    // Use mask as alpha value for transparency
                    FragColor = vec4(color.rgb, alpha);
                }
            ";

        string colVertexShaderCode =
                @"
                #version 330 core

                layout (location = 0) in vec3 aPosition;
                layout (location = 1) in vec3 aColor;
                layout (location = 2) in vec2 aTexCoord;
                layout (location = 3) in vec2 aVisualCoord;

                out vec3 vColor;
                out vec2 TexCoord;
                out vec2 VisualCoord;

                uniform mat4 uView;
                uniform mat4 uProjection;

                void main()
                {
                    gl_Position = uProjection * uView * vec4(aPosition, 1.0);
                    vColor = aColor;
                    TexCoord = aTexCoord;
                    VisualCoord = aVisualCoord;
                }
                ";

        string colPixelShaderCode =
            @"
                #version 330 core

                in vec3 vColor;
                in vec2 TexCoord;
                in vec2 VisualCoord;

                out vec4 FragColor;

                uniform sampler2D maskTexture;   // Black & White Mask
                uniform sampler2D colorTexture;  // Foreground Texture

                void main()
                {
                    FragColor = vec4(vColor, 1);
                }
            ";

        // Global values for hardcoded sunshine limits
        public static float maxRegionHeight = 10240; // Highest a goop region can go

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
        private const float zNear = 0.05f;
        private const float zFar = 10000f;
        private const float zNearOrtho = -0.2f;
        private const float zFarOrtho = 40f;
        private float cameraFOV = (float)((70f * Math.PI) / 180f);
        private bool isOrthographic = true;
        private float orthoZoom = 1f;
        private const float orthoZoomBorder = 0.1f;

        private const float camYHeight = 10f;

        private const float matrixZoomAmount = 0.0001f; // Used for scaling the models down and moving the camera on the same scale

        // Editable settings
        private bool cameraMoveY = false;
        private float cameraSpeed = 1;

        private const float regionHeightIfOrthographic = 100000;

        private const float regionCornerwidthPercent = 0.2f;

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


        private Col mapCol; // Initial collision file we're using to preview the map and cut the pollution from

        public static Dictionary<int, Color> colNumColor = new Dictionary<int, Color> {
            { -1, Color.White },
            { 0, Color.White },
            { 40960, Color.White},
            { 1793, Color.FromArgb(227, 234, 176) },
            { 2048, Color.DarkSlateBlue },
            { 1536, Color.Gray },
            { 256, Color.FromArgb(163, 228, 241) },
            { 258, Color.FromArgb(135, 176, 239) },
            { 257, Color.FromArgb(194, 169, 248) },
            { 259, Color.FromArgb(176, 103, 246) },
            { 262, Color.FromArgb(30, 46, 137) },
            { 263, Color.FromArgb(215, 192, 149) }
        };

        private bool drawingLine = false;
        private bool lineDrawn = false;
        private Vector3 lineStart;
        private Vector3 lineEnd;

        public enum PollutionLayerType { Normal, NormalCopy, WallPlusX, WallMinusX, WallPlusZ, WallMinusZ, Wave}
        private PollutionLayerType newRegionLayerType = PollutionLayerType.Normal;

        public enum Corner { TopLeft, TopRight, BottomRight, BottomLeft }

        private PollutionLayerType[] supportedLayerTypes = new PollutionLayerType[] { PollutionLayerType.Normal, PollutionLayerType.NormalCopy };

        private bool editorPosIsCenter = true;
        private bool lockRegionSizeToPowers = true;
        private bool clampRegionSize = true;
        private decimal regionMinSize = 1024m;
        private decimal regionMaxSize = 16384m;

        private string savePath = "";
        private bool hasSaved = true;

        /// <summary>
        /// Goop information for the "creation" step of the editor
        /// Contains position info, height/image depth, and layer type
        /// </summary>
        public class GoopRegionBox
        {
            public Vector3 startPos;
            public float width;
            public float length;
            public float height = 0;
            public int texWorldScale = 32;
            public bool autoHeight = true;
            public PollutionLayerType layerType = PollutionLayerType.Normal;

            public GoopRegionBox Clone()
            {
                GoopRegionBox newGoopRegion = new GoopRegionBox();
                newGoopRegion.startPos = startPos;
                newGoopRegion.width = width;
                newGoopRegion.length = length;
                newGoopRegion.height = height;
                newGoopRegion.texWorldScale = texWorldScale;
                newGoopRegion.autoHeight = autoHeight;
                newGoopRegion.layerType = layerType;
                return newGoopRegion;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="corner">0 TL, 1 TR, 2 BR, 3 BL</param>
            /// <returns></returns>
            public Vector3 GetCornerPos(Corner corner)
            {
                float horizOffset = corner == Corner.TopLeft || corner == Corner.BottomLeft ? 0 : width;
                float verticalOffset = corner == Corner.TopLeft || corner == Corner.TopRight ? 0 : length;
                Vector3 offsetVector = new Vector3(horizOffset, height, verticalOffset);
                /*switch (layerType)
                {
                    case PollutionLayerType.WallMinusX:
                    case PollutionLayerType.WallPlusX:
                    case PollutionLayerType.WallMinusZ: // Noki
                        offsetVector = new Vector3(offsetVector.X, offsetVector.Z, offsetVector.Y);
                        break;
                    case PollutionLayerType.WallPlusZ:
                        offsetVector = new Vector3(offsetVector.X, -offsetVector.Z, offsetVector.Y);
                        break;
                }*/
                return startPos + offsetVector;
            }

            /// <summary>
            /// Gets the position of the 
            /// </summary>
            /// <param name="regionBox"></param>
            /// <param name="corner">0 TL, 1 TR, 2 BR, 3 BL</param>
            /// <returns></returns>
            public Vector3 GetResizeCornerPos(Corner corner)
            {
                // Get the percentage of the shortest side length
                float shortestSide = length <= width ? length : width;
                shortestSide *= regionCornerwidthPercent;
                int horizOffset = corner == Corner.TopLeft || corner == Corner.BottomLeft ? 1 : -1;
                int vertOffset = corner == Corner.TopLeft || corner == Corner.TopRight ? 1 : -1;
                return GetCornerPos(corner) + new Vector3(shortestSide * horizOffset, 0, shortestSide * vertOffset);
            }

            public Vector3 ForwardDirection()
            {
                switch (layerType)
                {
                    case PollutionLayerType.WallPlusX:
                        return Vector3.UnitX;
                    case PollutionLayerType.WallMinusX:
                        return -Vector3.UnitX;
                    case PollutionLayerType.WallPlusZ:
                        return Vector3.UnitZ;
                    case PollutionLayerType.WallMinusZ:
                        return -Vector3.UnitZ;
                }
                return Vector3.UnitY;
            }
        }

        private List<GoopRegionBox> goopCutRegions = new List<GoopRegionBox>();
        private List<int> selectedRegions = new List<int>(); // For storing which regions are selected in the editor list boxes
        private List<Vector3> selectedRegionOffset = new List<Vector3>(); // Position offsets for regions when dragging them around
        private bool goopSelectionByCode;

        private const float autoHeightOffset = -50; // Hight we add to auto-heighted goop regionboxes (-40 since goop doesn't seem to like if the first pixel is completly black)

        private enum GoopRegionClick { ClickedNothing, ClickedCenter, ClickedCorner, Null }
        private GoopRegionClick currentClickType;
        private Corner currentResizeCorner;

        /// <summary>
        /// State information for redoing/undoing changes to the goop regions in the creation editor
        /// </summary>
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
                var list1Subset = currentRegions.Select(i => new { i.startPos, i.width, i.length });
                var list2Subset = otherRegionState.currentRegions.Select(i => new { i.startPos, i.width, i.length });

                bool regionsEqual = list1Subset.SequenceEqual(list2Subset);

                bool selectedEqual = selectedRegions.SequenceEqual(otherRegionState.selectedRegions);

                return regionsEqual && selectedEqual;
            }
        }
        // Stacks for 
        private Stack<RegionState> undoStack = new Stack<RegionState>();
        private Stack<RegionState> redoStack = new Stack<RegionState>();

        // Snapping settings (TODO: allow for setting changes for this)
        public class SnapSettings
        {
            public bool snapToRegionEdge = false;
            public bool snapToRegionCorner = false;

            public bool snapToGrid = true;
            public int snapInterval = 64;

            public bool resizeRegionToPower = true;

            public SnapSettings() {
                snapToRegionEdge = Properties.Settings.Default.snapToRegionEdge;
                snapToRegionCorner = Properties.Settings.Default.snapToRegionCorner;

                snapToGrid = Properties.Settings.Default.snapToGrid;
                snapInterval = Properties.Settings.Default.snapInterval;
            }

            public void SettingsChanged()
            {
                Properties.Settings.Default.snapToRegionEdge = snapToRegionEdge;
                Properties.Settings.Default.snapToRegionCorner = snapToRegionCorner;

                Properties.Settings.Default.snapToGrid = snapToGrid;
                Properties.Settings.Default.snapInterval = snapInterval;

                Properties.Settings.Default.Save();
            }
        }
        public SnapSettings snapSettings = new SnapSettings();
        public float snapRange = 2048f;

        public Vector3[] storedCorners = new Vector3[8];
        // Tries to snap to grid, then region edge, then region corner
        public Vector3 GetSnapPoint(Vector3 worldPoint)
        {
            Vector3 modifiedPoint = worldPoint;
            // Grid snapping
            if (snapSettings.snapToGrid) {
                modifiedPoint = new Vector3(RoundToInterval(worldPoint.X), RoundToInterval(worldPoint.Y), RoundToInterval(worldPoint.Z));
            }
            // Snap to corner/edge only if moving one region
            if(selectedRegions.Count == 1 && (snapSettings.snapToRegionCorner || snapSettings.snapToRegionEdge)) {
                Vector3? edgeSnapPos = null;
                float edgeMinDist = snapRange;
                Vector3? cornerSnapPos = null;
                float cornerMinDist = snapRange;
                for (int i = 0; i < goopCutRegions.Count; i++) {
                    if(i == selectedRegions[0]) { continue; } // Don't check if can snap to self

                    // Snap to corner logic
                    if (snapSettings.snapToRegionCorner) { 
                        // Check distance between each corner of selected and current region
                        for(int x = 0; x < 4; x++) {
                            for(int y = 0; y < 4; y++) {
                                Vector3 otherCorner = goopCutRegions[i].GetCornerPos((Corner)x);
                                otherCorner.Y = 20000;
                                Vector3 selectedCorner = goopCutRegions[selectedRegions[0]].GetCornerPos((Corner)y);
                                selectedCorner.Y = 20000;
                                storedCorners[x] = otherCorner;
                                storedCorners[4 + y] = selectedCorner;
                                float cornerDist = Vector3.Distance(otherCorner, selectedCorner);
                                // Save the position we need to move the cursor to if this distance is the smallest
                                if(cornerDist < cornerMinDist) {
                                    cornerMinDist = cornerDist;
                                    cornerSnapPos = selectedCorner - otherCorner;
                                }
                            }
                        }
                    }

                    // Snap to edge logic
                    if (snapSettings.snapToRegionEdge) { 

                    }
                }
                if(cornerSnapPos != null) {
                    modifiedPoint = goopCutRegions[selectedRegions[0]].GetCornerPos(Corner.TopLeft) - cornerSnapPos.Value;
                }
            }
            return modifiedPoint;
        }

        public float RoundToInterval(float i)
        {
            return ((float)Math.Round(i / snapSettings.snapInterval)) * snapSettings.snapInterval;
        }

        // Rounds to the closest value that is a power of 2, ex. 1023 would output 1024
        public int RoundToNearestPower(float num)
        {
            return 1 << (int)(BitConverter.DoubleToInt64Bits(num + num/3) >> 52) - 1023;
        }

        public decimal Clamp(decimal min, decimal max, decimal val)
        {
            if (val < min) return min;
            else if (val > max) return max;
            else return val;
        }

        public Vector3 RotateVector(Vector3 v, float radians)
        {
            float ca = (float)Math.Cos(radians);
            float sa = (float)Math.Sin(radians);
            return new Vector3(ca * v.X - sa * v.Z, v.Y, sa * v.X + ca * v.Z);
        }

        // Displays the snap settings window when the menuStripItem is clicked
        private SnapSettingsSubform snapSettingsForm;
        private void snapSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(snapSettingsForm == null) {
                snapSettingsForm = new SnapSettingsSubform(this);
            }
            snapSettingsForm.Show();
        }

        public static void CheckForGlError()
        {
            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Console.WriteLine("OpenGL Error after DrawArrays: " + error);
            }
        }

        // Main functions
        public EditorWindow()
        {
            undoStack.Push(new RegionState()); // Push empty goop region to stack to start
            
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            if(Program.startingForm != null)
                this.Location = Program.startingForm.Location;

            // Timer event for 
            UpdateTimer.Interval = 16; // 60 fps
            UpdateTimer.Elapsed += UpdateTimer_Triggered;

            UpdateRegionInfoArea();

            paintingPanel.Visible = false;

            regionTypeComboBox.SelectedIndex = 0;
            pollutionTypeComboBox.SelectedIndex = 2;

            // Update the pollution dropdown options when the folder is updated
            FileSystemWatcher watcher = new FileSystemWatcher(GoopResources.GetResourcesFolderPath());
            watcher.Created += ResourceFolderChanged;
            watcher.Changed += ResourceFolderChanged;
            watcher.Deleted += ResourceFolderChanged;
            watcher.Renamed += ResourceFolderChanged;
            watcher.EnableRaisingEvents = true;
        }

        public void ResourceFolderChanged(object Sender, FileSystemEventArgs e)
        {
            // Reload the visuals in case the preview image changed
            if (visualPollutionRegions.Count > 0) {
                foreach(VisualPollutionRegion curVisualRegion in visualPollutionRegions)
                {
                    curVisualRegion.ChangeVisual(curVisualRegion.visualType);
                }
            }
            // Reload the options
            this.Invoke(new MethodInvoker(delegate ()
            {
                UpdateVisualComboBox();
            }));
        }

        public void UpdateWindowTitle()
        {
            string formTitle = "Goopify";
            if(!hasSaved) { formTitle += "*"; }
            if (savePath != "") { formTitle += " [" + Path.GetFileNameWithoutExtension(savePath) + "]"; }
            this.Text = formTitle;
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
            Vector3 cameraForwardVector = new Vector3((float)(Math.Cos(cameraRotation.X) * Math.Cos(cameraRotation.Y)),
                                                   (float)(Math.Sin(cameraRotation.Y)),
                                                   (float)(Math.Sin(cameraRotation.X) * Math.Cos(cameraRotation.Y)));

            cameraForwardVector = cameraForwardVector.Normalized();
            cameraMatrix = Matrix4.LookAt(cameraPosition, cameraPosition + cameraForwardVector, Vector3.UnitY);

            


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

        private int LoadTexture(Bitmap bitmap, int quality = 0, bool repeat = true)
        {
            bitmap = (Bitmap)bitmap.Clone();

            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);

            switch (quality)
            {
                case 0:
                default://Low quality
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                    break;
                case 1://High quality
                       //This is in my opinion the best since it doesnt average the result and not blurred to shit
                       //but most consider this low quality...
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                    break;
            }

            if (repeat)
            {
                //This will repeat the texture past its bounds set by TexImage2D
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);
            }
            else
            {
                //This will clamp the texture to the edge, so manipulation will result in skewing
                //It can also be useful for getting rid of repeating texture bits at the borders
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

            //Load the data from are loaded image into virtual memory so it can be read at runtime
            System.Drawing.Imaging.BitmapData bitmap_data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, bitmap.Width, bitmap.Height, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap_data.Scan0);

            //Release from memory
            bitmap.UnlockBits(bitmap_data);

            //get rid of bitmap object its no longer needed in this method
            bitmap.Dispose();

            GL.BindTexture(TextureTarget.Texture2D, 0);
            
            return texture;
        }

        /// <summary>
        /// Loads the glControl1 window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;

            glControl1.MakeCurrent();

            // Enables depth so the models have render order
            GL.Enable(EnableCap.DepthTest);
            GL.DepthRange(0.1f, 100000000f);
            GL.DepthFunc(DepthFunction.Less);

            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);
            //float[] spotdirection = { -1.0f, -1.0f, -1.0f };
            //GL.Light(LightName.Light0, LightParameter.SpotDirection, spotdirection);

            GL.FrontFace(FrontFaceDirection.Cw); // Markes front face as clockwise

            // Setup Camera and display settings
            cameraPosition = new Vector3(0f, 1f, 0f);
            cameraRotation = new Vector3((float)Math.PI * 1.5f, (float)Math.PI * -0.5f + 0.01f, 0f);

            if (isOrthographic) // Starts the orthographic camera looking down
            {
                cameraPosition = new Vector3(0f, 10f, 0f);
                cameraRotation = new Vector3((float)Math.PI * 1.5f, (float)Math.PI * -0.5f + 0.00001f, 0f);
            }

            UpdateViewport();
            UpdateCamera();

            GL.ClearColor(Color.FromArgb(2, 4, 18));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            visualShader = new Shader(vertexShaderCode, pixelShaderCode);
            colShader = new Shader(colVertexShaderCode, colPixelShaderCode);

            loaded = true;

            glControl1.Refresh();
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

            bool onVisualStep = visualPollutionRegions.Count > 0;

            //Draw a circle for painting
            int numOfSides = 20;
            if (onVisualStep && bmpPen != null && !cursorHidden)
            {
                GL.LineWidth(2);
                GL.Begin(PrimitiveType.Lines);
                double step = Math.PI * 2 / numOfSides;
                float radius = bmpPen.Width/2*32;
                Vector3 mouseWorldPos = ScreenToWorld(prevMousePos.X, prevMousePos.Y);
                for (int i = 0; i < numOfSides; i++)
                {
                    GL.Color3(Color.Red);
                    Vector3 firstPoint = new Vector3((float)Math.Cos(i * step) * radius, 0, (float)Math.Sin(i * step) * radius);
                    Vector3 secondPoint = new Vector3((float)Math.Cos((i + 1) * step) * radius, 0, (float)Math.Sin((i + 1) * step) * radius);
                    GL.Vertex3(firstPoint + mouseWorldPos);
                    GL.Vertex3(secondPoint + mouseWorldPos);
                }

                GL.End();
            }

            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);


            /**COLLISION MODEL**/
            colShader.Use();
            colShader.SetMatrix4("uView", cameraMatrix);
            colShader.SetMatrix4("uProjection", projectionMatrix);
            // Render the collision object
            if (mapCol != null)
            {
                mapCol.RenderCol();
            }
            colShader.Stop();


            /**VISUAL MODELS**/
            GL.DepthMask(false);
            visualShader.Use();
            visualShader.SetMatrix4("uView", cameraMatrix);
            visualShader.SetMatrix4("uProjection", projectionMatrix);
            foreach (VisualPollutionRegion visualRegion in visualPollutionRegions)
            {
                int current_texture = 0;
                // Load the visual texture from the correct goop visual type, otherwise use a fallback one
                if (visualRegion.visualBitmap != null) {
                    current_texture = LoadTexture(visualRegion.visualBitmap, 1); // Generates the texture for us to use
                } else {
                    current_texture = LoadTexture(Properties.Resources.defaultGoopTexture, 1); // Generates the texture for us to use
                }
                int pollution_texture = LoadTexture(visualRegion.pollutionBmp, 0); // Generates the texture for us to use

                visualRegion.pollutionModel.RenderCol(pollution_texture, current_texture);
            }
            visualShader.Stop();
            GL.DepthMask(true);



            GL.Disable(EnableCap.Light0);
            GL.Disable(EnableCap.Lighting);

            // For drawing a line onscreen to cut the model
            if (drawingLine || lineDrawn)
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
            for (int i = 0; i < goopCutRegions.Count; i++)
            {
                var isSelectedRegion = selectedRegions.Contains(i);

                // Draw box lines
                GL.LineWidth(1f);
                Color lineColor = onVisualStep ? Color.Maroon : Color.DarkGray;
                if(isSelectedRegion)
                {
                    GL.LineWidth(4f);
                    lineColor = onVisualStep ? Color.Red : Color.Gray;
                }
                GL.Begin(PrimitiveType.Lines);

                Vector3 boxTopLeft = goopCutRegions[i].GetCornerPos(Corner.TopLeft);
                Vector3 boxTopRight = goopCutRegions[i].GetCornerPos(Corner.TopRight);
                Vector3 boxBottomRight = goopCutRegions[i].GetCornerPos(Corner.BottomRight);
                Vector3 boxBottomLeft = goopCutRegions[i].GetCornerPos(Corner.BottomLeft);
                if(isOrthographic)
                {
                    boxTopLeft.Y = regionHeightIfOrthographic;
                    boxTopRight.Y = regionHeightIfOrthographic;
                    boxBottomRight.Y = regionHeightIfOrthographic;
                    boxBottomLeft.Y = regionHeightIfOrthographic;
                }

                DrawGlLine(boxTopLeft, boxTopRight, lineColor);
                DrawGlLine(boxTopRight, boxBottomRight, lineColor);
                DrawGlLine(boxBottomRight, boxBottomLeft, lineColor);
                DrawGlLine(boxBottomLeft, boxTopLeft, lineColor);

                // Draw height lines for the regionbox when not in orthographic
                if (!isOrthographic)
                {
                    GL.LineWidth(0.7f);
                    Vector3 heightOffsetVector = new Vector3(0, maxRegionHeight, 0);
                    // Draw another square on the top limit of goop
                    DrawGlLine(boxTopLeft + heightOffsetVector, boxTopRight + heightOffsetVector, lineColor);
                    DrawGlLine(boxTopRight + heightOffsetVector, boxBottomRight + heightOffsetVector, lineColor);
                    DrawGlLine(boxBottomRight + heightOffsetVector, boxBottomLeft + heightOffsetVector, lineColor);
                    DrawGlLine(boxBottomLeft + heightOffsetVector, boxTopLeft + heightOffsetVector, lineColor);
                    // Draw the lines upwards from the box
                    DrawGlLine(boxTopLeft, boxTopLeft + heightOffsetVector, lineColor);
                    DrawGlLine(boxTopRight, boxTopRight + heightOffsetVector, lineColor);
                    DrawGlLine(boxBottomRight, boxBottomRight + heightOffsetVector, lineColor);
                    DrawGlLine(boxBottomLeft, boxBottomLeft + heightOffsetVector, lineColor);
                    GL.LineWidth(1f);
                }

                GL.End();

                // Draw box verts
                GL.PointSize(onVisualStep ? 4 : 5);
                Color dotColor = onVisualStep? Color.DarkRed : Color.Gray;
                if (isSelectedRegion)
                {
                    GL.PointSize(7);
                    dotColor = onVisualStep ? Color.Maroon : Color.White;
                }
                GL.Begin(PrimitiveType.Points);

                GL.Color4(dotColor);
                GL.Vertex3(boxTopLeft);
                GL.Vertex3(boxTopRight);
                GL.Vertex3(boxBottomRight);
                GL.Vertex3(boxBottomLeft);
                // Center point for selected boxes
                if(isSelectedRegion) {
                    Vector3 centerPoint = (boxBottomLeft + boxTopRight) / 2f;
                    centerPoint.Y = boxBottomLeft.Y;
                    GL.Vertex3(centerPoint);
                }

                //GL.Vertex3(goopCutRegions[i].startPos);

                GL.End();

                // Draw direction arrows for wall goop
                Vector3 regionForwardDir = goopCutRegions[i].ForwardDirection();
                if(regionForwardDir != Vector3.UnitY)
                {
                    Vector3 arrowStartPos = goopCutRegions[i].startPos;
                    Vector3 arrowEndPos = arrowStartPos + (regionForwardDir * 1024f);
                    if(isOrthographic)
                    {
                        arrowStartPos.Y = regionHeightIfOrthographic;
                        arrowEndPos.Y = regionHeightIfOrthographic;
                    }

                    Color arrowColor = isSelectedRegion ? Color.HotPink : Color.DeepPink;
                    // Arrow Line
                    GL.Begin(PrimitiveType.Lines);
                    DrawGlLine(arrowStartPos, arrowEndPos, arrowColor);
                    GL.End();
                    // Arrow Head
                    float arrowStrength = isSelectedRegion ? 1 : 0.5f;
                    Vector3 regionRightDir = new Vector3(-regionForwardDir.Z, regionForwardDir.Y, regionForwardDir.X);
                    Vector3 arrowLeftEdge = arrowStartPos + (regionForwardDir * 256f * arrowStrength) - (regionRightDir * 128f * arrowStrength);
                    Vector3 arrowRightEdge = arrowStartPos + (regionForwardDir * 256f * arrowStrength) + (regionRightDir * 128f * arrowStrength);
                    GL.Begin(PrimitiveType.Triangles);
                    GL.Color3(arrowColor);
                    GL.Vertex3(arrowStartPos);
                    GL.Vertex3(arrowLeftEdge);
                    GL.Vertex3(arrowRightEdge);
                    GL.End();
                }

                if (!onVisualStep) {
                    // Draw corners for boxes to make it easer to rewidth
                    Color selectLineColor = Color.Cyan;
                    GL.Begin(PrimitiveType.Lines);

                    Vector3 selectBoxTopLeft = goopCutRegions[i].GetResizeCornerPos(Corner.TopLeft);
                    Vector3 selectBoxTopRight = goopCutRegions[i].GetResizeCornerPos(Corner.TopRight);
                    Vector3 selectBoxBottomRight = goopCutRegions[i].GetResizeCornerPos(Corner.BottomRight);
                    Vector3 selectBoxBottomLeft = goopCutRegions[i].GetResizeCornerPos(Corner.BottomLeft);

                    if (isOrthographic)
                    {
                        selectBoxTopLeft.Y = regionHeightIfOrthographic;
                        selectBoxTopRight.Y = regionHeightIfOrthographic;
                        selectBoxBottomRight.Y = regionHeightIfOrthographic;
                        selectBoxBottomLeft.Y = regionHeightIfOrthographic;
                    }
                    selectLineColor = currentClickType == GoopRegionClick.ClickedCorner && currentResizeCorner == Corner.TopLeft ? Color.AliceBlue : Color.Cyan;
                    DrawGlLine(selectBoxTopLeft, new Vector3(selectBoxTopLeft.X, selectBoxTopLeft.Y, boxTopLeft.Z), selectLineColor);
                    DrawGlLine(selectBoxTopLeft, new Vector3(boxTopLeft.X, selectBoxTopLeft.Y, selectBoxTopLeft.Z), selectLineColor);

                    selectLineColor = currentClickType == GoopRegionClick.ClickedCorner && currentResizeCorner == Corner.TopRight ? Color.AliceBlue : Color.Cyan;
                    DrawGlLine(selectBoxTopRight, new Vector3(selectBoxTopRight.X, selectBoxTopRight.Y, boxTopRight.Z), selectLineColor);
                    DrawGlLine(selectBoxTopRight, new Vector3(boxTopRight.X, selectBoxTopRight.Y, selectBoxTopRight.Z), selectLineColor);

                    selectLineColor = currentClickType == GoopRegionClick.ClickedCorner && currentResizeCorner == Corner.BottomRight ? Color.AliceBlue : Color.Cyan;
                    DrawGlLine(selectBoxBottomRight, new Vector3(selectBoxBottomRight.X, selectBoxBottomRight.Y, boxBottomRight.Z), selectLineColor);
                    DrawGlLine(selectBoxBottomRight, new Vector3(boxBottomRight.X, selectBoxBottomRight.Y, selectBoxBottomRight.Z), selectLineColor);

                    selectLineColor = currentClickType == GoopRegionClick.ClickedCorner && currentResizeCorner == Corner.BottomLeft ? Color.AliceBlue : Color.Cyan;
                    DrawGlLine(selectBoxBottomLeft, new Vector3(selectBoxBottomLeft.X, selectBoxBottomLeft.Y, boxBottomLeft.Z), selectLineColor);
                    DrawGlLine(selectBoxBottomLeft, new Vector3(boxBottomLeft.X, selectBoxBottomLeft.Y, selectBoxBottomLeft.Z), selectLineColor);

                    GL.End();
                }
            }

            // Draw out corner points of moving regions (for testing)
            GL.PointSize(12);
            GL.Begin(PrimitiveType.Points);

            for(int i = 0; i < 8; i++)
            {
                if(i < 4) { GL.Color4(Color.Orange); } else { GL.Color4(Color.Purple); }
                GL.Vertex3(storedCorners[i]);
            }
            /*GL.Color4(Color.Gray);
            GL.Vertex3(testClickPos);
            GL.PointSize(10);
            GL.Color4(Color.BlueViolet);
            GL.Vertex3(testClickPos1);
            GL.Color4(Color.DeepPink);
            GL.Vertex3(testClickPos2);

            GL.End();

            GL.Begin(PrimitiveType.Lines);

            DrawGlLine(testClickPos1, testClickPos2, Color.Orange);*/

            GL.End();

            editorCamPosLabel.Text = "Cam Pos: " + (cameraPosition / matrixZoomAmount);
            editorCamPosLabel.Refresh();

            glControl1.SwapBuffers(); // Takes from the GL and puts into control
        }

        private void DrawGlLine(Vector3 pos1, Vector3 pos2, Color lineColor)
        {
            GL.Color4(lineColor);
            GL.Vertex3(pos1);
            GL.Vertex3(pos2);
        }

        /// <summary>
        /// Event for glControl1 being rewidthd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();

            UpdateViewport();
        }

        /* MOUSE EVENTS */
        private Vector3 testClickPos = new Vector3();
        private Vector3 testClickPos1 = new Vector3();
        private Vector3 testClickPos2 = new Vector3();

        private Point prevMousePos;
        private bool cursorHidden = false;
        private bool[] paintedOnVisuals;

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            prevMousePos = e.Location;

            if (e.Button == MouseButtons.Right) // Right Click pressed down
            {
                if(!isMouseLookMode) // Start mouse look mode
                {
                    // Start the camera update loop
                    UpdateTimer.Start();

                    isMouseLookMode = true;
                    Cursor.Position = ViewerCenter;
                    Cursor.Hide();
                    cursorHidden = true;
                    return;
                }
            }

            if(e.Button == MouseButtons.Left)
            {
                // Select the goop region based on clicked corners or center points
                // Already selected regions take priority, center points over corners
                Vector3 mouseWorldPos = ScreenToWorld(e.X, e.Y);
                Vector3 correctMousePos = ScreenToWorldNormal(e.X, e.Y);
                Vector3 camPos = cameraPosition / matrixZoomAmount;
                testClickPos = mouseWorldPos;
                testClickPos1 = camPos;
                testClickPos2 = correctMousePos;
                glControl1.Refresh();
                currentClickType = visualPollutionRegions.Count > 0 ? GoopRegionClick.Null : ClickGoopZones(mouseWorldPos);
                mouseOnePressed = true;
                glControl1.Refresh();


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

                paintedOnVisuals = new bool[visualPollutionRegions.Count];
                // Draw a dot if just click
                bool onVisuals = visualPollutionRegions.Count > 0;
                Vector3 worldPos = isOrthographic ? ScreenToWorld(e.X, e.Y) : cameraPosition / matrixZoomAmount;
                Vector3 direction = isOrthographic ? -Vector3.UnitY : Vector3.Normalize(ScreenToWorldNormal(e.X, e.Y) - worldPos);
                if(onVisuals) {
                    bool didPaint = false;
                    for (int i = 0; i < visualPollutionRegions.Count; i++)
                    {
                        // Do raycast from center, and then "outerPointsToCheck" points around the circurmfrunce of the brush to check if the brush isn't exactly on the model
                        for (int j = 0; j < outerPointsToCheck+1; j++)
                        {
                            Vector3 paintOffset = Vector3.Zero;
                            if(j != 0) {
                                float radians = 360f / outerPointsToCheck * (j - 1) * ((float)Math.PI / 180f);
                                paintOffset = RotateVector(Vector3.UnitX * bmpPen.Width/2*32, radians);
                            }
                            Vector2? gottenUv = visualPollutionRegions[i].pollutionModel.GetUVFromPosAndDir(worldPos + paintOffset, direction);
                            float preWidth = bmpPen.Width;
                            bmpPen.Width = 32 / visualPollutionRegions[i].pollutionRegion.texWorldScale * bmpPen.Width;
                            if (gottenUv != null)
                            {
                                paintedOnVisuals[i] = true;
                                didPaint = true;
                                Vector3 uvOffset3D = paintOffset * visualPollutionRegions[i].pollutionModel.goopUvScale;
                                uvOffset3D.X /= (visualPollutionRegions[i].pollutionRegion.heightMapWidth * visualPollutionRegions[i].pollutionRegion.texWorldScale);
                                uvOffset3D.Z /= (visualPollutionRegions[i].pollutionRegion.heightMapLength * visualPollutionRegions[i].pollutionRegion.texWorldScale);
                                Vector2 uvOffset2D = new Vector2(uvOffset3D.X, uvOffset3D.Z);
                                visualPollutionRegions[i].PaintOnBmp(bmpPen, gottenUv.Value - uvOffset2D, gottenUv.Value - uvOffset2D);
                                bmpPen.Width = preWidth;
                                break;
                            }
                            bmpPen.Width = preWidth;
                        }
                    }
                    if(didPaint) { glControl1.Refresh(); }
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
                    cursorHidden = false;

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
                    UpdateRegionBoxHeights();
                    SaveAction();
                }
                currentClickType = GoopRegionClick.Null;
                glControl1.Refresh();
                // Save Pollution drawing
                if(paintedOnVisuals.Length > 0 && paintedOnVisuals.Any(x => x == true)) {
                    SavePaint(true);
                    UpdateVisualRegionInfoArea();
                }
            }
        }

        
        private void UpdateRegionBoxHeights()
        {
            foreach (GoopRegionBox region in goopCutRegions)
            {
                if (region.autoHeight)
                {
                    float lowestPoint = mapCol.ReturnLowestVertHeight(region.startPos.X, region.startPos.X + region.width,
                        region.startPos.Z, region.startPos.Z + region.length, Properties.Settings.Default.colTypesToRemove);
                    region.height = lowestPoint != float.MaxValue ? lowestPoint + autoHeightOffset : 0;
                }
            }
            UpdateRegionInfoArea();
        }

        private GoopRegionBox resizeGoopBoxReference;
        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            // Set this as input focus if mouse is on area
            /*if (!glControl1.Focused)
            {
                glControl1.Focus();
            }*/

            bool mouseDifferent = false;
            if (prevMousePos != e.Location) {
                mouseDifferent = true;
                //if(visualPollutionRegions.Count > 0)
                    //glControl1.Refresh();
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

                } else // Pan Mode
                {
                    //Move camera
                    cameraPosition.X -= cursorXOffset * 0.01f;
                    cameraPosition.Z -= cursorYOffset * 0.01f;
                }

                UpdateViewport();
                UpdateCamera();
                glControl1.Refresh();

            }

            if(drawingLine)
            {
                lineEnd = ScreenToWorld(e.X, e.Y);
                glControl1.Refresh();
            }

            // Moving mouse around the screen for goop sections
            if(mouseOnePressed)
            {
                bool onVisuals = visualPollutionRegions.Count > 0;

                Vector3 worldPos = isOrthographic ? ScreenToWorld(e.X, e.Y) : cameraPosition/matrixZoomAmount;
                Vector3 direction = isOrthographic ? -Vector3.UnitY : Vector3.Normalize(ScreenToWorldNormal(e.X, e.Y) - worldPos);
                /*if (!onVisuals) {
                    worldPos = GetSnapPoint(worldPos);
                }*/
                Console.WriteLine(worldPos);

                if(mouseDifferent)
                {
                    Vector3 prevWorldPos = isOrthographic ? ScreenToWorld(prevMousePos.X, prevMousePos.Y) : cameraPosition / matrixZoomAmount;
                    Vector3 prevDirection = isOrthographic ? -Vector3.UnitY : Vector3.Normalize(ScreenToWorldNormal(prevMousePos.X, prevMousePos.Y) - prevWorldPos);
                    if (!onVisuals) {
                        // Moves and scales the goop regions
                        GoopRegionMouseMove(worldPos);
                        UpdateRegionInfoArea();
                    } else {
                        // Draw on the bmp
                        bool didPaint = false;
                        for(int i = 0; i < visualPollutionRegions.Count; i++)
                        {
                            // Do raycast from center, and then "outerPointsToCheck" points around the circurmfrunce of the brush to check if the brush isn't exactly on the model
                            for (int j = 0; j < outerPointsToCheck + 1; j++)
                            {
                                Vector3 paintOffset = Vector3.Zero;
                                if (j != 0)
                                {
                                    float radians = 360f / outerPointsToCheck * (j - 1) * ((float)Math.PI / 180f);
                                    paintOffset = RotateVector(Vector3.UnitX * bmpPen.Width / 2 * 32, radians);
                                }
                                Vector2? prevUv = visualPollutionRegions[i].pollutionModel.GetUVFromPosAndDir(prevWorldPos + paintOffset, prevDirection);
                                Vector2? gottenUv = visualPollutionRegions[i].pollutionModel.GetUVFromPosAndDir(worldPos + paintOffset, direction);
                                float preWidth = bmpPen.Width;
                                bmpPen.Width = 32 / visualPollutionRegions[i].pollutionRegion.texWorldScale * bmpPen.Width;
                                if (gottenUv != null && prevUv != null)
                                {
                                    paintedOnVisuals[i] = true;
                                    didPaint = true;
                                    Vector3 uvOffset3D = paintOffset * visualPollutionRegions[i].pollutionModel.goopUvScale;
                                    uvOffset3D.X /= (visualPollutionRegions[i].pollutionRegion.heightMapWidth * visualPollutionRegions[i].pollutionRegion.texWorldScale);
                                    uvOffset3D.Z /= (visualPollutionRegions[i].pollutionRegion.heightMapLength * visualPollutionRegions[i].pollutionRegion.texWorldScale);
                                    Vector2 uvOffset2D = new Vector2(uvOffset3D.X, uvOffset3D.Z);
                                    visualPollutionRegions[i].PaintOnBmp(bmpPen, prevUv.Value - uvOffset2D, gottenUv.Value - uvOffset2D);
                                    bmpPen.Width = preWidth;
                                    break;
                                }
                                bmpPen.Width = preWidth;
                            }
                        }
                        if (didPaint) {
                            glControl1.Refresh();
                        }
                    }
                }
            }

            // Update the position
            prevMousePos = e.Location;

            // Update the painting cursor if we're on the visual step
            if(visualPollutionRegions.Count > 0)
            {
                glControl1.Refresh();
            }

            /*Vector2 mousePos = new Vector2(e.X, e.Y);
            Console.WriteLine(mousePos + ":" + lineEnd);*/
        }

        private void GoopRegionMouseMove(Vector3 mouseWorldPos)
        {
            if (currentClickType == GoopRegionClick.ClickedCenter) // Moves all selected regions
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    Vector3 newStartPos = new Vector3(mouseWorldPos.X, 0, mouseWorldPos.Z) + selectedRegionOffset[i];
                    newStartPos = GetSnapPoint(newStartPos);
                    goopCutRegions[selectedRegions[i]].startPos = newStartPos;
                }
                glControl1.Refresh();
            }
            else if (currentClickType == GoopRegionClick.ClickedCorner) // Resizes the region
            {
                GoopRegionBox firstRegion = goopCutRegions[selectedRegions[0]];
                float widthDifference = 0;
                float lengthDifference = 0;
                switch (currentResizeCorner)
                {
                    case Corner.TopLeft:
                        widthDifference = resizeGoopBoxReference.startPos.X - mouseWorldPos.X - selectedRegionOffset[0].X;
                        lengthDifference = resizeGoopBoxReference.startPos.Z - mouseWorldPos.Z - selectedRegionOffset[0].Z;
                        if (snapSettings.resizeRegionToPower)
                        { // Rounds to power of 2s since that's the sizes all the ingame regions use
                            widthDifference = RoundToNearestPower(Math.Abs(resizeGoopBoxReference.width + widthDifference)) - resizeGoopBoxReference.width;
                            lengthDifference = RoundToNearestPower(Math.Abs(resizeGoopBoxReference.length + lengthDifference)) - resizeGoopBoxReference.length;
                        }
                        firstRegion.startPos.X = resizeGoopBoxReference.startPos.X - widthDifference;
                        firstRegion.startPos.Z = resizeGoopBoxReference.startPos.Z - lengthDifference;
                        firstRegion.width = resizeGoopBoxReference.width + widthDifference;
                        firstRegion.length = resizeGoopBoxReference.length + lengthDifference;
                        break;
                    case Corner.TopRight:
                        widthDifference = resizeGoopBoxReference.startPos.X - mouseWorldPos.X - selectedRegionOffset[0].X - resizeGoopBoxReference.width;
                        lengthDifference = resizeGoopBoxReference.startPos.Z - mouseWorldPos.Z - selectedRegionOffset[0].Z;
                        if (snapSettings.resizeRegionToPower)
                        { // Rounds to power of 2s since that's the sizes all the ingame regions use
                            widthDifference = Math.Sign(widthDifference) * RoundToNearestPower(Math.Abs(widthDifference));
                            lengthDifference = RoundToNearestPower(Math.Abs(resizeGoopBoxReference.length + lengthDifference)) - resizeGoopBoxReference.length;
                        }
                        firstRegion.startPos.Z = resizeGoopBoxReference.startPos.Z - lengthDifference;
                        firstRegion.width = -widthDifference;
                        firstRegion.length = resizeGoopBoxReference.length + lengthDifference;
                        break;
                    case Corner.BottomRight:
                        widthDifference = resizeGoopBoxReference.startPos.X - mouseWorldPos.X - selectedRegionOffset[0].X - resizeGoopBoxReference.width;
                        lengthDifference = resizeGoopBoxReference.startPos.Z - mouseWorldPos.Z - selectedRegionOffset[0].Z - resizeGoopBoxReference.length;
                        if (snapSettings.resizeRegionToPower)
                        { // Rounds to power of 2s since that's the sizes all the ingame regions use
                            widthDifference = Math.Sign(widthDifference) * RoundToNearestPower(Math.Abs(widthDifference));
                            lengthDifference = Math.Sign(lengthDifference) * RoundToNearestPower(Math.Abs(lengthDifference));
                        }
                        firstRegion.width = -widthDifference;
                        firstRegion.length = -lengthDifference;
                        break;
                    case Corner.BottomLeft:
                        widthDifference = resizeGoopBoxReference.startPos.X - mouseWorldPos.X - selectedRegionOffset[0].X;
                        lengthDifference = resizeGoopBoxReference.startPos.Z - mouseWorldPos.Z - selectedRegionOffset[0].Z - resizeGoopBoxReference.length;
                        if (snapSettings.resizeRegionToPower)
                        { // Rounds to power of 2s since that's the sizes all the ingame regions use
                            widthDifference = RoundToNearestPower(Math.Abs(resizeGoopBoxReference.width + widthDifference)) - resizeGoopBoxReference.width;
                            lengthDifference = Math.Sign(lengthDifference) * RoundToNearestPower(Math.Abs(lengthDifference));
                        }
                        firstRegion.startPos.X = resizeGoopBoxReference.startPos.X - widthDifference;
                        firstRegion.width = resizeGoopBoxReference.width + widthDifference;
                        firstRegion.length = -lengthDifference;
                        break;
                }
                if (clampRegionSize) { 
                    firstRegion.length = (float)Clamp(regionMinSize, regionMaxSize, (decimal)firstRegion.length);
                    firstRegion.width = (float)Clamp(regionMinSize, regionMaxSize, (decimal)firstRegion.width);
                }
                glControl1.Refresh();
            }
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


        public void NewGoopMap(string colFilePath)
        {
            // Read the contents of the file into a stream
            var fileStream = File.Open(colFilePath, FileMode.Open);

            mapCol = new Col(fileStream); // Create the col
            //mapCol.ScaleModel(0.01f);
            fileStream.Close();

            InitializeCamera();
        }

        public void LoadGoopMap(string goopFilePath)
        {
            // Read the contents of the file into a stream
            var fileStream = File.Open(goopFilePath, FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(fileStream);

            // Loads the map col
            mapCol = new Col(binaryReader);

            // Loads the goop cut regions
            int goopRegionCount = binaryReader.ReadInt32();
            for (int i = 0; i < goopRegionCount; i++)
            {
                GoopRegionBox newGoopBox = new GoopRegionBox();
                newGoopBox.startPos.X = binaryReader.ReadSingle();
                newGoopBox.startPos.Y = binaryReader.ReadSingle();
                newGoopBox.startPos.Z = binaryReader.ReadSingle();

                newGoopBox.width = binaryReader.ReadSingle();
                newGoopBox.length = binaryReader.ReadSingle();
                newGoopBox.height = binaryReader.ReadSingle();
                newGoopBox.texWorldScale = binaryReader.ReadInt32();
                newGoopBox.autoHeight = binaryReader.ReadBoolean();
                newGoopBox.layerType = (PollutionLayerType)binaryReader.ReadInt32();

                goopCutRegions.Add(newGoopBox);
            }

            // Loads the goop visual regions
            int visualRegionCount = binaryReader.ReadInt32();
            for (int i = 0; i < visualRegionCount; i++)
            {
                Col tempPolutionModel = new Col(binaryReader);
                VisualPollutionRegion newVisualRegion = new VisualPollutionRegion(goopCutRegions[i], tempPolutionModel);

                // The pollution image
                int length = binaryReader.ReadInt32();
                byte[] imageData = binaryReader.ReadBytes(length);
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    using (var original = new Bitmap(ms))
                    {
                        newVisualRegion.pollutionBmp = new Bitmap(original);
                        newVisualRegion.SetupPollutionGraphics();
                    }
                }

                // The heightmap image
                int hightmapLength = binaryReader.ReadInt32();
                byte[] heightmapImageData = binaryReader.ReadBytes(hightmapLength);
                using (MemoryStream ms = new MemoryStream(heightmapImageData))
                {
                    using (var original = new Bitmap(ms))
                    {
                        newVisualRegion.pollutionRegion.heightMap = new Bitmap(original);
                    }
                }

                newVisualRegion.ChangeVisual(binaryReader.ReadString());

                visualPollutionRegions.Add(newVisualRegion);
            }

            fileStream.Close();

            // Setup pen if we're on the visual step
            if (visualPollutionRegions.Count > 0)
            {
                CuttingThreadDone();
            }

            SyncListBoxToRegions();
            goopSelectionByCode = false;

            InitializeCamera();

            savePath = goopFilePath;
            hasSaved = true;
            UpdateWindowTitle();

            // Update the undos so we can't reset our changes
            undoStack.Pop();
            undoStack.Push(new RegionState(goopCutRegions, selectedRegions));
        }

        /// <summary>
        /// Saves the current GoopMap progress into a .goop file that can be loaded in
        /// </summary>
        private void SaveGoopMap(string saveLocation = "")
        {
            FileStream outStream;
            // Prompts for save location if we don't have one
            if (saveLocation == "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "GoopMap File (*.goo)|*.goo";
                saveFileDialog.Title = "Select the save location of the GoopMap file";
                saveFileDialog.InitialDirectory = Properties.Settings.Default.saveGoopMapRestore;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.saveGoopMapRestore = Path.GetDirectoryName(saveFileDialog.FileName);
                    Properties.Settings.Default.Save();
                    savePath = saveFileDialog.FileName; // Save the path so we can quick save here later

                    outStream = (FileStream)saveFileDialog.OpenFile();
                } else { return; }
            } else {
                if (saveLocation == "") { return; }
                outStream = File.OpenWrite(saveLocation);
            }

            // Create the GoopMap file
            using (var writer = new BinaryWriter(outStream))
            {
                // Writes the map col
                mapCol.Write(writer);

                // Writes the goop cut regions
                writer.Write(goopCutRegions.Count);
                foreach(GoopRegionBox region in goopCutRegions)
                {
                    writer.Write(region.startPos.X);
                    writer.Write(region.startPos.Y);
                    writer.Write(region.startPos.Z);

                    writer.Write(region.width);
                    writer.Write(region.length);
                    writer.Write(region.height);
                    writer.Write(region.texWorldScale);
                    writer.Write(region.autoHeight);
                    writer.Write((int)region.layerType);
                }

                // Writes the goop visual regions
                writer.Write(visualPollutionRegions.Count);
                foreach (VisualPollutionRegion region in visualPollutionRegions)
                {
                    region.pollutionModel.Write(writer);

                    //Pollution Region is currently just made from the matching GoopRegionBox so we can remake this on load
                    //region.pollutionRegion

                    // The pollution and heightmap image
                    using (MemoryStream ms = new MemoryStream())
                    {
                        region.pollutionBmp.Save(ms, ImageFormat.Bmp);
                        byte[] imageData = ms.ToArray();

                        writer.Write(imageData.Length);
                        writer.Write(imageData);
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        region.pollutionRegion.heightMap.Save(ms, ImageFormat.Bmp);
                        byte[] imageData = ms.ToArray();

                        writer.Write(imageData.Length);
                        writer.Write(imageData);
                    }

                    writer.Write(region.visualType);
                }
            }
            outStream.Close();

            hasSaved = true;
            UpdateWindowTitle();
        }

        public void InitializeCamera()
        {
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
                orthoZoom *= -(modelHeight / screenWorldHeight + orthoZoomBorder);

            }
            else // Model width is focus for zoom
            {
                float screenWorldWidth = camTopLeft.X - camBottomRight.X;
                orthoZoom *= -(modelWidth / screenWorldWidth + orthoZoomBorder);
            }
            //Center the camera to the model
            cameraPosition = new Vector3((colBottomRightBound.X + modelWidth / 2) * matrixZoomAmount, camYHeight, (colBottomRightBound.Y + modelHeight / 2) * matrixZoomAmount);

            UpdateViewport();
            UpdateCamera();
            glControl1.Invalidate();
        }

        /// <summary>
        /// Open Collision File Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openCollisionButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Collision File (*.col)|*.col";
            fileDialog.InitialDirectory = Properties.Settings.Default.loadColDialogueRestore;
            if (fileDialog.ShowDialog() == DialogResult.OK) // If the Ok button is hit
            {
                // Save the directory for next time
                Properties.Settings.Default.loadColDialogueRestore = Path.GetDirectoryName(fileDialog.FileName);
                Properties.Settings.Default.Save();
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
            for (int i = 0; i < goopCutRegions.Count; i++)
            {
                // Checks if clicked in region
                bool clickedInArea = ClickedWithinBounds(goopCutRegions[i].GetCornerPos(Corner.TopLeft), goopCutRegions[i].GetCornerPos(Corner.BottomRight), mouseWorldPos);
                // Checks if clicked in a region corner and which one
                Corner clickedCorner = Corner.TopLeft;
                bool clickedInCorner = false;
                if(ClickedWithinBounds(goopCutRegions[i].GetCornerPos(Corner.TopLeft), goopCutRegions[i].GetResizeCornerPos(Corner.TopLeft), mouseWorldPos))
                {
                    clickedInCorner = true;
                    clickedCorner = Corner.TopLeft;
                } else if(ClickedWithinBounds(goopCutRegions[i].GetCornerPos(Corner.TopRight), goopCutRegions[i].GetResizeCornerPos(Corner.TopRight), mouseWorldPos))
                {
                    clickedInCorner = true;
                    clickedCorner = Corner.TopRight;
                } else if(ClickedWithinBounds(goopCutRegions[i].GetCornerPos(Corner.BottomRight), goopCutRegions[i].GetResizeCornerPos(Corner.BottomRight), mouseWorldPos))
                {
                    clickedInCorner = true;
                    clickedCorner = Corner.BottomRight;
                } else if(ClickedWithinBounds(goopCutRegions[i].GetCornerPos(Corner.BottomLeft), goopCutRegions[i].GetResizeCornerPos(Corner.BottomLeft), mouseWorldPos))
                {
                    clickedInCorner = true;
                    clickedCorner = Corner.BottomLeft;
                }

                if (clickedInArea) // Clicked in square
                {
                    // Selected a corner without multiselecting
                    if (clickedInCorner && !shiftKeyPressed)
                    {
                        currentClick = GoopRegionClick.ClickedCorner;
                        currentResizeCorner = clickedCorner;
                        selectedIndex = i;
                        resizeGoopBoxReference = goopCutRegions[i].Clone();
                    } else // Didn't select a corner or multiselecting
                    {
                        if (shiftKeyPressed && selectedRegions.Contains(i)) // Deselect this if we're multiselecting and already have this box
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
                //mouseWorldPos = GetSnapPoint(mouseWorldPos);
                selectedRegionOffset.Add(goopCutRegions[index].startPos - new Vector3(mouseWorldPos.X, 0, mouseWorldPos.Z));
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
            topLeftXPos.Enabled = true;
            topLeftZPos.Enabled = true;
            regionWidth.Enabled = true;
            regionLength.Enabled = true;
            pixelHeight.Enabled = true;

            goopSelectionByCode = true;
            goopRegionListBox.SelectedIndices.Clear();
            for (int i = 0; i < selectedRegions.Count(); i++)
            {
                goopSelectionByCode = true;
                int listIndex = selectedRegions[i];
                goopRegionListBox.SetSelected(listIndex, true);
            }

            UpdateRegionInfoArea();
        }

        private bool refreshingRegionInfoDisplay = false;
        public void UpdateRegionInfoArea()
        {
            refreshingRegionInfoDisplay = true;
            if (selectedRegions.Count == 0)
            {
                topLeftXPos.Enabled = false;
                topLeftZPos.Enabled = false;
                regionWidth.Enabled = false;
                regionLength.Enabled = false;
                autoHeightCheckBox.Enabled = false;
                regionHeight.Enabled = false;
                pixelHeight.Enabled = false;
                pixelHeight.Value = 0;
                topLeftXPos.Text = "0";
                topLeftZPos.Text = "0";
                regionWidth.Value = 0;
                regionLength.Value = 0;
                autoHeightCheckBox.Checked = false;
                regionHeight.Value = 0;
            } else
            {
                int lastIndex = selectedRegions[selectedRegions.Count - 1];
                if (editorPosIsCenter) {
                    topLeftXPos.Text = "" + ((decimal)goopCutRegions[lastIndex].startPos.X + (decimal)(goopCutRegions[lastIndex].width / 2f));
                    topLeftZPos.Text = "" + ((decimal)goopCutRegions[lastIndex].startPos.Z + (decimal)(goopCutRegions[lastIndex].length / 2f));
                }
                else {
                    topLeftXPos.Text = "" + (decimal)goopCutRegions[lastIndex].startPos.X;
                    topLeftZPos.Text = "" + (decimal)goopCutRegions[lastIndex].startPos.Z;
                }
                regionWidth.Value = (decimal)goopCutRegions[lastIndex].width;
                regionLength.Value = (decimal)goopCutRegions[lastIndex].length;
                if(lockRegionSizeToPowers) {
                    regionWidth.Increment = (decimal)Math.Abs(goopCutRegions[lastIndex].width);
                    regionLength.Increment = (decimal)Math.Abs(goopCutRegions[lastIndex].length);
                }
                autoHeightCheckBox.Checked = goopCutRegions[lastIndex].autoHeight;
                regionHeight.Value = (decimal)goopCutRegions[lastIndex].height;
                pixelHeight.Value = goopCutRegions[lastIndex].texWorldScale;
                topLeftXPos.Enabled = true;
                topLeftZPos.Enabled = true;
                regionWidth.Enabled = true;
                regionLength.Enabled = true;
                autoHeightCheckBox.Enabled = true;
                regionHeight.Enabled = !autoHeightCheckBox.Checked;
            }
            refreshingRegionInfoDisplay = false;
        }

        public void SyncListBoxToRegions()
        {
            goopSelectionByCode = true;
            goopRegionListBox.Items.Clear();
            for (int i = 0; i < goopCutRegions.Count; i++)
            {
                goopRegionListBox.Items.Add("Goop Region (" + goopCutRegions[i].layerType.ToString() + ")");
            }
            goopRegionListBox.EndUpdate(); // Allow listbox to update
            UpdateListBoxByCode();
        }
        
        public Vector3 ScreenToWorld(int screenX, int screenY)
        {
            float xPos = (2.0f * screenX) / (float)glControl1.Width - 1f;
            float yPos = (2f * screenY / (float)glControl1.Height) - 1f;


            Matrix4 viewProjectionInverse = Matrix4.Invert(projectionMatrix *
             cameraMatrix);

            Vector4 point = new Vector4(xPos, 0, yPos, 0);

            Vector4 transformedPoint = point * viewProjectionInverse;

            return new Vector3(transformedPoint.X, transformedPoint.Z, transformedPoint.Y) + cameraPosition/matrixZoomAmount;
        }

        public Vector3 ScreenToWorldNormal(int screenX, int screenY)
        {
            float xPos = 2f * screenX / (float)glControl1.Width - 1f;
            float yPos = -(2f * screenY / (float)glControl1.Height - 1);

            Matrix4 viewInverse = Matrix4.Invert(cameraMatrix);
            Matrix4 projInverse = Matrix4.Invert(projectionMatrix);

            Vector4 point = new Vector4(xPos, yPos, 1, 1);

            Vector4.Transform(ref point, ref projInverse, out point);
            Vector4.Transform(ref point, ref viewInverse, out point);

            if (point.W > float.Epsilon || point.W < -float.Epsilon)
            {
                point.X /= point.W;
                point.Y /= point.W;
                point.Z /= point.W;
            }

            return new Vector3(point.X, point.Y, point.Z);
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

        private bool ClickedWithinBounds(Vector3 point1, Vector3 point2, Vector3 clickPoint)
        {
            float greatestX = point1.X > point2.X ? point1.X : point2.X;
            float smallestX = point1.X > point2.X ? point2.X : point1.X;
            float greatestZ = point1.Z > point2.Z ? point1.Z : point2.Z;
            float smallestZ = point1.Z > point2.Z ? point2.Z : point1.Z;
            return (clickPoint.X <= greatestX && clickPoint.X >= smallestX && clickPoint.Z <= greatestZ && clickPoint.Z >= smallestZ);
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
                cameraRotation = new Vector3((float)Math.PI * 1.5f, (float)Math.PI * -0.5f + 0.00001f, 0f);
            } else
            {
                cameraPosition = new Vector3(0f, 1f, 0f);
                cameraRotation = new Vector3((float)Math.PI * 1.5f, (float)Math.PI * -0.5f + 0.01f, 0f);
            }

            UpdateViewport();
            UpdateCamera();
            glControl1.Refresh();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Z && e.Control && !e.Shift) // Undo
            {
                if(visualPollutionRegions.Count > 0) {
                    UndoPaint();
                } else {
                    UndoAction();
                }
            }

            if (e.KeyCode == Keys.Y && e.Control) // Redo
            {
                if(visualPollutionRegions.Count > 0) {
                    RedoPaint();
                } else {
                    RedoAction();
                }
            }

            if (e.KeyCode == Keys.Z && e.Control && e.Shift) // Redo
            {
                if(visualPollutionRegions.Count > 0) {
                    RedoPaint();
                } else {
                    RedoAction();
                }
            }

            if (e.KeyCode == Keys.Delete) // Delete regions
            {
                if(selectedRegions.Count > 0)
                {
                    DeleteSelectedRegions();
                }
            }
        }

        #region ADD/DUPLICATE/DELETE REGIONS

        private void DeleteSelectedRegions()
        {
            List<GoopRegionBox> regionsToRemove = new List<GoopRegionBox>();
            foreach (int i in selectedRegions)
            {
                regionsToRemove.Add(goopCutRegions[i]);
            }
            foreach (GoopRegionBox removeRegion in regionsToRemove)
            {
                goopCutRegions.Remove(removeRegion);
            }
            selectedRegions.Clear();

            SaveAction();
            SyncListBoxToRegions();
            glControl1.Invalidate();
        }

        private void AddNewRegion()
        {
            GoopRegionBox newGoopBox = new GoopRegionBox();
            newGoopBox.width = 8192;
            newGoopBox.length = newGoopBox.width;
            Vector3 newStartPos = Vector3.Zero;
            switch(newRegionLayerType)
            {
                case PollutionLayerType.Normal:
                case PollutionLayerType.NormalCopy:
                case PollutionLayerType.Wave:
                    newStartPos = new Vector3(-4096f, 0, -4096f);
                    break;
            }
            newGoopBox.startPos = newStartPos;
            newGoopBox.autoHeight = true;
            //newGoopBox.startPos = new Vector3(camPos.X+ 8192, 10000, camPos.Z+ 8192);
            newGoopBox.layerType = newRegionLayerType;
            goopCutRegions.Add(newGoopBox);
            UpdateRegionBoxHeights();
            selectedRegions.Clear();
            selectedRegions.Add(goopCutRegions.Count - 1);
            SaveAction();

            SyncListBoxToRegions();

            glControl1.Invalidate();
        }

        // TODO: Add later as polish
        private void DulplicateSelectedRegions()
        {

        }

        #endregion

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
                Console.WriteLine("UNDID STARTPOS: " + stateToUndoTo.currentRegions[0].startPos);
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
            undoStack.Push(newState);

            hasSaved = false;
            UpdateWindowTitle();
        }

        #endregion

        #region UI Region Viewer Callbacks

        /// <summary>
        /// Sets what the newly added region type will be when created based on the selected index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void regionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            newRegionLayerType = (PollutionLayerType)regionTypeComboBox.SelectedIndex;
        }

        /// <summary>
        /// Adds new region to the map that will be used to cut out a goop section
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void addRegionButton_Click(object sender, EventArgs e)
        {
            AddNewRegion();
        }

        private void deleteRegionButton_Click(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0)
            {
                DeleteSelectedRegions();
            }
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
            if (goopSelectionByCode)
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
            UpdateRegionInfoArea();
            glControl1.Invalidate();
        }

        #endregion

        #region UI Region Info Callbacks

        private void topLeftXPos_ValueChanged(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0 && !refreshingRegionInfoDisplay)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    if(editorPosIsCenter)
                    {
                        goopCutRegions[selectedRegions[i]].startPos.X = (float)topLeftXPos.Value - (goopCutRegions[selectedRegions[i]].width/2f);
                    } else {
                        goopCutRegions[selectedRegions[i]].startPos.X = (float)topLeftXPos.Value;
                    }
                    
                }
                glControl1.Refresh();
            }
        }

        private void topLeftXPos_Validated(object sender, EventArgs e)
        {
            if(selectedRegions.Count > 0)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    if(editorPosIsCenter)
                    {
                        goopCutRegions[selectedRegions[i]].startPos.X = (float)topLeftXPos.Value - (goopCutRegions[selectedRegions[i]].width/2f);
                    } else {
                        goopCutRegions[selectedRegions[i]].startPos.X = (float)topLeftXPos.Value;
                    }
                }
                SaveAction();
                glControl1.Refresh();
            }
        }

        private void topLeftZPos_ValueChanged(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0 && !refreshingRegionInfoDisplay)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    if(editorPosIsCenter)
                    {
                        goopCutRegions[selectedRegions[i]].startPos.Z = (float)topLeftZPos.Value - (goopCutRegions[selectedRegions[i]].length / 2f);
                    } else {
                        goopCutRegions[selectedRegions[i]].startPos.Z = (float)topLeftZPos.Value;
                    }
                    
                }
                glControl1.Refresh();
            }
        }

        private void topLeftZPos_Validated(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    if(editorPosIsCenter)
                    {
                        goopCutRegions[selectedRegions[i]].startPos.Z = (float)topLeftZPos.Value - (goopCutRegions[selectedRegions[i]].length / 2f);
                    } else {
                        goopCutRegions[selectedRegions[i]].startPos.Z = (float)topLeftZPos.Value;
                    }
                }
                SaveAction();
                glControl1.Refresh();
            }
        }
        // Visually Updates
        private void regionWidth_ValueChanged(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0 && !refreshingRegionInfoDisplay)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    if (lockRegionSizeToPowers) {
                        regionWidth.Value = (decimal)RoundToNearestPower((float)regionWidth.Value);
                    }
                    if(clampRegionSize) { regionWidth.Value = Clamp(regionMinSize, regionMaxSize, regionWidth.Value); }
                    goopCutRegions[selectedRegions[i]].width = (float)regionWidth.Value;
                }
                glControl1.Refresh();
            }
        }
        // Saves the update (and updates just in case)
        private void regionWidth_Validated(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    if(lockRegionSizeToPowers) {
                        regionWidth.Value = (decimal)RoundToNearestPower((float)regionWidth.Value);
                    }
                    if(clampRegionSize) { regionWidth.Value = Clamp(regionMinSize, regionMaxSize, regionWidth.Value); }
                    goopCutRegions[selectedRegions[i]].width = (float)regionWidth.Value;
                }
                SaveAction();
                glControl1.Refresh();
            }
        }
        // Visually Updates
        private void regionLength_ValueChanged(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0 && !refreshingRegionInfoDisplay)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    if(lockRegionSizeToPowers) {
                        regionLength.Value = (decimal)RoundToNearestPower((float)regionLength.Value);
                    }
                    if(clampRegionSize) { regionLength.Value = Clamp(regionMinSize, regionMaxSize, regionLength.Value); }
                    goopCutRegions[selectedRegions[i]].length = (float)regionLength.Value;
                }
                glControl1.Refresh();
            }
        }
        // Saves the update (and updates just in case)
        private void regionLength_Validated(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    if(lockRegionSizeToPowers) {
                        regionLength.Value = (decimal)RoundToNearestPower((float)regionLength.Value);
                    }
                    if(clampRegionSize) { regionLength.Value = Clamp(regionMinSize, regionMaxSize, regionLength.Value); }
                    goopCutRegions[selectedRegions[i]].length = (float)regionLength.Value;
                }
                SaveAction();
                glControl1.Refresh();
            }
        }
        // Visually Updates
        private void regionHeight_ValueChanged(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0 && !refreshingRegionInfoDisplay)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    goopCutRegions[selectedRegions[i]].height = (float)regionHeight.Value;
                }
                glControl1.Refresh();
            }
        }
        // Saves the update (and updates just in case)
        private void regionHeight_Validated(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    goopCutRegions[selectedRegions[i]].height = (float)regionHeight.Value;
                }
                SaveAction();
                glControl1.Refresh();
            }
        }

        private void pixelHeight_ValueChanged(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0 && !refreshingRegionInfoDisplay)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    goopCutRegions[selectedRegions[i]].texWorldScale = (int)pixelHeight.Value;
                }
                glControl1.Refresh();
            }
        }

        private void pixelHeight_Validated(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    goopCutRegions[selectedRegions[i]].texWorldScale = (int)pixelHeight.Value;
                }
                SaveAction();
                glControl1.Refresh();
            }
        }

        private void autoHeightCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedRegions.Count > 0 && !refreshingRegionInfoDisplay)
            {
                for (int i = 0; i < selectedRegions.Count; i++)
                {
                    goopCutRegions[selectedRegions[i]].autoHeight = autoHeightCheckBox.Checked;
                }
                UpdateRegionBoxHeights();
                SaveAction();
                glControl1.Refresh();
            }
            
        }

        // Makes the enter key not make a noise when pressed and validates the matching control

        private void topLeftXPos_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                topLeftXPos.Validate();
            }
        }

        private void topLeftZPos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                topLeftZPos.Validate();
            }
        }

        private void regionWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                regionWidth.Validate();
            }
        }

        private void regionLength_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                regionLength.Validate();
            }
        }

        private void regionHeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                regionHeight.Validate();
            }
        }

        private void pixelHeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                pixelHeight.Validate();
            }
        }

        #endregion

        public class ArrowlessNumericUpDown : NumericUpDown
        {
            public ArrowlessNumericUpDown()
            {
                Controls[0].Hide();
            }

            protected override void OnTextBoxResize(object source, EventArgs e)
            {
                Controls[1].Width = Width - 4;
            }
        }

        private Pen bmpPen;
        // Number of points around pen to check for painting, allowing us to paint seamlessly across edges/corners of goopmaps
        private const int outerPointsToCheck = 8;

        public class VisualPollutionRegion
        {
            public Col pollutionModel;
            public PollutionRegion pollutionRegion;
            /// <summary>
            /// Image that stores the initial pollution that the goop starts with ingame
            /// </summary>
            public Bitmap pollutionBmp;
            private Graphics pollutionGraphics;
            /// <summary>
            /// String name for the folder we're using for visuals of this goop
            /// (string so we can load .goop files and still use the same pollution instead of an index which could change if adding more custom goops)
            /// </summary>
            public string visualType = GoopResources.GetDefaultGoop();
            public Bitmap visualBitmap = Properties.Resources.defaultGoopTexture;

            public List<Bitmap> paintUndoList = new List<Bitmap>();
            public List<Bitmap> paintRedoList = new List<Bitmap>();

            /// <summary>
            /// Saves the collision model, creates the PollutionRegion data using the regionBox, and offsets the model visually
            /// </summary>
            /// <param name="regionBox"></param>
            /// <param name="model"></param>
            public VisualPollutionRegion(GoopRegionBox regionBox, Col model)
            {
                pollutionModel = model;
                // Convert the data to a PollutionRegion
                pollutionRegion = new PollutionRegion(regionBox);
                /*if(regionBox.autoHeight) { // Automatically set the height of the region based on the lowest vert if enabled
                    float lowestPoint = pollutionModel.ReturnLowestVertHeight();
                    pollutionRegion.pointYPos = lowestPoint != float.MaxValue ? lowestPoint : 0;
                }*/
                // Offset the model height at the end so the goop affects the ground but the model doesn't overlap
                pollutionModel.OffsetAllVerts(new Vector3(0, 7, 0));
                // Set the default goop texture to be our default goop
                ChangeVisual(visualType);
            }

            public void CreateRegionHeightmap(string path)
            {
                System.Drawing.Imaging.PixelFormat format8bppIndexed = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                // This would be accurate to the vanilia game, aka model width/32 (8192x8192 model would become a 256x256 texture
                float pixelScale = pollutionRegion.texWorldScale;
                Bitmap heightMap = new Bitmap(pollutionRegion.heightMapWidth, pollutionRegion.heightMapLength, format8bppIndexed);

                // Lock the image data
                BitmapData heightMapData = heightMap.LockBits(new Rectangle(0, 0, heightMap.Width, heightMap.Height), ImageLockMode.ReadWrite, format8bppIndexed);
                // Get the address of the first line.
                IntPtr ptr = heightMapData.Scan0;
                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(heightMapData.Stride) * heightMap.Height;
                byte[] rgbValues = new byte[bytes];

                //Create grayscale color table
                ColorPalette palette = heightMap.Palette;
                for (int i = 0; i < 256; i++)
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                heightMap.Palette = palette;

                // Set the colors for each pixel
                for (int x = 0; x < pollutionRegion.heightMapWidth; x++)
                {
                    for(int y = 0; y < pollutionRegion.heightMapLength; y++)
                    {
                        Vector3 intersectionPoint = new Vector3(pollutionRegion.startXPos + x * pixelScale, pollutionModel.ReturnHighestVertHeight() + 100, pollutionRegion.startZPos + y * pixelScale);
                        float? topHeight = pollutionModel.GetHighestIntersectionPosition(intersectionPoint);
                        int colorVal = 255; // White
                        if(topHeight != null && topHeight >= pollutionRegion.pointYPos) { // Set the pixel color darker based on height
                            colorVal = (int)((topHeight - pollutionRegion.pointYPos) / 40);
                            if(colorVal > 255) { colorVal = 255; }
                        }
                        int position = (y * heightMapData.Stride) + (x * System.Drawing.Image.GetPixelFormatSize(heightMapData.PixelFormat) / 8);
                        rgbValues[position] = (byte)colorVal;
                    }
                }
                // Copy the colors to the data
                Marshal.Copy(rgbValues, 0, ptr, bytes);
                // Unlock the bits.
                heightMap.UnlockBits(heightMapData);


                pollutionRegion.heightMap = heightMap;
                //heightMap.Save("C:\\Users\\alexh\\Downloads\\test.png", System.Drawing.Imaging.ImageFormat.Png);

                // Use the heightmap to create an outline of the pollution and use it as the temp pollution map
                Bitmap newPollutionBmp = new Bitmap(pollutionRegion.heightMapWidth, pollutionRegion.heightMapLength, format8bppIndexed);
                newPollutionBmp.Palette = palette;

                // Lock the image data
                BitmapData pollutionBmpData = newPollutionBmp.LockBits(new Rectangle(0, 0, newPollutionBmp.Width, newPollutionBmp.Height), ImageLockMode.ReadWrite, format8bppIndexed);
                // Get the address of the first line.
                IntPtr polPtr = pollutionBmpData.Scan0;
                // Convert nonwhite pixel colors to black
                for (int i = 0; i < rgbValues.Length; i++) {
                    rgbValues[i] = (byte)255;
                }
                // Copy the colors to the data
                Marshal.Copy(rgbValues, 0, polPtr, bytes);
                // Unlock the bits.
                newPollutionBmp.UnlockBits(pollutionBmpData);

                pollutionBmp = new Bitmap(newPollutionBmp);

                SetupPollutionGraphics();
            }

            public void SetupPollutionGraphics()
            {
                pollutionGraphics = Graphics.FromImage(pollutionBmp);
            }

            public void CreateRegionHeightmapParallel(string path)
            {
                System.Drawing.Imaging.PixelFormat format8bppIndexed = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                // This would be accurate to the vanilia game, aka model width/32 (8192x8192 model would become a 256x256 texture)
                float pixelScale = pollutionRegion.texWorldScale;
                Bitmap heightMap = new Bitmap(pollutionRegion.heightMapWidth, pollutionRegion.heightMapLength, format8bppIndexed);

                // Lock the image data
                BitmapData heightMapData = heightMap.LockBits(new Rectangle(0, 0, heightMap.Width, heightMap.Height), ImageLockMode.ReadWrite, format8bppIndexed);
                // Get the address of the first line.
                IntPtr ptr = heightMapData.Scan0;
                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(heightMapData.Stride) * heightMap.Height;
                byte[] rgbValues = new byte[bytes];

                //Create grayscale color table
                ColorPalette palette = heightMap.Palette;
                for (int i = 0; i < 256; i++)
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                heightMap.Palette = palette;

                // Set the colors for each pixel
                Parallel.For(0, pollutionRegion.heightMapWidth, x =>
                {
                    for (int y = 0; y < pollutionRegion.heightMapLength; y++)
                    {
                        Vector3 intersectionPoint = new Vector3(pollutionRegion.startXPos + x * pixelScale, pollutionModel.ReturnHighestVertHeight() + 100, pollutionRegion.startZPos + y * pixelScale);
                        float? topHeight = pollutionModel.GetHighestIntersectionPosition(intersectionPoint);
                        int colorVal = 255; // White
                        if (topHeight != null && topHeight >= pollutionRegion.pointYPos)
                        { // Set the pixel color darker based on height
                            colorVal = (int)((topHeight - pollutionRegion.pointYPos) / 40);
                            if (colorVal > 255) { colorVal = 255; }
                        }
                        int position = (y * heightMapData.Stride) + (x * System.Drawing.Image.GetPixelFormatSize(heightMapData.PixelFormat) / 8);
                        rgbValues[position] = (byte)colorVal;
                    }
                });
                // Copy the colors to the data
                Marshal.Copy(rgbValues, 0, ptr, bytes);
                // Unlock the bits.
                heightMap.UnlockBits(heightMapData);

                // Set resolution (idk, just matching sunshines stuff)
                float resolution = (heightMap.Width > 256 || heightMap.Height > 256) ? 96.012f : 71.9836f;
                heightMap.SetResolution(resolution, resolution);
                // Set image flag???

                pollutionRegion.heightMap = heightMap;
                //heightMap.Save("C:\\Users\\alexh\\Downloads\\test.png", System.Drawing.Imaging.ImageFormat.Png);

                // Use the heightmap to create an outline of the pollution and use it as the temp pollution map
                Bitmap newPollutionBmp = new Bitmap(pollutionRegion.heightMapWidth, pollutionRegion.heightMapLength, format8bppIndexed);
                newPollutionBmp.Palette = palette;

                // Lock the image data
                BitmapData pollutionBmpData = newPollutionBmp.LockBits(new Rectangle(0, 0, newPollutionBmp.Width, newPollutionBmp.Height), ImageLockMode.ReadWrite, format8bppIndexed);
                // Get the address of the first line.
                IntPtr polPtr = pollutionBmpData.Scan0;
                // Convert nonwhite pixel colors to black
                Parallel.For(0, rgbValues.Length, i =>
                {
                    rgbValues[i] = (byte)255;
                });
                // Copy the colors to the data
                Marshal.Copy(rgbValues, 0, polPtr, bytes);
                // Unlock the bits.
                newPollutionBmp.UnlockBits(pollutionBmpData);

                pollutionBmp = new Bitmap(newPollutionBmp);

                SetupPollutionGraphics();
            }

            public Bitmap GetFormattedBmp()
            {
                Bitmap formatedBmp = new Bitmap(pollutionBmp.Width, pollutionBmp.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                // Locks the bitmap data into memory
                BitmapData data = formatedBmp.LockBits(new Rectangle(0, 0, pollutionBmp.Width, pollutionBmp.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                // Gets the data for colors
                byte[] bytes = new byte[data.Height * data.Stride];
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                // Change the data to match the colors from the pollution bmp
                for (int x = 0; x < pollutionBmp.Width; x++)
                {
                    for (int y = 0; y < pollutionBmp.Height; y++)
                    {
                        Color colorBefore = pollutionBmp.GetPixel(x, y);
                        bytes[y * data.Stride + x] = colorBefore.R;
                    }
                }
                // Copy the bytes back into the image
                Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
                // Unlock the bmp and return
                formatedBmp.UnlockBits(data);
                return formatedBmp;
            }

            public void PaintOnBmp(Pen pen, Vector2 startUV, Vector2 endUv)
            {
                float startXPos = (pollutionGraphics.VisibleClipBounds.Width * startUV.X);
                float startYPos = (pollutionGraphics.VisibleClipBounds.Height * startUV.Y);
                float endXPos = (pollutionGraphics.VisibleClipBounds.Width * endUv.X);
                float endYPos = (pollutionGraphics.VisibleClipBounds.Height * endUv.Y);
                if (startUV == endUv) { // Draw dot if uv points the same
                    pollutionGraphics.FillEllipse(pen.Brush, startXPos - pen.Width / 2, startYPos - pen.Width / 2, pen.Width, pen.Width);
                }
                else { // Draw a line otherwise
                    pollutionGraphics.DrawLine(pen, startXPos, startYPos, endXPos, endYPos);
                }
            }

            // TODO: Add floodfill somehow (might be able to edit bmp and it auto updates graphics?)
            public void FloodFillOnBmp(Vector2 uvPoint)
            {
                // copy bmp, do floodfill algorithm on it, overwrite bmp, save paint (from wherever calls this)

                Bitmap filledBmp = (Bitmap)pollutionBmp.Clone();
                // Lock the image data
                BitmapData pollutionData = filledBmp.LockBits(new Rectangle(0, 0, filledBmp.Width, filledBmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                // Get the address of the first line.
                IntPtr ptr = pollutionData.Scan0;
                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(pollutionData.Stride) * pollutionData.Height;
                int pixelFormatSize = System.Drawing.Image.GetPixelFormatSize(System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                byte[] rgbValues = new byte[bytes];

                // Flood fill algorithm
                
                // Copy the colors to the data
                Marshal.Copy(ptr, rgbValues, 0, bytes);
                // Unlock the bits.
                filledBmp.UnlockBits(pollutionData);

                // Overwrite the pollution bmp with the filled one
                OverwriteBmp(filledBmp);
            }

            public void OverwriteBmp(Bitmap newImage)
            {
                pollutionGraphics.DrawImage(newImage, new Rectangle(0, 0, pollutionBmp.Width, pollutionBmp.Height));
            }

            public void SaveBmp(string path)
            {
                Bitmap unformattedBitmap = GetBitmap();
                Bitmap formatedBitmap = new Bitmap(unformattedBitmap.Width, unformattedBitmap.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                
                formatedBitmap.Save(path);
            }

            public Bitmap GetBitmap()
            {
                Bitmap gottenBitmap = new Bitmap(pollutionBmp.Width, pollutionBmp.Height);
                using (Graphics gr = Graphics.FromImage(gottenBitmap)) {
                    gr.DrawImage(pollutionBmp, new Rectangle(0, 0, gottenBitmap.Width, gottenBitmap.Height));
                }
                return gottenBitmap;
            }

            public void ChangeVisual(string newVisualType)
            {
                visualType = newVisualType;
                // Try to load an image, otherwise use the default goop texture
                visualBitmap = GoopResources.GetGoopVisual(visualType);
            }
        }
        public List<VisualPollutionRegion> visualPollutionRegions = new List<VisualPollutionRegion>();


        private List<List<Bitmap>> paintUndoList = new List<List<Bitmap>>();
        private List<List<Bitmap>> paintRedoList = new List<List<Bitmap>>();
        public void SavePaint(bool forceAll = false)
        {
            List<Bitmap> savedImages = new List<Bitmap>();
            for (int i = 0; i < visualPollutionRegions.Count; i++)
            {
                if(forceAll || paintedOnVisuals[i]) {
                    savedImages.Add(visualPollutionRegions[i].GetBitmap());
                } else {
                    savedImages.Add(null);
                }
            }
            paintUndoList.Add(savedImages);
            if (paintUndoList.Count > Properties.Settings.Default.paintUndoSize + 1) {
                UndoPaintRemoveAt(0);
            }
            RedoPaintClear();

            hasSaved = false;
            UpdateWindowTitle();
        }

        public void UndoPaint()
        {
            if (paintUndoList.Count > 1)
            {
                List<Bitmap> undoImages = paintUndoList[paintUndoList.Count - 1];
                UndoPaintRemoveAt(paintUndoList.Count - 1);
                paintRedoList.Add(undoImages);
                for (int i = 0; i < visualPollutionRegions.Count; i++)
                {
                    if(undoImages[i] == null) { continue; } // Only revert images that had their state changed
                    int closestUndoIndex = paintUndoList.Count - 1;
                    while(closestUndoIndex > 0) {
                        if(paintUndoList[closestUndoIndex][i] == null) {
                            closestUndoIndex--;
                        } else {
                            break;
                        }
                    }
                    Bitmap undoState = paintUndoList[closestUndoIndex][i];
                    visualPollutionRegions[i].OverwriteBmp(undoState);
                }
                UpdateVisualRegionInfoArea();
                glControl1.Refresh();
            }
        }

        public void RedoPaint()
        {
            if (paintRedoList.Count > 0)
            {
                List<Bitmap> redoImages = paintRedoList[paintRedoList.Count - 1];
                RedoPaintRemoveAt(paintRedoList.Count - 1);
                paintUndoList.Add(redoImages);
                for (int i = 0; i < visualPollutionRegions.Count; i++)
                {
                    Bitmap redoState = redoImages[i];
                    if(redoState == null) {
                        continue;
                    }
                    visualPollutionRegions[i].OverwriteBmp(redoState);
                }
                UpdateVisualRegionInfoArea();
                glControl1.Refresh();
            }
        }

        public void UndoPaintRemoveAt(int index)
        {
            List<Bitmap> undoEntry = paintUndoList[index];
            foreach(Bitmap undoBitmap in undoEntry) {
                undoBitmap?.Dispose();
            }
            paintUndoList.RemoveAt(index);
        }

        public void RedoPaintRemoveAt(int index)
        {
            List<Bitmap> redoEntry = paintRedoList[index];
            foreach (Bitmap redoBitmap in redoEntry)
            {
                redoBitmap?.Dispose();
            }
            paintRedoList.RemoveAt(index);
        }

        public void RedoPaintClear()
        {
            foreach(List<Bitmap> redoEntry in paintRedoList) {
                foreach (Bitmap redoBitmap in redoEntry) {
                    redoBitmap?.Dispose();
                }
            }
            paintRedoList.Clear();
        }

        public void UndoPaintClear()
        {
            foreach (List<Bitmap> undoEntry in paintUndoList)
            {
                foreach (Bitmap undoBitmap in undoEntry)
                {
                    undoBitmap?.Dispose();
                }
            }
            paintUndoList.Clear();
        }

        private bool ignoreTabChange = true;
        private async void cutRegionsButton_Click(object sender, EventArgs e)
        {
            

            if (goopCutRegions.Count > 0)
            {

                for (int i = 0; i < goopCutRegions.Count; i++) {
                    // Return if we have an unsupported layer type
                    if (!supportedLayerTypes.Contains(goopCutRegions[i].layerType)) {
                        MessageBox.Show("The pollution layer type \"" + goopCutRegions[i].layerType.ToString() + "\" on region (" + i + ") is not supported yet! Tell Alex to get his butt in gear and work on it!", "Cutting Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    // Return if we have a region that would have a texture bigger than 1024
                    if(goopCutRegions[i].height / goopCutRegions[i].texWorldScale > 1024 || goopCutRegions[i].width / goopCutRegions[i].texWorldScale > 1024) {
                        MessageBox.Show("The bmp texture generated for region (" + i + ") would be larger than 1024, please resize the region or raise the world pixel scale", "Cutting Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                cutProgressBar.Visible = true;
                cuttingProgress = 0;
                cutProgressBar.Value = (int)cuttingProgress;
                // Start the cutting model thread
                await CutTask();
                CuttingThreadDone();
            } else {
                MessageBox.Show("You don't have any goop regions, please create some first!", "Cutting Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        Task CutTask()
        {
            return Task.Run(() => { CutRegionsThread(); }); 
        }

        public void CutRegionsThread()
        {
            float progressRegionSize = 100f / goopCutRegions.Count;
            List<VisualPollutionRegion> storedRegions = new List<VisualPollutionRegion>();
            Console.WriteLine("Starting Cutting Process");
            long prevMiliseconds = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < goopCutRegions.Count; i++)
            {
                GoopRegionBox regionBox = goopCutRegions[i];
                // Generates the model
                Col regionModel = mapCol.SplitModelByLine(regionBox.GetCornerPos(Corner.TopLeft), regionBox.GetCornerPos(Corner.TopRight));
                AddToProgress(progressRegionSize * 0.2f);
                regionModel = regionModel.SplitModelByLine(regionBox.GetCornerPos(Corner.TopRight), regionBox.GetCornerPos(Corner.BottomRight));
                AddToProgress(progressRegionSize * 0.175f);
                regionModel = regionModel.SplitModelByLine(regionBox.GetCornerPos(Corner.BottomRight), regionBox.GetCornerPos(Corner.BottomLeft));
                AddToProgress(progressRegionSize * 0.175f);
                regionModel = regionModel.SplitModelByLine(regionBox.GetCornerPos(Corner.BottomLeft), regionBox.GetCornerPos(Corner.TopLeft));
                AddToProgress(progressRegionSize * 0.15f);
                regionModel.CleanupModelForGoop(Properties.Settings.Default.colTypesToRemove, regionBox.height);
                regionModel.SetUVsByProjection(regionBox.GetCornerPos(Corner.BottomLeft), regionBox.width, regionBox.length);
                Console.WriteLine("Time for region {1} cut in milliseconds: {0}", stopWatch.ElapsedMilliseconds - prevMiliseconds, i);
                prevMiliseconds = stopWatch.ElapsedMilliseconds;
                // Creates the visual region
                VisualPollutionRegion newVisualRegion = new VisualPollutionRegion(regionBox, regionModel);
                newVisualRegion.CreateRegionHeightmapParallel("generatedHeightmap" + i + ".png");
                storedRegions.Add(newVisualRegion);
                Console.WriteLine("Time for region {1} image in milliseconds: {0}", stopWatch.ElapsedMilliseconds - prevMiliseconds, i);
                prevMiliseconds = stopWatch.ElapsedMilliseconds;
                AddToProgress(progressRegionSize * 0.3f);
            }
            stopWatch.Stop();
            Console.WriteLine("Cutting time in milliseconds: {0}", stopWatch.ElapsedMilliseconds);
            // Cut all models so change the editor to the next state
            visualPollutionRegions = storedRegions;
            
        }

        public void CuttingThreadDone()
        {
            // Creates a pen for drawing on the bmp image
            bmpPen = new Pen(Color.White, 10);
            bmpPen.StartCap = bmpPen.EndCap = System.Drawing.Drawing2D.LineCap.Round; // Rounds the pen out

            cutProgressBar.Visible = false;
            UpdateVisualComboBox();
            paintingPanel.Visible = true;
            brushSizeTextBox.Text = "" + bmpPen.Width;
            paintedOnVisuals = new bool[goopCutRegions.Count];
            SavePaint(true);

            // Setup the info for the settings editor
            ignoreTabChange = false;
            tabControl1.SelectedIndex = 1;
            selectedRegions.Clear();
            UpdateVisualRegionListBox();
            UpdateVisualRegionInfoArea();

            glControl1.Refresh();
        }

        delegate void Func();
        public float cuttingProgress = 0;
        private void AddToProgress(float addAmt)
        {
            cuttingProgress += addAmt;
            if(cuttingProgress > 100) { cuttingProgress = 100; }
            Func del = delegate
            {
                cutProgressBar.Value = (int)Math.Ceiling(cuttingProgress);
                cutProgressBar.Refresh();
            };
            Invoke(del);
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if(ignoreTabChange) {
                e.Cancel = true;
            } else {
                if(e.TabPageIndex == 0)
                {
                    DialogResult confirmation = MessageBox.Show("Move back to the previous step? All of the setting on this step will be lost.",
                        "Return Confirmation", MessageBoxButtons.YesNo);
                    if(confirmation == DialogResult.Yes)
                    {
                        visualPollutionRegions.Clear();
                        selectedRegions.Clear();
                        RedoPaintClear();
                        UndoPaintClear();
                        glControl1.Refresh();
                        ignoreTabChange = true;
                        paintingPanel.Visible = false;
                    }
                }
                //ignoreTabChange = !ignoreTabChange;
            }
            
        }

        #region UI Global Goop Settings

        // Only allow numbers in the pollution number combo box
        private void pollutionTypeComboBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) {
                e.Handled = true;
            }
        }

        private void pollutionTypeComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            int inputIndex;
            if (e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                if(int.TryParse(pollutionTypeComboBox.Text, out inputIndex)) {
                    if(pollutionTypeComboBox.Items.Count > inputIndex) {
                        pollutionTypeComboBox.SelectedIndex = inputIndex;
                    }
                }
                pollutionTypeComboBox.Enabled = false;
                pollutionTypeComboBox.Enabled = true;
            }

            if(e.KeyCode == Keys.Back && !int.TryParse(pollutionTypeComboBox.Text, out inputIndex))
            {
                pollutionTypeComboBox.Text = "";
            }
        }




        #endregion
        /*private string btiFilePath;
        private Bitmap btiBitmap = null;
        private void textureBox_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Gamecube Image Format (*.bti)|*.bti";
            fileDialog.InitialDirectory = Properties.Settings.Default.goopTextureDialogRestore;
            if (fileDialog.ShowDialog() == DialogResult.OK) // If the Ok button is hit
            {
                // Save the directory for next time
                Properties.Settings.Default.goopTextureDialogRestore = Path.GetDirectoryName(fileDialog.FileName);
                Properties.Settings.Default.Save();
                // Open the texture, store it, use it for the goop, and replace the preview
                btiBitmap = Bti.ReadBtiToBitmap(fileDialog.OpenFile());
                btiFilePath = fileDialog.FileName;
                textureBox.Image = btiBitmap;
                glControl1.Refresh();
            }
        }*/

        private void githubRepoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Halleester/Goopify");
        }

        private bool HasNeededResources()
        {
            return false;
        }

        private void SetupNeededResources()
        {
            CommonOpenFileDialog folderDialogue = new CommonOpenFileDialog();
            folderDialogue.IsFolderPicker = true;
            folderDialogue.Title = "Goop Refrence Assets";
            folderDialogue.InitialDirectory = Properties.Settings.Default.resourceDialogueRestore;
            if (folderDialogue.ShowDialog() == CommonFileDialogResult.Ok) // If the Ok button is hit
            {
                // Save the directory for next time
                Properties.Settings.Default.resourceDialogueRestore = folderDialogue.FileName;
                Properties.Settings.Default.Save();
                //Get the path of specified file
                string savePath = folderDialogue.FileName;
            }
            this.TopMost = true;
        }

        public void UpdateVisualRegionListBox()
        {
            visualRegionsListBox.Items.Clear();
            for (int i = 0; i < visualPollutionRegions.Count; i++)
            {
                visualRegionsListBox.Items.Add("Goop Region (" + goopCutRegions[i].layerType.ToString() + ")");
            }
            visualRegionsListBox.EndUpdate(); // Allow listbox to update
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog folderDialogue = new CommonOpenFileDialog();
            folderDialogue.IsFolderPicker = true;
            folderDialogue.Title = "Select the extracted iso \"scene\" folder";
            folderDialogue.InitialDirectory = Properties.Settings.Default.extractDialogueRestore;
            if (folderDialogue.ShowDialog() == CommonFileDialogResult.Ok) // If the Ok button is hit
            {
                // Check if map path is valid
                string mapPath = folderDialogue.FileName + "\\map";
                if (!Directory.Exists(mapPath)) {
                    MessageBox.Show("Path not valid!", "Path Error");
                    return;
                }
                Properties.Settings.Default.extractDialogueRestore = folderDialogue.FileName;
                Properties.Settings.Default.Save();

                string pollutionPath = mapPath + "\\pollution";
                string ymapPath = mapPath + "\\ymap.ymp";
                // Prompt if the pollution and ymap already exist
                if (Directory.Exists(pollutionPath) || Directory.Exists(ymapPath)) {
                    var confirmation = MessageBox.Show("Pollution folder already exists, Continue and replace it?", "Pollution Path Error", MessageBoxButtons.OKCancel);
                    if(confirmation == DialogResult.Cancel) { return; }
                    if (Directory.Exists(pollutionPath)) { (new DirectoryInfo(pollutionPath)).Delete(true); }
                    if (Directory.Exists(ymapPath)) { File.Delete(ymapPath); }
                }
                Directory.CreateDirectory(pollutionPath);
                // Create the ymap file
                List<PollutionRegion> pollutionRegions = new List<PollutionRegion>();
                for(int i = 0; i < visualPollutionRegions.Count; i++) {
                    pollutionRegions.Add(visualPollutionRegions[i].pollutionRegion);
                }
                PollutionMap pollutionMap = new PollutionMap(pollutionRegions);
                pollutionMap.CreateYmapFile(ymapPath);

                // Create the pollution folder
                
                // Gets the particles and bti of the first goop
                string goopForResources = visualPollutionRegions[0].visualType;
                // Particles
                string[] particlePaths = GoopResources.GetParticlePaths(goopForResources);
                foreach(string particle in particlePaths) {
                    string particleName = Path.GetFileName(particle);
                    File.Copy(particle, pollutionPath + "\\" + particleName);
                }
                // Global texture for goop (bti)
                string errorString = "";
                string[] btiPaths = GoopResources.GetBtiPaths(goopForResources);
                foreach (string btiPath in btiPaths) {
                    string btiName = Path.GetFileName(btiPath);
                    File.Copy(btiPath, pollutionPath + "\\" + btiName);
                }
                //Bmd, bmps, and btks
                for (int i = 0; i < visualPollutionRegions.Count; i++) {
                    string regionPath = pollutionPath + "\\pollution" + i.ToString("00");
                    string localPollutionType = visualPollutionRegions[i].visualType;
                    // Btk
                    string btkPath = GoopResources.GetBtkPath(localPollutionType);
                    if(File.Exists(btkPath)) {
                        File.Copy(btkPath, regionPath + ".btk");
                    }
                    // Bmp
                    Bitmap formattedBmp = visualPollutionRegions[i].GetFormattedBmp();
                    formattedBmp.Save(regionPath + ".bmp", ImageFormat.Bmp);
                    // Bmd
                    string resourcesPath = GoopResources.GetGlobalResourcesPath(localPollutionType);
                    if(resourcesPath != "") {
                        string outputString = visualPollutionRegions[i].pollutionModel.CreateBmdFromCol(regionPath + ".bmd", resourcesPath, visualPollutionRegions[i].pollutionBmp.Size);
                        if(outputString != "") { errorString += "Error with region " + (i+1) + ": " + outputString; }
                    }
                }
                if(errorString != "") { MessageBox.Show(errorString, "Export Error"); } else { MessageBox.Show("Goop successfully created!", "Export Success"); }
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        private void visualRegionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (goopSelectionByCode)
            {
                goopSelectionByCode = false;
                return;
            }
            selectedRegions.Clear();
            foreach (int selectionIndex in visualRegionsListBox.SelectedIndices)
            {
                selectedRegions.Add(selectionIndex);
                Console.WriteLine("List Selected: " + selectionIndex);
            }
            UpdateVisualRegionInfoArea();
            glControl1.Invalidate();
        }

        /// <summary>
        /// Updates the goop region list box based on our currently selected goop regions
        /// </summary>
        public void UpdateVisualListBoxByCode()
        {
            visualConfigComboBox.Enabled = true;
            textureScaleNumericUpDown.Enabled = true;
            textureRotationNumericUpDown.Enabled = true;
            uploadBmpButton.Enabled = true;
            downloadBmpButton.Enabled = true;
            pollutionTypeComboBox.Enabled = true;

            goopSelectionByCode = true;
            visualRegionsListBox.SelectedIndices.Clear();
            for (int i = 0; i < visualPollutionRegions.Count(); i++)
            {
                goopSelectionByCode = true;
                int listIndex = selectedRegions[i];
                goopRegionListBox.SetSelected(listIndex, true);
            }

            UpdateVisualRegionInfoArea();
        }

        private bool refreshingVisualRegionInfoDisplay = false;
        public void UpdateVisualRegionInfoArea()
        {
            refreshingVisualRegionInfoDisplay = true;
            if (selectedRegions.Count == 0)
            {
                visualConfigComboBox.Enabled = false;
                textureScaleNumericUpDown.Enabled = false;
                textureRotationNumericUpDown.Enabled = false;
                uploadBmpButton.Enabled = false;
                downloadBmpButton.Enabled = false;
                pollutionTypeComboBox.Enabled = false;
                visualConfigComboBox.SelectedIndex = -1;
                visualConfigComboBox.Text = "---";
                textureScaleNumericUpDown.Value = 0;
                textureRotationNumericUpDown.Value = 0;
                pollutionTypeComboBox.SelectedIndex = -1;
                pollutionTypeComboBox.Text = "---";
                regionBpmPictureBox.Image = Goopify.Properties.Resources.defaultGoopTexture;
            }
            else
            {
                int lastIndex = selectedRegions[selectedRegions.Count - 1];
                // Sets the goop visual box value
                string visualType = visualPollutionRegions[lastIndex].visualType;
                int visualIndex = -1;
                string[] visualOptions = visualConfigComboBox.Items.Cast<string>().ToArray();
                for (int i = 0; i < visualOptions.Length; i++) {
                    if (visualOptions[i].Equals(visualType)) {
                        visualIndex = i;
                        break;
                    }
                }
                visualConfigComboBox.SelectedIndex = visualIndex;
                // Update texture scale display
                textureScaleNumericUpDown.Value = (decimal)visualPollutionRegions[lastIndex].pollutionModel.goopUvScale;
                textureRotationNumericUpDown.Value = (decimal)visualPollutionRegions[lastIndex].pollutionModel.goopUvRotation;
                // Update pollution type display
                int pollutionType = visualPollutionRegions[lastIndex].pollutionRegion.pollutionType;
                if (pollutionTypeComboBox.Items.Count > pollutionType) {
                    pollutionTypeComboBox.SelectedIndex = pollutionType;
                    pollutionTypeComboBox.Refresh();
                } else {
                    pollutionTypeComboBox.Text = "" + pollutionType;
                }
                // Update bmp image
                regionBpmPictureBox.Image = visualPollutionRegions[lastIndex].pollutionBmp;
                // Enable the proper displays
                visualConfigComboBox.Enabled = true;
                textureScaleNumericUpDown.Enabled = true;
                textureRotationNumericUpDown.Enabled = true;
                pollutionTypeComboBox.Enabled = true;
                if (selectedRegions.Count == 1)
                { // Only enable the texture buttons if we have one region selected
                    uploadBmpButton.Enabled = true;
                    downloadBmpButton.Enabled = true;
                }
            }
            refreshingVisualRegionInfoDisplay = false;
        }

        public void SyncVisualListBoxToRegions()
        {
            goopSelectionByCode = true;
            goopRegionListBox.Items.Clear();
            for (int i = 0; i < goopCutRegions.Count; i++)
            {
                goopRegionListBox.Items.Add("Goop Region (" + goopCutRegions[i].layerType.ToString() + ")");
            }
            goopRegionListBox.EndUpdate(); // Allow listbox to update
            UpdateListBoxByCode();
        }

        #region VISUAL REGION DATA

        /// <summary>
        /// For changing the texture/effects that the goop will have
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void visualConfigComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the region visuals
            for (int i = 0; i < selectedRegions.Count; i++) {
                visualPollutionRegions[selectedRegions[i]].ChangeVisual(visualConfigComboBox.Text);
            }

            glControl1.Refresh();
        }

        private void UpdateVisualComboBox()
        {
            visualConfigComboBox.Items.Clear();
            if(Directory.Exists(Directory.GetCurrentDirectory() + GoopResources.resourcePath))
            {
                string[] files = Directory.GetDirectories(Directory.GetCurrentDirectory() + GoopResources.resourcePath);
                foreach(string file in files)
                {
                    string nameString = file.Replace(Directory.GetCurrentDirectory() + GoopResources.resourcePath + "\\", "");
                    visualConfigComboBox.Items.Add(nameString);
                }
            }
        }

        /// <summary>
        /// For changing the scale of the image uvs of visual regions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textureScaleNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < selectedRegions.Count; i++) {
                visualPollutionRegions[selectedRegions[i]].pollutionModel.ChangeGoopUVScale((float)textureScaleNumericUpDown.Value);
            }

            glControl1.Refresh();
        }

        /// <summary>
        /// For changing the rotation of the image uvs of visual regions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textureRotationNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < selectedRegions.Count; i++)
            {
                visualPollutionRegions[selectedRegions[i]].pollutionModel.ChangeGoopUVRotation((float)textureRotationNumericUpDown.Value);
            }

            glControl1.Refresh();
        }

        /// <summary>
        /// For changing the pollution type of visual regions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pollutionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the pollution type
            for (int i = 0; i < selectedRegions.Count; i++) {
                visualPollutionRegions[selectedRegions[i]].pollutionRegion.pollutionType = (ushort)pollutionTypeComboBox.SelectedIndex;
            }
        }

        #endregion

        private void uploadBmpButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Bmp Import";
            fileDialog.Filter = "Bmp Files (*.bmp)|*.bmp";
            fileDialog.InitialDirectory = Properties.Settings.Default.bmpImportPath;
            if (fileDialog.ShowDialog() == DialogResult.OK) // If the Ok button is hit
            {
                // Save the directory for next time
                Properties.Settings.Default.bmpImportPath = Path.GetDirectoryName(fileDialog.FileName);
                Properties.Settings.Default.Save();
                // Copy over the bmp if it's the same size
                Bitmap gottenBmp = new Bitmap(fileDialog.FileName);
                Bitmap regionBmp = visualPollutionRegions[selectedRegions[selectedRegions.Count - 1]].pollutionBmp;
                if(gottenBmp.Size != regionBmp.Size) {
                    MessageBox.Show(this, "Bmp is not the correct size! (Should be " + regionBmp.Size + ")");
                    return;
                }
                // Draw the graphics onto the pollution bmp
                visualPollutionRegions[selectedRegions[selectedRegions.Count - 1]].OverwriteBmp(gottenBmp);
                SavePaint(true);
                // Refresh info
                UpdateVisualRegionInfoArea();
                glControl1.Invalidate();
            }
        }

        private void downloadBmpButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Bmp Export";
            saveFileDialog.Filter = "Bmp Files (*.bmp)|*.bmp";
            //saveFileDialog.FileName = "bmpExport" + selectedRegions[selectedRegions.Count - 1];
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                visualPollutionRegions[selectedRegions[selectedRegions.Count - 1]].pollutionBmp.Save(saveFileDialog.FileName);
            }
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open the 
            PropertiesSubform editorWindow = new PropertiesSubform();
            editorWindow.Show();
        }

        private void brushSizeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                ActiveControl = null;
            }
        }

        private void brushSizeTextBox_Validating(object sender, CancelEventArgs e)
        {
            if(float.TryParse(brushSizeTextBox.Text, out float newSize)) {
                bmpPen.Width = newSize;
            } else {
                brushSizeTextBox.Text = "" + bmpPen.Width;
            }
        }

        private void paintRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if(paintRadioButton.Checked) { bmpPen.Color = Color.White; }
        }

        private void eraseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (eraseRadioButton.Checked) { bmpPen.Color = Color.Black; }
        }

        /// <summary>
        /// Rewrites all selected bitmaps to be blank (black)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearButton_Click(object sender, EventArgs e)
        {
            if(selectedRegions.Count == 0) { return; }
            foreach(int selectedRegion in selectedRegions)
            {
                Bitmap whiteImage = new Bitmap(visualPollutionRegions[selectedRegion].pollutionBmp);
                using (Graphics graph = Graphics.FromImage(whiteImage)) {
                    Rectangle ImageSize = new Rectangle(0, 0, whiteImage.Width, whiteImage.Height);
                    graph.FillRectangle(Brushes.Black, ImageSize);
                }
                visualPollutionRegions[selectedRegion].OverwriteBmp(whiteImage);
            }
            
            SavePaint(true);
            // Refresh info
            UpdateVisualRegionInfoArea();
            glControl1.Invalidate();
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            SaveGoopMap(savePath);
        }

        private void saveAsMenuItem_Click(object sender, EventArgs e)
        {
            SaveGoopMap();
        }
    }
}
