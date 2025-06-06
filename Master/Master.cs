using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Master
{
    class Program
    {
        private static readonly ConcurrentDictionary<string, Dictionary<string, int>> aggregatedData = new();

        static async Task Main(string[] args)
        {
            Console.Title = "Master"; // Set console window title
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << 2); // Core 2
            }
            else
            {
                LogError("CPU affinity not supported on this platform.");
            }

            LogHeader("Master Process Started");
            LogStatus("Listening for agents on pipes 'agent1' and 'agent2'...");

            try
            {
                var tasks = new[]
                {
                    Task.Factory.StartNew(() => HandlePipe("agent1")),
                    Task.Factory.StartNew(() => HandlePipe("agent2"))
                };

                LogStatus("Press any key to display results and exit...");
                Console.ReadKey();

                await Task.WhenAll(tasks);
                DisplayResults();
            }
            catch (Exception ex)
            {
                LogError($"Error in Master: {ex.Message}");
            }
        }

        private static void HandlePipe(string pipeName)
        {
            try
            {
                using var pipe = new NamedPipeServerStream(pipeName, PipeDirection.In);
                LogStatus($"Waiting for connection on pipe {pipeName}...");
                pipe.WaitForConnection();
                LogStatus($"Connected to {pipeName}");

                using var reader = new StreamReader(pipe);
                while (true)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;

                    string[] parts = line.Split('|');
                    if (parts.Length == 3 && int.TryParse(parts[2], out int count))
                    {
                        string fileName = parts[0], word = parts[1];
                        aggregatedData.AddOrUpdate(
                            fileName,
                            new Dictionary<string, int> { { word, count } },
                            (key, dict) =>
                            {
                                dict[word] = dict.GetValueOrDefault(word, 0) + count;
                                return dict;
                            });
                        LogData($"Received from {pipeName}: {fileName,-30} | Word: {word,-15} | Count: {count}");
                    }
                }
                LogStatus($"Pipe {pipeName} closed.");
            }
            catch (Exception ex)
            {
                LogError($"Error handling pipe {pipeName}: {ex.Message}");
            }
        }

        private static void DisplayResults()
        {
            LogHeader("Consolidated Word Index");
            if (aggregatedData.IsEmpty)
            {
                LogError("No data received from agents.");
                return;
            }

            int totalFiles = aggregatedData.Count;
            int totalUniqueWords = aggregatedData.Sum(f => f.Value.Count);
            int totalWordCount = aggregatedData.Sum(f => f.Value.Sum(w => w.Value));

            LogStatus($"Summary: {totalFiles} files, {totalUniqueWords} unique words, {totalWordCount} total occurrences");

            foreach (var file in aggregatedData.OrderBy(f => f.Key))
            {
                Console.WriteLine();
                LogData($"File: {file.Key}");
                Console.WriteLine($"  {"Word",-15} {"Count",5}");
                Console.WriteLine($"  {"-",15} {"-",5}");
                foreach (var word in file.Value.OrderBy(w => w.Key))
                {
                    Console.WriteLine($"  {word.Key,-15} {word.Value,5}");
                }
            }
            LogHeader("End of Report");
        }

        // Helper methods for formatted, colored output
        private static void LogHeader(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ===== {message} =====");
            Console.ResetColor();
        }

        private static void LogStatus(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
            Console.ResetColor();
        }

        private static void LogData(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
            Console.ResetColor();
        }

        private static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
            Console.ResetColor();
        }
    }
}