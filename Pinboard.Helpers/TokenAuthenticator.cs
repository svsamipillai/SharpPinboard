using RestSharp;
namespace Pinboard.Helpers
{
    public class TokenAuthenticator : IAuthenticator
    {
        private string token = null;
        public TokenAuthenticator(string Token)
        {
            this.token = Token;
        }
        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddParameter("auth_token", this.token);
        }
    }
}
