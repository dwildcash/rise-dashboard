using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using rise.Hubs;
using rise.Models;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rise.Services
{
    public class TailLogService : BackgroundService
    {
        private readonly IHubContext<NotificationHub> _notificationHub;
        private string filename = "";
        private FileSystemWatcher fileSystemWatcher = null;
        private long previousSeekPosition;

        public delegate void MoreDataHandler(object sender, string newData);

        private int maxBytes = 1024 * 16;

        public int MaxBytes
        {
            get { return this.maxBytes; }
            set { this.maxBytes = value; }
        }

        public TailLogService(IHubContext<NotificationHub> notificationHub)
        {
            _notificationHub = notificationHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                this.filename = AppSettingsProvider.NodeLogFile;
                FileInfo targetFile = new FileInfo(filename);

                FileStream fs = new FileStream(this.filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                this.previousSeekPosition = (int)fs.Seek(this.previousSeekPosition, SeekOrigin.Begin);
                fs.Dispose();

                previousSeekPosition = 0;

                fileSystemWatcher = new FileSystemWatcher();
                fileSystemWatcher.IncludeSubdirectories = false;
                fileSystemWatcher.Path = targetFile.DirectoryName;
                fileSystemWatcher.Filter = targetFile.Name;
                fileSystemWatcher.Changed += new FileSystemEventHandler(TargetFile_Changed);
                fileSystemWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Received Exception from TailLogService {0}", ex.Message);
            }
        }

        public void TargetFile_Changed(object source, FileSystemEventArgs e)
        {
            //read from current seek position to end of file
            byte[] bytesRead = new byte[maxBytes];
            FileStream fs = new FileStream(this.filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (fs.Length > maxBytes)
            {
                this.previousSeekPosition = fs.Length - maxBytes;
            }
            this.previousSeekPosition = (int)fs.Seek(this.previousSeekPosition, SeekOrigin.Begin);
            int numBytes = fs.Read(bytesRead, 0, maxBytes);
            fs.Close();
            this.previousSeekPosition += numBytes;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < numBytes; i++)
            {
                sb.Append((char)bytesRead[i]);
            }

            //call delegates with the string
            //this.MoreData(this, sb.ToString());
            _notificationHub.Clients.All.SendAsync("Send", sb.ToString() + "\n");
        }
    }
}