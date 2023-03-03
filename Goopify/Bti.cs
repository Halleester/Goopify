/*
    Copyright 2016-2017 shibboleet, miluaces
    This file is part of DouBOL Dash.
    DouBOL Dash is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.
    DouBOL Dash is distributed in the hope that it will be useful, but WITHOUT ANY
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
    You should have received a copy of the GNU General Public License along
    with DouBOL Dash. If not, see http://www.gnu.org/licenses/.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.IO;
using Goopify;

namespace Goopify
{
    public class Bti
    {

        /* Reads BTI file and loads texture into OpenGL */
        public static Bitmap ReadBtiToBitmap(Stream btiStream)
        {
            BinaryReaderBE binaryReader = new BinaryReaderBE(btiStream);
            /* Header */
            byte format = binaryReader.ReadByte();
            byte unknown1 = binaryReader.ReadByte();
            UInt16 width = binaryReader.ReadUInt16();
            UInt16 height = binaryReader.ReadUInt16();
            UInt16 unknown2 = binaryReader.ReadUInt16();
            byte unknown3 = binaryReader.ReadByte();
            byte paletteFormat = binaryReader.ReadByte();
            UInt16 paletteCount = binaryReader.ReadUInt16();
            UInt32 paletteOffset = binaryReader.ReadUInt32();
            UInt32 unknown4 = binaryReader.ReadUInt32();
            UInt16 unknown5 = binaryReader.ReadUInt16();
            UInt16 unknown6 = binaryReader.ReadUInt16();
            byte mipmapCount = binaryReader.ReadByte();
            byte unknown7 = binaryReader.ReadByte();
            UInt16 unknown8 = binaryReader.ReadUInt16();
            UInt32 dataOffset = binaryReader.ReadUInt32();

            UInt16[] palette = new UInt16[paletteCount];
            if (paletteCount != 0)
            {
                if (paletteFormat != 0 && paletteFormat != 1 && paletteFormat != 2)
                    throw new NotImplementedException();

                binaryReader.BaseStream.Seek(paletteOffset, System.IO.SeekOrigin.Begin);
                for (int i = 0; i < paletteCount; i++)
                    palette[i] = binaryReader.ReadUInt16();
            }

            TextureWrapMode[] wrapmodes = { TextureWrapMode.ClampToEdge, TextureWrapMode.Repeat, TextureWrapMode.MirroredRepeat };
            TextureMinFilter[] minfilters = { TextureMinFilter.Nearest, TextureMinFilter.Linear,
                                                TextureMinFilter.NearestMipmapNearest, TextureMinFilter.LinearMipmapNearest,
                                                TextureMinFilter.NearestMipmapLinear, TextureMinFilter.LinearMipmapLinear };
            TextureMagFilter[] magfilters = { TextureMagFilter.Nearest, TextureMagFilter.Linear,
                                                TextureMagFilter.Nearest, TextureMagFilter.Linear,
                                                TextureMagFilter.Nearest, TextureMagFilter.Linear };

            int texture = GL.GenTexture();

            Bitmap bmimg = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bmd = bmimg.LockBits(new Rectangle(0, 0, bmimg.Width, bmimg.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmimg.PixelFormat);
            const int FORMATSIZE = 4;

            unsafe
            {
                switch (format)
                {
                    case 0: // I4
                        {
                            for (int by = 0; by < height; by += 8)
                            {
                                for (int bx = 0; bx < width; bx += 8)
                                {
                                    for (int y = 0; y < 8; y++)
                                    {
                                        for (int x = 0; x < 8; x += 2)
                                        {
                                            byte b = binaryReader.ReadByte();

                                            byte* cur = (byte*)bmd.Scan0 + ((by + y) * bmd.Stride) + ((bx + x) * FORMATSIZE);

                                            byte i1 = (byte)((b & 0xF0) | (b >> 4));
                                            byte i2 = (byte)((b << 4) | (b & 0x0F));
                                            *(cur++) = i1;
                                            *(cur++) = i1;
                                            *(cur++) = i1;
                                            *(cur++) = i1;

                                            *(cur++) = i2;
                                            *(cur++) = i2;
                                            *(cur++) = i2;
                                            *(cur++) = i2;
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case 1: // I8
                        {
                            for (int by = 0; by < height; by += 4)
                            {
                                for (int bx = 0; bx < width; bx += 8)
                                {
                                    for (int y = 0; y < 4; y++)
                                    {
                                        for (int x = 0; x < 8; x++)
                                        {
                                            byte b = binaryReader.ReadByte();
                                            byte* cur = (byte*)bmd.Scan0 + ((by + y) * bmd.Stride) + ((bx + x) * FORMATSIZE);

                                            *(cur++) = b;
                                            *(cur++) = b;
                                            *(cur++) = b;
                                            *(cur++) = b;
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case 2: // I4A4
                        {
                            for (int by = 0; by < height; by += 4)
                            {
                                for (int bx = 0; bx < width; bx += 8)
                                {
                                    for (int y = 0; y < 4; y++)
                                    {
                                        for (int x = 0; x < 8; x++)
                                        {
                                            byte b = binaryReader.ReadByte();
                                            byte* cur = (byte*)bmd.Scan0 + ((by + y) * bmd.Stride) + ((bx + x) * FORMATSIZE);

                                            byte i = (byte)((b << 4) | (b & 0x0F));
                                            byte a = (byte)((b & 0xF0) | (b >> 4));

                                            *(cur++) = i;
                                            *(cur++) = i;
                                            *(cur++) = i;
                                            *(cur++) = a;
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case 3: // I8A8
                        {
                            for (int by = 0; by < height; by += 4)
                            {
                                for (int bx = 0; bx < width; bx += 4)
                                {
                                    for (int y = 0; y < 4; y++)
                                    {
                                        for (int x = 0; x < 4; x++)
                                        {
                                            byte a = binaryReader.ReadByte();
                                            byte i = binaryReader.ReadByte();

                                            byte* cur = (byte*)bmd.Scan0 + ((by + y) * bmd.Stride) + ((bx + x) * FORMATSIZE);

                                            *(cur++) = i;
                                            *(cur++) = i;
                                            *(cur++) = i;
                                            *(cur++) = a;
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case 4: // RGB565
                        {
                            for (int by = 0; by < height; by += 4)
                            {
                                for (int bx = 0; bx < width; bx += 4)
                                {
                                    for (int y = 0; y < 4; y++)
                                    {
                                        for (int x = 0; x < 4; x++)
                                        {
                                            ushort col = binaryReader.ReadUInt16();

                                            byte* cur = (byte*)bmd.Scan0 + ((by + y) * bmd.Stride) + ((bx + x) * FORMATSIZE);

                                            byte b = (byte)(((col & 0x001F) << 3) | ((col & 0x001F) >> 2));
                                            byte g = (byte)(((col & 0x07E0) >> 3) | ((col & 0x07E0) >> 8));
                                            byte r = (byte)(((col & 0xF800) >> 8) | ((col & 0xF800) >> 13));

                                            *(cur++) = b;
                                            *(cur++) = g;
                                            *(cur++) = r;
                                            *(cur++) = 255;
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    //RGB5A3 Added 1/20/15
                    case 5:
                        {
                            for (int by = 0; by < height; by += 4)
                            {
                                for (int bx = 0; bx < width; bx += 4)
                                {
                                    for (int y = 0; y < 4; y++)
                                    {
                                        for (int x = 0; x < 4; x++)
                                        {
                                            byte r, g, b, a;
                                            ushort srcPixel = binaryReader.ReadUInt16();
                                            byte* cur = (byte*)bmd.Scan0 + ((by + y) * bmd.Stride) + ((bx + x) * FORMATSIZE);
                                            if ((srcPixel & 0x8000) == 0x8000)
                                            {
                                                r = (byte)((srcPixel & 0x7c00) >> 10);
                                                r = (byte)((r << (8 - 5)) | (r >> (10 - 8)));

                                                g = (byte)((srcPixel & 0x3e0) >> 5);
                                                g = (byte)((g << (8 - 5)) | (g >> (10 - 8)));

                                                b = (byte)(srcPixel & 0x1f);
                                                b = (byte)((b << (8 - 5)) | (b >> (10 - 8)));

                                                a = 0xff;
                                            }
                                            else //a3rgb4
                                            {
                                                r = (byte)((srcPixel & 0x7000) >> 12);
                                                r = (byte)((r << (8 - 3)) | (r << (8 - 6)) | (r >> (9 - 8)));

                                                g = (byte)((srcPixel & 0xf00) >> 8);
                                                g = (byte)((g << (8 - 4)) | g);

                                                b = (byte)((srcPixel & 0xf0) >> 4);
                                                b = (byte)((b << (8 - 4)) | b);

                                                a = (byte)(srcPixel & 0xf);
                                                a = (byte)((a << (8 - 4)) | a);
                                            }

                                            *(cur++) = b;
                                            *(cur++) = g;
                                            *(cur++) = r;
                                            *(cur++) = a;
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    //ARGB8 Added 1/20/15
                    case 6:
                        {
                            for (int by = 0; by < height; by += 4)
                            {
                                for (int bx = 0; bx < width; bx += 4)
                                {
                                    for (int y = 0; y < 4; y++)
                                    {
                                        for (int x = 0; x < 4; x++)
                                        {
                                            if (x + bx < width && y + by < height)
                                            {
                                                byte* cur = (byte*)bmd.Scan0 + ((by + y) * bmd.Stride) + ((bx + x) * FORMATSIZE) + 2;

                                                byte a = binaryReader.ReadByte();
                                                byte r = binaryReader.ReadByte();

                                                *(cur++) = r;
                                                *(cur++) = a;
                                            }
                                        }
                                    }
                                    for (int y = 0; y < 4; y++)
                                    {
                                        for (int x = 0; x < 4; x++)
                                        {
                                            if (x + bx < width && y + by < height)
                                            {
                                                byte* cur = (byte*)bmd.Scan0 + ((by + y) * bmd.Stride) + ((bx + x) * FORMATSIZE);

                                                byte g = binaryReader.ReadByte();
                                                byte b = binaryReader.ReadByte();

                                                *(cur++) = b;
                                                *(cur++) = g;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case 14: // DXT1
                        {
                            for (int by = 0; by < height; by += 8)
                            {
                                for (int bx = 0; bx < width; bx += 8)
                                {
                                    for (int sby = 0; sby < 8; sby += 4)
                                    {
                                        for (int sbx = 0; sbx < 8; sbx += 4)
                                        {
                                            ushort c1 = binaryReader.ReadUInt16();
                                            ushort c2 = binaryReader.ReadUInt16();
                                            uint block = binaryReader.ReadUInt32();

                                            byte r1 = (byte)((c1 & 0xF800) >> 8);
                                            byte g1 = (byte)((c1 & 0x07E0) >> 3);
                                            byte b1 = (byte)((c1 & 0x001F) << 3);
                                            byte r2 = (byte)((c2 & 0xF800) >> 8);
                                            byte g2 = (byte)((c2 & 0x07E0) >> 3);
                                            byte b2 = (byte)((c2 & 0x001F) << 3);

                                            byte[,] colors = new byte[4, 4];
                                            colors[0, 0] = 255; colors[0, 1] = r1; colors[0, 2] = g1; colors[0, 3] = b1;
                                            colors[1, 0] = 255; colors[1, 1] = r2; colors[1, 2] = g2; colors[1, 3] = b2;
                                            if (c1 > c2)
                                            {
                                                int r3 = ((r1 << 1) + r2) / 3;
                                                int g3 = ((g1 << 1) + g2) / 3;
                                                int b3 = ((b1 << 1) + b2) / 3;

                                                int r4 = (r1 + (r2 << 1)) / 3;
                                                int g4 = (g1 + (g2 << 1)) / 3;
                                                int b4 = (b1 + (b2 << 1)) / 3;

                                                colors[2, 0] = 255; colors[2, 1] = (byte)r3; colors[2, 2] = (byte)g3; colors[2, 3] = (byte)b3;
                                                colors[3, 0] = 255; colors[3, 1] = (byte)r4; colors[3, 2] = (byte)g4; colors[3, 3] = (byte)b4;
                                            }
                                            else
                                            {
                                                colors[2, 0] = 255;
                                                colors[2, 1] = (byte)((r1 + r2) / 2);
                                                colors[2, 2] = (byte)((g1 + g2) / 2);
                                                colors[2, 3] = (byte)((b1 + b2) / 2);
                                                colors[3, 0] = 0; colors[3, 1] = r2; colors[3, 2] = g2; colors[3, 3] = b2;
                                            }

                                            for (int y = 0; y < 4; y++)
                                            {
                                                for (int x = 0; x < 4; x++)
                                                {
                                                    int c = (int)(block >> 30);
                                                    byte* cur = (byte*)bmd.Scan0 + ((by + sby + y) * bmd.Stride) + ((bx + sbx + x) * FORMATSIZE);

                                                    *(cur++) = (byte)(colors[c, 3] | (colors[c, 3] >> 5));
                                                    *(cur++) = (byte)(colors[c, 2] | (colors[c, 2] >> 5));
                                                    *(cur++) = (byte)(colors[c, 1] | (colors[c, 1] >> 5));
                                                    *(cur++) = colors[c, 0];
                                                    block <<= 2;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    default: throw new NotImplementedException("Bmd: unsupported texture format " + format.ToString());
                }
            }

            bmimg.UnlockBits(bmd);

            return bmimg;
        }
    }
}
