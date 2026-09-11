
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;

namespace System.Windows.Media.Imaging;

internal static class PngEncoder
{
    private const int _ADLER32_BASE = 65521;
    private static readonly byte[] _HEADER = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] _IHDR = [(byte)'I', (byte)'H', (byte)'D', (byte)'R'];
    private static readonly byte[] _GAMA = [(byte)'g', (byte)'A', (byte)'M', (byte)'A'];
    private static readonly byte[] _IDAT = [(byte)'I', (byte)'D', (byte)'A', (byte)'T'];
    private static readonly byte[] _IEND = [(byte)'I', (byte)'E', (byte)'N', (byte)'D'];
    private static readonly byte[] _4BYTEDATA = [0, 0, 0, 0];
    private static readonly byte[] _ARGB = [0, 0, 0, 0, 0, 0, 0, 0, 8, 6, 0, 0, 0];
    private static readonly uint[] _crcTable = MakeCRCTable();

    public static int Encode(int[] pixels, int width, int height, bool swapRedBlue, byte[] destination)
    {
        var encoder = new Encoder(pixels, width, height, swapRedBlue);
        return encoder.Encode(destination);
    }

    internal static int GetRequiredBufferSize(int width, int height)
    {
        // Conservative zlib/deflate bound (deflateBound) including 2-byte header + Adler-32.
        long n = ((long)width * 4 + 1) * height;
        long fixedBound = n + ((n + 7) >> 3) + ((n + 63) >> 6) + 5;
        long storedBound = n + (n >> 5) + (n >> 7) + (n >> 11) + 7;
        long zlibBound = Math.Max(fixedBound, storedBound) + 6;

        // signature + IHDR + gAMA + IDAT framing + IEND + zlib payload
        long pngBound = 73 + zlibBound;

        ArgumentOutOfRangeException.ThrowIfGreaterThan(pngBound, int.MaxValue);

        return (int)pngBound;
    }

    private static void WriteBytes(byte[] destination, ref int offset, byte[] data)
    {
        Buffer.BlockCopy(data, 0, destination, offset, data.Length);
        offset += data.Length;
    }

    private static void WriteUInt32BigEndian(byte[] destination, ref int offset, uint value)
    {
        BinaryPrimitives.WriteUInt32BigEndian(destination.AsSpan(offset), value);
        offset += sizeof(uint);
    }

    private static void WriteChunk(byte[] destination, ref int offset, byte[] type, byte[] data)
    {
        WriteUInt32BigEndian(destination, ref offset, (uint)data.Length);
        int crcStart = offset;
        WriteBytes(destination, ref offset, type);
        WriteBytes(destination, ref offset, data);
        WriteUInt32BigEndian(destination, ref offset, GetCRC(destination, crcStart, type.Length + data.Length));
    }

    private static uint[] MakeCRCTable()
    {
        uint[] crcTable = new uint[256];

        uint c;

        for (int n = 0; n < 256; n++)
        {
            c = (uint)n;
            for (int k = 0; k < 8; k++)
            {
                if ((c & 0x00000001) > 0)
                {
                    c = 0xEDB88320 ^ c >> 1;
                }
                else
                {
                    c = c >> 1;
                }
            }

            crcTable[n] = c;
        }

        return crcTable;
    }

    private static uint UpdateCRC(uint crc, byte[] buf, int offset, int len)
    {
        uint c = crc;

        int end = offset + len;
        for (int n = offset; n < end; n++) c = _crcTable[(c ^ buf[n]) & 0xFF] ^ c >> 8;

        return c;
    }

    /* Return the CRC of the bytes buf[offset..offset+len-1]. */
    private static uint GetCRC(byte[] buf, int offset, int len)
    {
        return UpdateCRC(0xFFFFFFFF, buf, offset, len) ^ 0xFFFFFFFF;
    }

    private static void UpdateAdler32(ref uint s1, ref uint s2, byte value)
    {
        s1 = (s1 + value) % _ADLER32_BASE;
        s2 = (s2 + s1) % _ADLER32_BASE;
    }

    private readonly struct Encoder
    {
        private readonly int[] _pixels;
        private readonly int _width;
        private readonly int _height;
        private readonly bool _swapRedBlue;

        public Encoder(int[] pixels, int width, int height, bool swapRedBlue)
        {
            _pixels = pixels;
            _width = width;
            _height = height;
            _swapRedBlue = swapRedBlue;
        }

        public int Encode(byte[] destination)
        {
            // See http://www.libpng.org/pub/png//spec/1.2/PNG-Chunks.html
            // See http://www.libpng.org/pub/png/book/chapter08.html#png.ch08.div.4
            // See http://www.gzip.org/zlib/rfc-zlib.html (ZLIB format)
            // See ftp://ftp.uu.net/pub/archiving/zip/doc/rfc1951.txt (ZLIB compression format)

            int offset = 0;

            // Write PNG header
            WriteBytes(destination, ref offset, _HEADER);

            // Write IHDR
            //  Width:              4 bytes
            //  Height:             4 bytes
            //  Bit depth:          1 byte
            //  Color type:         1 byte
            //  Compression method: 1 byte
            //  Filter method:      1 byte
            //  Interlace method:   1 byte

            int ihdrOffset = 0;
            WriteUInt32BigEndian(_ARGB, ref ihdrOffset, (uint)_width);
            WriteUInt32BigEndian(_ARGB, ref ihdrOffset, (uint)_height);

            // Write IHDR chunk
            WriteChunk(destination, ref offset, _IHDR, _ARGB);

            // Set gamma
            int gammaOffset = 0;
            WriteUInt32BigEndian(_4BYTEDATA, ref gammaOffset, (uint)Math.Round(1.0 / 2.2 * 100000));

            // Write gAMA chunk
            WriteChunk(destination, ref offset, _GAMA, _4BYTEDATA);

            // Write IDAT chunk
            // ZLIB wrapper around DEFLATE (RFC 1950 / 1951):
            //
            // CMF Byte: 78
            //  CINFO = 7 (32K window size)
            //  CM = 8 (deflate compression)
            // FLG Byte: DA
            //  FLEVEL = 3 (bits 6 and 7 - informational)
            //  FDICT = 0 (bit 5, 0 - no preset dictionary)
            //  FCHCK = 26 (bits 0-4 - ensure CMF*256+FLG / 31 has no remainder)
            // Compressed scanlines: 0 [RGBA] [RGBA] ...
            // ADLER32 of the uncompressed scanlines

            int idatLengthPosition = offset;
            offset += sizeof(uint);
            int idatCrcStart = offset;
            WriteBytes(destination, ref offset, _IDAT);

            destination[offset++] = 0x78;
            destination[offset++] = 0xDA;

            uint s1 = 1;
            uint s2 = 0;
            int deflateStart = offset;
            using (var destStream = new MemoryStream(destination, deflateStart, destination.Length - deflateStart, writable: true, publiclyVisible: true))
            {
                using (var deflate = new DeflateStream(destStream, CompressionLevel.Fastest, leaveOpen: true))
                {
                    WriteScanlines(deflate, ref s1, ref s2);
                }

                offset += (int)destStream.Position;
            }

            WriteUInt32BigEndian(destination, ref offset, (s2 << 16) + s1);

            int lengthOffset = idatLengthPosition;
            WriteUInt32BigEndian(destination, ref lengthOffset, (uint)(offset - idatCrcStart - 4));
            WriteUInt32BigEndian(destination, ref offset, GetCRC(destination, idatCrcStart, offset - idatCrcStart));

            // Write IEND chunk
            WriteChunk(destination, ref offset, _IEND, []);

            return offset;
        }

        private void WriteScanlines(Stream destination, ref uint s1, ref uint s2)
        {
            if (_height == 0)
            {
                return;
            }

            int scanlineLength = _width * 4 + 1;
            byte[] scanline = ArrayPool<byte>.Shared.Rent(scanlineLength);
            try
            {
                int pixelIndex = 0;
                for (int y = 0; y < _height; y++)
                {
                    int offset = 0;
                    scanline[offset++] = 0;
                    UpdateAdler32(ref s1, ref s2, 0);

                    for (int x = 0; x < _width; x++)
                    {
                        WritePixel(scanline, ref offset, ref s1, ref s2, _pixels[pixelIndex++]);
                    }

                    destination.Write(scanline, 0, scanlineLength);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(scanline);
            }
        }

        private void WritePixel(byte[] destination, ref int offset, ref uint s1, ref uint s2, int pixel)
        {
            byte b0, b1, b2, b3;

            if (_swapRedBlue)
            {
                b0 = (byte)(pixel >> 16);
                b1 = (byte)(pixel >> 8);
                b2 = (byte)pixel;
                b3 = (byte)(pixel >> 24);
            }
            else
            {
                b0 = (byte)pixel;
                b1 = (byte)(pixel >> 8);
                b2 = (byte)(pixel >> 16);
                b3 = (byte)(pixel >> 24);
            }

            destination[offset++] = b0;
            destination[offset++] = b1;
            destination[offset++] = b2;
            destination[offset++] = b3;

            UpdateAdler32(ref s1, ref s2, b0);
            UpdateAdler32(ref s1, ref s2, b1);
            UpdateAdler32(ref s1, ref s2, b2);
            UpdateAdler32(ref s1, ref s2, b3);
        }
    }
}