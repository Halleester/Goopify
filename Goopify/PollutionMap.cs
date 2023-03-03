using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goopify
{
    /// <summary>
    /// Pollution Map is a storage of all the pollution regions
    /// </summary>
    public class PollutionMap
    {
        public class Header
        {
            public UInt16 regionCount;
            public UInt32 dataOffset = 8;

            public Header() { }

            public Header(UInt16 regionNumber, UInt32 offset = 8)
            {
                regionCount = regionNumber;
                dataOffset = offset;
            }
        }
        // Data needed to make up a pollution map
        public Header pollutionMapHeader;
        public List<PollutionRegion> pollutionRegions = new List<PollutionRegion>();
        


        public PollutionMap(Stream ympStream)
        {
            BinaryReaderBE binaryReader = new BinaryReaderBE(ympStream); // Convert stream to BigEndian Reader (So we can get values from the file data)

            // Converts the header
            pollutionMapHeader = new Header();
            pollutionMapHeader.regionCount = binaryReader.ReadUInt16();
            binaryReader.ReadBytes(2); // Padding
            pollutionMapHeader.dataOffset = binaryReader.ReadUInt32();

            if(pollutionMapHeader.dataOffset != 8)
            {
                Console.WriteLine("Data offset for ymp is not 8 bytes!");
                return;
            }
            // Converts the Pollution Regions
            binaryReader.BaseStream.Seek(pollutionMapHeader.dataOffset, SeekOrigin.Begin); // Goto vertex offset
            for (int i = 0; i < pollutionMapHeader.regionCount; i++)
            {
                PollutionRegion loadedRegion = new PollutionRegion(ympStream); // Creates a goop region from the current point we're at in the file stream
                pollutionRegions.Add(loadedRegion);
            }
        }

        public PollutionMap(List<PollutionRegion> regions)
        {
            pollutionRegions = regions;
            pollutionMapHeader = new Header((UInt16)regions.Count);
        }

        /// <summary>
        /// Used to save the heightmaps of each region as a black and white png
        /// </summary>
        public void ExportHeightmaps(string path, string ympName)
        {
            // Check if there's regions to export
            if(pollutionRegions == null || pollutionRegions.Count <= 0)
            {
                Console.WriteLine("No pollution regions to export!");
                return;
            }

            // Check if file exists and warn if overwriting first
            string nonOverwriteAddition = "";
            FileInfo heightmapExistingFile = new FileInfo(path + "\\" + ympName + "0" + ".png");
            if (heightmapExistingFile.Exists)
            {
                DialogResult result = MessageBox.Show("There already heightmaps for \"" + ympName + "\", do you want to overwrite the existing ones?", "File Name Conflict", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Yes:
                        break;
                    case DialogResult.No:
                        // Change the path name
                        int additionAmt = 0;
                        do
                        {
                            additionAmt++;
                            nonOverwriteAddition = "(" + additionAmt + ")";
                            heightmapExistingFile = new FileInfo(path + "\\" + ympName + "0" + nonOverwriteAddition + ".png");
                        } while (heightmapExistingFile.Exists);

                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }

            // Save each region in the map
            for (int i = 0; i < pollutionRegions.Count; i++)
            {
                // Modify the file name to add numbers on after the first region
                string additionalPart = "\\" + ympName + i;
                pollutionRegions[i].heightMap.Save(path + additionalPart + nonOverwriteAddition + ".png", ImageFormat.Png);
            }
        }

        public void ExportInfo(string path, string ympName)
        {
            // Check if file exists and warn if overwriting first
            string nonOverwriteAddition = "";
            FileInfo heightmapExistingFile = new FileInfo(path + "\\" + ympName + ".txt");
            if (heightmapExistingFile.Exists)
            {
                DialogResult result = MessageBox.Show("There's already a dump for \"" + ympName + "\", do you want to overwrite the existing one?", "File Name Conflict", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Yes: // Delete the file so the streamwriter doesn't keep writing to the same file
                        File.Delete(path + "\\" + ympName + ".txt");
                        break;
                    case DialogResult.No: // Add "(i)" to the end of the path until there's not a conflicting file
                        int additionAmt = 0;
                        do {
                            additionAmt++;
                            nonOverwriteAddition = "(" + additionAmt + ")";
                            heightmapExistingFile = new FileInfo(path + "\\" + ympName + nonOverwriteAddition + ".png");
                        } while (heightmapExistingFile.Exists);

                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }

            // Writes the Pollution map data to a text file
            using (var txtStream = new StreamWriter(path + "\\" + ympName + nonOverwriteAddition + ".txt", true))
            {
                // Write the header info
                txtStream.WriteLine("# Dump of " + ympName + ".ymp, " + DateTime.Now.ToString("MM/dd/yyyy"));
                txtStream.WriteLine();

                txtStream.WriteLine("HEADER");
                txtStream.WriteLine("\tRegion Count (UInt16): " + pollutionMapHeader.regionCount);
                txtStream.WriteLine("\tData Offset (UInt32): " + pollutionMapHeader.dataOffset.ToString("X8"));

                // Write region info
                for (int i = 0; i < pollutionRegions.Count; i++)
                {
                    txtStream.WriteLine();
                    txtStream.WriteLine("REGION " + i);
                    txtStream.WriteLine("\tPollution Type (UInt16): " + pollutionRegions[i].pollutionType);
                    txtStream.WriteLine("\tUnknown 1 (UInt16): " + pollutionRegions[i].unknown1);
                    txtStream.WriteLine("\tWall Layer Type (UInt16): " + pollutionRegions[i].wallLayerType);
                    txtStream.WriteLine("\tUnknown 2 (UInt16): " + pollutionRegions[i].unknown2);
                    txtStream.WriteLine("\tRegion Height (float): " + pollutionRegions[i].pointYPos);
                    txtStream.WriteLine("\tImage World Scale (float): " + pollutionRegions[i].texWorldScale);
                    txtStream.WriteLine("\tStart X Position (float): " + pollutionRegions[i].startXPos);
                    txtStream.WriteLine("\tStart Z Position (float): " + pollutionRegions[i].startZPos);
                    txtStream.WriteLine("\tEnd X Position (float): " + pollutionRegions[i].endXPos);
                    txtStream.WriteLine("\tEnd Z Position (float): " + pollutionRegions[i].endZPos);
                    txtStream.WriteLine("\tImage Width Base (UInt16): " + pollutionRegions[i].imageWidthBase);
                    txtStream.WriteLine("\tImage Height Base (UInt16): " + pollutionRegions[i].imageLengthBase);
                    txtStream.WriteLine("\tUnknown 3 (UInt32): " + pollutionRegions[i].unknown3);
                    txtStream.WriteLine("\tHeight Map Offset (UInt32): " + pollutionRegions[i].heightMapOffset.ToString("X8"));
                }

                txtStream.Close();
            }      
        }

        private byte[] PADDING_BYTES = Encoding.ASCII.GetBytes("This is padding to align");
        // Set variables in the editor window and then read it in here to create the info and regions
        public void CreateYmapFile(string path)
        {
            Stream writeStream = File.Create(path);
            BinaryWriterBE binaryWriter = new BinaryWriterBE(writeStream);

            // Writes the header (NOT SURE ABOUT THIS)
            binaryWriter.Write(pollutionMapHeader.regionCount);
            binaryWriter.Write(new byte[2]); // Padding
            binaryWriter.Write(pollutionMapHeader.dataOffset); // TODO: Need to set?
            for(int i = 0; i < pollutionMapHeader.dataOffset - 8; i++) // Extra padding
            {
                var pos = i % PADDING_BYTES.Length;
                binaryWriter.Write(new byte[] { PADDING_BYTES[pos] });
            }
            var headerEnd = binaryWriter.BaseStream.Position;

            // Create 44 bytes for each region
            foreach (PollutionRegion region in pollutionRegions)
            {
                binaryWriter.Write(new byte[44]);
            }

            // Write heightmaps
            List<long> heightmapOffsets = new List<long>();
            foreach (PollutionRegion region in pollutionRegions)
            {
                WritePadding(writeStream);
                heightmapOffsets.Add(writeStream.Position);
                region.WriteHeightmap(writeStream);
            }

            // Go back and write to 44 byte region
            binaryWriter.Seek((int)headerEnd, SeekOrigin.Begin);
            for(int i = 0; i < pollutionRegions.Count; i++)
            {
                pollutionRegions[i].WriteRegion(writeStream, heightmapOffsets[i]);
            }

            // Done, so close write stream
            writeStream.Close();
        }

        public void WritePadding(Stream writeStream, int multiple = 0x20)
        {
            BinaryWriterBE binaryWriter = new BinaryWriterBE(writeStream);

            var nextAligned = (writeStream.Position + (multiple - 1)) & ~(multiple - 1);
            var diff = nextAligned - writeStream.Position;
            for(int i = 0; i < diff; i++)
            {
                var pos = i % PADDING_BYTES.Length;
                binaryWriter.Write(new byte[] { PADDING_BYTES[pos] });
            }
        }
    }
}
