using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Model;
using Model.Exception;
using UnityEngine;
using Web.Dto;

namespace Web
{
    public class ClientAuthenticator : IClientAuthenticator
    {
        private readonly ClientConfig _config;

        public ClientAuthenticator(ClientConfig config) => _config = config;

        public async Task<IAuthorizedClient> SignIn(string playerId)
        {
            using var http = new HttpClient();
            http.BaseAddress = new Uri(_config.GatewayUrl);

            var token = await GetToken(http, playerId);

            return AuthorizedClient.Launch(_config, token);
        }

        private static async Task<string> GetToken
        (
            HttpClient http,
            string playerId
        )
        {
            var login = await http.GetAsync($"/hosts/{playerId}/access_token");
            if (login.StatusCode == HttpStatusCode.NotFound)
            {
                throw new UserNotFound();
            }

            var jsonString = await login.Content.ReadAsStringAsync();
            var result = JsonUtility.FromJson<TokenResultDto>(jsonString);

            return result.token;
        }
    }
}
