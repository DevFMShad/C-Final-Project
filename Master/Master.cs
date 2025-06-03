using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Master
{
    class Program
    {
        
        private static readonly ConcurrentDictionary<string, Dictionary<string, int>> aggregatedData = new();

        static void Main(string[] args)
        {
            // Set CPU affinity (Core 2)
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << 2);
            Console.WriteLine("Master started. Listening for agents...");

            try
            {
                // Start separate tasks to handle each agent's pipe
                Task.Factory.StartNew(() => HandlePipe("agent1"));
                Task.Factory.StartNew(() => HandlePipe("agent2"));

                // Wait for user input to display results and exit
                Console.WriteLine("Press any key to display results and exit.");
                Console.ReadKey();

                // Display aggregated results
                DisplayResults();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Handle data from a named pipe
        private static void HandlePipe(string pipeName)
        {
            try
            {
                using var pipe = new NamedPipeServerStream(pipeName, PipeDirection.In);
                Console.WriteLine($"Waiting for connection on pipe {pipeName}...");
                pipe.WaitForConnection();
                Console.WriteLine($"Connected to {pipeName}");

                using var reader = new StreamReader(pipe);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;

                    // Parse data: filename|word|count
                    string[] parts = line.Split('|');
                    if (parts.Length == 3 && int.TryParse(parts[2], out int count))
                    {
                        string fileName = parts[0], word = parts[1];

                        // Update aggregated data thread-safely
                        aggregatedData.AddOrUpdate(
                            fileName,
                            new Dictionary<string, int> { { word, count } },
                            (key, dict) =>
                            {
                                dict[word] = dict.GetValueOrDefault(word, 0) + count;
                                return dict;
                            });
                        Console.WriteLine($"Received from {pipeName}: {line}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling pipe {pipeName}: {ex.Message}");
            }
        }

        // Display aggregated word index
        private static void DisplayResults()
        {
            Console.WriteLine("\nConsolidated Word Index:");
            foreach (var file in aggregatedData.OrderBy(f => f.Key))
            {
                foreach (var word in file.Value.OrderBy(w => w.Key))
                {
                    Console.WriteLine($"{file.Key}:{word.Key}:{word.Value}");
                }
            }
        }
    }
}