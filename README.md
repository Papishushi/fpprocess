# fpprocess v1.1.1 [![CodeQL](https://github.com/Papishushi/fpprocess/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Papishushi/fpprocess/actions/workflows/codeql-analysis.yml) [![.NET](https://github.com/Papishushi/fpprocess/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Papishushi/fpprocess/actions/workflows/dotnet.yml)
Terminal tool targeted for Windows, its purpose is to find the full list of parent processes based on supplied PIDs or process names by stdin.
# Usage
* ### Get process info from PID '4'.
      .\fpprocess.exe 4
* ### Get process info from the first process named 'System'.
      .\fpprocess.exe System
* ### Get process info from the first processes named 'eldenring' 'steam'.
      .\fpprocess.exe eldenring steam  
* ### Get process info from the first process named 'msedgewebview2' then from PID '4' and last from the first process named 'svchost' .
      .\fpprocess.exe msedgewebview2 4 svchost   
* ### Get process info from the first process named 'fpprocess' then from the first process named 'winlogon' specifying output with no color.
      .\fpprocess.exe fpprocess winlogon --no-color
* ### Periodically get process info from PID '1420' every 1500 miliseconds, to stop press *Crtl+C*.
      .\fpprocess.exe 1420 -i 1500
* ### Output processes info to a file.
      .\fpprocess.exe conhost MiniSearchHost dllhost >> hostingProcesses.txt
* ### Log to a file every 10000 miliseconds, to stop press *Crtl+C*.
      .\fpprocess.exe -i 10000 1420 >> wininitProcess.txt
# Installing
***(Extra) You can add it to PATH to use it on any working directory by just typing fpprocess.***
  * Search "edit ENV" on windows.                                                                 
  * Open the result.                                                                              
  * Follow this actions: User Variables For "Your Username">Edit>New.                                      
  * To end add the destination path for the installation.
## Release
* Download last release.
* Unzip *'fpprocess-v1.x.x.7z'*.
* Move **ALL** the contents together to their final destination.
* Use it on your terminal!
## NuGet Package
* ### Package Manager
      Install-Package fpprocess -Version 1.1.1
* ### .NET CLI
      dotnet add package fpprocess --version 1.1.1
* ### PackageReference (XML Node)
      <PackageReference Include="fpprocess" Version="1.1.1" />
* ### Paket CLI 
      paket add fpprocess --version 1.1.1
* ### Script & Interactive
      #r "nuget: fpprocess, 1.1.1"
* ### Cake
      // Install fpprocess as a Cake Addin
      #addin nuget:?package=fpprocess&version=1.1.1

      // Install fpprocess as a Cake Tool
      #tool nuget:?package=fpprocess&version=1.1.1
* ### Github Package   
      pip install git+git://github.com/Papishushi/fpprocess.git
## DIY Compilation
* Clone the repo 

      git clone git@github.com:Papishushi/fpprocess.git
* Open the .sln on VS or use dotnet CLI to build the project

      dotnet build --configuration Release fpprocess.sln
* Get the final compiled binaries on the directory ***bin\\Release\net6.0-windows10.0.22000.0\\***
