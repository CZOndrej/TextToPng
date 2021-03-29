using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Collections.Generic;

namespace TextToPng
{
    class Program
    {
        private  struct Parameter
        {
            public string imageName;
            public string text;
        }
        private static Font font;
        private static Color fontColor = Color.Black;
        private static List<Parameter> parameters= new List<Parameter>();
        private static int pictureProcessed = 0;
        private static StreamReader OpenFile(string path)
        {
            StreamReader file;

            if (File.Exists(path))
            {
                file = new StreamReader(path);
            }
            else
            {
                Console.WriteLine($"Error, file '{path}' not found!");
                return  null;
            }
            return file;
        }
        private static void LoadOptions(StreamReader optionsFile)
        {
            FontFamily fontFamily = null;
            int fontSize = 30;
            FontStyle fontStyle = FontStyle.Regular;
            Color color = Color.Black;
            string options = "";
            int index = 0;

            while ((options = optionsFile.ReadLine()) != null)
            {
                switch (index)
                {
                    case 0:
                        if(!SystemFonts.TryFind(options, out fontFamily))
                        {
                            fontFamily = SystemFonts.Find("Arial");
                            Console.WriteLine($"Font family '{options}' not found!");
                        }
                        break;
                    case 1:
                        if (!int.TryParse(options, out fontSize))
                        {
                            fontSize = 30;
                            Console.WriteLine($"Incorrect size format '{options}'!");

                        }
                        break;
                    case 2:
                        if (!Enum.TryParse<FontStyle>(options, out fontStyle))
                        {
                            fontStyle = FontStyle.Regular;
                            Console.WriteLine($"Undefined font style '{options}'!");
                        }
                        break;
                    case 3:
                        if (!Color.TryParse(options, out color))
                        {
                            color = Color.Black;
                            Console.WriteLine($"Undefined color '{options}'!");
                        }
                        break;
                }
                index++;
            }
            
            font = fontFamily.CreateFont(fontSize, fontStyle);
        }
        private static void LoadData(StreamReader inputData)
        {
            string tmpLine = "";

            while ((tmpLine = inputData.ReadLine()) != null)
            {
                string[] lineArr = tmpLine.Split("\t");
                Parameter parameter = new Parameter
                {
                    imageName = lineArr[0],
                    text = lineArr[1]
                };
                parameters.Add(parameter);
            }
        }
        static void Main(string[] args)
        {

            StreamReader options = OpenFile("./options.txt");
            StreamReader data = OpenFile("./data.txt");

            LoadOptions(options);
            LoadData(data);


            foreach (Parameter par in parameters)
            {
                Image image = null;
                try
                {
                    image = Image.Load($"./input/{par.imageName}");

                }
                catch (Exception)
                {
                    Console.WriteLine($"Input image '{par.imageName}' not found!");
                    image = Image.Load($"./default.png");
                }

                image.Mutate(x => x.DrawText(par.text, font, fontColor, new PointF(0, 0)));
                if (File.Exists($"./output/{par.imageName}"))
                {
                    Console.WriteLine($"In output folder image '{par.imageName}' already exist!");
                }
                else
                {
                    image.Save($"./output/{par.imageName}");
                    pictureProcessed++;
                }
            }

            Console.WriteLine($"Done, processed {pictureProcessed}/{parameters.Count} images");
            Console.ReadKey();
        }
    }
}
