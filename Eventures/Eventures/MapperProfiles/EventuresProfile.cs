using AutoMapper;
using Eventures.Models;
using Eventures.Web.ViewModels;

namespace Eventures.Web.MapperProfiles
{
    public class EventuresProfile : Profile
    {
        public EventuresProfile()
        {
            CreateMap<Event, CreateEventViewModel>()
                .ReverseMap();
            CreateMap<User, RegisterViewModel>()
                .ForMember(
                    dest => dest.Username, 
                    from => from.MapFrom(src => src.UserName))
                .ReverseMap();
            //.ForMember(evm => evm.Start,
            //    e => e.MapFrom(s => s.Start.ToString("dd-MMM-yy hh:mm:ss tt", CultureInfo.InvariantCulture)))
            //.ForMember(evm => evm.End,
            //    e => e.MapFrom(s => s.End.ToString("dd-MMM-yy hh:mm:ss tt", CultureInfo.InvariantCulture)))
            //.ReverseMap();
        }
    }
}
