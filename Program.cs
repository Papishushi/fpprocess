using System.Diagnostics;

bool running = true;
bool isColorOutput = true;
bool isIntervalMode = false;
Stopwatch stopWatch = new Stopwatch();
int interval = 1500;

void StopRunning(object? sender, ConsoleCancelEventArgs args) => running = false;

Console.CancelKeyPress += StopRunning;

if (args.Length > 0 && args.Contains("--no-color"))
{
    isColorOutput = false;
    args = args.Except(new[] { "--no-color" }).ToArray();
}

if (args.Length > 0 && args.Contains("-i"))
{
    var temp = string.Empty;
    for (int i = 0; i < args.Length; i++)
        if (args[i] == "-i")
        {
            try
            {
                temp = args[i + 1];
                interval = Convert.ToInt32(temp);
                break;
            }
            catch
            {
                return;
            }
        }
    isIntervalMode = true;
    args = args.Except(new[] { "-i" , temp}).ToArray();
}

stopWatch.Start();
do
{
    if (stopWatch.ElapsedMilliseconds < interval && isIntervalMode) continue;
    stopWatch.Restart();
    MainAlgorithm();
} while (isIntervalMode);
stopWatch.Stop();

void MainAlgorithm()
{
    foreach (var arg in args)
    {
        var nameMode = false;
        var tab = string.Empty;
        var tid = 0;
        var id = 0;
        Process p = Process.GetProcessById(0);

        try
        {
            id = Convert.ToInt32(arg);
        }
        catch
        {
            try
            {
                p = Process.GetProcessesByName(arg).First();
                nameMode = true;
            }
            catch
            {
                WriteLineColor($"\nArgument not correctly formatted or process not running '{arg}'", isColorOutput ? ConsoleColor.DarkRed : Console.ForegroundColor);
                continue;
            }
        }

        Process? parent = null;

        while (running)
        {
            if (tid != 0)
                id = tid;
            try
            {
                if (tid != 0 || !nameMode) p = Process.GetProcessById(id);
                if (p.HasExited) break;

                WriteLineColor($"\n{tab}Get Info from process {p.Id} ({p.ProcessName})", isColorOutput ? ConsoleColor.Green : Console.ForegroundColor);

                parent = GetParent(p);

                if (parent != null)
                {
                    tid = parent.Id;
                    if (tid == 0) break;
                }

                if (isColorOutput)
                    DisplayProcessInfo(tab, p, parent);
                else
                    DisplayProcessInfoWhite(tab, p, parent);

                if (parent == null) break;
                else if (parent.HasExited)
                {
                    tab += "\t";
                    WriteLineColor($"\n{tab}Process {parent.Id} has exited... Aborting operations", isColorOutput ? ConsoleColor.DarkYellow : Console.ForegroundColor);
                    break;
                }
            }
            catch (ArgumentException ex)
            {
                WriteLineColor($"\n{ex.Message} {ex.InnerException?.Message}", isColorOutput ? ConsoleColor.DarkRed : Console.ForegroundColor);
                break;
            }
            catch (Exception ex)
            {
                WriteLineColor($"{ex.Message} {ex.InnerException?.Message}", isColorOutput ? ConsoleColor.DarkRed : Console.ForegroundColor);
                if (parent == null) break;
                tab += "\t";
                continue;
            }

            tab += "\t";
        }
    }
}

static void WriteLineColor(string content, ConsoleColor color, params ConsoleColor[] background)
{
    Console.BackgroundColor = background.FirstOrDefault();
    Console.ForegroundColor = color;
    Console.WriteLine(content);
    Console.ResetColor();
}

static string FindIndexedProcessName(int pId)
{
    try
    {
        var processName = Process.GetProcessById(pId).ProcessName;
        var processesByName = Process.GetProcessesByName(processName);
        var processIdName = string.Empty;

        for (var i = 0; i < processesByName.Length; i++)
        {
            processIdName = i == 0 ? processName : $"{processName}#{i}";
            var processId = new PerformanceCounter("Process", "ID Process", processIdName);
            if ((int)processId.NextValue() == pId) return processIdName;
        }

        return processIdName;
    }
    catch
    {
        return string.Empty;
    }
}

static Process? FindPidFromIndexedProcessName(string indexedProcessName)
{
    try
    {
        return Process.GetProcessById((int)new PerformanceCounter("Process", "Creating Process ID", indexedProcessName).NextValue());
    }
    catch
    {
        return null;
    }
}

static Process? GetParent(Process process) => FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));

static void DisplayProcessInfo(string tab, Process p, Process? parent)
{
    Console.Write($"{tab}Parent PID         |  "); WriteLineColor($"{parent?.Id}", ConsoleColor.Gray, ConsoleColor.DarkBlue);
    Console.Write($"{tab}Name               |  "); WriteLineColor($"{p.ProcessName}", ConsoleColor.Gray);
    Console.Write($"{tab}Working Set (x64)  |  "); WriteLineColor($"{(float)(p.WorkingSet64 * 0.0000001)} mb", ConsoleColor.Gray);
    Console.Write($"{tab}Module Name        |  "); WriteLineColor($"{p.MainModule?.ModuleName}", ConsoleColor.DarkMagenta);
    Console.Write($"{tab}Module Path        |  "); WriteLineColor($"{p.MainModule?.FileName.Replace(p.MainModule?.ModuleName ?? string.Empty, string.Empty)}", ConsoleColor.DarkMagenta);
}

static void DisplayProcessInfoWhite(string tab, Process p, Process? parent)
{
    Console.WriteLine($"{tab}Parent PID         |  {parent?.Id}");
    Console.WriteLine($"{tab}Name               |  {p.ProcessName}");
    Console.WriteLine($"{tab}Working Set (x64)  |  {(float)(p.WorkingSet64 * 0.0000001)} mb");
    Console.WriteLine($"{tab}Module Name        |  {p.MainModule?.ModuleName}");
    Console.WriteLine($"{tab}Module Path        |  {p.MainModule?.FileName.Replace(p.MainModule?.ModuleName ?? string.Empty, string.Empty)}"); 
}