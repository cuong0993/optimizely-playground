using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using EPiServer.Logging;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace AlloyDemo.Business.Channels
{
    public class PDFChannelHelper
    {
        public static void GeneratePDF(string Html, string filename)
        {
            var context = HttpContext.Current;

            HttpContext.Current.Response.ContentType = "application/pdf";
            HttpContext.Current.Response.AddHeader("content-disposition", $"attachment;filename={filename}.pdf");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //Create PDF document
            var doc = new Document(PageSize.A4);
            var parser = new HTMLWorker(doc);

            PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream);
            doc.Open();

            try
            {
                /********************************************************************************/
                //Try adding source strings for each image in content
                var tempContent = getImage(Html);
                /*********************************************************************************/
                var rdr = new StringReader(tempContent);

                //Parse Html and dump the result in PDF file
                parser.Parse(rdr);
            }
            catch (Exception ex)
            {
                //Display parser errors in PDF.
                var paragraph = new Paragraph("Error! " + ex.Message);
                var text = paragraph.Chunks[0];
                if (text != null) text.Font.Color = BaseColor.RED;
                doc.Add(paragraph);
            }
            finally
            {
                doc.Close();
            }

            HttpContext.Current.Response.Write(doc);
            HttpContext.Current.Response.End();
        }

        public static string getImage(string input)
        {
            if (input == null)
                return string.Empty;
            var tempInput = input;
            var pattern = @"<img(.|\n)+?>";
            var src = string.Empty;
            var context = HttpContext.Current;

            //Change the relative URL's to absolute URL's for an image, if any in the HTML code.
            foreach (Match m in Regex.Matches(input, pattern,
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.RightToLeft))
                if (m.Success)
                {
                    var tempM = m.Value;
                    var pattern1 = "src=[\'|\"](.+?)[\'|\"]";
                    var reImg = new Regex(pattern1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    var mImg = reImg.Match(m.Value);

                    if (mImg.Success)
                    {
                        src = mImg.Value.ToLower().Replace("src=", "").Replace("\"", "");

                        if (src.ToLower().Contains("http://") == false)
                        {
                            //Insert new URL in img tag
                            src = "src=\"" + context.Request.Url.Scheme + "://" +
                                  context.Request.Url.Authority + src + "\"";
                            try
                            {
                                tempM = tempM.Remove(mImg.Index, mImg.Length);
                                tempM = tempM.Insert(mImg.Index, src);

                                //insert new url img tag in whole html code
                                tempInput = tempInput.Remove(m.Index, m.Length);
                                tempInput = tempInput.Insert(m.Index, tempM);
                            }
                            catch (Exception e)
                            {
                                var _logger = LogManager.GetLogger(typeof(PDFChannelHelper));
                                _logger.Warning("Could not insert url img tag in PDF", e);
                            }
                        }
                    }
                }

            return tempInput;
        }
    }
}