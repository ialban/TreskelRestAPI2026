using AutoMapper;
using TreskelRest.Data.Models;

namespace TreskelRest.Biz.Models.Responses
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<UsuarioEmpresa, AuthenticationResponse>()
                .ReverseMap();
        }
    }
}
