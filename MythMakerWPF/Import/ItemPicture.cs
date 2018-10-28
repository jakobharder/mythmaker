using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using MythMaker.Math;

namespace MythMaker.Import
{
    class ItemPicture
    {
        public static Bitmap Extract(Bitmap source)
        {
            Vector scale = new Vector(590.0f / source.Width, 860.0f / source.Height); 
            IntRect colorRegion = (IntRect)(new Rect(90, 230, 30, 30) * scale);

            IntRect pictureRegion = (IntRect)(new Rect(90, 140, 430, 250) * scale);
            var bmpData = lockBitmap(source, ImageLockMode.ReadWrite);

            Color averageColor = Rendering.Utils.BitmapUtils.GetAverageColor(bmpData, colorRegion);

            IntRect removeTop = (IntRect)(new Rect(0, 0, 590, 90) * scale);
            IntRect removeBottom = new IntRect(0, pictureRegion.BottomPixel, source.Width, source.Height - pictureRegion.BottomPixel);
            IntRect removeLeft = new IntRect(0, pictureRegion.Y, pictureRegion.X + 2, pictureRegion.Height);
            IntRect removeRight = new IntRect(pictureRegion.RightPixel, pictureRegion.Y, source.Width - pictureRegion.RightPixel, pictureRegion.Height);

            remove(bmpData, removeTop);
            remove(bmpData, removeBottom);
            remove(bmpData, removeLeft);
            remove(bmpData, removeRight);

            removeColor(bmpData, pictureRegion, averageColor);
            denoiseAlpha(bmpData, pictureRegion);
            removeFrame(bmpData, pictureRegion);

            var calculatedRegion = removeOuterRegions(bmpData, pictureRegion);
            calculatedRegion.Inflate(2);

            unlockBitmap(source, bmpData);

            Bitmap result = new Bitmap((int)calculatedRegion.Width, (int)calculatedRegion.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(result);
            g.DrawImage(source, new RectangleF(-0.5f, -0.5f, result.Width, result.Height), calculatedRegion, GraphicsUnit.Pixel);
            //SolidBrush brush = new SolidBrush(Color.FromArgb(80, 255, 255, 255));
            //g.FillRectangle(brush, new RectangleF(calculatedRegion.X - pictureRegion.X, calculatedRegion.Y - pictureRegion.Y, calculatedRegion.Width, calculatedRegion.Height));

            return result;
        }

        private static BitmapData lockBitmap(Bitmap bmp, ImageLockMode mode)
        {
            return bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), mode, PixelFormat.Format32bppArgb);
        }

        private static void unlockBitmap(Bitmap bmp, BitmapData bmpData)
        {
            bmp.UnlockBits(bmpData);
        }

        private static unsafe void removeFrame(BitmapData bmpData, IntRect region)
        {
            byte* p = (byte*)(void*)bmpData.Scan0;

            byte* topLine = p + 0 * bmpData.Stride;
            byte* bottomLine = p + region.Bottom * bmpData.Stride;
            for (int x = region.X; x < region.Right; x++)
            {
                topLine[x * 4 + 3] = 0;
                bottomLine[x * 4 + 3] = 0;
            }

            for (int y = region.Y; y < region.Bottom; y++)
            {
                byte* line = p + y * bmpData.Stride;

                line[0 * 4 + 3] = 0;
                line[region.Right * 4 + 3] = 0;
            }
        }

        private static unsafe void remove(BitmapData bmpData, IntRect region)
        {
            byte* p = (byte*)(void*)bmpData.Scan0;

            for (int y = region.Y; y < region.Bottom; y++)
            {
                byte* line = p + y * bmpData.Stride;
                for (int x = region.X; x < region.Right; x++)
                {
                    line[x * 4 + 0] = 0;
                    line[x * 4 + 1] = 0;
                    line[x * 4 + 2] = 0;
                    line[x * 4 + 3] = 0;
                }
            }
        }

        private static unsafe void denoiseAlpha(BitmapData bmpData, IntRect region)
        {
            byte* p = (byte*)(void*)bmpData.Scan0;

            int kernel = 9;

            for (int y = region.Y + kernel; y <= region.Bottom - kernel; y++)
            {
                byte* line = p + y * bmpData.Stride;
                for (int x = region.X + kernel; x <= region.Right - kernel; x++)
                {
                    int count = 0;
                    for (int m = -2; m <= kernel; m++)
                    {
                        for (int n = -2; n <= kernel; n++)
                        {
                            byte* kcur = p + (y + m) * bmpData.Stride + (x + n) * 4;
                            if (kcur[3] == 0)
                                count++;
                        }
                    }
                    float rate = count / ((float)(kernel * 2 + 1) * (kernel * 2 + 1));

                    if (rate > 0.6f)
                    {
                        //line[x * 4 + 0] = 0;
                        //line[x * 4 + 1] = 0;
                        //line[x * 4 + 2] = 255;
                        line[x * 4 + 3] = 0;
                    }
                    else if (rate < 0.1f)
                    {
                        //line[x * 4 + 0] = 0;
                        //line[x * 4 + 1] = 255;
                        //line[x * 4 + 2] = 0;
                        line[x * 4 + 3] = 255;
                    }
                }
            }
        }

        class RegionCounter
        {
            class Label
            {
                public byte Name { get; set; }
                public Label Root { get; set; }
                public int Rank { get; set; }

                public Label(byte Name)
                {
                    this.Name = Name;
                    Root = this;
                    Rank = 0;
                }

                internal Label GetRoot()
                {
                    if (this.Root != this)
                    {
                        this.Root = this.Root.GetRoot();//Compact tree
                    }

                    return this.Root;
                }

                internal void Join(Label root2)
                {
                    if (root2.Rank < this.Rank)//is the rank of Root2 less than that of Root1 ?
                    {
                        root2.Root = this;//yes! then Root1 is the parent of Root2 (since it has the higher rank)
                    }
                    else //rank of Root2 is greater than or equal to that of Root1
                    {
                        this.Root = root2;//make Root2 the parent

                        if (this.Rank == root2.Rank)//both ranks are equal ?
                        {
                            root2.Rank++;//increment Root2, we need to reach a single root for the whole tree
                        }
                    }
                }
            }

            List<int> regionCount = new List<int>();
            Dictionary<byte, Label> labelSets = new Dictionary<byte, Label>();

            public RegionCounter()
            {
                labelSets.Add(0, new Label(0));
            }

            public byte AddNewLabel()
            {
                if (labelSets.Count < 255)
                {
                    byte newIndex = (byte)labelSets.Count;
                    labelSets.Add(newIndex, new Label(newIndex));
                    return newIndex;
                }
                else
                {
                    return 0;
                }
            }

            public void JoinRegions(byte labelA, byte labelB)
            {
                if (IsNeighbor(labelA, labelB))
                    return;

                labelSets[labelA].Join(labelSets[labelB]);
            }

            public byte GetLowestLabel(byte label)
            {
                if (label == 0)
                    return 0;
                return labelSets[label].GetRoot().Name;
            }

            public void Increase(byte region, int number = 1)
            {
                byte mappedRegion = region;// neighbors[region];
                //regionCount[mappedRegion] += number;
                //biggest = Math.Max(biggest, regionCount[mappedRegion]);
            }

            public bool IsNeighbor(byte regionA, byte regionB)
            {
                return labelSets[regionB].GetRoot().Name == labelSets[regionA].GetRoot().Name;
            }
        }

        private static unsafe Rect removeOuterRegions(BitmapData bmpData, IntRect region)
        {
            Rect calculatedRegion = new Rect(region.X, region.Y, region.Width, region.Height);

            byte* p = (byte*)(void*)bmpData.Scan0;

            byte[] map = new byte[region.Width * region.Height];

            RegionCounter regionCount = new RegionCounter();

            // label
            for (int y = region.Y + 1; y <= region.Bottom - 1; y++)
            {
                byte* line = p + y * bmpData.Stride;
                for (int x = region.X + 1; x <= region.Right - 1; x++)
                {
                    if (line[x * 4 + 3] != 0)
                    {
                        int mapX = x - region.X;
                        int mapY = y - region.Y;

                        List<byte> neighbors = new List<byte>();
                        byte[] neighborCandidates = new byte[3] {
                            regionCount.GetLowestLabel(map[mapY * region.Width + (mapX - 1)]),
                            regionCount.GetLowestLabel(map[(mapY - 1) * region.Width + mapX]),
                            regionCount.GetLowestLabel(map[(mapY - 1) * region.Width + (mapX + 1)])
                        };

                        foreach (byte neighbor in neighborCandidates)
                            if (neighbor > 0)
                                neighbors.Add(neighbor);

                        // prediction
                        for (int k = 2; k < 5; k++)
                        {
                            if (x + k > region.Right || line[(x + k) * 4 + 3] == 0)
                                break;
                            byte label = regionCount.GetLowestLabel(map[(mapY - 1) * region.Width + (mapX + k)]);
                            if (label > 0)
                                neighbors.Add(label);
                        }

                        if (!neighbors.Any())
                        {
                            byte newRegion = regionCount.AddNewLabel();
                            map[mapY * region.Width + mapX] = newRegion;
                        }
                        else
                        {
                            byte minimumLabel = neighbors.Min();
                            foreach (byte neighbor in neighbors)
                            {
                                if (minimumLabel != neighbor)
                                    regionCount.JoinRegions(minimumLabel, neighbor);
                                
                            }
                            map[mapY * region.Width + mapX] = minimumLabel;
                        }
                    }
                }
            }

            // collect
            Random rand = new Random();
            List<Color> colors = new List<Color>();
            for (int i = 0; i < 255; i ++)
            {
                Color c = Color.FromArgb(255, rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                colors.Add(c);
            }

            Rect[] labelRegions = new Rect[255];

            for (int y = region.Y + 1; y <= region.Bottom; y++)
            {
                byte* line = p + y * bmpData.Stride;
                for (int x = region.X + 1; x <= region.Right; x++)
                {
                    if (line[x * 4 + 3] != 0)
                    {
                        int mapX = x - region.X;
                        int mapY = y - region.Y;

                        byte label = regionCount.GetLowestLabel(map[mapY * region.Width + mapX]);
                        if (label != 0)
                        {
                            if (labelRegions[label] == null)
                                labelRegions[label] = new Rect(mapX, mapY, 1, 1);
                            else
                                labelRegions[label].AddPoint(mapX, mapY);
                        }
                    }
                }
            }

            bool foundOne = false;

            // decide on labels
            bool[] removeLabel = new bool[255];
            for (byte i = 1; i < 255; i++)
            {
                if (labelRegions[i] == null)
                    continue;
                removeLabel[i] = true;

                var relativeRegion = labelRegions[i] * new Vector(1.0f / region.Width, 1.0f / region.Height);

                // cut corners
                if ((relativeRegion.Center.X < 0.2f && relativeRegion.Center.Y < 0.3f) ||
                    (relativeRegion.Center.X < 0.2f && relativeRegion.Center.Y > 0.7f) ||
                    (relativeRegion.Center.X > 0.7f && relativeRegion.Center.Y < 0.4f) ||
                    (relativeRegion.Center.X > 0.8f && relativeRegion.Center.Y > 0.7f))
                    continue;

                // cut bottom
                if (relativeRegion.Center.Y > 0.8f)
                    continue;

                if (relativeRegion.Width < 0.02 || relativeRegion.Height < 0.02f)
                    continue;

                removeLabel[i] = false;
                if (!foundOne)
                {
                    foundOne = true;
                    calculatedRegion = labelRegions[i];
                }
                else
                    calculatedRegion.AddRect(labelRegions[i]);
            }
            if (foundOne)
                calculatedRegion.Move(region.Location);

            // remove
            for (int y = region.Y + 1; y <= region.Bottom; y++)
            {
                float yRelative = (y - region.Y) / (float)region.Height;
                byte* line = p + y * bmpData.Stride;
                for (int x = region.X + 1; x <= region.Right; x++)
                {
                    float xRelative = (x - region.X) / (float)region.Width;

                    if (line[x * 4 + 3] != 0)
                    {
                        int mapX = x - region.X;
                        int mapY = y - region.Y;

                        byte label = regionCount.GetLowestLabel(map[mapY * region.Width + mapX]);
                        if (label != 0 && removeLabel[label])
                        {
                            //line[x * 4 + 0] = 255;
                            //line[x * 4 + 1] = 0;
                            //line[x * 4 + 2] = 0;
                            line[x * 4 + 3] = 0;
                        }
                        else if (label != 0)
                        {
                            //line[x * 4 + 0] = colors[label].B;
                            //line[x * 4 + 1] = colors[label].G;
                            //line[x * 4 + 2] = colors[label].R;
                            //line[x * 4 + 3] = 0;
                        }
                    }
                }
            }

            return calculatedRegion;
        }

        private static unsafe void removeColor(BitmapData bmpData, IntRect region, Color color)
        {
            byte* p = (byte*)(void*)bmpData.Scan0;

            Rendering.ColorHSL average = new Rendering.ColorHSL(color.R, color.G, color.B);

            float averageSaturation = average.S;
            float averageLight = average.L;

            for (int y = region.Y; y <= region.Bottom; y++)
            {
                float yRelative = (y - region.Y) / (float)region.Height;
                float lineAverage = 0;
                float lineAverageL = 0;
                int lineAverageCount = 0;
                

                byte* line = p + y * bmpData.Stride;
                for (int x = region.X; x <= region.Right; x++)
                {
                    float xRelative = (x - region.X) / (float)region.Width;

                    Rendering.ColorHSL current = new Rendering.ColorHSL(line[x * 4 + 2], line[x * 4 + 1], line[x * 4 + 0]);

                    float diffS = System.Math.Abs(averageSaturation - current.S);
                    float diffH = System.Math.Abs(average.H - current.H) / 6;

                    bool remove = false;

                    if (diffS < 0.2f && diffH < 0.2f && averageLight > current.L - 0.05f ||
                        diffS < 0.1f && diffH < 0.1f && averageLight > current.L - 0.2f)
                    {
                        if (current.L > 0.05f)
                        {
                            lineAverage += current.S;
                            lineAverageL += current.L;
                            lineAverageCount++;
                            remove = true;
                        }
                    }

                    // corner special treatment
                    if (false && (xRelative < 0.2f || xRelative > 0.8f) && (yRelative < 0.4f || yRelative > 0.6f))
                    {
                        // remove whites
                        if (current.L >= 0.8f)
                            remove = true;
                        // remove grays
                        int avg = (line[x * 4 + 2] + (int)line[x * 4 + 1] + (int)line[x * 4 + 0]) / 3;
                        int diffAvg = System.Math.Abs(line[x * 4 + 2] - avg) + System.Math.Abs((int)line[x * 4 + 1] - avg) + System.Math.Abs((int)line[x * 4 + 0] - avg);
                        if (diffAvg < 20)
                            remove = true;
                        // remove reds
                        if (current.H < 0.2f || current.H > 5.8f)
                            remove = true;
                    }

                    // class text
                    if (false && yRelative > 0.8f)
                    {
                        // remove whites
                        if (current.L >= 0.8f)
                        {
                            line[x * 4 + 0] = 255;
                            line[x * 4 + 1] = 255;
                            line[x * 4 + 2] = 255;
                            remove = true;
                        }
                        else if (current.L > 0.7f)
                        {
                            line[x * 4 + 3] = (byte)((current.L - 0.7f) * 255);
                        }
                    }

                    if (remove)
                    {
                        line[x * 4 + 3] = 0;
                    }
                }

                if (lineAverageCount > 20)
                {
                    averageSaturation = lineAverage / lineAverageCount;
                    averageLight = lineAverageL / lineAverageCount;
                }
            }
        }
    }
}
