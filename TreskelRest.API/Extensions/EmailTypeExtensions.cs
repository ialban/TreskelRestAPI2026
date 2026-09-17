using TreskelRest.API.Models.Mensajes;

namespace TreskelRest.API.Extensions
{
    public static class EmailTypeExtensions
    {
        public static bool RequiereUnsubscribe(this EmailType tipo)
        {
            return tipo switch
            {
                EmailType.Marketing => true,
                EmailType.Newsletter => true,
                _ => false
            };
        }
    }
}
