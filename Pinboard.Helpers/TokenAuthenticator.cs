using System;
using RestSharp;
namespace Pinboard.Helpers
{
    /// <summary>
    /// Implements the auth_token required for Pinboard authentication
    /// </summary>
    public class TokenAuthenticator : IAuthenticator
    {
        private string token;
        public TokenAuthenticator(string Token)
        {
            if (!Token.Contains(":"))
            {
                throw new ApplicationException("Pinboard tokens need to be in the form of username:token");
            }
            token = Token;
        }
        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddParameter("auth_token", token);
        }
    }
}
