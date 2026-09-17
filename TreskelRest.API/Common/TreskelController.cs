using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TreskelRest.API.Common
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreskelController : ControllerBase
    {
        protected string TokenKey;
        protected IMediator _mediator;
        private readonly IConfiguration _configuration;
        protected IMapper _mapper;

        public TreskelController(IConfiguration configuration, IMediator mediator, IMapper mapper)
        {
            _configuration = configuration;
            _mediator = mediator;
            _mapper = mapper;
            //TokenKey = _configuration.GetValue<string>("TokenKey");
            TokenKey = _configuration.GetSection("jwt:Key").Value;
        }

        protected string ValidateToken(string token)
        {
            return Utils.validaToken(token, TokenKey);
        }
    }
}
