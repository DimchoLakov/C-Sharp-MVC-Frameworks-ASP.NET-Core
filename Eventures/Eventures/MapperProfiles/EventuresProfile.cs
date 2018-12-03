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

            CreateMap<Order, CreateOrderViewModel>()
                .ReverseMap();

            CreateMap<Order, OrderViewModel>()
                .ForMember(dest => dest.OrderedOn, mapFrom => mapFrom.MapFrom(src => src.OrderedOn))
                .ForMember(dest => dest.CustomerName, mapFrom => mapFrom.MapFrom(src => src.Customer.UserName))
                .ForMember(dest => dest.EventName, mapFrom => mapFrom.MapFrom(src => src.Event.Name))
                .ReverseMap();
        }
    }
}
