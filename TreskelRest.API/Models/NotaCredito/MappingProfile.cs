using AutoMapper;
using TreskelRest.API.Common;
using TreskelRest.API.Sri;

namespace TreskelRest.API.Models.NotaCredito
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NotaCreditoModel, Biz.Models.NotaCredito>()
                .ReverseMap();

            CreateMap<NotaCreditoDetalle, Biz.Models.NotaCreditoDetalle>()
                .ReverseMap();
        }
    }
}
