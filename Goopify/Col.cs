using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;
using System.Linq;
using System.Diagnostics;
using System.Collections.Concurrent;
using static Goopify.EditorWindow;

namespace Goopify
{
    public class Col
    {

        private class Header
        {
            public UInt32 vertCount;
            public UInt32 vertOffset;
            public UInt32 groupCount;
            public UInt32 groupOffset;
        }

        private class ColGroup
        {
            public UInt16 collisionType;
            public UInt16 triangleCount;
            public uint groupFlags;
            public uint hasColParam;
            public UInt32 vertIndexOffset;
            public UInt32 terrainTypeOffset;
            public UInt32 unknownOffset;
            public UInt32 colParameterOffset;

            public Triangle[] triangleArray;
        }

        public class Triangle
        {
            public int[] vertexIndices = new int[3];
            /*public int colType = 0;
            public int terrainType = 0;
            public int unknown = 0;*/
            public int colParameter;

            public Triangle Clone()
            {
                Triangle clone = new Triangle();
                clone.vertexIndices = (int[])vertexIndices.Clone();
                /*clone.colType = colType;
                clone.terrainType = terrainType;
                clone.unknown = unknown;*/
                clone.colParameter = colParameter;
                return clone;
            }

            public Triangle() { }

            public Triangle(int colValue) {
                colParameter = colValue;
            }
        }

        // Variables needed to display a col file
        private List<Vector3> vertList = new List<Vector3>();
        private Triangle[] allTrianglesArray;
        private Vector2[] bitmapUvArray;
        private Vector2[] goopUvArray;

        private float[] vertexData;
        private int vao;
        private int vbo;
        private bool bound = false;

        // Variables that define the bounds of the col
        private Vector2 topLeftBound;
        private Vector2 bottomRightBound;

        private bool colSetup = false;
        private bool uvSetup = false;

        private bool showNormals = false;
        private bool showWireframe = false;

        public float goopUvScale = 1;
        public float goopUvRotation = 0;

        /// <summary>
        /// Constructor, create col file using a Stream to a .col file
        /// Saves the vertexes, the triangles, and the model bounds, then enables colSetup
        /// </summary>
        /// <param name="colStream">Stream to a .col file</param>
        public Col(Stream colStream)
        {
            BinaryReaderBE binaryReader = new BinaryReaderBE(colStream); // Convert stream to BigEndian Reader (So we can get values from the file data)

            // Read all the header info
            Header colHeader = new Header();
            colHeader.vertCount = binaryReader.ReadUInt32();
            colHeader.vertOffset = binaryReader.ReadUInt32();
            colHeader.groupCount = binaryReader.ReadUInt32();
            colHeader.groupOffset = binaryReader.ReadUInt32();

            binaryReader.BaseStream.Seek(colHeader.vertOffset, SeekOrigin.Begin); // Goto vertex offset

            float maxXPos = float.MaxValue;
            float maxZPos = float.MaxValue;
            float minXPos = float.MaxValue;
            float minZPos = float.MaxValue;

            // Reads and saves all the verts
            for(int i = 0; i < colHeader.vertCount; i++)
            {
                Vector3 currentVert = new Vector3();
                currentVert.X = binaryReader.ReadSingle();
                currentVert.Y = binaryReader.ReadSingle();
                currentVert.Z = binaryReader.ReadSingle();
                vertList.Add(currentVert);

                // Find the max and min bounds
                if(maxXPos == float.MaxValue)
                {
                    maxXPos = currentVert.X;
                    minXPos = currentVert.X;
                    maxZPos = currentVert.Z;
                    minZPos = currentVert.Z;
                }

                if(currentVert.Z < minZPos)
                {
                    minZPos = currentVert.Z;
                }
                if (currentVert.Z > maxZPos)
                {
                    maxZPos = currentVert.Z;
                }
                if (currentVert.X < minXPos)
                {
                    minXPos = currentVert.X;
                }
                if (currentVert.X > maxXPos)
                {
                    maxXPos = currentVert.X;
                }
            }
            // Save the bounds of the col (positive x is left)
            topLeftBound = new Vector2(maxXPos, maxZPos);
            bottomRightBound = new Vector2(minXPos, minZPos);

            binaryReader.BaseStream.Seek(colHeader.groupOffset, SeekOrigin.Begin); // Goto group offset

            // Reads and saves all the groups
            ColGroup[] groupArray = new ColGroup[colHeader.groupCount];
            for (int i = 0; i < colHeader.groupCount; i++)
            {
                ColGroup currentGroup = new ColGroup();

                currentGroup.collisionType = binaryReader.ReadUInt16();
                currentGroup.triangleCount = binaryReader.ReadUInt16();
                currentGroup.groupFlags = binaryReader.ReadByte();
                currentGroup.hasColParam = binaryReader.ReadByte();
                binaryReader.ReadUInt16(); // Padding
                currentGroup.vertIndexOffset = binaryReader.ReadUInt32();
                currentGroup.terrainTypeOffset = binaryReader.ReadUInt32();
                currentGroup.unknownOffset = binaryReader.ReadUInt32();
                currentGroup.colParameterOffset = binaryReader.ReadUInt32();

                groupArray[i] = currentGroup;
            }

            // Converts the groups into triangles, also creates array of all triangles
            List<Triangle> triangleList = new List<Triangle>();
            foreach (ColGroup currentGroup in groupArray)
            {
                currentGroup.triangleArray = new Triangle[currentGroup.triangleCount]; // Create array of group triangles
                binaryReader.BaseStream.Seek(currentGroup.vertIndexOffset, SeekOrigin.Begin); // Goto current groups vertex index offset
                for(int i = 0; i < currentGroup.triangleArray.Length; i++)
                {
                    Triangle currentTriangle = new Triangle();
                    // Set the triangle vertex indicies
                    currentTriangle.vertexIndices[0] = binaryReader.ReadUInt16();
                    currentTriangle.vertexIndices[1] = binaryReader.ReadUInt16();
                    currentTriangle.vertexIndices[2] = binaryReader.ReadUInt16();
                    // Set the triangle collision type
                    currentTriangle.colParameter = currentGroup.collisionType;

                    currentGroup.triangleArray[i] = currentTriangle;
                }
                triangleList.AddRange(currentGroup.triangleArray);
            }

            // Convert list of all triangles to array
            allTrianglesArray = triangleList.ToArray();

            Console.WriteLine(colHeader.vertCount + ", " + colHeader.vertOffset + ", " + colHeader.groupCount + ", " + colHeader.groupOffset);

            SetUVsByProjection(Vector3.Zero, 1, 1); // Setup uvs just so we can render
            //current_texture = LoadTexture("C://Users//alexh//Downloads//custom_event_thumbnail.png", 1);
            //GL.Enable(EnableCap.Texture2D);

            //uvArray = SetUVByProjection(); // Unwraps the model to use a projected texture

            colSetup = true;
        }

        /// <summary>
        /// Constructor, used to make a col from verts and triangles already saved from a split model
        /// Saves the vertexes, the triangles, and the model bounds, then enables colSetup
        /// </summary>
        /// <param name="gottenVerts">Vertexes from the cut half of a col</param>
        /// <param name="gottenTriangles">Triangles from the cut half of a col</param>
        public Col(List<Vector3> gottenVerts, Triangle[] gottenTriangles)
        {
            // Save the triangles and verts
            vertList = new List<Vector3>(gottenVerts);
            allTrianglesArray = new Triangle[gottenTriangles.Length];
            for(int i = 0; i < allTrianglesArray.Length; i++)
            {
                allTrianglesArray[i] = gottenTriangles[i].Clone();
            }

            CalculateBounds();

            colSetup = true;
        }

        private void CalculateBounds()
        {
            // Calculate the model bounds
            float maxXPos = float.MaxValue;
            float maxZPos = float.MaxValue;
            float minXPos = float.MaxValue;
            float minZPos = float.MaxValue;
            for (int i = 0; i < vertList.Count; i++)
            {

                // Find the max and min bounds
                if (maxXPos == float.MaxValue)
                {
                    maxXPos = vertList[i].X;
                    minXPos = vertList[i].X;
                    maxZPos = vertList[i].Z;
                    minZPos = vertList[i].Z;
                }

                if (vertList[i].Z < minZPos)
                {
                    minZPos = vertList[i].Z;
                }
                if (vertList[i].Z > maxZPos)
                {
                    maxZPos = vertList[i].Z;
                }
                if (vertList[i].X < minXPos)
                {
                    minXPos = vertList[i].X;
                }
                if (vertList[i].X > maxXPos)
                {
                    maxXPos = vertList[i].X;
                }
            }
            // Save the bounds of the col
            topLeftBound = new Vector2(maxXPos, maxZPos);
            bottomRightBound = new Vector2(minXPos, minZPos);
        }

        private Color MixColor(Color color, Color backColor, double amount)
        {
            byte r = (byte)(color.R * amount + backColor.R * (1 - amount));
            byte g = (byte)(color.G * amount + backColor.G * (1 - amount));
            byte b = (byte)(color.B * amount + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }

        // Call to render this col file
        public void RenderCol(int textureToLoad = 0, int pollutionTexture = 0)
        {
            if(!colSetup)
            {
                Console.WriteLine("Col file not created first");
                return;
            }

            // Render
            if(vertexData != null)
            {
                if(!bound) {
                    BindCol();
                }

                GL.Enable(EnableCap.Texture2D);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textureToLoad);
                GL.Uniform1(GL.GetUniformLocation(EditorWindow.visualShader.Handle, "maskTexture"), 0);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, pollutionTexture);
                GL.Uniform1(GL.GetUniformLocation(EditorWindow.visualShader.Handle, "colorTexture"), 1);

                GL.BindVertexArray(vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, allTrianglesArray.Length * 3);
            } else
            {
                //Draw triangles

                if (uvSetup)
                {
                    GL.Enable(EnableCap.Texture2D);
                }
                else
                {
                    GL.Disable(EnableCap.Texture2D);
                }

                // Culling

                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Front);

                //GL.Color4(Color.White);

                GL.Begin(PrimitiveType.Triangles);

                foreach (Triangle currentTriangle in allTrianglesArray)
                {
                    Vector3 triangleNormal = Vector3.Cross(vertList[currentTriangle.vertexIndices[0]] - vertList[currentTriangle.vertexIndices[1]], vertList[currentTriangle.vertexIndices[0]] - vertList[currentTriangle.vertexIndices[2]]);

                    Color triangleColor;
                    if (EditorWindow.colNumColor.ContainsKey(currentTriangle.colParameter))
                    {
                        triangleColor = EditorWindow.colNumColor[currentTriangle.colParameter];
                    }
                    else
                    {
                        Random rnd = new Random(currentTriangle.colParameter);
                        triangleColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    }

                    // Get the color of the normal
                    if (triangleNormal == Vector3.Zero)
                    {
                        triangleNormal = new Vector3(0, 1, 0);
                    }
                    Vector3 triangleNormalized = triangleNormal.Normalized();
                    Vector3 lightDirection = new Vector3(0.5f, -0.5f, 0.5f);
                    double distance = Vector3.CalculateAngle(triangleNormalized, lightDirection) / Math.PI;
                    Color lightColor = Color.FromArgb((int)(distance * 255), (int)(distance * 255), (int)(distance * 255));

                    //if (!uvSetup)
                    GL.Color3(MixColor(triangleColor, lightColor, currentTriangle.colParameter == -1 ? 1 : 0.5f));

                    for (int i = 0; i < 3; i++)
                    {
                        if (uvSetup)
                        {
                            Vector2 textCoord = bitmapUvArray[currentTriangle.vertexIndices[i]];
                            GL.TexCoord2(new Vector2(textCoord.X * -1, textCoord.Y));
                        }
                        GL.Vertex3(vertList[currentTriangle.vertexIndices[i]]);
                    }
                }



                GL.End();

                GL.BindTexture(TextureTarget.Texture2D, 0);

                // Draw wireframe
                if (showWireframe)
                {
                    GL.LineWidth(0.5f);
                    GL.Begin(PrimitiveType.Lines);

                    foreach (Triangle currentTriangle in allTrianglesArray)
                    {
                        GL.Color4(Color.White);
                        GL.Vertex3(vertList[currentTriangle.vertexIndices[0]] + new Vector3(0, 20, 0));
                        GL.Vertex3(vertList[currentTriangle.vertexIndices[1]] + new Vector3(0, 20, 0));
                        GL.Color4(Color.White);
                        GL.Vertex3(vertList[currentTriangle.vertexIndices[1]] + new Vector3(0, 20, 0));
                        GL.Vertex3(vertList[currentTriangle.vertexIndices[2]] + new Vector3(0, 20, 0));
                        GL.Color4(Color.White);
                        GL.Vertex3(vertList[currentTriangle.vertexIndices[2]] + new Vector3(0, 20, 0));
                        GL.Vertex3(vertList[currentTriangle.vertexIndices[0]] + new Vector3(0, 20, 0));
                    }

                    GL.End();
                }

                // Draw normal lines
                if (showNormals)
                {
                    GL.LineWidth(1);
                    GL.Begin(PrimitiveType.Lines);

                    foreach (Triangle currentTriangle in allTrianglesArray)
                    {
                        Vector3 triangleNormal = Vector3.Cross(vertList[currentTriangle.vertexIndices[1]] - vertList[currentTriangle.vertexIndices[0]], vertList[currentTriangle.vertexIndices[2]] - vertList[currentTriangle.vertexIndices[0]]);
                        Vector3 midPoint = (vertList[currentTriangle.vertexIndices[0]] + vertList[currentTriangle.vertexIndices[1]] + vertList[currentTriangle.vertexIndices[2]]) / 3;

                        GL.Color4(Color.Red);
                        GL.Vertex3(midPoint);
                        GL.Vertex3(midPoint + Vector3.Normalize(triangleNormal) * 100f);
                    }

                    GL.End();
                }
            }
        }

        public Vector2 ReturnTopLeft()
        {
            return topLeftBound;
        }

        public Vector2 ReturnBottomRight()
        {
            return bottomRightBound;
        }

        public float ReturnLowestVertHeight()
        {
            float lowestHeight = float.MaxValue;
            foreach(Vector3 vert in vertList)
            {
                if(vert.Y < lowestHeight) { lowestHeight = vert.Y; }
            }
            return lowestHeight;
        }

        public float ReturnLowestVertHeight(float xMin, float xMax, float zMin, float zMax, int[] colsToIgnore = null)
        {
            float lowestHeight = float.MaxValue;
            foreach (Triangle currentTriangle in allTrianglesArray) {
                // Ignore triangles that have a collision we're ignoring
                if(colsToIgnore != null && colsToIgnore.Contains(currentTriangle.colParameter)) {
                    continue;
                }
                // Ignore triangles that are not within tolerance
                // TODO: Fix for different layer types
                Vector3 triangleNormal = Vector3.Cross(vertList[currentTriangle.vertexIndices[1]] - vertList[currentTriangle.vertexIndices[0]], vertList[currentTriangle.vertexIndices[2]] - vertList[currentTriangle.vertexIndices[0]]).Normalized();
                if (Vector3.CalculateAngle(Vector3.UnitY, triangleNormal) > (Properties.Settings.Default.goopAngleTolerance * Math.PI / 180)) // If normal is within angle tolerance of facing upward, keep it
                {
                    continue;
                }
                // Check each vert in triangle for lowest point (if in valid area)
                for (int i = 0; i < 3; i++) {
                    Vector3 vert = vertList[currentTriangle.vertexIndices[i]];
                    // Check if the vert is within bounds
                    if(vert.X >= xMin && vert.X <= xMax && vert.Z >= zMin && vert.Z <= zMax) {
                        if (vert.Y < lowestHeight) { lowestHeight = vert.Y; }
                    }
                }
            }
            return lowestHeight;
        }

        public float GetAutoHight(GoopRegionBox goopRegionBox, int[] colsToIgnore = null)
        {
            float lowestHeight = float.MaxValue;
            foreach (Triangle currentTriangle in allTrianglesArray)
            {
                // Ignore triangles that have a collision we're ignoring
                if (colsToIgnore != null && colsToIgnore.Contains(currentTriangle.colParameter))
                {
                    continue;
                }
                // Ignore triangles that are not within tolerance
                Vector3 goopDirection = goopRegionBox.ForwardDirection();
                Vector3 triangleNormal = Vector3.Cross(vertList[currentTriangle.vertexIndices[1]] - vertList[currentTriangle.vertexIndices[0]], vertList[currentTriangle.vertexIndices[2]] - vertList[currentTriangle.vertexIndices[0]]).Normalized();
                if (Vector3.CalculateAngle(goopDirection, triangleNormal) > (Properties.Settings.Default.goopAngleTolerance * Math.PI / 180)) // If normal is within angle tolerance of facing upward, keep it
                {
                    continue;
                }
                // Check each vert in triangle for lowest point (if in valid area)
                for (int i = 0; i < 3; i++)
                {
                    Vector3 vert = vertList[currentTriangle.vertexIndices[i]];
                    // Check if the vert is within bounds
                    if(goopDirection == Vector3.UnitY)
                    {
                        float xMin = goopRegionBox.startPos.X;
                        float xMax = goopRegionBox.startPos.X + goopRegionBox.width;
                        float zMin = goopRegionBox.startPos.Z;
                        float zMax = goopRegionBox.startPos.Z + goopRegionBox.length;
                        if (vert.X >= xMin && vert.X <= xMax && vert.Z >= zMin && vert.Z <= zMax)
                        {
                            if (vert.Y < lowestHeight) { lowestHeight = vert.Y; }
                        }
                    } else if (goopDirection == Vector3.UnitZ)
                    {
                        float xMin = goopRegionBox.startPos.X;
                        float xMax = goopRegionBox.startPos.X + goopRegionBox.width;
                        float yMin = goopRegionBox.startPos.Y;
                        float yMax = goopRegionBox.startPos.Y + goopRegionBox.length;
                        if (vert.X >= xMin && vert.X <= xMax && vert.Y >= yMin && vert.Y <= yMax)
                        {
                            if (vert.Z < lowestHeight) { lowestHeight = vert.Z; }
                        }
                    }

                }
            }
            return lowestHeight;
        }

        public float ReturnHighestVertHeight()
        {
            float highestHeight = float.MinValue;
            foreach (Vector3 vert in vertList)
            {
                if (vert.Y > highestHeight) { highestHeight = vert.Y; }
            }
            return highestHeight;
        }

        public Vector2? GetUVFromPosAndDir(Vector3 worldPos, Vector3 dir)
        {
            if(RaycastToModel(worldPos, dir, out RaycastHitInfo hit) && bitmapUvArray.Length == vertList.Count)
            {
                // Convert the point to a barycentric point in the triangle and use it to lerp the uv
                
                // Get triangle points
                Vector3 p1 = vertList[hit.triangle.vertexIndices[0]];
                Vector3 p2 = vertList[hit.triangle.vertexIndices[1]];
                Vector3 p3 = vertList[hit.triangle.vertexIndices[2]];
                // Get point uvs
                Vector2 uv1 = bitmapUvArray[hit.triangle.vertexIndices[0]];
                Vector2 uv2 = bitmapUvArray[hit.triangle.vertexIndices[1]];
                Vector2 uv3 = bitmapUvArray[hit.triangle.vertexIndices[2]];
                // Calculate vectors from hit point triangle vertices
                Vector3 f1 = p1 - hit.hitPos;
                Vector3 f2 = p2 - hit.hitPos;
                Vector3 f3 = p3 - hit.hitPos;
                // calculate the areas
                Vector3 va = Vector3.Cross(p1 - p2, p1 - p3); // main triangle cross product
                Vector3 va1 = Vector3.Cross(f2, f3); // p1's triangle cross product
                Vector3 va2 = Vector3.Cross(f3, f1); // p2's triangle cross product
                Vector3 va3 = Vector3.Cross(f1, f2); // p3's triangle cross product
                float a = va.Length; // main triangle area
                // calculate barycentric coordinates with sign
                float a1 = va1.Length / a * Math.Sign(Vector3.Dot(va, va1));
                float a2 = va2.Length / a * Math.Sign(Vector3.Dot(va, va2));
                float a3 = va3.Length / a * Math.Sign(Vector3.Dot(va, va3));
                // find the uv corresponding to point f (uv1/uv2/uv3 are associated to p1/p2/p3)
                Vector2 uv = (uv1 * a1 + uv2 * a2 + uv3 * a3);
                return new Vector2(uv.X, 1 - uv.Y);
            }
            return null;
        }

        // Multithreaded version of line splitting (Was not faster if I remember right)
        public Col SplitModelByLineParallel(Vector3 pointOne, Vector3 pointTwo, bool returnRight = false)
        {
            Col modelCopy = new Col(vertList, allTrianglesArray);
            ConcurrentBag<Triangle> leftTriangleList = new ConcurrentBag<Triangle>();
            ConcurrentBag<Triangle> rightTriangleList = new ConcurrentBag<Triangle>();

            System.Threading.Tasks.Parallel.For(0, modelCopy.allTrianglesArray.Length, i =>
            {
                bool? vertOneLeft = modelCopy.IsLeftOf(pointOne, pointTwo, modelCopy.vertList[modelCopy.allTrianglesArray[i].vertexIndices[0]]);
                bool? vertTwoLeft = modelCopy.IsLeftOf(pointOne, pointTwo, modelCopy.vertList[modelCopy.allTrianglesArray[i].vertexIndices[1]]);
                bool? vertThreeLeft = modelCopy.IsLeftOf(pointOne, pointTwo, modelCopy.vertList[modelCopy.allTrianglesArray[i].vertexIndices[2]]);

                int leftValue = 0;
                int rightValue = 0;

                if (vertOneLeft == true) { leftValue++; } else if (vertOneLeft == false) { rightValue++; }
                if (vertTwoLeft == true) { leftValue++; } else if (vertTwoLeft == false) { rightValue++; }
                if (vertThreeLeft == true) { leftValue++; } else if (vertThreeLeft == false) { rightValue++; }

                if (rightValue == 0) // Triangle is all in the left (might have point on line)
                {
                    leftTriangleList.Add(modelCopy.allTrianglesArray[i]);
                }
                else if (leftValue == 0) // Triangle is all in the right (might have point on line)
                {
                    rightTriangleList.Add(modelCopy.allTrianglesArray[i]);
                }
                else // Triangle intersects with the line
                {

                    if (vertOneLeft != null && vertTwoLeft != null && vertThreeLeft != null) // Not case of a left-intersect-right triangle
                    {
                        // One and two on the same side
                        if (vertOneLeft == vertTwoLeft)
                        {
                            Triangle[] splitTriangles = modelCopy.SplitTriangle(0, 1, 2, modelCopy.allTrianglesArray[i], pointOne, pointTwo);
                            if (vertOneLeft == true)
                            {
                                leftTriangleList.Add(splitTriangles[0]);
                                leftTriangleList.Add(splitTriangles[1]);
                                rightTriangleList.Add(splitTriangles[2]);
                            }
                            else
                            {
                                rightTriangleList.Add(splitTriangles[0]);
                                rightTriangleList.Add(splitTriangles[1]);
                                leftTriangleList.Add(splitTriangles[2]);
                            }
                        }
                        // Two and three on the same side
                        if (vertTwoLeft == vertThreeLeft)
                        {
                            Triangle[] splitTriangles = modelCopy.SplitTriangle(1, 2, 0, modelCopy.allTrianglesArray[i], pointOne, pointTwo);
                            if (vertTwoLeft == true)
                            {
                                leftTriangleList.Add(splitTriangles[0]);
                                leftTriangleList.Add(splitTriangles[1]);
                                rightTriangleList.Add(splitTriangles[2]);
                            }
                            else
                            {
                                rightTriangleList.Add(splitTriangles[0]);
                                rightTriangleList.Add(splitTriangles[1]);
                                leftTriangleList.Add(splitTriangles[2]);
                            }
                        }
                        // Three and one on the same side
                        if (vertThreeLeft == vertOneLeft)
                        {
                            Triangle[] splitTriangles = modelCopy.SplitTriangle(2, 0, 1, modelCopy.allTrianglesArray[i], pointOne, pointTwo);
                            if (vertThreeLeft == true)
                            {
                                leftTriangleList.Add(splitTriangles[0]);
                                leftTriangleList.Add(splitTriangles[1]);
                                rightTriangleList.Add(splitTriangles[2]);
                            }
                            else
                            {
                                rightTriangleList.Add(splitTriangles[0]);
                                rightTriangleList.Add(splitTriangles[1]);
                                leftTriangleList.Add(splitTriangles[2]);
                            }
                        }
                    }
                }
            });
            Col cutCol = new Col(modelCopy.vertList, returnRight ? rightTriangleList.ToArray() : leftTriangleList.ToArray());
            cutCol.CleanUpVerts();
            return cutCol;
        }

        /// <summary>
        /// Splits the col into two different models 
        /// </summary>
        /// <param name="pointOne"></param>
        /// <param name="pointTwo"></param>
        /// <returns>Right side collision</returns>
        public Col SplitModelByLine(Vector3 pointOne, Vector3 pointTwo, bool returnRight = false)
        {
            Col modelCopy = new Col(vertList, allTrianglesArray);
            List<Triangle> leftTriangleList = new List<Triangle>();
            List<Triangle> rightTriangleList = new List<Triangle>();

            foreach (Triangle currentTriangle in modelCopy.allTrianglesArray)
            {
                bool? vertOneLeft = modelCopy.IsLeftOf(pointOne, pointTwo, modelCopy.vertList[currentTriangle.vertexIndices[0]]);
                bool? vertTwoLeft = modelCopy.IsLeftOf(pointOne, pointTwo, modelCopy.vertList[currentTriangle.vertexIndices[1]]);
                bool? vertThreeLeft = modelCopy.IsLeftOf(pointOne, pointTwo, modelCopy.vertList[currentTriangle.vertexIndices[2]]);

                int leftValue = 0;
                int rightValue = 0;

                if(vertOneLeft == true) { leftValue++; } else if(vertOneLeft == false) { rightValue++; }
                if (vertTwoLeft == true) { leftValue++; } else if (vertTwoLeft == false) { rightValue++; }
                if (vertThreeLeft == true) { leftValue++; } else if (vertThreeLeft == false) { rightValue++; }

                if (rightValue == 0) // Triangle is all in the left (might have point on line)
                {
                    leftTriangleList.Add(currentTriangle);
                } else if(leftValue == 0) // Triangle is all in the right (might have point on line)
                {
                    rightTriangleList.Add(currentTriangle);
                } else // Triangle intersects with the line
                {

                    if(vertOneLeft != null && vertTwoLeft != null && vertThreeLeft != null) // Not case of a left-intersect-right triangle
                    {
                        // One and two on the same side
                        if (vertOneLeft == vertTwoLeft)
                        {
                            Triangle[] splitTriangles = modelCopy.SplitTriangle(0, 1, 2, currentTriangle, pointOne, pointTwo);
                            if(vertOneLeft == true)
                            {
                                leftTriangleList.Add(splitTriangles[0]);
                                leftTriangleList.Add(splitTriangles[1]);
                                rightTriangleList.Add(splitTriangles[2]);
                            } else
                            {
                                rightTriangleList.Add(splitTriangles[0]);
                                rightTriangleList.Add(splitTriangles[1]);
                                leftTriangleList.Add(splitTriangles[2]);
                            }
                        }
                        // Two and three on the same side
                        if (vertTwoLeft == vertThreeLeft)
                        {
                            Triangle[] splitTriangles = modelCopy.SplitTriangle(1, 2, 0, currentTriangle, pointOne, pointTwo);
                            if (vertTwoLeft == true)
                            {
                                leftTriangleList.Add(splitTriangles[0]);
                                leftTriangleList.Add(splitTriangles[1]);
                                rightTriangleList.Add(splitTriangles[2]);
                            }
                            else
                            {
                                rightTriangleList.Add(splitTriangles[0]);
                                rightTriangleList.Add(splitTriangles[1]);
                                leftTriangleList.Add(splitTriangles[2]);
                            }
                        }
                        // Three and one on the same side
                        if (vertThreeLeft == vertOneLeft)
                        {
                            Triangle[] splitTriangles = modelCopy.SplitTriangle(2, 0, 1, currentTriangle, pointOne, pointTwo);
                            if (vertThreeLeft == true)
                            {
                                leftTriangleList.Add(splitTriangles[0]);
                                leftTriangleList.Add(splitTriangles[1]);
                                rightTriangleList.Add(splitTriangles[2]);
                            }
                            else
                            {
                                rightTriangleList.Add(splitTriangles[0]);
                                rightTriangleList.Add(splitTriangles[1]);
                                leftTriangleList.Add(splitTriangles[2]);
                            }
                        }
                    }
                }

            }

            // Combine all triangles into new triangle list
            /*allTrianglesArray = leftTriangleList.Concat(rightTriangleList).ToArray();

            foreach(Triangle tri in leftTriangleList)
            {
                tri.colParameter = 10;
            }
            foreach (Triangle tri in rightTriangleList)
            {
                tri.colParameter = 1120;
            }*/

            Col cutCol = new Col(modelCopy.vertList, returnRight ? rightTriangleList.ToArray() : leftTriangleList.ToArray());
            cutCol.CleanUpVerts();

            /*foreach(Triangle tri in cutCol.allTrianglesArray)
            {
                tri.colParameter = 10;
            }*/
            return cutCol;
        }

        /// <summary>
        /// Splits the triangle into three parts, with the first two in the returned array always being on the same side
        /// Adds the vertexes to the vertex list too
        /// </summary>
        /// <param name="pointOrderOne"></param>
        /// <param name="pointOrderTwo"></param>
        /// <param name="pointOrderThree"></param>
        /// <param name="currentTriangle"></param>
        /// <param name="pointOne"></param>
        /// <param name="pointTwo"></param>
        /// <returns></returns>
        private Triangle[] SplitTriangle(int pointOrderOne, int pointOrderTwo, int pointOrderThree, Triangle currentTriangle, Vector3 pointOne, Vector3 pointTwo)
        {
            int colVal = currentTriangle.colParameter;
            Triangle[] splitTriangles = new Triangle[3] { new Triangle(colVal), new Triangle(colVal), new Triangle(colVal) };

            Vector3 intersectPointOne = FindIntersection(vertList[currentTriangle.vertexIndices[pointOrderOne]], vertList[currentTriangle.vertexIndices[pointOrderThree]], pointOne, pointTwo);
            Vector3 intersectPointTwo = FindIntersection(vertList[currentTriangle.vertexIndices[pointOrderTwo]], vertList[currentTriangle.vertexIndices[pointOrderThree]], pointOne, pointTwo);

            // Calculate out the index that the verts would have for the triangles
            vertList.Add(intersectPointOne);
            int intersectOneIndex = vertList.Count - 1;
            vertList.Add(intersectPointTwo);
            int intersectTwoIndex = vertList.Count - 1;


            int[] currentVertIndices = (int[])currentTriangle.vertexIndices.Clone();

            splitTriangles[0].vertexIndices = new int[] { currentVertIndices[pointOrderOne], currentVertIndices[pointOrderTwo], intersectTwoIndex };
            splitTriangles[1].vertexIndices = new int[] { intersectTwoIndex, intersectOneIndex, currentVertIndices[pointOrderOne] };
            splitTriangles[2].vertexIndices = new int[] { intersectTwoIndex, currentVertIndices[pointOrderThree], intersectOneIndex };

            return splitTriangles;
        }

        /// <summary>
        /// Checks if point is left of the given line (Ignores height of model)
        /// https://rosettacode.org/wiki/Sutherland-Hodgman_polygon_clipping#C.23
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="modelPoint"></param>
        /// <returns></returns>
        private bool? IsLeftOf(Vector3 startPoint, Vector3 endPoint, Vector3 modelPoint)
        {
            // Vectors of line and line end to point
            Vector2 tmp1 = new Vector2(endPoint.X, endPoint.Z) - new Vector2(startPoint.X, startPoint.Z);
            Vector2 tmp2 = new Vector2(modelPoint.X, modelPoint.Z) - new Vector2(endPoint.X, endPoint.Z);

            double x = (tmp1.X * tmp2.Y) - (tmp1.Y * tmp2.X);       //	dot product of perpendicular?

            if (x < 0)
            {
                return false;
            }
            else if (x > 0)
            {
                return true;
            }
            else
            {
                //	Point is on line
                return null;
            }
        }

        /// <summary>
        /// Finds intersection point between two lines
        /// Ignores Y height for caclucation but still returns the value it should be for line 1
        /// </summary>
        /// <param name="line1Start"></param>
        /// <param name="line1End"></param>
        /// <param name="line2Start"></param>
        /// <param name="line2End"></param>
        /// <returns></returns>
        private Vector3 FindIntersection(Vector3 line1Start, Vector3 line1End, Vector3 line2Start, Vector3 line2End)
        {
            Vector3 direction1 = line1End - line1Start;
            Vector3 direction2 = line2End - line2Start;
            float determinant = (direction1.X * direction2.Z) - (direction1.Z * direction2.X); // Dot product of only x and z axis

            if(determinant == 0) // Lines the same so infinitly intersect
            {
                Console.WriteLine("ERROR: Lines perfectly intersect for a triangle you are trying to clip");
                return Vector3.Zero;
            }

            // Caclulates even if outside line segment?
            Vector3 c = line2Start - line1Start;
            float t = (c.X * direction2.Z - direction2.X * c.Z) / determinant;

            return line1Start + t*direction1;
        }

        /// <summary>
        /// Checks each triangle in the model at that position and returns the highest intersection point
        /// </summary>
        /// <param name="pointToCheck"></param>
        /// <returns></returns>
        public float? GetHighestIntersectionPosition(Vector3 pointToCheck)
        {
            float topHeight = float.MinValue;
            // TODO: Change that Unit Vector based on the pollution layer type
            Vector3 rayDirection = -Vector3.UnitY;
            foreach (Triangle triangle in allTrianglesArray)
            {
                Vector3 edge1 = vertList[triangle.vertexIndices[1]] - vertList[triangle.vertexIndices[0]];
                Vector3 edge2 = vertList[triangle.vertexIndices[2]] - vertList[triangle.vertexIndices[0]];

                
                Vector3 pVec = Vector3.Cross(rayDirection, edge2); 
                float det = Vector3.Dot(edge1, pVec); // Determinant

                //if determinant is near zero, ray lies in plane of triangle otherwise not
                if (det > -float.Epsilon && det < float.Epsilon) { continue; }
                float invDet = 1.0f / det;

                //calculate distance from p1 to ray origin
                Vector3 tVec = pointToCheck - vertList[triangle.vertexIndices[0]];

                //Calculate u parameter
                float u = Vector3.Dot(tVec, pVec) * invDet;

                //Check for ray hit
                if (u < 0 || u > 1) { continue; }

                //Prepare to test v parameter
                Vector3 qVec = Vector3.Cross(tVec, edge1);

                //Calculate v parameter
                float v = Vector3.Dot(rayDirection, qVec) * invDet;

                //Check for ray hit
                if (v < 0 || u + v > 1) { continue; }

                float t = Vector3.Dot(edge2, qVec) * invDet;
                if (t > float.Epsilon) //ray does intersect
                {
                    float gottenHeight = (rayDirection * t + pointToCheck).Y;
                    if(gottenHeight > topHeight) { topHeight = gottenHeight; }
                }
            }
            // Return the height if it exists
            if (topHeight != float.MinValue) {
                return topHeight;
            }
            return null;
        }

        public float? GetHighestIntersectionPositionParallel(Vector3 pointToCheck)
        {
            float topHeight = float.MinValue;
            // TODO: Change that Unit Vector based on the pollution layer type
            Vector3 rayDirection = -Vector3.UnitY;
            System.Threading.Tasks.Parallel.For(0, allTrianglesArray.Length, i =>
            {
                Triangle triangle = allTrianglesArray[i];
                Vector3 edge1 = vertList[triangle.vertexIndices[1]] - vertList[triangle.vertexIndices[0]];
                Vector3 edge2 = vertList[triangle.vertexIndices[2]] - vertList[triangle.vertexIndices[0]];


                Vector3 pVec = Vector3.Cross(rayDirection, edge2);
                float det = Vector3.Dot(edge1, pVec); // Determinant

                //if determinant is near zero, ray lies in plane of triangle otherwise not
                if (det > -float.Epsilon && det < float.Epsilon) { return; }
                float invDet = 1.0f / det;

                //calculate distance from p1 to ray origin
                Vector3 tVec = pointToCheck - vertList[triangle.vertexIndices[0]];

                //Calculate u parameter
                float u = Vector3.Dot(tVec, pVec) * invDet;

                //Check for ray hit
                if (u < 0 || u > 1) { return; }

                //Prepare to test v parameter
                Vector3 qVec = Vector3.Cross(tVec, edge1);

                //Calculate v parameter
                float v = Vector3.Dot(rayDirection, qVec) * invDet;

                //Check for ray hit
                if (v < 0 || u + v > 1) { return; }

                float t = Vector3.Dot(edge2, qVec) * invDet;
                if (t > float.Epsilon) //ray does intersect
                {
                    float gottenHeight = (rayDirection * t + pointToCheck).Y;
                    if (gottenHeight > topHeight) { topHeight = gottenHeight; }
                }
            });
            // Return the height if it exists
            if (topHeight != float.MinValue)
            {
                return topHeight;
            }
            return null;
        }

        public class RaycastHitInfo
        {
            public Triangle triangle;
            public Vector3 hitPos;

            public RaycastHitInfo(Triangle tri, Vector3 pos)
            {
                triangle = tri;
                hitPos = pos;
            }
        }

        public bool RaycastToModel(Vector3 origin, Vector3 direction, out RaycastHitInfo hit)
        {
            hit = null;
            float shortestT = float.MaxValue;
            foreach (Triangle triangle in allTrianglesArray)
            {
                Vector3 edge1 = vertList[triangle.vertexIndices[1]] - vertList[triangle.vertexIndices[0]];
                Vector3 edge2 = vertList[triangle.vertexIndices[2]] - vertList[triangle.vertexIndices[0]];


                Vector3 pVec = Vector3.Cross(direction, edge2);
                float det = Vector3.Dot(edge1, pVec); // Determinant

                //if determinant is near zero, ray lies in plane of triangle otherwise not
                if (det > -float.Epsilon && det < float.Epsilon) { continue; }
                float invDet = 1.0f / det;

                //calculate distance from p1 to ray origin
                Vector3 tVec = origin - vertList[triangle.vertexIndices[0]];

                //Calculate u parameter
                float u = Vector3.Dot(tVec, pVec) * invDet;

                //Check for ray hit
                if (u < 0 || u > 1) { continue; }

                //Prepare to test v parameter
                Vector3 qVec = Vector3.Cross(tVec, edge1);

                //Calculate v parameter
                float v = Vector3.Dot(direction, qVec) * invDet;

                //Check for ray hit
                if (v < 0 || u + v > 1) { continue; }

                float t = Vector3.Dot(edge2, qVec) * invDet;
                if (t > float.Epsilon) //ray does intersect
                {
                    if(t < shortestT) { // Take the closest raycast
                        hit = new RaycastHitInfo(triangle, (direction * t + origin));
                        shortestT = t;
                    }
                }
            }
            return hit != null;
        }

        /// <summary>
        /// Sets the uv coords of the cols vertexes projection style
        /// Bottom left is 0,0 in uv coordinates
        /// </summary>
        /// <returns></returns>
        public void SetUVsByProjection(Vector3 boxBottomLeftCorner, float boxWidth, float boxLength)
        {
            Vector2[] vertBitmapUVArray = new Vector2[vertList.Count];
            Vector2[] vertGoopUVArray = new Vector2[vertList.Count];

            for (int i = 0; i < vertList.Count; i++)
            {
                if (vertList[i].Y == topLeftBound.Y)
                {
                    Console.WriteLine("Smallest Vert X");
                }
                float vertWidth = vertList[i].X - boxBottomLeftCorner.X;// Left is positive x so doing left minus right
                float vertHeight = boxBottomLeftCorner.Z - vertList[i].Z; // Looking on the z axis
                // Set the bitmap UV for the vert
                float vertBitmapUVX = vertWidth / boxWidth;
                float vertBitmapUVY = vertHeight / boxLength;
                vertBitmapUVArray[i] = new Vector2(vertBitmapUVX, vertBitmapUVY);
                // Set the goop UV for the vert
                Vector2 goopUv = new Vector2(vertList[i].X / 2048, vertList[i].Z / 2048);
                vertGoopUVArray[i] = goopUv;
            }

            uvSetup = true;
            bitmapUvArray = vertBitmapUVArray;
            goopUvArray = vertGoopUVArray;
            UpdateVertexData();
        }

        /// <summary>
        /// Removes any verts that are unused by triangles or if they already exist
        /// </summary>
        private void CleanUpVerts()
        {
            List<Vector3> cleanedVerts = new List<Vector3>();

            for(int i = 0; i < vertList.Count; i++)
            {
                if(cleanedVerts.Contains(vertList[i])) // Already have the vert in the list
                {
                    int firstCopyIndex = cleanedVerts.IndexOf(vertList[i]);
                    foreach(Triangle tri in allTrianglesArray) // Update the triangle vert ref to remove the duplicate
                    {
                        for(int j = 0; j < 3; j++)
                        {
                            if(tri.vertexIndices[j] == i)
                            {
                                tri.vertexIndices[j] = firstCopyIndex;
                            }
                        }
                    }
                } else // Check if the triangles even uses the vert
                {
                    bool triangleHasVert = false;
                    foreach (Triangle tri in allTrianglesArray) 
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (tri.vertexIndices[j] == i)
                            {
                                triangleHasVert = true;
                                tri.vertexIndices[j] = cleanedVerts.Count;
                            }
                        }
                    }

                    if(triangleHasVert)
                    {
                        cleanedVerts.Add(vertList[i]);
                    }
                }
            }
            //Console.WriteLine("Original Vert Count: " + vertList.Count);
            //Console.WriteLine("Cleaned Vert Count: " + cleanedVerts.Count);
            vertList = new List<Vector3>(cleanedVerts);

            CalculateBounds();
        }

        /// <summary>
        /// Removes all triangles that wouldn't apply for converting to goop bmds (facing sideways or down)
        /// </summary>
        public void CleanupModelForGoop(int[] colTypesToRemove, float heightMinimum = float.NegativeInfinity)
        {
            List<Triangle> newTriangleList = new List<Triangle>();
            foreach(Triangle currentTriangle in allTrianglesArray)
            {
                // Remove triangles that are of a collision type we don't want
                bool isInvalidCol = false;
                foreach(int removeColType in colTypesToRemove)
                {
                    if(currentTriangle.colParameter == removeColType || currentTriangle.colParameter == (removeColType + 32768)) {
                        isInvalidCol = true;
                        break;
                    }
                }
                if(isInvalidCol) { continue; }
                // Remove triangles that have a point below the minimum height or above the height limit of the region
                bool isInvalidHeight = false;
                foreach(int vertIndex in currentTriangle.vertexIndices)
                {
                    if(vertList[vertIndex].Y < heightMinimum || vertList[vertIndex].Y > heightMinimum + EditorWindow.maxRegionHeight) {
                        isInvalidHeight = true;
                    }
                }
                if (isInvalidHeight) { continue; }
                // Remove triangles not facing up enough for the goop to matter
                // TODO: Support wall goop, should just be a case of changing the UnitY based on layer type
                Vector3 triangleNormal = Vector3.Cross(vertList[currentTriangle.vertexIndices[1]] - vertList[currentTriangle.vertexIndices[0]], vertList[currentTriangle.vertexIndices[2]] - vertList[currentTriangle.vertexIndices[0]]).Normalized();
                if(Vector3.CalculateAngle(Vector3.UnitY, triangleNormal) < (Properties.Settings.Default.goopAngleTolerance * Math.PI/180)) // If normal is within angle tolerance of facing upward, keep it
                {
                    newTriangleList.Add(currentTriangle);
                }
            }

            allTrianglesArray = newTriangleList.ToArray();

            CleanUpVerts();

            // Set the collision to all be the same color
            foreach (Triangle tri in allTrianglesArray) {
                tri.colParameter = -1;
            }

            // Trim the overlapping triangles
            // TODO: Remove faces that are covered and theirfor should not be affected by the goop
            // Ideas: compare each triangle for overlap, cut the lowest one using the higher ones edges, remove the triangles inside, check the outside ones again
        }

        // Unused, Obj can only store one UV with means we can't convert it to a proper goop model
        /*public void CreateObjFromCol(string objPath)
        {
            // Delete the file in case we already made a local obj model for this path
            File.Delete(objPath);
            // Write the data into an obj format
            using (var outStream = File.OpenWrite(objPath))
            using (var writer = new StreamWriter(outStream))
            {
                // Write the vertexes
                for(int i = 0; i < vertList.Count; i++)
                {
                    writer.WriteLine(string.Format("v {0} {1} {2}", vertList[i].X, vertList[i].Y, vertList[i].Z));
                }
                // Write the uv coords
                if(uvSetup) {
                    for (int i = 0; i < bitmapUvArray.Length; i++)
                    {
                        writer.WriteLine(string.Format("vt {0} {1}", bitmapUvArray[i].X, bitmapUvArray[i].Y));
                    }
                }
                // Write the faces
                for(int i = 0; i < allTrianglesArray.Length; i++)
                {
                    if(!uvSetup) {
                        writer.WriteLine(string.Format("f {0} {1} {2}", allTrianglesArray[i].vertexIndices[0] + 1,
                            allTrianglesArray[i].vertexIndices[1] + 1, allTrianglesArray[i].vertexIndices[2] + 1));
                    } else {
                        writer.WriteLine(string.Format("f {0}/{0} {1}/{1} {2}/{2}", allTrianglesArray[i].vertexIndices[0] + 1,
                            allTrianglesArray[i].vertexIndices[1] + 1, allTrianglesArray[i].vertexIndices[2] + 1));
                    }
                }
                Console.WriteLine(outStream.Name);
            }
            
        }*/

        public void CreateDaeFromCol(string daePath)
        {
            // Load and duplicate the example dae
            string daeString = "";
            using(StreamReader r = new StreamReader("Resources\\goopFormatExample.dae"))
            {
                daeString = r.ReadToEnd();
            }

            // Replace the data

            // Vertex data
            string vertextString = " ";
            string weightString = "";
            for (int i = 0; i < vertList.Count; i++) {
                vertextString += "" + vertList[i].X + " " + vertList[i].Y + " " + vertList[i].Z + " ";
                weightString += "" + 1 + " ";
            }
            daeString = daeString.Replace("VERT_DATA", vertextString);
            daeString = daeString.Replace("VERT_COUNT", "" + vertList.Count);
            daeString = daeString.Replace("VERT_DOUBLE_COUNT", "" + (vertList.Count * 2));
            daeString = daeString.Replace("VERT_TRIPLE_COUNT", "" + (vertList.Count * 3));
            daeString = daeString.Replace("VERT_WEIGHTS", weightString);
            // Other vert info
            string otherWeightString = "";
            for(int i = 0; i < (vertList.Count * 2); i++) {
                otherWeightString += "0 ";
            }
            daeString = daeString.Replace("VERT_OTHER_WEIGHTS", otherWeightString);
            // UV 0 data
            string uv0String = " ";
            for (int i = 0; i < bitmapUvArray.Length; i++) {
                uv0String += "" + bitmapUvArray[i].X + " " + bitmapUvArray[i].Y + " ";
            }
            daeString = daeString.Replace("UV0_DATA", uv0String);
            // UV 1 data
            string uv1String = " ";
            for (int i = 0; i < goopUvArray.Length; i++) {
                uv1String += "" + goopUvArray[i].X + " " + goopUvArray[i].Y + " ";
            }
            daeString = daeString.Replace("UV1_DATA", uv1String);
            // Color0 data
            string colorString = " ";
            for (int i = 0; i < goopUvArray.Length; i++)
            {
                //colorString += "" + goopUvArray[i].X + " " + goopUvArray[i].Y + " ";
                colorString += "0 0 0 ";
            }
            daeString = daeString.Replace("COLOR_DATA", colorString);
            // Triangle data
            string triangleList = "";
            string triangleVertList = "";
            for (int i = 0; i < allTrianglesArray.Length; i++) {
                string triangleString = "" + allTrianglesArray[i].vertexIndices[0] + " " + allTrianglesArray[i].vertexIndices[1] + " " + allTrianglesArray[i].vertexIndices[2] + " ";
                triangleList += triangleString;
                triangleVertList += "3 ";
            }
            daeString = daeString.Replace("TRIANGLE_DATA", triangleList);
            daeString = daeString.Replace("TRIANGLE_VERTCOUNTS", triangleVertList);
            daeString = daeString.Replace("TRIANGLE_COUNT", "" + allTrianglesArray.Length);

            // Export
            // Delete the file in case we already made a local obj model for this path
            File.Delete(daePath);
            using (var outStream = File.OpenWrite(daePath))
            using (var writer = new StreamWriter(outStream))
            {
                writer.Write(daeString);
            }
        }

        public string CreateBmdFromCol(string bmdPath, string resourcePath, Size bmpSize)
        {
            if(Properties.Settings.Default.superBmdPath != "null")
            {
                string daePath = "modelExport.dae";
                CreateDaeFromCol(daePath);

                string localPath = Directory.GetCurrentDirectory();

                // Save an image the size of the bmp so the pollution can use it to compile the model correctly
                string pollutionImgPath = resourcePath + "\\BmpTexture.png";
                if (File.Exists(pollutionImgPath)) { File.Delete(pollutionImgPath); }
                Bitmap bmp = new Bitmap(bmpSize.Width, bmpSize.Height);
                using (Graphics graph = Graphics.FromImage(bmp))
                {
                    Rectangle ImageSize = new Rectangle(0, 0, bmpSize.Width, bmpSize.Height);
                    graph.FillRectangle(Brushes.White, ImageSize);
                }
                bmp.Save(pollutionImgPath, System.Drawing.Imaging.ImageFormat.Png);

                Process p = new Process();
                p.StartInfo.FileName = Properties.Settings.Default.superBmdPath;
                p.StartInfo.Arguments = "\"" + localPath + "\\" + daePath + "\" \"" + bmdPath + "\" --mat \"" + resourcePath + "\\goop_materials.json\" --texheader \"" + resourcePath + "\\goop_texheaders.json\" --texfloat32";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                //p.StartInfo.CreateNoWindow = true;
                p.Start();

                string errorText = p.StandardError.ReadToEnd();
                // Don't continue until superbmd has exported the model info
                p.WaitForExit();
                return errorText;
            }
            return "SuperBMD path not setup!";
        }

        public void UpdateVertexData()
        {
            // Vertex data container
            List<float> tempVertexData = new List<float>();

            // Iterate through all triangles
            foreach (Triangle tri in allTrianglesArray)
            {
                // Get the vert color we want based on the collision and faked lighting
                Color triangleColor;
                if (EditorWindow.colNumColor.ContainsKey(tri.colParameter)) {
                    triangleColor = EditorWindow.colNumColor[tri.colParameter];
                }
                else {
                    Random rnd = new Random(tri.colParameter);
                    triangleColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                }
                // Get the color of the normal
                Vector3 triangleNormal = Vector3.Cross(vertList[tri.vertexIndices[0]] - vertList[tri.vertexIndices[1]], vertList[tri.vertexIndices[0]] - vertList[tri.vertexIndices[2]]);
                if (triangleNormal == Vector3.Zero) {
                    triangleNormal = new Vector3(0, 1, 0);
                }
                Vector3 triangleNormalized = triangleNormal.Normalized();
                Vector3 lightDirection = new Vector3(0.5f, -0.5f, 0.5f);
                double distance = Vector3.CalculateAngle(triangleNormalized, lightDirection) / Math.PI;
                Color lightColor = Color.FromArgb((int)(distance * 255), (int)(distance * 255), (int)(distance * 255));

                triangleColor = MixColor(triangleColor, lightColor, tri.colParameter == -1 ? 1 : 0.5f);

                // Get vertex positions and UV coordinates for each vertex of the triangle
                for (int i = 0; i < 3; i++)
                {
                    int vertexIndex = tri.vertexIndices[i];

                    // Position
                    Vector3 position = vertList[vertexIndex];
                    tempVertexData.Add(position.X); // Add x position
                    tempVertexData.Add(position.Y); // Add y position
                    tempVertexData.Add(position.Z); // Add z position

                    // Color
                    tempVertexData.Add(triangleColor.R / 255f); // Add color r
                    tempVertexData.Add(triangleColor.G / 255f); // Add color g
                    tempVertexData.Add(triangleColor.B / 255f); // Add color b

                    // UV
                    Vector2 uv = bitmapUvArray != null ? bitmapUvArray[vertexIndex] : new Vector2(0,0);
                    tempVertexData.Add(uv.X); // Add u texture coordinate
                    tempVertexData.Add(uv.Y); // Add v texture coordinate
                    // Visual UV
                    Vector2 goopUv = goopUvArray != null ? goopUvArray[vertexIndex] : new Vector2(0, 0);
                    tempVertexData.Add(goopUv.X); // Add u texture coordinate
                    tempVertexData.Add(goopUv.Y); // Add v texture coordinate
                }
            }

            vertexData = tempVertexData.ToArray();
        }

        public void BindCol()
        {
            if (bound) {
                GL.DeleteBuffer(vbo);
                GL.DeleteVertexArray(vao);
            }

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            // Upload your vertex data (float[] with position, color, texcoord, visualCoord per vertex)
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

            // Attribute 0: aPosition (vec3)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 10 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Attribute 1: aColor (vec3)
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 10 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // Attribute 2: aTexCoord (vec2)
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 10 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            // Attribute 3: aVisualCoord (vec2)
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, 10 * sizeof(float), 8 * sizeof(float));
            GL.EnableVertexAttribArray(3);

            GL.BindVertexArray(0); // Done

            bound = true;
        }

        public void Write(BinaryWriter writer)
        {
            // Write the verts
            writer.Write(vertList.Count);
            foreach(Vector3 vert in vertList)
            {
                writer.Write(vert.X);
                writer.Write(vert.Y);
                writer.Write(vert.Z);
            }
            // Write the triangles
            writer.Write(allTrianglesArray.Length);
            foreach (Triangle triangle in allTrianglesArray)
            {
                writer.Write(triangle.vertexIndices[0]);
                writer.Write(triangle.vertexIndices[1]);
                writer.Write(triangle.vertexIndices[2]);

                writer.Write(triangle.colParameter);
            }
            // Write the bitmap uv
            int bitmapUvLength = bitmapUvArray == null ? 0 : bitmapUvArray.Length;
            writer.Write(bitmapUvLength);
            for (int i = 0; i < bitmapUvLength; i++)
            {
                writer.Write(bitmapUvArray[i].X);
                writer.Write(bitmapUvArray[i].Y);
            }
            // Write the goop uv
            int goopUvLength = goopUvArray == null ? 0 : goopUvArray.Length;
            writer.Write(goopUvLength);
            for (int i = 0; i < goopUvLength; i++)
            {
                writer.Write(goopUvArray[i].X);
                writer.Write(goopUvArray[i].Y);
            }

            writer.Write(goopUvScale);
            writer.Write(goopUvRotation);
        }

        public Col(BinaryReader reader)
        {
            // Read the verts
            int vertCount = reader.ReadInt32();
            for (int i = 0; i < vertCount; i++)
            {
                Vector3 newVert = new Vector3();
                newVert.X = reader.ReadSingle();
                newVert.Y = reader.ReadSingle();
                newVert.Z = reader.ReadSingle();

                vertList.Add(newVert);
            }
            // Read the triangles
            int triangleCount = reader.ReadInt32();
            List<Triangle> triangleList = new List<Triangle>();
            for (int i = 0; i < triangleCount; i++)
            {
                Triangle newTriangle = new Triangle();
                newTriangle.vertexIndices[0] = reader.ReadInt32();
                newTriangle.vertexIndices[1] = reader.ReadInt32();
                newTriangle.vertexIndices[2] = reader.ReadInt32();

                newTriangle.colParameter = reader.ReadInt32();

                triangleList.Add(newTriangle);
            }
            allTrianglesArray = triangleList.ToArray();
            // Write the bitmap uv
            int bitmapUvCount = reader.ReadInt32();
            List<Vector2> bitmapUvList = new List<Vector2>();
            for (int i = 0; i < bitmapUvCount; i++)
            {
                Vector2 tempUv = new Vector2();
                tempUv.X = reader.ReadSingle();
                tempUv.Y = reader.ReadSingle();

                bitmapUvList.Add(tempUv);
            }
            bitmapUvArray = bitmapUvList.Count == 0 ? null : bitmapUvList.ToArray();
            // Write the goop uv
            int goopUvCount = reader.ReadInt32();
            List<Vector2> goopUvList = new List<Vector2>();
            for (int i = 0; i < goopUvCount; i++)
            {
                Vector2 tempUv = new Vector2();
                tempUv.X = reader.ReadSingle();
                tempUv.Y = reader.ReadSingle();

                goopUvList.Add(tempUv);
            }
            goopUvArray = goopUvList.Count == 0 ? null : goopUvList.ToArray();

            goopUvScale = reader.ReadSingle();
            goopUvRotation = reader.ReadSingle();

            CalculateBounds();
            if(bitmapUvCount > 0) { UpdateVertexData(); }

            colSetup = true;
        }

        #region SCALE CHANGES

        public void OffsetAllVerts(Vector3 offset)
        {
            for(int i = 0; i < vertList.Count; i++)
            {
                vertList[i] += offset;
            }
            UpdateVertexData();
        }

        public void ScaleModel(float scale)
        {
            for (int i = 0; i < vertList.Count; i++) {
                vertList[i] = vertList[i] * scale;
            }
            UpdateVertexData();
        }

        public void ChangeGoopUVScale(float newScale)
        {
            if(newScale <= 0) { return; }
            for(int i = 0; i < goopUvArray.Length; i++)
            {
                goopUvArray[i] = goopUvArray[i] * goopUvScale / newScale;
            }
            goopUvScale = newScale;
            UpdateVertexData();
            BindCol();
        }

        public void ChangeGoopUVRotation(float newRoatation)
        {
            Vector2 centerPos = (topLeftBound + bottomRightBound) / 2;
            centerPos /= 2048; // Get the center uv we would have with our system
            centerPos = Vector2.Zero;
            for (int i = 0; i < goopUvArray.Length; i++)
            {
                goopUvArray[i] = RotateVector(goopUvArray[i] - centerPos, goopUvRotation - newRoatation) + centerPos;
            }
            goopUvRotation = newRoatation;
            UpdateVertexData();
            BindCol();
        }

        public Vector2 RotateVector(Vector2 v, float degrees)
        {
            float radians = degrees * ((float)Math.PI / 180f);
            float ca = (float)Math.Cos(radians);
            float sa = (float)Math.Sin(radians);
            return new Vector2(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }

        #endregion
    }
}
