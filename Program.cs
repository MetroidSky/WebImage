﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

    class Start
    {
        static async Task Main()
        {
            // Input URL
            Console.WriteLine("Enter URL:");
            uri = new Uri(Console.ReadLine());
            Console.WriteLine("\nDownloading...\n");

            await DownloadImages();

            Console.WriteLine("Done.");
            Console.ReadKey();
        }

        public static Uri uri { get; set; }
        static async Task DownloadImages()
        {
            // Create directory to download images to
            if (!Directory.Exists("Downloaded Images"))
            {
                Directory.CreateDirectory("Downloaded Images");
            }

            Regex imageURL = new Regex("(https?:)?/[^'\n\\\"<>]+?\\.(jpg|jpeg|gif|png|svg|webp)");

            using (HttpResponseMessage res = await new HttpClient().GetAsync(uri))
            {
                using (HttpContent content = res.Content)
                {
                    // Download the URL if it is already an image
                    if (imageURL.IsMatch(uri.ToString()))
                    {
                        try
                        {
                            using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\Downloaded Images\\" + uri.LocalPath, FileMode.Create, FileAccess.Write))
                            {
                                Console.WriteLine($"Downloading file {uri.ToString()}...");
                                await content.CopyToAsync(file);
                            }
                        }

                        catch (DirectoryNotFoundException)
                        {
                            using (FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\Downloaded Images\\" + "1", FileMode.Create, FileAccess.Write))
                            {
                                Console.WriteLine($"Downloading file {uri.ToString()}...");
                                await content.CopyToAsync(file);
                            }
                        }
                    }

                    else
                    {
                        // Find all image links in URL source code
                        var matches = imageURL.Matches(content.ReadAsStringAsync().Result);

                        for (int i = 0; i < matches.Count; i++)
                        {
                            string image = Regex.Match(matches[i].ToString(), imageURL.ToString()).ToString();

                            // Fix links without an authority 
                            if (!(matches[i].ToString().StartsWith("http")))
                            {
                                if (uri.ToString().EndsWith("/"))
                                {
                                    image = Regex.Match((uri.ToString().Remove(uri.ToString().Length - 1, 1) + matches[i].ToString()), imageURL.ToString()).ToString();
                                }

                                else
                                {
                                    image = Regex.Match((uri.ToString() + matches[i].ToString()), imageURL.ToString()).ToString();
                                }
                            }

                            Uri imageUri = new Uri(image);

                            Console.WriteLine($"Downloading file {image}...");
                            
                            // Download the image
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

                        Console.WriteLine($"\n{matches.Count} images downloaded.");
                    }
                }
            }

        }
    }
