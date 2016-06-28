using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI
{
    class Program
    {
        //        static string _address = "http://maps.googleapis.com/maps/api/staticmap?center=Redmond,WA&zoom=14&size=400x400&sensor=false";
        static string _address = "http://marketdata.websol.barchart.com/getQuote.json?key=ca9d9a00e8af923de14b7c9096acfa23&symbols=ZC*1,IBM";
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();

            // Send asynchronous request
            client.GetAsync(_address).ContinueWith(
                (requestTask) =>
                {
                    // Get HTTP response from completed task.
                    HttpResponseMessage response = requestTask.Result;

                    // Check that response was successful or throw exception
                    response.EnsureSuccessStatusCode();

                    // Read response asynchronously and save to file
                    response.Content.ReadAsFileAsync("output.png", true).ContinueWith(
                        (readTask) =>
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = "output.png";
                            process.Start();
                        });
                });
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }
    }

    public static class HttpContentExtensions
    {
        public static Task ReadAsFileAsync(this HttpContent content, string filename, bool overwrite)
        {
            string pathname = Path.GetFullPath(filename);
            if (!overwrite && File.Exists(filename))
            {
                throw new InvalidOperationException(string.Format("File {0} already exists.", pathname));
            }
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
                return content.CopyToAsync(fileStream).ContinueWith(
                (copyTask) =>
                {
                    fileStream.Close();
                });
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                throw;
            }
        }
    }
}


