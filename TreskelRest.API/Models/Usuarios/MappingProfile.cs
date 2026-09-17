using AutoMapper;

namespace TreskelRest.API.Models.Usuarios
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuthenticationRequest, Biz.Models.Requests.AuthenticationRequests>()
                .ReverseMap();

            CreateMap<Biz.Models.Responses.AuthenticationResponse, AuthenticationResponse>()
                .ReverseMap();
        }
    }
}
