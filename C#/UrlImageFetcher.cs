
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class UrlImageFetcher
{
    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task Main(string[] args)
    {
        // Define the list of URLs to process.
        List<string> urls = new List<string> 
        {
            "https://example.com", // Add URLs here
            "https://example.org",
            "https://example.net"
        };

        List<Task> tasks = urls.Select(url => ProcessUrlAsync(url)).ToList();
        
        await Task.WhenAll(tasks);
        
        Console.WriteLine("Processing complete. Check 'image_links_log.txt' for the results.");
    }

    public static async Task ProcessUrlAsync(string url)
    {
        try
        {
            string htmlContent = await httpClient.GetStringAsync(url);
            List<string> imageLinks = ExtractImageLinks(htmlContent);

            // Log results to a file
            await LogResultsAsync(url, imageLinks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing URL {url}: {ex.Message}");
            await File.AppendAllTextAsync("image_links_log.txt", $"Error processing {url}: {ex.Message}\n");
        }
    }

    public static List<string> ExtractImageLinks(string htmlContent)
    {
        List<string> imageLinks = new List<string>();
        Regex imgTagRegex = new Regex("<img[^>]*src=["']([^"']+)["']", RegexOptions.IgnoreCase);

        foreach (Match match in imgTagRegex.Matches(htmlContent))
        {
            imageLinks.Add(match.Groups[1].Value);
        }

        return imageLinks;
    }

    public static async Task LogResultsAsync(string url, List<string> imageLinks)
    {
        using (StreamWriter writer = new StreamWriter("image_links_log.txt", append: true))
        {
            await writer.WriteLineAsync($"URL: {url}");
            foreach (string link in imageLinks)
            {
                await writer.WriteLineAsync($"Image Link: {link}");
            }
            await writer.WriteLineAsync("---------------------------------------------------");
        }
    }
}
