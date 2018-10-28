using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace MythMaker
{
    class ShadowUtils
    {
        public static Bitmap GenerateShadow(Bitmap source)
        {
            float radius = 15;
            float transparency = 0.8f;
            Color color = Color.FromArgb((int)System.Math.Round(255 * transparency), 0, 0, 0);

            int kernelRadius = (int)System.Math.Ceiling(radius);

            float[][] kernel = new float[kernelRadius * 2 + 1][];
            for (int y = -kernelRadius; y <= kernelRadius; y++)
            {
                kernel[y + kernelRadius] = new float[kernelRadius * 2 + 1];
                for (int x = -kernelRadius; x <= kernelRadius; x++)
                {
                    kernel[y + kernelRadius][x + kernelRadius] = transparency * (1 - System.Math.Abs(y) / radius) * (1 - System.Math.Abs(x) / radius);
                }
            }

            Bitmap result = new Bitmap(source.Width + 2 * kernelRadius, source.Height + 2 * kernelRadius, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            result.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            byte[] sourceData = GetRGBAValues(source);
            int numBytes = 4 * source.Width * source.Height;

            for (int y = 0; y < source.Height + kernelRadius * 2; y++)
            {
                for (int x = 0; x < source.Width + kernelRadius * 2; x++)
                {
                    float value = 0;

                    for (int ky = -kernelRadius; ky <= kernelRadius; ky++)
                    {
                        for (int kx = -kernelRadius; kx <= kernelRadius; kx++)
                        {
                            if (x + kx - kernelRadius < 0 || x + kx - kernelRadius >= source.Width ||
                                y + ky - kernelRadius < 0 || y + ky - kernelRadius >= source.Height)
                                continue;

                            byte alpha = sourceData[((y + ky - kernelRadius) * source.Width + x + kx - kernelRadius) * 4 + 3];
                            if (alpha > 100)
                                value += kernel[ky + kernelRadius][kx + kernelRadius];
                        }
                    }

                    if (value > 0)
                        result.SetPixel(x, y, color);
                }
            }

            return result;
        }

        static private byte[] GetRGBAValues(Bitmap bmp)
        {

            // Lock the bitmap's bits. 
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
             bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
             bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes); bmp.UnlockBits(bmpData);

            return rgbValues;
        }
    }

    // https://stackoverflow.com/questions/7364026/algorithm-for-fast-drop-shadow-in-gdi
    public static class DropShadow
    {
        const int CHANNELS = 4;

        public static Bitmap CreateShadow(Bitmap bitmap, int radius, float opacity, float intensity = 1)
        {
            // Alpha mask with opacity
            var matrix = new ColorMatrix(new float[][] {
            new float[] {  0F,  0F,  0F, 0F,      0F },
            new float[] {  0F,  0F,  0F, 0F,      0F },
            new float[] {  0F,  0F,  0F, 0F,      0F },
            new float[] { -1F, -1F, -1F, opacity, 0F },
            new float[] {  1F,  1F,  1F, 0F,      1F }
        });

            var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            var shadow = new Bitmap(bitmap.Width + 4 * radius, bitmap.Height + 4 * radius);
            using (var graphics = Graphics.FromImage(shadow))
                graphics.DrawImage(bitmap, new Rectangle(2 * radius, 2 * radius, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttributes);

            // Gaussian blur
            var clone = shadow.Clone() as Bitmap;
            var shadowData = shadow.LockBits(new Rectangle(0, 0, shadow.Width, shadow.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var cloneData = clone.LockBits(new Rectangle(0, 0, clone.Width, clone.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            var boxes = DetermineBoxes(radius, 3);
            BoxBlur(shadowData, cloneData, shadow.Width, shadow.Height, (boxes[0] - 1) / 2);
            BoxBlur(shadowData, cloneData, shadow.Width, shadow.Height, (boxes[1] - 1) / 2);
            BoxBlur(shadowData, cloneData, shadow.Width, shadow.Height, (boxes[2] - 1) / 2);

            Intensify(shadowData, 3, intensity);

            shadow.UnlockBits(shadowData);
            clone.UnlockBits(cloneData);
            return shadow;
        }

        private static unsafe void Intensify(BitmapData data, byte channel, float intensity)
        {
            byte* p = (byte*)(void*)data.Scan0;

            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    int start = y * data.Stride + x * 4;
                    p[start + channel] = (byte)System.Math.Min(255, (p[start + channel] * intensity));
                }
            }
        }

        private static unsafe void BoxBlur(BitmapData data1, BitmapData data2, int width, int height, int radius)
        {
            byte* p1 = (byte*)(void*)data1.Scan0;
            byte* p2 = (byte*)(void*)data2.Scan0;

            int radius2 = 2 * radius + 1;
            int[] sum = new int[CHANNELS];
            int[] FirstValue = new int[CHANNELS];
            int[] LastValue = new int[CHANNELS];

            // Horizontal
            int stride = data1.Stride;
            for (var row = 0; row < height; row++)
            {
                int start = row * stride;
                int left = start;
                int right = start + radius * CHANNELS;

                for (int channel = 0; channel < CHANNELS; channel++)
                {
                    FirstValue[channel] = p1[start + channel];
                    LastValue[channel] = p1[start + (width - 1) * CHANNELS + channel];
                    sum[channel] = (radius + 1) * FirstValue[channel];
                }
                for (var column = 0; column < radius; column++)
                    for (int channel = 0; channel < CHANNELS; channel++)
                        sum[channel] += p1[start + column * CHANNELS + channel];
                for (var column = 0; column <= radius; column++, right += CHANNELS, start += CHANNELS)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += p1[right + channel] - FirstValue[channel];
                        p2[start + channel] = (byte)(sum[channel] / radius2);
                    }
                for (var column = radius + 1; column < width - radius; column++, left += CHANNELS, right += CHANNELS, start += CHANNELS)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += p1[right + channel] - p1[left + channel];
                        p2[start + channel] = (byte)(sum[channel] / radius2);
                    }
                for (var column = width - radius; column < width; column++, left += CHANNELS, start += CHANNELS)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += LastValue[channel] - p1[left + channel];
                        p2[start + channel] = (byte)(sum[channel] / radius2);
                    }
            }

            // Vertical
            stride = data2.Stride;
            for (int column = 0; column < width; column++)
            {
                int start = column * CHANNELS;
                int top = start;
                int bottom = start + radius * stride;

                for (int channel = 0; channel < CHANNELS; channel++)
                {
                    FirstValue[channel] = p2[start + channel];
                    LastValue[channel] = p2[start + (height - 1) * stride + channel];
                    sum[channel] = (radius + 1) * FirstValue[channel];
                }
                for (int row = 0; row < radius; row++)
                    for (int channel = 0; channel < CHANNELS; channel++)
                        sum[channel] += p2[start + row * stride + channel];
                for (int row = 0; row <= radius; row++, bottom += stride, start += stride)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += p2[bottom + channel] - FirstValue[channel];
                        p1[start + channel] = (byte)(sum[channel] / radius2);
                    }
                for (int row = radius + 1; row < height - radius; row++, top += stride, bottom += stride, start += stride)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += p2[bottom + channel] - p2[top + channel];
                        p1[start + channel] = (byte)(sum[channel] / radius2);
                    }
                for (int row = height - radius; row < height; row++, top += stride, start += stride)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += LastValue[channel] - p2[top + channel];
                        p1[start + channel] = (byte)(sum[channel] / radius2);
                    }
            }
        }

        private static int[] DetermineBoxes(double Sigma, int BoxCount)
        {
            double IdealWidth = System.Math.Sqrt((12 * Sigma * Sigma / BoxCount) + 1);
            int Lower = (int)System.Math.Floor(IdealWidth);
            if (Lower % 2 == 0)
                Lower--;
            int Upper = Lower + 2;

            double MedianWidth = (12 * Sigma * Sigma - BoxCount * Lower * Lower - 4 * BoxCount * Lower - 3 * BoxCount) / (-4 * Lower - 4);
            int Median = (int)System.Math.Round(MedianWidth);

            int[] BoxSizes = new int[BoxCount];
            for (int i = 0; i < BoxCount; i++)
                BoxSizes[i] = (i < Median) ? Lower : Upper;
            return BoxSizes;
        }

    }
}
