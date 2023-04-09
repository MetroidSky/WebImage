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
            if (!Directory.Exists("Downloaded Images"))
            {
                Directory.CreateDirectory("Downloaded Images");
            }

            Regex imageURL = new Regex("(https?:)//[^'\\\"<>]+?\\.(jpg|jpeg|gif|png|svg)");
            Regex imgTag = new Regex("<img.+?src=[\"'](.+?)[\"'].*?>");

            using (HttpResponseMessage res = await new HttpClient().GetAsync(uri))
            {
                using (HttpContent content = res.Content)
                {
                    if (imageURL.IsMatch(uri.ToString()))
                    {
                        using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\Downloaded Images\\" + uri.LocalPath, FileMode.Create, FileAccess.Write))
                        {
                            Console.WriteLine($"Downloading file {uri.ToString()}...");
                            await content.CopyToAsync(file);
                        }
                    }

                    else
                    {
                        var matches = imgTag.Matches(content.ReadAsStringAsync().Result);

                        for (int i = 0; i < matches.Count; i++)
                        {
                            string image = Regex.Match(matches[i].ToString(), imageURL.ToString()).ToString();
                            Uri imageUri = new Uri(image);

                            Console.WriteLine($"Downloading file {image}...");

                            using (HttpResponseMessage imageRes = await new HttpClient().GetAsync(image))
                            {
                                using (HttpContent imageContent = imageRes.Content)
                                {
                                    try
                                    {
                                        using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\Downloaded Images\\" + imageUri.LocalPath, FileMode.Create, FileAccess.Write))
                                        {
                                            await imageContent.CopyToAsync(file);
                                        }
                                    }

                                    catch (DirectoryNotFoundException)
                                    {
                                        using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\Downloaded Images\\" + (i + 1).ToString() + Path.GetExtension(imageUri.ToString()), FileMode.Create, FileAccess.Write))
                                        {
                                            await imageContent.CopyToAsync(file);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
    }
