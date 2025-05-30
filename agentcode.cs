// using System;
// using System.Collections.Concurrent;
// using System.Diagnostics;
// using System.IO;
// using System.IO.Pipes;
// using System.Linq;
// using System.Threading.Tasks;

// class Program
// {
//     static ConcurrentDictionary<string, Dictionary<string, int>> indexedData = new();

//     static void Main(string[] args)
//     {
//         Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << 0); // Core 0
//         string directoryPath = args.Length > 0 ? args[0] : "C:\\TestFiles";
//         string pipeName = "agent1";

//         Task.Factory.StartNew(() => ReadAndIndexFiles(directoryPath));
//         Task.Factory.StartNew(() => SendDataToMaster(pipeName));
//         Console.ReadLine(); // Keep console open
//     }

//     static void ReadAndIndexFiles(string directoryPath)
//     {
//         if (!Directory.Exists(directoryPath)) return;

//         foreach (var file in Directory.GetFiles(directoryPath, "*.txt"))
//         {
//             var words = File.ReadAllText(file).ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
//             var wordCounts = words.GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());
//             indexedData.TryAdd(Path.GetFileName(file), wordCounts);
//         }
//     }

//     static void SendDataToMaster(string pipeName)
//     {
//         using var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
//         pipe.Connect();
//         using var writer = new StreamWriter(pipe);
//         foreach (var file in indexedData)
//             foreach (var word in file.Value)
//                 writer.WriteLine($"{file.Key}|{word.Key}|{word.Value}");
//         writer.Flush();
//     }
// }



// all of these codes here is only sample for the agent or scanner 
// This is the basic codes for that file
// well since the two agents will have almost same type of code, this is the most important part