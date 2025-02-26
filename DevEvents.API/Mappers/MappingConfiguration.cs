using DevEvents.API.Domain.Entities;
using DevEvents.API.Models;
using Mapster;

namespace DevEvents.API.Mappers
{
    public static class MappingConfiguration
    {
        public static IServiceCollection RegisterMaps(this IServiceCollection services)
        {
            services.AddMapster();

            TypeAdapterConfig<Conference, ConferenceItemViewModel>
                .NewConfig()
                .Map(m =>
                m.Overview, s => $"{s.Title} - {s.Description} - {string.Join(",", s.Speakers.Select(s => s.Name))} - {s.Registrations.Count}");

            return services;
        }
    }
}
