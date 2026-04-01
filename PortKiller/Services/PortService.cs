using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using PortKiller.Models;

namespace PortKiller.Services
{
    public class PortService
    {
        public List<PortProcess> GetProcessesOnPorts(IEnumerable<int> ports)
        {
            var results = new List<PortProcess>();
            var portSet = new HashSet<int>(ports);

            try
            {
                var netstatOutput = RunNetstat();
                var lines = netstatOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines.Skip(4)) // Skip header lines
                {
                    var match = ParseNetstatLine(line);
                    if (match != null && portSet.Contains(match.Port))
                    {
                        // Get process name
                        try
                        {
                            var process = Process.GetProcessById(match.ProcessId);
                            match.ProcessName = process.ProcessName;
                        }
                        catch
                        {
                            match.ProcessName = "Unknown";
                        }

                        results.Add(match);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting port processes: {ex.Message}");
            }

            return results.DistinctBy(p => new { p.Port, p.ProcessId }).ToList();
        }

        public List<PortProcess> GetAllListeningPorts()
        {
            var results = new List<PortProcess>();

            try
            {
                var netstatOutput = RunNetstat();
                var lines = netstatOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines.Skip(4))
                {
                    var match = ParseNetstatLine(line);
                    if (match != null && match.State == "LISTENING")
                    {
                        try
                        {
                            var process = Process.GetProcessById(match.ProcessId);
                            match.ProcessName = process.ProcessName;
                        }
                        catch
                        {
                            match.ProcessName = "Unknown";
                        }

                        results.Add(match);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting listening ports: {ex.Message}");
            }

            return results.DistinctBy(p => new { p.Port, p.ProcessId }).OrderBy(p => p.Port).ToList();
        }

        private string RunNetstat()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "netstat",
                Arguments = "-ano",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) return string.Empty;

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private PortProcess? ParseNetstatLine(string line)
        {
            // Example line: TCP    0.0.0.0:3000           0.0.0.0:0              LISTENING       12345
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 5) return null;

            var protocol = parts[0];
            var localAddress = parts[1];
            var state = parts.Length >= 4 ? parts[3] : "";
            var pidStr = parts.Length >= 5 ? parts[4] : parts[parts.Length - 1];

            // Parse port from local address
            var colonIndex = localAddress.LastIndexOf(':');
            if (colonIndex < 0) return null;

            var portStr = localAddress.Substring(colonIndex + 1);
            if (!int.TryParse(portStr, out int port)) return null;
            if (!int.TryParse(pidStr, out int pid)) return null;

            return new PortProcess
            {
                Protocol = protocol,
                LocalAddress = localAddress,
                Port = port,
                State = state,
                ProcessId = pid
            };
        }

        public bool KillProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                process.Kill(true); // Kill entire process tree
                process.WaitForExit(3000);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error killing process {processId}: {ex.Message}");
                return false;
            }
        }
    }
}
