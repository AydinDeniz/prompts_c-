
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public class AsyncFileProcessor
{
    public static async Task Main(string[] args)
    {
        string inputFile = "large_text_file.txt"; // Path to the massive input file
        string outputFile = "filtered_results.txt"; // Path for the output file
        string pattern = "your_pattern"; // Define the pattern or keyword to search for

        using var cancellationTokenSource = new CancellationTokenSource();
        var progress = new Progress<int>(percent => Console.WriteLine($"{percent}% processed"));

        Console.WriteLine("Processing started. Press 'C' to cancel.");
        Task cancelTask = Task.Run(() =>
        {
            while (Console.ReadKey(true).Key != ConsoleKey.C) { }
            cancellationTokenSource.Cancel();
        });

        try
        {
            await ProcessFileAsync(inputFile, outputFile, pattern, progress, cancellationTokenSource.Token);
            Console.WriteLine("Processing completed.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Processing was canceled.");
        }
    }

    private static async Task ProcessFileAsync(string inputFile, string outputFile, string pattern, IProgress<int> progress, CancellationToken cancellationToken)
    {
        Regex regex = new Regex(pattern, RegexOptions.Compiled);
        int totalLines = File.ReadLines(inputFile).Count();
        int processedLines = 0;

        using var outputStream = new StreamWriter(outputFile, append: false);
        using var inputStream = new StreamReader(inputFile);

        while (!inputStream.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string line = await inputStream.ReadLineAsync();
            processedLines++;

            if (regex.IsMatch(line))
            {
                await outputStream.WriteLineAsync(line);
            }

            int percentComplete = (int)((double)processedLines / totalLines * 100);
            progress.Report(percentComplete);
        }
    }
}
