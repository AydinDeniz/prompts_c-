
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

public class FileSearchAndExtract
{
    public static void Main(string[] args)
    {
        // Define the directory to search, file type, and regular expression pattern.
        string rootDirectory = @"/path/to/your/directory";  // Update this path to your directory
        string fileType = "*.txt";  // Define the file type to search for
        string regexPattern = @"your_regex_pattern";  // Define the regex pattern to match lines

        // Perform search and extraction
        List<SearchResult> results = SearchAndExtract(rootDirectory, fileType, regexPattern);

        // Output the results to a CSV file
        WriteResultsToCsv(results, "output.csv");

        Console.WriteLine("Search and extraction complete. Results saved to output.csv.");
    }

    public static List<SearchResult> SearchAndExtract(string rootDirectory, string fileType, string regexPattern)
    {
        List<SearchResult> results = new List<SearchResult>();
        Regex regex = new Regex(regexPattern, RegexOptions.Compiled);

        foreach (string file in Directory.EnumerateFiles(rootDirectory, fileType, SearchOption.AllDirectories))
        {
            int lineNumber = 0;
            foreach (string line in File.ReadLines(file))
            {
                lineNumber++;
                if (regex.IsMatch(line))
                {
                    results.Add(new SearchResult
                    {
                        FilePath = file,
                        LineNumber = lineNumber,
                        Text = line
                    });
                }
            }
        }

        return results;
    }

    public static void WriteResultsToCsv(List<SearchResult> results, string outputPath)
    {
        using (var writer = new StreamWriter(outputPath))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(results);
        }
    }
}

public class SearchResult
{
    public string FilePath { get; set; }
    public int LineNumber { get; set; }
    public string Text { get; set; }
}
