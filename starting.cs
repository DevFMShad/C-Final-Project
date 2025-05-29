// well this is just a start 
// from here on out I will talk about the propject and what I have learned about the project so far
// my understanding of things and I will keeep on commiting them in github
// Now I understand that it a bit tough project but I have had prior knowledge of C# before
// so it could be easy for me or the basic could be a bit tough
// does not matter either way, casuse I will do it and not matter what
// I will do it better than most



// well it has been a few days
// I had an exam, so i way busy
// now from What I understand from the proejct specs that I just saw 
// here is what needs to be done-
// Scanner A: Reads .txt files from a directory, indexes words (filename, word, count), and sends data to the Master via a named pipe.

// Scanner B: Same as Scanner A but operates on a different directory and uses a separate named pipe.

// Master: Collects indexed word data from both scanners via named pipes, aggregates it, and outputs a consolidated word index (e.g., fileA.txt:example:3).

// Key Requirements:
// Multithreading: Each program must use multiple threads (e.g., one for file reading, another for communication).

// CPU Core Affinity: Each program must be pinned to a specific CPU core using ProcessorAffinity.

// Named Pipes: Use NamedPipeServerStream (Master) and NamedPipeClientStream (Agents) for communication.

// Output: Master outputs a consolidated word index.

// Deliverables:
// Code on a public GitHub repository with clear version history and readme.md.

// A test report (PDF) with UML diagram, implementation details, functionality, challenges, and screenshots.

// A zipped project with the compiled debug folder.

