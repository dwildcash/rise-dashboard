using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using rise.Hubs;
using rise.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rise.Services
{
    public class TailLogService : BackgroundService
    {
        private readonly IHubContext<NotificationHub> _notificationHub;
        private string filename = string.Empty;
        private FileSystemWatcher fileSystemWatcher = null;

        public TailLogService(IHubContext<NotificationHub> notificationHub)
        {
            _notificationHub = notificationHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                filename = AppSettingsProvider.NodeLogFile;

                FileInfo targetFile = new FileInfo(filename);

                fileSystemWatcher = new FileSystemWatcher
                {
                    IncludeSubdirectories = false,
                    Path = targetFile.DirectoryName,
                    Filter = targetFile.Name
                };

                fileSystemWatcher.Created += TargetFile_Created;
                fileSystemWatcher.Changed += TargetFile_Changed;

                fileSystemWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Received Exception from TailLogService {0}", ex.Message);
            }
        }

        public void TargetFile_Created(object source, FileSystemEventArgs e)
        {
            StartMonitoring();
        }

        public void StartMonitoring()
        {
            fileSystemWatcher.Changed += TargetFile_Changed;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void TargetFile_Changed(object source, FileSystemEventArgs e)
        {
            try
            {
                var lastLine = File.ReadLines(AppSettingsProvider.NodeLogFile).Last();

                if (!lastLine.Contains("Account not found"))
                {
                    _notificationHub.Clients.All.SendAsync("Send", lastLine);
                }

                // Send forged block in title
                if (lastLine.Contains("Received new block"))
                {
                    _notificationHub.Clients.All.SendAsync("ShowForged", lastLine);
                }
            }
            catch (Exception ex)
            {
                _notificationHub.Clients.All.SendAsync("Send", ex.Message);
            }
        }
    }
}