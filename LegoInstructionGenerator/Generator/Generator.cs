using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using LegoInstructionGenerator.Parser;

namespace LegoInstructionGenerator.Generator
{
    internal class Generator
    {
        private const int MainImageWidth = 4592;
        private const int MainImageHeight = 3448;
        private const int Border = 100;
        private const int PartsImageWidth = 2000;

        public void Generate(string inputPath, Page page, string outputPath, bool printFileName)
        {
            const int width = 3 * Border + PartsImageWidth + MainImageWidth;
            const int height = 2 * Border + MainImageHeight;
            const int resolution = 180;

            using (var image = new Bitmap(width, height))
            {
                image.SetResolution(resolution, resolution);
                using (var canvas = Graphics.FromImage(image))
                {
                    canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // Background
                    var rect = new Rectangle(0, 0, width, height);
                    var rectBrush = new SolidBrush(Color.DarkBlue);
                    canvas.FillRectangle(rectBrush, rect);

                    // Index
                    var indexFont = new Font(FontFamily.GenericSansSerif, 200, FontStyle.Bold);
                    var indexBrush = new SolidBrush(Color.GhostWhite);
                    var indexPoint = new PointF(20, 20);
                    canvas.DrawString(page.Index.ToString(), indexFont, indexBrush, indexPoint);

                    // Main image
                    var mainImageFile = Path.Combine(inputPath, page.MainImage);
                    if (!File.Exists(mainImageFile))
                        throw new ArgumentException($"Input file does not exist {mainImageFile}");

                    using (var mainImage = Image.FromFile(mainImageFile))
                    {
                        canvas.DrawImage(mainImage, Border + PartsImageWidth + Border, Border);
                    }

                    // Parts image
                    if (page.HasPartsImage)
                    {
                        var partsImageFile = Path.Combine(inputPath, page.PartsImage);
                        if (!File.Exists(partsImageFile))
                            throw new ArgumentException($"Input file does not exist {partsImageFile}");
                        using (var partsImage = Image.FromFile(partsImageFile))
                        {
                            var cropFactor = (100f - 2f * page.PartsImageCropFactor) / 100f;
                            var croppedWidth = (int) (partsImage.Width * cropFactor);
                            var croppedX = (partsImage.Width - croppedWidth) / 2;
                            var croppedHeight = (int) (partsImage.Height * cropFactor);
                            var croppedY = (partsImage.Height - croppedHeight) / 2;
                            var cropRect = new Rectangle(croppedX, croppedY, croppedWidth, croppedHeight);
                            using (var source = Image.FromFile(partsImageFile) as Bitmap)
                            {
                                using (var cropTarget = new Bitmap(cropRect.Width, cropRect.Height))
                                {
                                    cropTarget.SetResolution(resolution, resolution);
                                    using (var graphics = Graphics.FromImage(cropTarget))
                                    {
                                        graphics.DrawImage(
                                            source,
                                            new Rectangle(0, 0, cropTarget.Width, cropTarget.Height),
                                            cropRect,
                                            GraphicsUnit.Pixel);
                                    }

                                    var resizeFactor = PartsImageWidth / (float) cropTarget.Width;
                                    var resizedHeight = (int) (cropTarget.Height * resizeFactor);
                                    var resizeRectangle = new Rectangle(0, 0, PartsImageWidth, resizedHeight);
                                    using (var resizeTarget = new Bitmap(resizeRectangle.Width, resizeRectangle.Height))
                                    {
                                        resizeTarget.SetResolution(resolution, resolution);
                                        using (var graphics = Graphics.FromImage(resizeTarget))
                                        {
                                            graphics.CompositingMode = CompositingMode.SourceCopy;
                                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                                            using (var wrapMode = new ImageAttributes())
                                            {
                                                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                                                graphics.DrawImage(
                                                    cropTarget,
                                                    resizeRectangle,
                                                    0,
                                                    0,
                                                    cropTarget.Width,
                                                    cropTarget.Height,
                                                    GraphicsUnit.Pixel,
                                                    wrapMode);
                                            }
                                        }

                                        canvas.DrawImage(resizeTarget, Border, height - Border - resizedHeight);
                                    }
                                }
                            }
                        }
                    }

                    // image names
                    if (printFileName)
                    {
                        var namesFont = new Font(FontFamily.GenericSansSerif, 100);
                        var namesBrush = new SolidBrush(Color.GhostWhite);
                        var mainImagePoint = new PointF(2 * Border + PartsImageWidth, 200);
                        canvas.DrawString(page.MainImage, namesFont, namesBrush, mainImagePoint);

                        var partsImagePoint = new PointF(Border, (int)(height * 0.75));
                        canvas.DrawString(page.PartsImage, namesFont, namesBrush, partsImagePoint);
                    }

                    // text
                    if (page.HasText)
                    {
                        var textFont = new Font(FontFamily.GenericSansSerif, 80);
                        var textBrush = new SolidBrush(Color.GhostWhite);
                        var yPosition = 600;
                        foreach (var line in page.Text)
                        {
                            var textPoint = new PointF(Border, yPosition);
                            canvas.DrawString(line, textFont, textBrush, textPoint);
                            yPosition += 200;
                        }
                    }
                }

                image.Save(Path.Combine(outputPath, page.Index + ".png"), ImageFormat.Png);
            }
        }
    }
}