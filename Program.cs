using System.Diagnostics;

bool running = true;

void StopRunning(object? sender, ConsoleCancelEventArgs args) => running = false;

Console.CancelKeyPress += StopRunning;

foreach (var arg in args)
{
    var tab = string.Empty;
    var tid = 0;
    var id = 0;

    try
    {
        id = Convert.ToInt32(arg);
    }
    catch
    {
        WriteLineColor($"\nArgument not correctly formatted", ConsoleColor.DarkRed);
        continue;
    }

    while (running)
    {
        if (tid != 0)
            id = tid;
        try
        {
            var p = Process.GetProcessById(id);
            var parent = GetParent(p);
            if (parent == null) break;

            WriteLineColor($"\n{tab}Get Info from process {id} ({p.ProcessName})", ConsoleColor.Green);

            tid = parent.Id;
            if (tid == 0) break;

            Console.WriteLine($"{tab}Parent PID: {parent.Id}");
            Console.WriteLine($"{tab}Name: {p.ProcessName}");
            Console.WriteLine($"{tab}Working Set (64): {(float)(p.WorkingSet64 * 0.0000001)} mb");
            Console.WriteLine($"{tab}Module Name: {p.MainModule?.ModuleName}");
            Console.WriteLine($"{tab}Module Path: {p.MainModule?.FileName}");

            if (parent.HasExited)
            {
                WriteLineColor($"{tab}Process {parent.Id} has exited... Aborting operations", ConsoleColor.DarkYellow);
                continue;
            }
        }
        catch (Exception ex)
        {
            WriteLineColor($"{tab}{ex.Message} {ex.InnerException?.Message}", ConsoleColor.DarkRed);
            if (ex.GetType() == typeof(ArgumentException)) break;
            tab += "\t";
            continue;
        }

        tab += "\t";
    }
}

static void WriteLineColor(string content, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(content);
    Console.ResetColor();
}

static string FindIndexedProcessName(int pid)
{
    try
    {
        var processName = Process.GetProcessById(pid).ProcessName;
        var processesByName = Process.GetProcessesByName(processName);
        string processIndexdName = string.Empty;

        for (var index = 0; index < processesByName.Length; index++)
        {
            processIndexdName = index == 0 ? processName : processName + "#" + index;
            var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
            if ((int)processId.NextValue() == pid) return processIndexdName;
        }

        return processIndexdName;
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