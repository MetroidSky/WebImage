using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

    class Start
    {
        static async Task Main()
        {
            Console.WriteLine("Enter URL:");
            uri = new Uri(Console.ReadLine());

            await DownloadImages();

            Console.WriteLine("Done.");
            Console.ReadKey();
        }

        public static Uri uri { get; set; }
        static async Task DownloadImages()
        {
            Regex imageURL = new Regex("(https?:)//[^'\\\"<>]+?\\.(jpg|jpeg|gif|png|svg)");

            using (HttpResponseMessage res = await new HttpClient().GetAsync(uri))
            {
                using (HttpContent content = res.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    if (imageURL.IsMatch(uri.ToString()))
                    {
                        using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\" + uri.LocalPath, FileMode.Create, FileAccess.Write))
                        {
                            await content.CopyToAsync(file);
                        }
                    }
                }
            }

            Console.WriteLine($"Downloaded file {uri.ToString()}");
        }
    }
