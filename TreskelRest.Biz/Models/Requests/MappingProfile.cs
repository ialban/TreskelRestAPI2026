using AutoMapper;

namespace TreskelRest.Biz.Models.Requests
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<AuthenticationRequests, Data.Base.UsuarioRequest>()
                .ReverseMap();
        }
    }
}
