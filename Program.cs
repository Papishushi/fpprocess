using System.Diagnostics;

bool running = true;

void StopRunning(object? sender, ConsoleCancelEventArgs args) => running = false;

Console.CancelKeyPress += StopRunning;

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
        }
        catch
        {
            WriteLineColor($"\nArgument not correctly formatted '{arg}'", ConsoleColor.DarkRed);
            continue;
        }

        nameMode = true;
    }

    while (running)
    {
        if (tid != 0)
            id = tid;
        try
        {
            if (tid != 0 || !nameMode)
                p = Process.GetProcessById(id);
            var parent = GetParent(p);
            if (parent == null) break;

            WriteLineColor($"\n{tab}Get Info from process {p.Id} ({p.ProcessName})", ConsoleColor.Green);

            tid = parent.Id;
            if (tid == 0) break;

            DisplayProcessInfo(tab, p, parent);

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
    Console.WriteLine($"{tab}Parent PID: {parent?.Id}");
    Console.WriteLine($"{tab}Name: {p.ProcessName}");
    Console.WriteLine($"{tab}Working Set (64): {(float)(p.WorkingSet64 * 0.0000001)} mb");
    Console.WriteLine($"{tab}Module Name: {p.MainModule?.ModuleName}");
    Console.WriteLine($"{tab}Module Path: {p.MainModule?.FileName}");
}