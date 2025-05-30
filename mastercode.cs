// using System;
// using System.Collections.Concurrent;
// using System.Diagnostics;
// using System.IO.Pipes;
// using System.Threading.Tasks;

// class Program
// {
//     static ConcurrentDictionary<string, Dictionary<string, int>> aggregatedData = new();

//     static void Main(string[] args)
//     {
//         Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << 2); // Core 2
//         Task.Factory.StartNew(() => HandlePipe("agent1"));
//         Task.Factory.StartNew(() => HandlePipe("agent2"));
//         Console.ReadLine(); // Keep console open
//     }

//     static void HandlePipe(string pipeName)
//     {
//         using var pipe = new NamedPipeServerStream(pipeName, PipeDirection.In);
//         pipe.WaitForConnection();
//         using var reader = new StreamReader(pipe);
//         while (!reader.EndOfStream)
//         {
//             var line = reader.ReadLine();
//             var parts = line.Split('|');
//             if (parts.Length == 3)
//             {
//                 string fileName = parts[0], word = parts[1];
//                 int count = int.Parse(parts[2]);
//                 aggregatedData.AddOrUpdate(fileName,
//                     new Dictionary<string, int> { { word, count } },
//                     (key, dict) => { dict[word] = dict.GetValueOrDefault(word, 0) + count; return dict; });
//             }
//         }

//         // Print aggregated results
//         foreach (var file in aggregatedData)
//             foreach (var word in file.Value)
//                 Console.WriteLine($"{file.Key}:{word.Key}:{word.Value}");
//     }
// }

// well this here is a master coding file snippets
// i mean i am trying to reach the ending
// here I am adding the skeleton codes for now