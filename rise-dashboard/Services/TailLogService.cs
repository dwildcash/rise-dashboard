using Microsoft.Extensions.Hosting;
using rise.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rise.Services
{
    public class TailLogService : BackgroundService
    {

        private readonly NotifyService _service;

        public TailLogService(NotifyService service)
        {
            _service = service;
        }


        public Task Send(string message)
        {
            return _service.SendNotificationAsync(message);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {

                using (StreamReader reader = new StreamReader(new FileStream(AppSettingsProvider.NodeLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    //start at the end of the file
                    long lastMaxOffset = reader.BaseStream.Length;

                    while (true)
                    {
                        System.Threading.Thread.Sleep(100);

                        //if the file size has not changed, idle
                        if (reader.BaseStream.Length == lastMaxOffset)
                            continue;

                        //seek to the last max offset
                        reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

                        //read out of the file until the EOF
                        string line = string.Empty;
                        while ((line = reader.ReadLine()) != null)
                            await Send(line);

                        //update the last max offset
                        lastMaxOffset = reader.BaseStream.Position;
                    }
                }
            }

        }
    }
}
