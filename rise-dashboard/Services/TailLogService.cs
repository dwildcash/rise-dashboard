using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using rise.Hubs;
using rise.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace rise.Services
{
    public class TailLogService : BackgroundService
    {
        private readonly IHubContext<NotificationHub> _notificationHub;

        public TailLogService(IHubContext<NotificationHub> notificationHub)
        {
            _notificationHub = notificationHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (StreamReader reader = new StreamReader(new FileStream(AppSettingsProvider.NodeLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        //start at the end of the file
                        long lastMaxOffset = reader.BaseStream.Length;

                        while (true)
                        {
                            Thread.Sleep(100);

                            //if the file size has not changed, idle
                            if (reader.BaseStream.Length == lastMaxOffset)
                            {
                                continue;
                            }

                            //seek to the last max offset
                            reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

                            //read out of the file until the EOF
                            string line;

                            while ((line = reader.ReadLine()) != null)
                            {
                                await _notificationHub.Clients.All.SendAsync("Send", line);
                            }

                            //update the last max offset
                            lastMaxOffset = reader.BaseStream.Position;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Received Exception from TailLogService {0}", ex.Message);
            }
        }
    }
}