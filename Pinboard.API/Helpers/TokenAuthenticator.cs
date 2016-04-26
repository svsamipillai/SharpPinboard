using System;
using RestSharp;

namespace Pinboard.Helpers
{
    /// <summary>
    /// Implements the auth_token required for Pinboard authentication
    /// </summary>
    public class TokenAuthenticator : IAuthenticator
    {
        private readonly string _token;

        public TokenAuthenticator(string token)
        {
            if (!token.Contains(":"))
            {
                throw new ApplicationException("Pinboard tokens need to be in the form of username:token");
            }
            _token = token;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddParameter("auth_token", _token);
        }
    }
}