using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace web2book
{
    public class ImageCleaner
    {
        // Make grayscale, autocrop, dilate, resize, sharpen, then color-reduce

        public Bitmap Clean(Bitmap src, int width, int height)
        {
            Bitmap gsbmp = GrayscaleBT709(src);
            Bitmap gs4bmp = Reduce4BPP(gsbmp);
            Bitmap crbmp = Autocrop(gs4bmp);
            Bitmap dlbmp = Scrub(crbmp, 4);
            return Resize(dlbmp, width, height);
        }

        /// <summary>
        /// Create and initialize grayscale image
        /// </summary>
        public Bitmap CreateGrayscaleImage(int width, int height)
        {
            // create new image
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            // set palette to grayscale
            SetGrayscalePalette(bmp);
            // return new image
            return bmp;
        }

        /// <summary>
        /// Set pallete of the image to grayscale
        /// </summary>
        public void SetGrayscalePalette(Bitmap srcImg)
        {
            // check pixel format
            if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException();

            // get palette
            ColorPalette cp = srcImg.Palette;
            // init palette
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(i, i, i);
            }
            // set palette back
            srcImg.Palette = cp;
        }

        public Bitmap Reduce4BPP(Bitmap srcImg)
        {
            if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException();

            // get source image size
            int width = srcImg.Width;
            int height = srcImg.Height;

            Rectangle rect = new Rectangle(0, 0, width, height);

            // lock source bitmap data
            BitmapData srcData = srcImg.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

            // create new image
            Bitmap dstImg = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            // get palette
            ColorPalette cp = dstImg.Palette;
            // init palette
            for (int i = 0; i < 4; i++)
            {
                cp.Entries[i] = Color.FromArgb(i*85, i*85, i*85);
            }
            // set palette back
            dstImg.Palette = cp;

            // lock destination bitmap data
            BitmapData dstData = dstImg.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            int srcStride = srcData.Stride;
            int srcOffset = srcStride - width;
            int dstStride = dstData.Stride;
            int dstOffset = dstStride - width;


            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();
                byte* dst = (byte*)dstData.Scan0.ToPointer();

                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src++, dst++)
                    {
                        if (*src >= 230) *dst = 3;
                        else *dst = (byte)((*src)/85);

                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
            // unlock both images
            dstImg.UnlockBits(dstData);
            srcImg.UnlockBits(srcData);

            return dstImg;
        }

        public Bitmap Dilate(Bitmap srcImg)
        {
            return Morph(srcImg, true);
        }

        public Bitmap Erode(Bitmap srcImg)
        {
            return Morph(srcImg, false);
        }
      
        public Bitmap Morph(Bitmap srcImg, bool dilate)
        {
            if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException();

            // get source image size
            int width = srcImg.Width;
            int height = srcImg.Height;

            Rectangle rect = new Rectangle(0, 0, width, height);

            // lock source bitmap data
            BitmapData srcData = srcImg.LockBits(rect, ImageLockMode.ReadOnly, srcImg.PixelFormat);

            // create new grayscale image
            Bitmap dstImg = new Bitmap(width, height, srcImg.PixelFormat);
            ColorPalette cp = srcImg.Palette;
            dstImg.Palette = cp;

            // lock destination bitmap data
            BitmapData dstData = dstImg.LockBits(rect, ImageLockMode.ReadWrite, srcImg.PixelFormat);

            int size = 3;
            int stride = dstData.Stride;
            int offset = stride - width;
            int t, ir, jr, i, j, r = size >> 1;
            byte max, min, v;

            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();
                byte* dst = (byte*)dstData.Scan0.ToPointer();

                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src++, dst++)
                    {
                        max = 0;
                        min = 255;

                        // for each SE row
                        for (i = 0; i < size; i++)
                        {
                            ir = i - r;
                            t = y + ir;

                            // skip row
                            if (t < 0)
                                continue;
                            // break
                            if (t >= height)
                                break;

                            // for each SE column
                            for (j = 0; j < size; j++)
                            {
                                jr = j - r;
                                
                                if (ir == 0 && jr == 0) continue;

                                t = x + jr;

                                // skip column
                                if (t < 0)
                                    continue;
                                if (t < width)
                                {
                                    //if (se[i, j] == 1)
                                    //{
                                        // get new MAX value
                                        v = src[ir * stride + jr];
                                        if (v > max)
                                            max = v;
                                        if (v < min)
                                            min = v;
                                    //}
                                }
                            }
                        }
                        // result pixel
                        *dst = dilate ? min : max;
                    }
                    src += offset;
                    dst += offset;
                }
            }
            // unlock both images
            dstImg.UnlockBits(dstData);
            srcImg.UnlockBits(srcData);

            return dstImg;
        }

        public Bitmap Autocrop(Bitmap srcImg)
        {
            if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException();

            // get source image size
            int width = srcImg.Width;
            int height = srcImg.Height;

            Rectangle rect = new Rectangle(0, 0, width, height);

            // lock source bitmap data
            BitmapData srcData = srcImg.LockBits(rect, ImageLockMode.ReadOnly, srcImg.PixelFormat);

            int stride = srcData.Stride;
            int offset = stride - width;

            int left = width, right = 0;
            int top = height, bottom = 0;

            ColorPalette cp = srcImg.Palette;

            int maxCol = (srcImg.PixelFormat == PixelFormat.Format4bppIndexed) ? 15 : 255;
            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();

                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src++)
                    {
                        Color c = cp.Entries[*src];
                        if (c.B < maxCol || c.R < maxCol || c.B < maxCol)
                        {
                            if (x < left) left = x;
                            if (x > right) right = x;
                            if (y < top) top = y;
                            if (y > bottom) bottom = y;
                        }
                    }
                    src += offset;
                }
            }
            srcImg.UnlockBits(srcData);
            if (left > 0) --left;
            if (top > 0) --top;
            if ((right + 1) < width) ++right;
            if ((bottom + 1) < height) ++bottom;
            return Crop(srcImg, new Rectangle(left, top, right - left + 1, bottom - top + 1));
        }

        public Bitmap Scrub(Bitmap srcImg,int numGrayLevels)
        {
            if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException();

            // get source image size
            int width = srcImg.Width;
            int height = srcImg.Height;

            Rectangle rect = new Rectangle(0, 0, width, height);

            // lock source bitmap data
            BitmapData srcData = srcImg.LockBits(rect, ImageLockMode.ReadOnly, srcImg.PixelFormat);

            // create new grayscale image
            Bitmap dstImg = new Bitmap(width, height, srcImg.PixelFormat);
            ColorPalette cp = srcImg.Palette;
            dstImg.Palette = cp;

            // lock destination bitmap data
            BitmapData dstData = dstImg.LockBits(rect, ImageLockMode.ReadWrite, srcImg.PixelFormat);

            int size = 3;
            int stride = dstData.Stride;
            int offset = stride - width;
            int t, ir, jr, i, j, r = size >> 1;
            int maxCol = (srcImg.PixelFormat == PixelFormat.Format4bppIndexed) ? 15 : 255;

            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();
                byte* dst = (byte*)dstData.Scan0.ToPointer();

                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src++, dst++)
                    {
                        int count = 0;

                        // for each SE row
                        for (i = 0; i < size; i++)
                        {
                            ir = i - r;
                            t = y + ir;

                            // skip row
                            if (t < 0)
                                continue;
                            // break
                            if (t >= height)
                                break;

                            // for each SE column
                            for (j = 0; j < size; j++)
                            {
                                jr = j - r;

                                if (ir == 0 && jr == 0) continue;

                                t = x + jr;

                                // skip column
                                if (t < 0)
                                    continue;
                                if (t < width)
                                {
                                    byte v = src[ir * stride + jr];
                                    if (v < maxCol)
                                    {
                                        ++count;
                                    }
                                }
                            }
                        }
                        // result pixel
                        *dst = (count > 1) ? (*src) : ((byte)maxCol);
                    }
                    src += offset;
                    dst += offset;
                }
            }
            // unlock both images
            dstImg.UnlockBits(dstData);
            srcImg.UnlockBits(srcData);

            return dstImg;
        }

        public Bitmap Sharpen(Bitmap src)
        {
            return ApplyCorrelationFilter(src, new int[,] {
										{0, -1, 0},
										{-1, 5, -1},
										{0, -1, 0}});
        }

        // Apply filter
        public Bitmap ApplyCorrelationFilter(Bitmap srcImg, int[,] kernel)
        {
            int size = kernel.GetLength(0);

            // get source image size
            int width = srcImg.Width;
            int height = srcImg.Height;

            Rectangle rect = new Rectangle(0, 0, width, height);

            // lock source bitmap data
            BitmapData srcData = srcImg.LockBits(rect, ImageLockMode.ReadOnly, srcImg.PixelFormat);

            // create new image
            Bitmap dstImg = new Bitmap(width, height, srcImg.PixelFormat);
            ColorPalette cp = srcImg.Palette;
            dstImg.Palette = cp;

            // lock destination bitmap data
            BitmapData dstData = dstImg.LockBits(rect, ImageLockMode.ReadWrite, srcImg.PixelFormat);

            int stride = srcData.Stride;
            int offset = stride - width;

            int i, j, t, k, ir, jr;
            int radius = size >> 1;
            long g, div;
            int maxCol = (srcImg.PixelFormat == PixelFormat.Format4bppIndexed) ? 15 : 255;

            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();
                byte* dst = (byte*)dstData.Scan0.ToPointer();

                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src++, dst++)
                    {
                        g = div = 0;

                        // for each kernel row
                        for (i = 0; i < size; i++)
                        {
                            ir = i - radius;
                            t = y + ir;

                            // skip row
                            if (t < 0)
                                continue;
                            // break
                            if (t >= height)
                                break;

                            // for each kernel column
                            for (j = 0; j < size; j++)
                            {
                                jr = j - radius;
                                t = x + jr;

                                // skip column
                                if (t < 0)
                                    continue;

                                if (t < width)
                                {
                                    k = kernel[i, j];

                                    div += k;
                                    g += k * src[ir * stride + jr];
                                }
                            }
                        }

                        // check divider
                        if (div != 0)
                        {
                            g /= div;
                        }
                        *dst = (g > maxCol) ? (byte)maxCol : ((g < 0) ? (byte)0 : (byte)g);
                    }
                    src += offset;
                    dst += offset;
                }
            }

            // unlock both images
            dstImg.UnlockBits(dstData);
            srcImg.UnlockBits(srcData);

            return dstImg;
        }

        public Bitmap Crop(Bitmap srcImg, Rectangle rect)
        {
            // get source image size
            int width = srcImg.Width;
            int height = srcImg.Height;

            // destination image dimension
            int xmin = Math.Max(0, Math.Min(width - 1, rect.Left));
            int ymin = Math.Max(0, Math.Min(height - 1, rect.Top));
            int xmax = Math.Min(width - 1, xmin + rect.Width - 1 + ((rect.Left < 0) ? rect.Left : 0));
            int ymax = Math.Min(height - 1, ymin + rect.Height - 1 + ((rect.Top < 0) ? rect.Top : 0));

            int dstWidth = xmax - xmin + 1;
            int dstHeight = ymax - ymin + 1;

            // lock source bitmap data
            BitmapData srcData = srcImg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, srcImg.PixelFormat);

            // create new image
            Bitmap dstImg = new Bitmap(dstWidth, dstHeight, srcImg.PixelFormat);
            ColorPalette cp = srcImg.Palette;
            dstImg.Palette = cp;

            // lock destination bitmap data
            BitmapData dstData = dstImg.LockBits(
                new Rectangle(0, 0, dstWidth, dstHeight),
                ImageLockMode.ReadWrite, srcImg.PixelFormat);

            int srcStride = srcData.Stride;
            int dstStride = dstData.Stride;
            int pixelSize = 1;
            int copySize = dstWidth * pixelSize;

            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer() + ymin * srcStride + xmin * pixelSize;
                byte* dst = (byte*)dstData.Scan0.ToPointer();

                // for each line
                for (int y = ymin; y <= ymax; y++)
                {
                    Win32.memcpy(dst, src, copySize);
                    src += srcStride;
                    dst += dstStride;
                }
            }
            // unlock both images
            dstImg.UnlockBits(dstData);
            srcImg.UnlockBits(srcData);

            return dstImg;
        }


        // Apply filter
        public Bitmap Resize(Bitmap srcImg, int newWidth, int newHeight)
        {
            // get source image size
            int width = srcImg.Width;
            int height = srcImg.Height;

            if ((newWidth == width) && (newHeight == height))
            {
                return srcImg;
            }

            // lock source bitmap data
            BitmapData srcData = srcImg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, srcImg.PixelFormat);

            // create new image
            Bitmap dstImg = new Bitmap(newWidth, newHeight, srcImg.PixelFormat);
            ColorPalette cp = srcImg.Palette;
            dstImg.Palette = cp;

            // lock destination bitmap data
            BitmapData dstData = dstImg.LockBits(
                new Rectangle(0, 0, newWidth, newHeight),
                ImageLockMode.ReadWrite, srcImg.PixelFormat);

            int pixelSize = 1;
            int srcStride = srcData.Stride;
            int dstOffset = dstData.Stride - pixelSize * newWidth;
            float xFactor = (float)width / newWidth;
            float yFactor = (float)height / newHeight;

            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();
                byte* dst = (byte*)dstData.Scan0.ToPointer();

                // ------------------------------------
                // resize using  bilinear interpolation
                // ------------------------------------

                float ox, oy, dx1, dy1, dx2, dy2;
                int ox1, oy1, ox2, oy2;
                int ymax = height - 1;
                int xmax = width - 1;
                byte v1, v2;
                byte* tp1, tp2;

                byte* p1, p2, p3, p4;

                // for each line
                for (int y = 0; y < newHeight; y++)
                {
                    // Y coordinates
                    oy = (float)y * yFactor;
                    oy1 = (int)oy;
                    oy2 = (oy1 == ymax) ? oy1 : oy1 + 1;
                    dy1 = oy - (float)oy1;
                    dy2 = 1.0f - dy1;

                    // get temp pointers
                    tp1 = src + oy1 * srcStride;
                    tp2 = src + oy2 * srcStride;

                    // for each pixel
                    for (int x = 0; x < newWidth; x++)
                    {
                        // X coordinates
                        ox = (float)x * xFactor;
                        ox1 = (int)ox;
                        ox2 = (ox1 == xmax) ? ox1 : ox1 + 1;
                        dx1 = ox - (float)ox1;
                        dx2 = 1.0f - dx1;

                        // get four points
                        p1 = tp1 + ox1 * pixelSize;
                        p2 = tp1 + ox2 * pixelSize;
                        p3 = tp2 + ox1 * pixelSize;
                        p4 = tp2 + ox2 * pixelSize;

                        // interpolate using 4 points
                        for (int i = 0; i < pixelSize; i++, dst++, p1++, p2++, p3++, p4++)
                        {
                            v1 = (byte)(dx2 * (*p1) + dx1 * (*p2));
                            v2 = (byte)(dx2 * (*p3) + dx1 * (*p4));
                            *dst = (byte)(dy2 * v1 + dy1 * v2);
                        }
                    }
                    dst += dstOffset;
                }
            }

            // unlock both images
            dstImg.UnlockBits(dstData);
            srcImg.UnlockBits(srcData);

            return dstImg;
        }

        /// Makes an images grayscale using BT709 algorithm
        /// </summary>
        public Bitmap GrayscaleBT709(Bitmap srcImg)
        {
            return Grayscale(srcImg, 0.2125f, 0.7154f, 0.0721f);
        }

        // Apply filter
        public Bitmap Grayscale(Bitmap srcImg, float cr, float cg, float cb)
        {
            if (srcImg.PixelFormat != PixelFormat.Format24bppRgb && srcImg.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ArgumentException();

            // get source image size
            int width = srcImg.Width;
            int height = srcImg.Height;

            // lock source bitmap data
            BitmapData srcData = srcImg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, srcImg.PixelFormat);

            // create new grayscale image
            Bitmap dstImg = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            SetGrayscalePalette(dstImg);

            // lock destination bitmap data
            BitmapData dstData = dstImg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            int pw = (srcImg.PixelFormat == PixelFormat.Format32bppArgb) ? 4 : 3;
            int srcOffset = srcData.Stride - width * pw;
            int dstOffset = dstData.Stride - width;

            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();
                byte* dst = (byte*)dstData.Scan0.ToPointer();

                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src += pw, dst++)
                    {
                        *dst = (byte)(cr * src[RGB.R] + cg * src[RGB.G] + cb * src[RGB.B]);
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
            // unlock both images
            dstImg.UnlockBits(dstData);
            srcImg.UnlockBits(srcData);

            return dstImg;
        }
    }
    public class RGB
    {
        public const short R = 2;
        public const short G = 1;
        public const short B = 0;

        public byte Red;
        public byte Green;
        public byte Blue;

        // Color property
        public System.Drawing.Color Color
        {
            get { return Color.FromArgb(Red, Green, Blue); }
            set
            {
                Red = value.R;
                Green = value.G;
                Blue = value.B;
            }
        }


        // Constructors
        public RGB()
        {
        }
        public RGB(byte red, byte green, byte blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }
        public RGB(System.Drawing.Color color)
        {
            this.Red = color.R;
            this.Green = color.G;
            this.Blue = color.B;
        }
    }

    internal class Win32
    {
        // memcpy - copy a block of memory
        [DllImport("ntdll.dll")]
        public static extern IntPtr memcpy(
            IntPtr dst,
            IntPtr src,
            int count);
        [DllImport("ntdll.dll")]
        public static extern unsafe byte* memcpy(
            byte* dst,
            byte* src,
            int count);

        // memset - fill memory with specified values
        [DllImport("ntdll.dll")]
        public static extern IntPtr memset(
            IntPtr dst,
            int filler,
            int count);
    }
}
