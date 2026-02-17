using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Globalization;

// Simple AppHost launcher: locate the solution root (where the ApiProjectInternetVeikals.Server folder lives)
// and run `dotnet run --project` for the server project. This avoids depending on generated Aspire
// types that may not be present in this environment.

DirectoryInfo? dir = new DirectoryInfo(AppContext.BaseDirectory);
DirectoryInfo? solutionRoot = null;

while (dir != null)
{
    if (dir.GetDirectories("ApiProjectInternetVeikals.Server").Any())
    {
        solutionRoot = dir;
        break;
    }
    dir = dir.Parent;
}

if (solutionRoot == null)
{
    Console.Error.WriteLine("Could not find ApiProjectInternetVeikals.Server folder from the current location.");
    return;
}

var serverDir = Path.Combine(solutionRoot.FullName, "ApiProjectInternetVeikals.Server");

string NormalizePath(string p)
{
    if (string.IsNullOrWhiteSpace(p)) return p;
    // Handle file: URIs that can appear in some environments
    if (p.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
    {
        try { p = new Uri(p).LocalPath; } catch { }
    }
    p = p.Trim();
    // Remove control and format characters that sometimes sneak into paths (BOM, zero-width, etc.)
    p = new string(p.Where(c => !char.IsControl(c) &&
                               CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.Format)
                   .ToArray());

    // Get full path and validate
    p = Path.GetFullPath(p);
    var invalid = Path.GetInvalidPathChars();
    if (p.IndexOfAny(invalid) >= 0)
        throw new ArgumentException("Path contains invalid characters", nameof(p));
    return p;
}

var serverProj = NormalizePath(Path.Combine(serverDir, "ApiProjectInternetVeikals.Server.csproj"));

try
{
    if (!File.Exists(serverProj))
    {
        Console.Error.WriteLine($"Server project file not found: {serverProj}");
        return;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Invalid server project path: {serverProj}");
    Console.Error.WriteLine($"Error: {ex.GetType().Name}: {ex.Message}");
    Console.Error.WriteLine("Path characters (code points):");
    foreach (var ch in serverProj)
    {
        Console.Error.Write($"{(int)ch} ");
    }
    Console.Error.WriteLine();
    return;
}

// Quick automated sanitization: remove non-ASCII and control characters which
// commonly cause "illegal characters in path" errors when passed to the CLI.
string SanitizeToAscii(string p)
{
    if (string.IsNullOrEmpty(p)) return p;
    var arr = p.Where(c => c <= 0x7F && !char.IsControl(c)).ToArray();
    return new string(arr);
}

var sanitizedServerProj = SanitizeToAscii(serverProj);
var sanitizedServerDir = SanitizeToAscii(serverDir);

if (sanitizedServerProj != serverProj)
{
    Console.Error.WriteLine("Warning: server project path contained non-ASCII/control characters; they were removed for execution.");
}

// final existence check
if (!File.Exists(sanitizedServerProj))
{
    Console.Error.WriteLine($"Server project file not found after sanitization: {sanitizedServerProj}");
    return;
}

// Use `dotnet watch` so the server restarts on code changes. Pass arguments in the form:
// dotnet watch --project "<path>" run --no-launch-profile
var psi = new ProcessStartInfo
{
    FileName = "dotnet",
    WorkingDirectory = serverDir,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = false
};

// Use ArgumentList to avoid quoting/illegal-character issues when passing paths.
psi.ArgumentList.Add("watch");
psi.ArgumentList.Add("--project");
psi.ArgumentList.Add(sanitizedServerProj);
psi.ArgumentList.Add("run");
psi.ArgumentList.Add("--no-launch-profile");

// Use sanitized working directory if needed
psi.WorkingDirectory = sanitizedServerDir;

using var process = new Process { StartInfo = psi };

process.OutputDataReceived += (s, e) => { if (e.Data != null) Console.Out.WriteLine(e.Data); };
process.ErrorDataReceived += (s, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };

try
{
    process.Start();
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();
    process.WaitForExit();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed to start server: {ex.Message}");
}
