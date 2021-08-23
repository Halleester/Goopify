using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Collections.Generic;
using OpenTK;
using System.Linq;

namespace Goopify
{
    class Col
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
            public int colType = 0;
            public int terrainType = 0;
            public int unknown = 0;
            public int colParameter;

        }

        // Variables needed to display a col file
        private List<Vector3> vertList = new List<Vector3>();
        private Triangle[] allTrianglesArray;
        private Vector2[] uvArray;

        // Variables that define the bounds of the col
        private Vector2 topLeftBound;
        private Vector2 bottomRightBound;

        private bool colSetup = false;
        private bool uvSetup = false;

        private int current_texture;

        private bool showNormals = false;
        private bool showWireframe = false;

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

            //uvArray = SetUVByProjection();
            current_texture = LoadTexture("C://Users//alexh//Downloads//custom_event_thumbnail.png", 1);
            GL.Enable(EnableCap.Texture2D);

            //uvArray = SetUVByProjection(); // Unwraps the model to use a projected texture

            colSetup = true;
        }

        /// <summary>
        /// Constructor, used to make a col from verts and triangles already saved from a split model
        /// Saves the vertexes, the triangles, and the model bounds, then enables colSetup
        /// </summary>
        /// <param name="gottenVerts">Vertexes from the cut half of a col</param>
        /// <param name="gottenTriangles">Triangles from the cut half of a col</param>
        public Col(Vector3[] gottenVerts, Triangle[] gottenTriangles)
        {
            // Save the triangles and verts
            vertList = new List<Vector3>(gottenVerts);
            allTrianglesArray = gottenTriangles;

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
            topLeftBound = new Vector2(minXPos, maxZPos);
            bottomRightBound = new Vector2(maxXPos, minZPos);

            colSetup = true;
        }

        // Call to render this col file
        public void RenderCol()
        {
            if(!colSetup)
            {
                Console.WriteLine("Col file not created first");
                return;
            }

            //Draw triangles

            // Lighting

            /* GL.Enable(EnableCap.Lighting);
             float[] lightDiffuse = { 1.0f, 0.0f, 0.0f };
             GL.Light(LightName.Light0, LightParameter.Diffuse, lightDiffuse);
             GL.Enable(EnableCap.Light0);*/

            GL.BindTexture(TextureTarget.Texture2D, current_texture);

            // Culling

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            GL.Color4(Color.White);

            GL.Begin(PrimitiveType.Triangles);

            foreach(Triangle currentTriangle in allTrianglesArray)
            {
                Vector3 triangleNormal = Vector3.Cross(vertList[currentTriangle.vertexIndices[1]] - vertList[currentTriangle.vertexIndices[0]], vertList[currentTriangle.vertexIndices[2]] - vertList[currentTriangle.vertexIndices[0]]);
                GL.Normal3(Vector3.Normalize(triangleNormal));
                Random rnd = new Random(currentTriangle.colParameter);

                Color triangleColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                for (int i = 0; i < 3; i++)
                {
                    
                    if(uvSetup)
                    {
                        GL.TexCoord2(uvArray[currentTriangle.vertexIndices[i]]);
                    } else
                    {
                        GL.Color3(triangleColor);
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

        public Vector2 ReturnTopLeft()
        {
            return topLeftBound;
        }

        public Vector2 ReturnBottomRight()
        {
            return bottomRightBound;
        }

        /// <summary>
        /// Splits the col into two different models 
        /// </summary>
        /// <param name="pointOne"></param>
        /// <param name="pointTwo"></param>
        /// <returns></returns>
        public Col SplitModelByLine(Vector3 pointOne, Vector3 pointTwo)
        {
            List<Triangle> leftTriangleList = new List<Triangle>();
            List<Triangle> rightTriangleList = new List<Triangle>();

            List<Vector3> addedVertList = new List<Vector3>();
            List<Triangle> addedTriangleList = new List<Triangle>();

            foreach (Triangle currentTriangle in allTrianglesArray)
            {
                bool? vertOneLeft = IsLeftOf(pointOne, pointTwo, vertList[currentTriangle.vertexIndices[0]]);
                bool? vertTwoLeft = IsLeftOf(pointOne, pointTwo, vertList[currentTriangle.vertexIndices[1]]);
                bool? vertThreeLeft = IsLeftOf(pointOne, pointTwo, vertList[currentTriangle.vertexIndices[2]]);

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
                            Triangle[] splitTriangles = SplitTriangle(0, 1, 2, currentTriangle, pointOne, pointTwo);
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
                            Triangle[] splitTriangles = SplitTriangle(1, 2, 0, currentTriangle, pointOne, pointTwo);
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
                            Triangle[] splitTriangles = SplitTriangle(2, 0, 1, currentTriangle, pointOne, pointTwo);
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
            allTrianglesArray = leftTriangleList.Concat(rightTriangleList).ToArray();

            foreach(Triangle tri in leftTriangleList)
            {
                tri.colParameter = 10;
            }
            foreach (Triangle tri in rightTriangleList)
            {
                tri.colParameter = 1120;
            }

            CleanUpVerts();

            return null;
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
            Triangle[] splitTriangles = new Triangle[3] { new Triangle(), new Triangle(), new Triangle() };

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
        /// Sets the uv coords of the cols vertexes projection style
        /// Bottom left is 0,0 in uv coordinates
        /// </summary>
        /// <returns></returns>
        private Vector2[] SetUVByProjection()
        {
            Vector2[] vertUVArray = new Vector2[vertList.Count];

            float modelWidth = topLeftBound.X - bottomRightBound.X;
            float modelHeight = topLeftBound.Y - bottomRightBound.Y;

            for (int i = 0; i < vertList.Count; i++)
            {
                if(vertList[i].Y == topLeftBound.Y)
                {
                    Console.WriteLine("Smallest Vert X");
                }
                float vertWidth = vertList[i].X - topLeftBound.X;// Left is positive x so doing left minus right
                float vertHeight = vertList[i].Z - bottomRightBound.Y; // Looking on the z axis

                float vertUVX = 1 - vertWidth / modelWidth;
                float vertUVY = 1 - vertHeight / modelHeight;

                vertUVArray[i] = new Vector2(vertUVX, vertUVY);
            }

            uvSetup = true;
            return vertUVArray;
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
            Console.WriteLine("Original Vert Count: " + vertList.Count);
            Console.WriteLine("Cleaned Vert Count: " + cleanedVerts.Count);
            vertList = new List<Vector3>(cleanedVerts);
        }

        /// <summary>
        /// Removes all triangles that wouldn't apply for converting to goop bmds (facing sideways or down)
        /// DOESN'T WORK ATM
        /// </summary>
        private void CleanupModelForGoop()
        {
            List<Triangle> newTriangleList = new List<Triangle>();
            foreach(Triangle currentTriangle in allTrianglesArray)
            {
                Vector3 triangleNormal = Vector3.Cross(vertList[currentTriangle.vertexIndices[1]] - vertList[currentTriangle.vertexIndices[0]], vertList[currentTriangle.vertexIndices[2]] - vertList[currentTriangle.vertexIndices[0]]);
                if(Vector3.CalculateAngle(Vector3.UnitY, triangleNormal) < 90) // If normal is within 90 degrees of facing upward, keep it
                {
                    newTriangleList.Add(currentTriangle);
                }
            }

            allTrianglesArray = newTriangleList.ToArray();
        }

        private int LoadTexture(string path, int quality = 0, bool repeat = true, bool flip_y = false)
        {
            Bitmap bitmap = new Bitmap(path);

            //Flip the image
            if (flip_y)
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            //Generate a new texture target in gl
            int texture = GL.GenTexture();

            //Will bind the texture newly/empty created with GL.GenTexture
            //All gl texture methods targeting Texture2D will relate to this texture
            GL.BindTexture(TextureTarget.Texture2D, texture);

            //The reason why your texture will show up glColor without setting these parameters is actually
            //TextureMinFilters fault as its default is NearestMipmapLinear but we have not established mipmapping
            //We are only using one texture at the moment since mipmapping is a collection of textures pre filtered
            //I'm assuming it stops after not having a collection to check.
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

            //Creates a definition of a texture object in opengl
            /* Parameters
             * Target - Since we are using a 2D image we specify the target Texture2D
             * MipMap Count / LOD - 0 as we are not using mipmapping at the moment
             * InternalFormat - The format of the gl texture, Rgba is a base format it works all around
             * Width;
             * Height;
             * Border - must be 0;
             * 
             * Format - this is the images format not gl's the format Bgra i believe is only language specific
             *          C# uses little-endian so you have ARGB on the image A 24 R 16 G 8 B, B is the lowest
             *          So it gets counted first, as with a language like Java it would be PixelFormat.Rgba
             *          since Java is big-endian default meaning A is counted first.
             *          but i could be wrong here it could be cpu specific :P
             *          
             * PixelType - The type we are using, eh in short UnsignedByte will just fill each 8 bit till the pixelformat is full
             *             (don't quote me on that...)
             *             you can be more specific and say for are RGBA to little-endian BGRA -> PixelType.UnsignedInt8888Reversed
             *             this will mimic are 32bit uint in little-endian.
             *             
             * Data - No data at the moment it will be written with TexSubImage2D
             */
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

            //Load the data from are loaded image into virtual memory so it can be read at runtime
            System.Drawing.Imaging.BitmapData bitmap_data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //Writes data to are texture target
            /* Target;
             * MipMap;
             * X Offset - Offset of the data on the x axis
             * Y Offset - Offset of the data on the y axis
             * Width;
             * Height;
             * Format;
             * Type;
             * Data - Now we have data from the loaded bitmap image we can load it into are texture data
             */
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, bitmap.Width, bitmap.Height, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap_data.Scan0);

            //Release from memory
            bitmap.UnlockBits(bitmap_data);

            //get rid of bitmap object its no longer needed in this method
            bitmap.Dispose();

            /*Binding to 0 is telling gl to use the default or null texture target
            *This is useful to remember as you may forget that a texture is targeted
            *And may overflow to functions that you dont necessarily want to
            *Say you bind a texture
            *
            * Bind(Texture);
            * DrawObject1();
            *                <-- Insert Bind(NewTexture) or Bind(0)
            * DrawObject2();
            * 
            * Object2 will use Texture if not set to 0 or another.
            */
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return texture;
        }
    }
}
