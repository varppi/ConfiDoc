using Markdig;
using Microsoft.VisualBasic;
using PdfiumViewer;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Encodings.Web;

namespace ConfidocLib
{
    public static class Pdf
    {
        public static async void Initialize()
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
        }

        public static Image Datamark(Image inputImage)
        {
            Bitmap input = new Bitmap(inputImage);

            string data = "testing data";
            List<int> bits = data
                .SelectMany(c =>
                    Convert.ToString(c, 2)
                        .PadLeft(8, '0')
                        .Select(b => b - '0'))
                .ToList();

            int bitCount = bits.Count;

            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    Color original = input.GetPixel(x, y);

                    int bit = bits[(int)Math.Round((decimal)x/2) % bitCount];

                    int r = original.R;
                    int g = original.G;
                    int b = original.B;

                    // If bit is 1, darken slightly
                    if (bit == 1)
                    {
                        r = Math.Max(r-1, 0);
                        g = Math.Max(g-1, 0);
                        b = Math.Max(b-1, 0);
                    }

                    input.SetPixel(
                        x,
                        y,
                        Color.FromArgb(original.A, r, g, b)
                    );
                }
            }

            return input;
        }

        /// <summary>
        /// Converts PDF into a series of images in a PDF form.
        /// </summary>
        /// <param name="pdfData"></param>
        /// <returns></returns>
        public static byte[] PdfToImagePdf(byte[] pdfData)
        {
            var pageImages = new List<byte[]>();

            using var document = PdfiumViewer.PdfDocument.Load(new MemoryStream(pdfData));
            var pageCount = document.PageCount;
            for (int i = 0; i < pageCount; i++)
            {
                using var image = document.Render(i, 300, 300, PdfRenderFlags.CorrectFromDpi);
                var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Png.Guid);
                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] =
                    new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                var markedImage = Datamark(image);
                using (var imageStream = new MemoryStream())
                {
                    markedImage.Save(imageStream, ImageFormat.Png);
                    pageImages.Add(imageStream.ToArray());
                }
            }

            var newDocument = new PdfSharp.Pdf.PdfDocument();
            foreach (var imageData in pageImages)
            {
                var image = Image.FromStream(new MemoryStream(imageData));
                var page = newDocument.AddPage();
                page.Width = image.Width * 72 / image.HorizontalResolution;
                page.Height = image.Height * 72 / image.VerticalResolution;

                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var xImage = XImage.FromStream(new MemoryStream(imageData));
                    gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                }
            }

            using (var outputStream = new MemoryStream())
            {
                newDocument.Save(outputStream);
                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Inserts tracking elements and formats the HTML document.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string TemplateHtml(string html)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<style>");
            builder.AppendLine(@"
html {
    font-family: sans-serif !important;
}
.dot {
    z-index: -100;
    opacity: 1;
}
            ");

            builder.AppendLine("</style>");
            builder.AppendLine("<main>");
            builder.AppendLine(html);
            builder.AppendLine("</main>");
            return builder.ToString();
        }

        /// <summary>
        /// Converts HTML to A4 sized PDF. This is used
        /// for embedding tracking elements into the 
        /// read only versions of the document.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static async Task<byte[]> HtmlToPdf(string html)
        {
            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }))
            using (var page = await browser.NewPageAsync())
            {
                await page.SetContentAsync(TemplateHtml(html));

                // Enabling links in the pdf generated
                var pdfOptions = new PdfOptions { DisplayHeaderFooter = false, Landscape = false, PrintBackground = true, Format = PaperFormat.A4, MarginOptions = new MarginOptions { Top = "1cm", Bottom = "1cm", Left = "1cm", Right = "1cm" } };
                return await page.PdfDataAsync(pdfOptions);
            }
        }

        /// <summary>
        /// Converts markdown to A4 sized PDF. This is used
        /// for embedding tracking elements into the 
        /// read only versions of the document.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static byte[] MdToPdf(string md)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var html = Markdown.ToHtml(md, pipeline);
            return HtmlToPdf(html).GetAwaiter().GetResult();
            
        }
    }
}
