using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ScannerB
{
    class Program
    {
        private static readonly ConcurrentDictionary<string, Dictionary<string, int>> indexedData = new();

        static async Task Main(string[] args)
        {
            Console.Title = "Scanner B"; // Set console window title
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << 1); // Core 1
            }
            else
            {
                LogError("CPU affinity not supported on this platform.");
            }

            LogHeader("Scanner B Started");
            Console.WriteLine("Enter directory path or press Enter to use default (C:\\TestFiles):");

            string directoryPath = args.Length > 0 ? args[0].TrimStart('-') : Console.ReadLine() ?? @"C:\TestFiles";
            directoryPath = directoryPath.Trim();

            try
            {
                LogStatus($"\nIndexing files in {directoryPath}...");
                await Task.Factory.StartNew(() => ReadAndIndexFiles(directoryPath));
                LogStatus("Indexing complete. Sending data to Master...");
                await Task.Factory.StartNew(() => SendDataToMaster("agent2"));
            }
            catch (Exception ex)
            {
                LogError($"Error in Scanner B: {ex.Message}");
            }

            LogHeader("Scanner B Completed");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void ReadAndIndexFiles(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                LogError($"Directory {directoryPath} does not exist.");
                return;
            }

            var files = Directory.GetFiles(directoryPath, "*.txt");
            if (files.Length == 0)
            {
                LogError("No .txt files found in directory.");
                return;
            }

            foreach (string file in files)
            {
                try
                {
                    string content = File.ReadAllText(file).ToLower();
                    string[] words = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var wordCounts = words.GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());
                    indexedData.TryAdd(Path.GetFileName(file), wordCounts);
                    LogData($"Indexed: {Path.GetFileName(file),-30} | Words: {wordCounts.Count}");
                }
                catch (IOException ex)
                {
                    LogError($"Error reading file {file}: {ex.Message}");
                }
            }
            LogStatus($"Total files indexed: {files.Length}");
        }

        private static void SendDataToMaster(string pipeName)
        {
            try
            {
                using var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
                LogStatus($"Connecting to Master pipe {pipeName}...");
                
                int retries = 5;
                while (!pipe.IsConnected && retries-- > 0)
                {
                    try { pipe.Connect(1000); }
                    catch { Task.Delay(1000).Wait(); }
                }

                if (!pipe.IsConnected)
                {
                    LogError($"Failed to connect to Master pipe {pipeName}.");
                    return;
                }

                LogStatus($"Connected to {pipeName}");
                using var writer = new StreamWriter(pipe) { AutoFlush = true };
                foreach (var file in indexedData)
                {
                    foreach (var word in file.Value)
                    {
                        string data = $"{file.Key}|{word.Key}|{word.Value}";
                        writer.WriteLine(data);
                        LogData($"Sent: {file.Key,-30} | Word: {word.Key,-15} | Count: {word.Value}");
                    }
                }
                LogStatus($"Data transfer to {pipeName} complete.");
            }
            catch (Exception ex)
            {
                LogError($"Error sending data to Master: {ex.Message}");
            }
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