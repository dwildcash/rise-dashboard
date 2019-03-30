using rise_tgbot.Models;
using System;

namespace rise_tgbot.Helpers
{
    public static class LogManager
    {
        public static void WriteLog(string msg)
        {
            Console.WriteLine(msg);

            using (var context = new ApplicationDbContext())
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = msg
                };

                context.Add(log);
                context.SaveChanges();
            }
        }
    }
}