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
        
        private static readonly ConcurrentDictionary<string, Dictionary<string, int>> indexedData = new();

        static void Main(string[] args)
        {
            
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << 0); 
            Console.WriteLine("Scanner A started. Enter directory path or press Enter to use default.");

            
            string directoryPath = args.Length > 0 ? args[0] : Console.ReadLine();
            if (string.IsNullOrEmpty(directoryPath)) directoryPath = @"C:\TestFiles";

            string pipeName = "agent1"; 

            try
            {
                
                Task.Factory.StartNew(() => ReadAndIndexFiles(directoryPath));
                Task.Factory.StartNew(() => SendDataToMaster(pipeName)).Wait(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Scanner A completed. Press any key to exit.");
            Console.ReadKey();
        }

        
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