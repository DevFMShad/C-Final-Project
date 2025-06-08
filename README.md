# 📚 Distributed Word Indexing System 🚀

Welcome to the **Distributed Word Indexing System**, a robust C# project that indexes words from text files using a distributed architecture! 🌟 Built as a final project, this system leverages **named pipes**, **multithreading**, and **CPU affinity** to process text files in parallel across three console applications: `ScannerA`, `ScannerB`, and `Master`. The output is vibrant, color-coded, and formatted with timestamps and tabulated results for a professional and engaging experience.

---

## ✨ Project Description

This project demonstrates efficient inter-process communication and parallel processing to create a consolidated word index from `.txt` files. Here’s how it works:

- **ScannerA** 📑: Indexes words from `.txt` files in a specified directory (e.g., `C:\Users\fuade\Downloads\Tests\small test`) and sends results to `Master` via the `agent1` named pipe, pinned to CPU Core 0.
- **ScannerB** 📜: Similar to `ScannerA`, but operates independently, uses the `agent2` named pipe, and is pinned to CPU Core 1.
- **Master** 🧠: Receives word counts from both Scanners, aggregates them into a `ConcurrentDictionary`, and displays a formatted word index, pinned to CPU Core 2.

🔑 **Key Features**:
- 🎨 **Color-Coded Output**: Green headers, cyan status messages, yellow data, and red errors for clarity.
- ⏰ **Timestamps**: Real-time logging for all actions.
- 📊 **Tabulated Results**: Neatly formatted word counts with summary statistics (files, unique words, total occurrences).
- 🧵 **Multithreading**: Concurrent file indexing and data transfer using `Task.Factory`.
- ⚙️ **CPU Affinity**: Optimizes performance by pinning processes to specific CPU cores.
- 🔗 **Named Pipes**: Reliable inter-process communication with retry logic for robust connections.
- 🛡️ **Error Handling**: Handles invalid paths, empty directories, and connection failures gracefully.

---

## 📋 Table of Contents

- [✨ Project Description](#-project-description)
- [🛠️ How to Install](#️-how-to-install)
- [🚀 How to Use](#-how-to-use)
- [📸 Example Output](#-example-output)
- [🧑‍💻 Contributing](#-contributing)
- [📜 License](#-license)

---

## 🛠️ How to Install

Let’s get this word-indexing powerhouse up and running! 🖥️ Follow these steps to set up the project on a Windows machine (tested on Windows 10/11, version 10.0.26100.4061).

### Prerequisites
- **.NET 8 SDK** 🛠️: [Download here](https://dotnet.microsoft.com/download/dotnet/8.0).
  - Verify with: `dotnet --version` (should show `8.0.xxx`).
- **Git** 🌐: For cloning the repository.
- **Visual Studio Code** (optional) 📝: Recommended for development and debugging.
  - Install the **C# Extension** (by Microsoft) for IntelliSense and debugging support.

### Installation Steps
1. **Clone the Repository** 📂:
   ```bash
   git clone https://github.com/your-username/C-Final-Project.git
   cd C-Final-Project


Verify Project Structure :
Ensure the following structure exists in C:\Users\fuade\OneDrive\Documents\GitHub\C-Final-Project:


C-Final-Project/
├── ScannerA/
│   ├── Program.cs
│   ├── ScannerA.csproj
├── ScannerB/
│   ├── Program.cs
│   ├── ScannerB.csproj
├── Master/
│   ├── Program.cs
│   ├── Master.csproj
├── README.md


Create Test Files :
Create a directory for test files, e.g., C:\Users\fuade\Downloads\Tests\small test.

Add sample .txt files:
mkdir "C:\Users\fuade\Downloads\Tests\small test"
echo hello world hello > "C:\Users\fuade\Downloads\Tests\small test\Test document.txt"
echo test hello world > "C:\Users\fuade\Downloads\Tests\small test\test2.txt"

Build the Solution :
cd C:\Users\fuade\OneDrive\Documents\GitHub\C-Final-Project
dotnet build WordIndexingSolution.sln

If the solution file is missing, create it:
dotnet new sln -n WordIndexingSolution
dotnet sln WordIndexingSolution.sln add ScannerA\ScannerA.csproj
dotnet sln WordIndexingSolution.sln add ScannerB\ScannerB.csproj
dotnet sln WordIndexingSolution.sln add Master\Master.csproj
dotnet build WordIndexingSolution.sln

 How to Use
Launch the applications in separate terminal windows to see the system in action!  The Master must run first to listen for connections from ScannerA and ScannerB.
Step-by-Step Guide

Open Visual Studio Code (optional):
cd C:\Users\fuade\OneDrive\Documents\GitHub\C-Final-Project
code .

Run Master :
Open a terminal (in VS Code or Command Prompt).Start the Master to listen on named pipes agent1 and agent2:

cd C:\Users\fuade\OneDrive\Documents\GitHub\C-Final-Project\Master
dotnet run

Expected output:

[09:13:00] ===== Master Process Started =====
[09:13:00] Listening for agents on pipes 'agent1' and 'agent2'...
[09:13:01] Waiting for connection on pipe agent1...
[09:13:01] Waiting for connection on pipe agent2...
[09:13:01] Press any key to display results and exit...

Run ScannerA :
Open a new terminal (in VS Code, use Terminal > New Terminal). Start ScannerA to index files and send data to Master:

cd C:\Users\fuade\OneDrive\Documents\GitHub\C-Final-Project\ScannerA
dotnet run -- "C:\Users\fuade\Downloads\Tests\small test"

Expected output:

[09:13:02] ===== Scanner A Started =====
Enter directory path or press Enter to use default (C:\TestFiles):
[09:13:02] Indexing files in C:\Users\fuade\Downloads\Tests\small test...
[09:13:02] Indexed: Test document.txt                | Words: 2
[09:13:02] Total files indexed: 1
[09:13:02] Indexing complete. Sending data to Master...
[09:13:02] Connecting to Master pipe agent1...
[09:13:03] Connected to agent1
[09:13:03] Sent: Test document.txt                | Word: hello           | Count: 2
[09:13:03] Sent: Test document.txt                | Word: world           | Count: 1
[09:13:03] Data transfer to agent1 complete.
[09:13:03] ===== Scanner A Completed =====
Press any key to exit.

Run ScannerB :
Open another terminal. Start ScannerB:

cd C:\Users\fuade\OneDrive\Documents\GitHub\C-Final-Project\ScannerB
dotnet run -- "C:\Users\fuade\Downloads\Tests\small test"

Expected output (similar to ScannerA, with “Scanner B” and agent2).

View Results in Master :
Return to the Master terminal.

Press any key to display the consolidated word index:

[09:13:05] ===== Consolidated Word Index =====
[09:13:05] Summary: 2 files, 3 unique words, 10 total occurrences

[09:13:05] File: Test document.txt
  Word            Count
  --------------- -----
  hello             4
  world             2

[09:13:05] File: test2.txt
  Word            Count
  --------------- -----
  hello             2
  test              1
  world             1
[09:13:05] ===== End of Report =====


Tips

Run Master First : Ensures named pipes are available for Scanners.
Test with Multiple Files : Add more .txt files to demonstrate aggregation.
Debug in VS Code : Use breakpoints and the C# extension for step-by-step debugging (F5).
Handle Spaces in Paths : The code supports paths with spaces (e.g., small test) using proper quoting.


 License
This project is licensed under the MIT License. Feel free to use, modify, and share! 

 Happy Indexing! 
 Created by Fuad Mahmud Shad for the C# Final Project, June 2025.
 GitHub Repository
 Last Updated: June 8, 2025

---

### How to Add the README
1. **Create the File**:
   - Navigate to your project directory:
     ```bash
     cd C:\Users\fuade\OneDrive\Documents\GitHub\C-Final-Project
     echo. > README.md
     ```
   - Open `README.md` in VS Code and paste the content above.

2. **Customize**:
   - Replace `your-username` with your actual GitHub username in the clone URL and footer.
   - Add your name in the footer (“Created by [Your Name]”).
   - If you have specific test file names or additional features, update the example output or description.

3. **Commit to Git**:
   ```bash
   git add README.md
   git commit -m "Added polished README with emojis, instructions, and example output"
   git push origin main



