# NLog Installation Instructions

Since the solution is currently open in Visual Studio, please follow these steps to install NLog:

## Option 1: Using NuGet Package Manager (Recommended)

1. In Visual Studio, go to **Tools** ? **NuGet Package Manager** ? **Manage NuGet Packages for Solution**
2. Click on the **Browse** tab
3. Search for "NLog"
4. Select "NLog" (version 4.7.15 or later)
5. Check the box next to your project "NFC-ACR122U-Reader-Writer"
6. Click **Install**
7. Accept any license agreements

## Option 2: Using Package Manager Console

1. In Visual Studio, go to **Tools** ? **NuGet Package Manager** ? **Package Manager Console**
2. Run the following command:
   ```
   Install-Package NLog -Version 4.7.15
   ```

## Option 3: Close Visual Studio and Install via Command Line

1. Close Visual Studio completely
2. Open PowerShell or Command Prompt
3. Navigate to the project directory:
   ```powershell
   cd "C:\Jobb\NFC-ACR-Reader-Writer"
   ```
4. Install NLog using dotnet CLI:
   ```powershell
   dotnet add package NLog --version 4.7.15
   ```
5. Reopen Visual Studio

## After Installation

1. Build the solution (Ctrl+Shift+B)
2. Verify there are no errors
3. Run the application (F5)

## Verifying Installation

After installation, you should see:
- `NLog.dll` in your `bin\Debug` or `bin\Release` folder
- NLog reference in your project's References
- No compilation errors related to NLog

## Note

The `packages.config` file already has NLog configured, so Visual Studio should automatically restore it if you:
1. Right-click on the solution in Solution Explorer
2. Select "Restore NuGet Packages"

If you continue to have issues, please ensure:
- You have internet connectivity
- NuGet package sources are configured correctly (Tools ? Options ? NuGet Package Manager ? Package Sources)
