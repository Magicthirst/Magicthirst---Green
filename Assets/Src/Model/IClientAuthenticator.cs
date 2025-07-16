using System.Threading.Tasks;

namespace Model
{
    public interface IClientAuthenticator
    {
        /// <param name="playerId"/>
        /// <returns><see cref="IAuthorizedClient"/></returns>
        /// <exception cref="Model.Exception.UserNotFound"/>
        public Task<IAuthorizedClient> SignIn(string playerId);
    }
}
