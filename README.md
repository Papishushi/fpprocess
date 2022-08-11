# fpprocess [![CodeQL](https://github.com/Papishushi/fpprocess/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Papishushi/fpprocess/actions/workflows/codeql-analysis.yml) [![.NET](https://github.com/Papishushi/fpprocess/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Papishushi/fpprocess/actions/workflows/dotnet.yml)
Terminal tool targeted for Windows, its purpose is  to find the full list of parent processes based on supplied PIDs by stdin.
## Usage
* Examples:
<h1 align="left"><a href="https://git.io/typing-svg"><img src="https://readme-typing-svg.herokuapp.com?duration=5000&width=500&font=VT323&color=FFFFFF&center=true&lines=>+fpprocess+20560;>+fpprocess+20560+0+4+160+1624"></a></h1>

# Installing
## Release
* Download last release
* Unzip *'fpprocess-v1.x.x.7z'*
* Move **ALL** the contents together to their final destination
* Use it on your terminal!
* ***(Extra) You can add it to PATH to use it on any working directory by just typing fpprocess. \
  -Search "edit ENV" on windows \
  -Open the result \
  -Follow this actions: User Variables For $User$>Edit>New \
  -To end add the destination path for the installation.***
## NuGet Package
* ### Package Manager
      Install-Package fpprocess -Version 1.0.1
* ### .NET CLI
      dotnet add package fpprocess --version 1.0.1
* ### PackageReference (XML Node)
      <PackageReference Include="fpprocess" Version="1.0.1" />
* ### Paket CLI 
      paket add fpprocess --version 1.0.1
* ### Script & Interactive
      #r "nuget: fpprocess, 1.0.1"
* ### Cake
      // Install fpprocess as a Cake Addin
      #addin nuget:?package=fpprocess&version=1.0.1

      // Install fpprocess as a Cake Tool
      #tool nuget:?package=fpprocess&version=1.0.1
## DIY Compilation
* Clone the repo 

      git clone git@github.com:Papishushi/fpprocess.git
* Open the .sln on VS or use dotnet CLI to build the project

      dotnet build --configuration Release fpprocess.sln
* Get the final compiled binaries on the directory bin\\$CompilationMode$\net6.0-windows$Target$\
