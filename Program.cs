using System.Diagnostics;

const string NO_COLOR = "--no-color";
const string INTERVAL = "-i";
const string HELP = "-h";

bool running = true;
bool isColorOutput = true;
bool isIntervalMode = false;
int interval = 1500;
int paramIndex = 0;
var usedParameters = new List<string>();
var availableParameters = new[] { new Parameter { parameter=HELP    , execution=HelpParameter     },
                                  new Parameter { parameter=INTERVAL, execution=IntervalParameter },
                                  new Parameter { parameter=NO_COLOR, execution=NoColorParameter  } };

void StopRunning(object? sender, ConsoleCancelEventArgs args) => running = false;

Console.CancelKeyPress += StopRunning;

ProcessParameters();

if (isIntervalMode)
{
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();
    do
    {
        if (stopWatch.ElapsedMilliseconds < interval) continue;
        stopWatch.Restart();
        MainAlgorithm();
    } while (running);
}
else
    MainAlgorithm();

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
void ProcessParameters()
{
    if (args.Length <= 0) return;
    for (int i = 0; i < args.Length; i++)
    {
        if(args[i][0] != '-') continue;
        foreach (var param in availableParameters)
        {
            if (param.parameter != args[i]) continue;
            paramIndex = i;
            param.execution();
            paramIndex = 0; 
            break;
        }
    }
    args = args.Except(usedParameters).ToArray();
    usedParameters.Clear();
}

#region PARAMETERS
    void NoColorParameter()
    {
        isColorOutput = false;
        usedParameters.Add(NO_COLOR);
    }

    void IntervalParameter()
    {
        isIntervalMode = true;
        try
        {
            var temp = args[paramIndex + 1];
            interval = Convert.ToInt32(temp);
            usedParameters.AddRange(new[] { args[paramIndex], temp });
        }
        catch
        {
            usedParameters.Add(args[paramIndex]);
        }
    }

    void HelpParameter()
    {
        running = false;
        try
        {
            var temp = args[paramIndex + 1];
            //Do something with temp.
            usedParameters.AddRange(new[] { args[paramIndex], temp });
        }
        catch
        {
            usedParameters.Add(args[paramIndex]);
        }
    }
#endregion PARAMETERS

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
            using var processId = new PerformanceCounter("Process", "ID Process", processIdName);
            if ((int)processId.NextValue() == pId) return processIdName;
        }

        return processIdName;
    }
    catch
    {
        return string.Empty;
    }
}
static Process? FindCreatingProcessID(string indexedProcessName)
{
    try
    {
        using var processParent = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
        return Process.GetProcessById((int)processParent.NextValue());
    }
    catch
    {
        return null;
    }
}
static Process? GetParent(Process process) => FindCreatingProcessID(FindIndexedProcessName(process.Id));

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
    Console.Write($"{tab}Parent PID         |  "); Console.WriteLine($"{parent?.Id}");
    Console.Write($"{tab}Name               |  "); Console.WriteLine($"{p.ProcessName}");
    Console.Write($"{tab}Working Set (x64)  |  "); Console.WriteLine($"{(float)(p.WorkingSet64 * 0.0000001)} mb");
    Console.Write($"{tab}Module Name        |  "); Console.WriteLine($"{p.MainModule?.ModuleName}");
    Console.Write($"{tab}Module Path        |  "); Console.WriteLine($"{p.MainModule?.FileName.Replace(p.MainModule?.ModuleName ?? string.Empty, string.Empty)}"); 
}

struct Parameter
{
    public string parameter;
    public Action execution;
}