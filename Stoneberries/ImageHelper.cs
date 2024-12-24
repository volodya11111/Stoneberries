using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.AspNetCore.Components.Forms;


namespace Stoneberries
{
    public class ImageHelper
    {
        public static async void CropImageAndSave(string imagePath, string outputPath, int width, int height)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            foreach (var file in Directory.GetFiles(imagePath, "*.jpg"))
            {
                try
                {
                    using (Image<Rgba32> image = Image.Load<Rgba32>(file))
                    {
                        var targetAspectRatio = 7.0 / 8.0;
                        var imageAspectRatio = (double)image.Width / image.Height;

                        if (imageAspectRatio > targetAspectRatio)
                        {
                            int targetWidth = (int)(image.Height * targetAspectRatio);
                            int offsetX = (image.Width - targetWidth) / 2;
                            image.Mutate(x => x.Crop(new Rectangle(offsetX, 0, targetWidth, image.Height)));
                        }
                        else
                        {
                            int targetHeight = (int)(image.Width / targetAspectRatio);
                            int offsetY = (image.Height - targetHeight) / 2;
                            image.Mutate(x => x.Crop(new Rectangle(0, offsetY, image.Width, targetHeight)));
                        }

                        image.Mutate(x => x.Resize(new Size(175, 200)));

                        // Сохраняем результат
                        string fileName = Path.GetFileName(file);
                        string outputFile = Path.Combine(outputPath, fileName);
                        image.Save(outputFile);

                        Console.WriteLine($"Successfully resized and saved: {outputFile}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {file}: {ex.Message}");
                }
            }
        }
    }
}
