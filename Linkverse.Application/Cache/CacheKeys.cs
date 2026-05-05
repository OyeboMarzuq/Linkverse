using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Cache
{
    public static class CacheKeys
    {
        public static string HousingListings(string filterHash)
            => $"housing:listings:{filterHash}";

        public static string HousingListing(Guid id)
            => $"housing:listing:{id}";

        public static string HousingByProvider(Guid providerId)
            => $"housing:provider:{providerId}";

        public static string StudyPDFs(string filterHash)
            => $"studypdf:listings:{filterHash}";

        public static string StudyPDF(Guid id)
            => $"studypdf:{id}";

        public static string StudyPDFsByProvider(Guid providerId)
            => $"studypdf:provider:{providerId}";

        public static string ServiceListings(string filterHash)
            => $"service:listings:{filterHash}";

        public static string ServiceListing(Guid id)
            => $"service:listing:{id}";

        public const string SubscriptionPlans = "subscription:plans";

        public static string UserSubscription(Guid userId)
            => $"subscription:user:{userId}";

        public static string ProviderProfile(Guid providerId)
            => $"provider:profile:{providerId}";

        public static string ProviderByUser(Guid userId)
            => $"provider:user:{userId}";

        public static string CampusLocations(string campus)
            => $"campus:locations:{campus.ToLowerInvariant()}";

        public static string CampusLocationsByType(string campus, string type)
            => $"campus:locations:{campus.ToLowerInvariant()}:{type.ToLowerInvariant()}";

        public static string MatchResults(Guid userId, string filterHash)
            => $"match:results:{userId}:{filterHash}";

        public static string UserAgreements(Guid userId)
            => $"agreements:user:{userId}";
    }
}
