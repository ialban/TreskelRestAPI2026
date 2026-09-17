using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TreskelRest.API.Models.Usuarios;

namespace TreskelRest.API.Common
{
    public class Utils
    {
        public static string validaToken(string token, string TokenKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(TokenKey);
            var aToken = token.Split(' ');

            if (aToken.Length > 1)
            {
                if (aToken[0] != "Bearer") return "";
                token = aToken[1];
            }

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var account = jwtToken.Claims.First(x => x.Type == "usuario").Value;

                // return account id from JWT token if validation successful
                return account;
            }
            catch (Exception e)
            {
                // return null if validation fails
                return null;
            }
        }

        public static string generaToken(AuthenticationResponse user, string TokenKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(TokenKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("ruc", user.Ruc.ToString()),
                    new Claim("usuario", user.Usuario.ToString()),
                    new Claim("empresaid", user.EmpresaId.ToString()),
                    new Claim("guid", Guid.NewGuid().ToString()),
                    new Claim("application", "TreskelAPI"),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
