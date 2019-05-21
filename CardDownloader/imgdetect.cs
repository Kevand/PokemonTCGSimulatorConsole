using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace CardDownloader
{
    public class imgdetect
    {

        public static bool Detect(Bitmap image, string type) {

            if (type != "Ultrarare")
            {
                int sumPoint = 0;

                if (type == "Common")
                    sumPoint = 130;
                else if (type == "Uncommon")
                    sumPoint = 70;
                else if (type == "Rare")
                    sumPoint = 30;



                int breakPoint = 41;
                bool black = Black(image);

                if (black)
                {
                    breakPoint = 230;
                }

                int sum = 0;

                for (int x = 50; x < 65; x++)
                {
                    for (int y = 320; y < 328; y++)
                    {

                        Color[,] c = new Color[5, 5];


                        for (int i = -2; i < 2; i++)
                        {
                            for (int z = -2; z < 2; z++)
                            {
                                c[i + 2, z + 2] = image.GetPixel(x - z, y - i);
                                double average = (c[i + 2, z + 2].R + c[i + 2, z + 2].G + c[i + 2, z + 2].B) / 3;

                                if (black)
                                {
                                    if (average >= breakPoint)
                                        sum++;

                                }
                                else
                                {
                                    if (average <= breakPoint)
                                        sum++;

                                }

                            }

                        }

                    }
                }

                image.Dispose();

                if (sum >= sumPoint)return true;  
                else                return false; 
                    
            }
            else if (type == "Ultrarare")
            {

                double[,] ultraCheck = new double[7, 4];
                double ultraSum = 0;
                for (int x = 86; x < 93; x++)
                {
                    for (int y = 323; y < 327; y++)
                    {
                        Color px = image.GetPixel(x, y);

                        for (int q = 0; q < 7; q++)
                        {
                            for (int w = 0; w < 4; w++)
                            {
                                ultraCheck[q, w] = (px.R + px.G + px.B) / 3;
                                ultraSum += ultraCheck[q, w];
                            }

                        }

                    }
                }

                ultraSum /= 784;

                image.Dispose();

                if (ultraSum < 15) return true; 
                else return false;
            }
            else {
                return false;
            }
        }

        private static bool Black(Bitmap image)
        {
            List<double> colors = new List<double>();
                

            for(int x = 0; x < image.Width; x++)
            {
                for(int y = 0; y < image.Height; y++)
                {
                    Color p = image.GetPixel(x,y);
                    double average = (p.R + p.G + p.B) / 3;
                    colors.Add(average);
                }
            }

            int sum = 0;

            foreach(double d in colors)
            {
                if(d < 41)
                {
                    sum++;
                }
            }

            if(sum > 12000)
                return true;
            else
                return false;

        }

    }
}
