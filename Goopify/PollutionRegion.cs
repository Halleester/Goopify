using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Goopify.EditorWindow;

namespace Goopify
{
    public class PollutionRegion
    {
        // Info needed
        public UInt16 pollutionType;
        public UInt16 unknown1;
        public UInt16 wallLayerType;
        public UInt16 unknown2;
        public float pointYPos;
        public float texWorldScale; // Position Units per pixel in heightmap
        public float startXPos;
        public float startZPos;
        public float endXPos;
        public float endZPos;
        public UInt16 imageWidthBase; // Put to power of 2 for correct value
        public UInt16 imageLengthBase; // Put to power of 2 for correct value
        public UInt32 unknown3; // Only filled in bianco heightmaps
        public UInt32 heightMapOffset;

        public Vector3 startPoint;
        public Vector3 endPoint;
        public int heightMapWidth;
        public int heightMapLength;
        public Bitmap heightMap;

        public PollutionRegion(Stream ympStream)
        {
            BinaryReaderBE binaryReader = new BinaryReaderBE(ympStream); // Convert stream to BigEndian Reader (So we can get values from the file data)

            pollutionType = binaryReader.ReadUInt16();
            unknown1 = binaryReader.ReadUInt16();
            wallLayerType = binaryReader.ReadUInt16();
            unknown2 = binaryReader.ReadUInt16();

            pointYPos = binaryReader.ReadSingle(); // is float
            texWorldScale = binaryReader.ReadSingle();
            startXPos = binaryReader.ReadSingle();
            startZPos = binaryReader.ReadSingle();
            endXPos = binaryReader.ReadSingle();
            endZPos = binaryReader.ReadSingle();
            // Vector 3 versions of the start/end positions
            startPoint = new Vector3(startXPos, pointYPos, startZPos);
            endPoint = new Vector3(endXPos, pointYPos, endZPos);

            imageWidthBase = binaryReader.ReadUInt16();
            imageLengthBase = binaryReader.ReadUInt16();
            // Actual height/width values we need
            heightMapWidth = (int)Math.Pow(2, imageWidthBase); //2^width we get from stream
            heightMapLength = (int)Math.Pow(2, imageLengthBase);

            unknown3 = binaryReader.ReadUInt32();
            heightMapOffset = binaryReader.ReadUInt32();

            long regionStreamPos = binaryReader.BaseStream.Position; // Saves our position in the stream
            binaryReader.BaseStream.Seek(heightMapOffset, SeekOrigin.Begin); // Goto heightmap offset

            // Loads the heightmap image
            heightMap = new Bitmap(heightMapWidth, heightMapLength);

            int blocksHorizontal = heightMapWidth / 8;
            int blocksVertical = heightMapLength / 4;
            // Sets each pixel in the bitmap
            for(int iy = 0; iy < blocksVertical; iy++)
            {
                for (int ix = 0; ix < blocksHorizontal; ix++)
                {
                    byte[] block = binaryReader.ReadBytes(8 * 4);
                    for(int y = 0; y < 4; y++)
                    {
                        for(int x = 0; x < 8; x++)
                        {
                            byte val = block[x + y * 8];
                            int imgx = ix * 8 + x;
                            int imgy = iy * 4 + y;

                            if(imgx >= heightMapWidth || imgy >= heightMapLength) { continue; } // skip blocks out of the image?
                            Color pixelColor = Color.FromArgb(val, val, val);
                            heightMap.SetPixel(imgx, imgy, pixelColor);
                        }
                    }
                }
            }

            binaryReader.BaseStream.Seek(regionStreamPos, SeekOrigin.Begin); // Goto region offset again
        }

        // Give us the model and texture resolution and we create the heightmap
        public PollutionRegion(GoopRegionBox regionBox)
        {
            // Set the position info
            startPoint = regionBox.GetCornerPos(Corner.TopLeft);
            endPoint = regionBox.GetCornerPos(Corner.BottomRight);
            startXPos = startPoint.X;
            startZPos = startPoint.Z;
            endXPos = endPoint.X;
            endZPos = endPoint.Z;

            pointYPos = regionBox.height;

            // heightmap info
            texWorldScale = regionBox.texWorldScale; // controls the scale of image to world position, how we make a 8192x8192 render to a heightmap of 256x256
            heightMapWidth = (int)((endXPos - startXPos) / texWorldScale);
            heightMapLength = (int)((endZPos - startZPos) / texWorldScale);
            imageWidthBase = (ushort)Math.Log(heightMapWidth, 2);
            imageLengthBase = (ushort)Math.Log(heightMapLength, 2);

            wallLayerType = (ushort)regionBox.layerType;
            pollutionType = 2; // Update this later through the settings menu

            unknown1 = 0;
            unknown2 = 0;
            unknown3 = 0;

            
        }

        public void WriteHeightmap(Stream writeStream)
        {
            if(heightMap.Width % 8 != 0 || heightMap.Height % 4 != 0)
            {
                MessageBox.Show("Heightmap is invalid size!", "Heightmap Writing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            BinaryWriterBE binaryWriter = new BinaryWriterBE(writeStream);
            int blocksHorizontal = (int)(heightMap.Width / 8);
            int blocksVertical = (int)(heightMap.Height / 4);

            for(int iy = 0; iy < blocksVertical; iy++) {
                for(int ix = 0; ix < blocksHorizontal; ix++) {
                    int blockX = ix * 8;
                    int blockY = iy * 4;
                    for(int y = 0; y < 4; y++) {
                        for(int x = 0; x < 8; x++) {
                            binaryWriter.Write(Convert.ToByte(heightMap.GetPixel(blockX + x, blockY + y).R));
                        }
                    }
                }
            }

            /*blocks_horizontal = int(hmap.width / 8.0)
            blocks_vertical = int(hmap.height / 4.0)


            for iy in range(blocks_vertical):
                for ix in range(blocks_horizontal):
                    block_x = ix * 8
                    block_y = iy * 4

                    for y in range(4):
                        for x in range(8):
                            write_uint8(f, hmap[block_x + x, block_y + y])*/
        }

        public void WriteRegion(Stream writeStream, long heightmapOffset)
        {
            BinaryWriterBE binaryWriter = new BinaryWriterBE(writeStream);

            binaryWriter.Write(pollutionType); //16
            binaryWriter.Write(unknown1); //16
            binaryWriter.Write(wallLayerType); //16
            binaryWriter.Write(unknown2); //16
            binaryWriter.Write(pointYPos); // float
            binaryWriter.Write(texWorldScale); // float
            binaryWriter.Write(startXPos); // float
            binaryWriter.Write(startZPos); // float
            binaryWriter.Write(endXPos); // float
            binaryWriter.Write(endZPos); // float

            int widthPower = (int)(Math.Log(heightMapWidth) / Math.Log(2));
            int heightPower = (int)(Math.Log(heightMapLength) / Math.Log(2));

            if (widthPower % 1 != 0 || heightPower % 1 != 0)
                Console.WriteLine("Height or width is wrong");

            binaryWriter.Write((short)widthPower); //16
            binaryWriter.Write((short)heightPower); //16
            binaryWriter.Write(unknown3); //32
            binaryWriter.Write((int)heightmapOffset); //32
        }
    }
}
