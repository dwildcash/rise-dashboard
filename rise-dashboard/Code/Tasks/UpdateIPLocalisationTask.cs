using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using rise.Code.DataFetcher;
using rise.Code.Scheduling;
using rise.Data;
using rise.Models;

namespace rise.Code.Tasks
{
    /// <summary>
    /// Defines the <see cref="UpdateIpLocalisationTask" />
    /// </summary>
    public class UpdateIpLocalisationTask : IScheduledTask
    {
        /// <summary>
        /// Defines the _provider
        /// </summary>
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Gets the Schedule
        /// </summary>
        public string Schedule => "*/2 * * * *";

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateIpLocalisationTask"/> class.
        /// </summary>
        /// <param name="serviceProvider">The serviceProvider<see cref="IServiceProvider"/></param>
        public UpdateIpLocalisationTask(IServiceProvider serviceProvider)
        {
            _provider = serviceProvider;
        }

        /// <summary>
        /// Fetch IP Localisation once a day
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var peersResult = await PeersFetcher.FetchPeers();

            if (peersResult != null)
            {
                PeersResult.Current = peersResult;

                using (var scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    if (PeersResult.Current != null)
                    {
                        foreach (var peer in PeersResult.Current.Peers)
                        {
                            if (!context.IPData.Any(x => x.ip == peer.Ip))
                            {
                                var ipdata = await IPsFetcher.FetchIPGeoLocation(peer.Ip);

                                if (ipdata != null)
                                {
                                    context.IPData.Add(ipdata);
                                    context.SaveChanges();
                                    Thread.Sleep(1000);
                                }
                            }

                            var ipobj = context.IPData.FirstOrDefault(x => x.ip == peer.Ip);

                            if (ipobj != null)
                            {
                                peer.Lattitude = ipobj.latitude;
                                peer.Longitude = ipobj.longitude;
                                peer.City = ipobj.city;
                                peer.Country = ipobj.country_name;
                            }
                        }
                    }
                }
            }
        }
    }
}