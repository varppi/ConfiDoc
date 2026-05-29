using Markdig;
using Markdig.Helpers;
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
using static System.Collections.Specialized.BitVector32;

namespace ConfidocLib
{
    public static class Pdf
    {
        public static async void Initialize()
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
        }

        /// <summary>
        /// Extracts data embedded in datamarked PDF images.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<string> ExtractDatamark(Image input)
        {
            List<string> outputs = new();
            for (int imageShift = 0; imageShift < 2; imageShift++)
            {
                Bitmap bmp = new(input);
                List<int> binaryValues = new();
                for (int x = imageShift; x < bmp.Width-imageShift; x++)
                {
                    var pixel = bmp.GetPixel(x, 5);
                    int r = pixel.R;
                    int g = pixel.G;
                    int b = pixel.B;
                    double brightness = 0.299 * r + 0.587 * g + 0.114 * b;
                    binaryValues.Add(brightness > 248 ? 0 : 1);
                }
                for (int shift = 0; shift < 17; shift++) {
                    for (int spacing = 1; spacing < 101; spacing++) {
                        var construct = "";
                        for (int i = 0; i < Math.Round((double)binaryValues.Count/spacing); i++)
                        {
                            int value = binaryValues[i*spacing];
                            construct += value;
                        }
                        construct =  new string('0', shift) + construct;
                        List<char> chars = new List<char>();
                        for (int i = 0; i < construct.Length; i += 8)
                        {
                            string byteString;
                            if (i + 8 > construct.Length)
                                continue;
                            byteString = construct.Substring(i, 8);
                            chars.Add((char)Convert.ToInt32(byteString, 2));
                        }
                        string decodedText = new string(chars.ToArray());
                        if (decodedText.Contains("000"))
                            outputs.Add(new string(decodedText.Where(c => !c.IsControl()).ToArray()));
                    }
                }
            }
            return outputs;
        }


        /// <summary>
        /// Tags the input image with whatever piece of text inputted.
        /// This will be used to tag every page of the PDF with the
        /// event ID of the download.
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Image Datamark(Image inputImage, string data)
        {
            data = $"000{data}000";
            Bitmap input = new Bitmap(inputImage);

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

                    int bit = bits[(int)Math.Round((decimal)x/6) % bitCount];

                    int r = original.R;
                    int g = original.G;
                    int b = original.B;

                    if (bit == 1)
                    {
                        r = Math.Max(r-10, 0);
                        g = Math.Max(g-10, 0);
                        b = Math.Max(b-10, 0);
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
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] PdfToImagePdf(byte[] pdfData, string data="a")
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
                var markedImage = Datamark(image, data);
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
