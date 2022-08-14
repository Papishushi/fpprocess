# Learn from fpprocess!

Hi! I'm [@Papishushi](https://github.com/Papishushi/) creator of [fpprocess](https://github.com/Papishushi/fpprocess), I will be guiding you through the creation of this command line tool. The purpose of this file is bring
the reader with any detail or technique used on this project, and explain .NET development superficially.

## Why?
I started the creation of this software on need of finding the IP from the servers of Elden Ring, for doing that I was going to need a tool 
to show if the parent process of the process opening a port was Elden Ring. And so, the process begun...

## Lets start coding...
(Option 1) Create a new console project using dotnet CLI ([Learn how to install dotnet CLI here](https://docs.microsoft.com/en-gb/dotnet/core/sdk)).

    dotnet new console --language "C#" --framework "net6.0" --name "YourProjectName" --output "YourSystemPath/YourProjectName"

(Option 2) Create a new console project using Visual Studio ([Learn how to install Visual Studio Community here](https://visualstudio.microsoft.com/vs/community/)).

  1. Create a new project
![Screenshot that shows the Create a new project window.](https://docs.microsoft.com/en-gb/visualstudio/get-started/csharp/media/vs-2022/create-new-project.png?view=vs-2022)
 2. Choose Console as project template. If Console project template doesn't appear on the listed project templates, please take a look at your **Visual Studio Installer**. You must check the option ***.NET desktop development***, to finalize click on Modify.![Install Visual Studio | Microsoft Docs](https://docs.microsoft.com/en-gb/visualstudio/install/media/vs-2022/vs-installer-workloads.png?view=vs-2022)
 3. Specify framework as **.NET 6**, this is the last *LTS release* of **.NET Core**, with tons of changes in the way C# is implemented on .NET. It also enhances *multiplatform support*, and brings *performance* and *functionallity* upgrades. A must have!
![Screenshot that shows .NET 6.0 selected in the Additional information window.](https://docs.microsoft.com/en-gb/visualstudio/get-started/csharp/media/vs-2022/csharp-target-framework.png?view=vs-2022)
 4. Press create. Wait for your project to generate...

## What are all these files?
A simple terminal .NET project is composed of 3 main files: A solution, a project, and a main entry point. 

 1. A *solution* (.**sln**) is a structure to manage different projects,
    and configurations. This file it's usually the one you will be
    opening to load on Visual Studio the whole project, or compile the
    project using dotnet CLI. In the case of fpprocess it only manages a single project. Check the looks of [fpprocess.sln](https://github.com/Papishushi/fpprocess/blob/main/fpprocess.sln) to familiarize with this type of file. 
   
 2. A *project* (.**csproj**)  is the configuration file for a specific project. That means you can have multiple files with this extension, one for each project. It holds the majority of info and metadata needed for different aspects of a project. Check the looks of [fpprocess.csproj](https://github.com/Papishushi/fpprocess/blob/main/fpprocess.csproj) to familiarize with this type of file. 
    
 3. The main entry point for a terminal program is a special C# source file named [Program.cs](https://github.com/Papishushi/fpprocess/blob/main/Program.cs) (*For more info on the new console template check https://aka.ms/new-console-template*). This file holds a special syntax different from an usual .cs source file, this is made to facilitate and reduce the complexity of simple terminal programs. It is also the only file able to recieve parameters from the *standard input* (*stdin*). This is the core of fppprocess, holding the whole code of this project.

### Additional files

 1. An [*app.manifest*](https://github.com/Papishushi/fpprocess/blob/main/app.manifest) file is a special project file for some specific settings and metadata for your .NET program running on windows. Like the requested execution level for the program, supported Windows OS versions, and other not that important stuff for this project.
 
 2. If you want to make your project a git repository, there are two files that will come handy for your needs. The [*.gitattributes*](https://github.com/Papishushi/fpprocess/blob/main/.gitattributes) and [*.gitignore*](https://github.com/Papishushi/fpprocess/blob/main/.gitignore) files. The first one *.gitattributes* is a repository specific configuration for git, usually is used for auto detection of text files and performing LF normalization. On the other hand *.gitignore* species a full set of constraints for git when tracking the files of your repository. For this project a good *.gitignore* template to include is [VisualStudio.gitignore](https://github.com/github/gitignore/blob/main/VisualStudio.gitignore).
 
 3. An open source license file for your project. Usually is called *LICENSE* or *LICENSE.md*, there is a ton of reason why a publicly published project should have a license, [so check your reason to have a license on yours](https://choosealicense.com).  For this project I decided using an [MIT License](), it is a very permissive open source license, one of my preferred ones.
 
 4. A readme file telling everybody what is your project, how to use it and install it, channels of comunnication and any detail you want about the repository. This file is usually present in the form of a *README.md* or *README*. Click here to check the [README.md](https://github.com/github/gitignore/blob/main/README.md) file of fpprocess.

## How does fpprocess works?
### What is a PID?
A process ID (*PID*) is the unique number identifying a single process. You can check the *PID* of a process on *Windows Task Manager > Details*.
### What is a parent process?
A parent process is defined as the creating process of another process. So for example process *dllhost* will follow this creation chain: *dllhost > svchost > services > wininit*. As you can see if you are able to know the creating process of another, you can recursivelly follow up to the original process.
### Performance Counters
fpprocess relies on [Windows NT Performance Counter component](https://docs.microsoft.com/en-gb/dotnet/api/system.diagnostics.performancecounter?view=dotnet-plat-ext-6.0) to read the existing predefined counter for processes named "Creating Process ID", thats the way to get the PID from the parent of the supplied PID. As such is included as a dependency from fpprocess on [fpprocess.csproj](https://github.com/Papishushi/fpprocess/blob/main/fpprocess.csproj).

    <PackageReference  Include="System.Diagnostics.PerformanceCounter"  Version="6.0.1" />
As you can imagine this is a Windows specific utility and can't be used on other OS. That's the reason why fpprocess is only available on Windows right now. These performance counters allocate unmanaged resources, so they implement the interface [IDisposable](https://docs.microsoft.com/en-gb/dotnet/api/system.idisposable?view=net-6.0) and must be disposed manually with [Dispose( )](https://docs.microsoft.com/en-gb/dotnet/api/system.idisposable.dispose?view=net-6.0), or automatically with [***using***](https://docs.microsoft.com/en-gb/dotnet/standard/garbage-collection/using-objects). fpprocess source code use the ***using*** keyword method.

### The code core
First off we need to create a way to get the correct process name from a supplied *PID*. For that purpose I created the method FindIndexedProcessName, this method takes as a parameter a supplied process ID, and iterates over all the processes with the same name to find the correct process name. It also performs simple exception catching for [Process.GetProcessById( )](https://docs.microsoft.com/en-gb/dotnet/api/system.diagnostics.process.getprocessbyid?view=net-6.0) and [Process.GetProcessesByName( )](https://docs.microsoft.com/en-gb/dotnet/api/system.diagnostics.process.getprocessesbyname?view=net-6.0).
 

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
     
Next we need to define a way of getting the parent process of the process name we just found, for doing that we can define a very simple method named FindCreatingProcessID. This method creates a Performance Counter to get the Creating Process ID predefined counter from the supplied name. Then supply it to [Process.GetProcessById( )](https://docs.microsoft.com/en-gb/dotnet/api/system.diagnostics.process.getprocessbyid?view=net-6.0) and return the result. It also performs simple exception catching for [Process.GetProcessById( )](https://docs.microsoft.com/en-gb/dotnet/api/system.diagnostics.process.getprocessbyid?view=net-6.0).

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
    
How to get the parent process? Easy just combine the functionallity from both, on a simple one-liner method.

    static Process? GetParent(Process process) => FindCreatingProcessID(FindIndexedProcessName(process.Id));

Once all of this is coded the main tool for the program is done, GetParent( ) method. So the above code is the "core" of the project, in some way.
