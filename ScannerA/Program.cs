using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace ScannerA
{
    class Program
    {
        // Thread-safe dictionary to store word index: filename -> word -> count
        private static readonly ConcurrentDictionary<string, Dictionary<string, int>> indexedData = new();

        static void Main(string[] args)
        {
            // Set CPU affinity (Core 0 for Scanner A, Core 1 for Scanner B)
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << 0); // Change to 1 << 1 for Scanner B
            Console.WriteLine("Scanner A started. Enter directory path or press Enter to use default.");

            // Get directory path from args or user input
            string directoryPath = args.Length > 0 ? args[0] : Console.ReadLine();
            if (string.IsNullOrEmpty(directoryPath)) directoryPath = @"C:\TestFiles";

            string pipeName = "agent1"; // Change to "agent2" for Scanner B

            try
            {
                // Start tasks for reading files and sending data
                Task.Factory.StartNew(() => ReadAndIndexFiles(directoryPath));
                Task.Factory.StartNew(() => SendDataToMaster(pipeName)).Wait(); // Wait for completion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Scanner A completed. Press any key to exit.");
            Console.ReadKey();
        }

        // Task 1: Read .txt files and index words
        private static void ReadAndIndexFiles(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Console.WriteLine($"Directory {directoryPath} does not exist.");
                    return;
                }

                foreach (string file in Directory.GetFiles(directoryPath, "*.txt"))
                {
                    try
                    {
                        // Read file content and split into words
                        string content = File.ReadAllText(file).ToLower();
                        string[] words = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        // Count word occurrences
                        var wordCounts = words.GroupBy(w => w)
                                             .ToDictionary(g => g.Key, g => g.Count());

                        // Store in indexedData
                        indexedData.TryAdd(Path.GetFileName(file), wordCounts);
                        Console.WriteLine($"Indexed file: {Path.GetFileName(file)}");
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Error reading file {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error indexing files: {ex.Message}");
            }
        }

        
    }
}