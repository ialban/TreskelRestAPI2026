using Microsoft.AspNetCore.Mvc;
using TreskelRest.API.Mappers;
using Wolverine;
using Wolverine.Runtime;

namespace TreskelRest.API.Common
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreskelController : ControllerBase
    {
        protected string TokenKey;
        protected IMessageBus _mediator;
        private readonly IConfiguration _configuration;
        protected APIMapperProfile _mapper;

        public TreskelController(IConfiguration configuration, IMessageBus mediator, APIMapperProfile mapper)
        {
            _configuration = configuration;
            _mediator = mediator;
            _mapper = mapper;
            
            TokenKey = _configuration.GetSection("jwt:Key").Value;
        }

        protected string ValidateToken(string token)
        {
            return Utils.validaToken(token, TokenKey);
        }
    }
}
