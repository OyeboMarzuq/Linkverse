using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Cache
{
    public static class CacheTTL
    {
        public static readonly TimeSpan MatchResults = TimeSpan.FromMinutes(1);

        public static readonly TimeSpan HousingListings = TimeSpan.FromMinutes(3);

        public static readonly TimeSpan StudyPDFListings = TimeSpan.FromMinutes(5);

        public static readonly TimeSpan ServiceListings = TimeSpan.FromMinutes(5);

        public static readonly TimeSpan UserAgreements = TimeSpan.FromMinutes(3);

        public static readonly TimeSpan ProviderProfile = TimeSpan.FromMinutes(10);

        public static readonly TimeSpan UserSubscription = TimeSpan.FromMinutes(10);

        public static readonly TimeSpan SubscriptionPlans = TimeSpan.FromHours(1);

        public static readonly TimeSpan CampusLocations = TimeSpan.FromHours(6);
    }
}
