using AutoMapper;
using TreskelRest.API.Common;

namespace TreskelRest.API.Models.Mensajes
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EmailPayload, Biz.Models.Correo.EmailPayload>()
                .ReverseMap();

            CreateMap<CuentaEmail, Biz.Models.Correo.CuentaEmail>()
                .ReverseMap();
        }
    }
}
