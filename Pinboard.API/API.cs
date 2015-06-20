using System;
using System.Collections.Generic;
using RestSharp;
using Pinboard.Types;
using System.Net;
using Pinboard.Helpers;
using System.Threading.Tasks;


namespace Pinboard
{
    public class API
    {
        private const string APIBaseURL = "https://api.pinboard.in/v1/";
        private static RestClient client;
        private readonly string token;
        private readonly string url;
        private readonly string username;
        /// <summary>
        /// Instantiates a new Pinboard API client 
        /// </summary>
        /// <param name="Username">The username used to connect to Pinboard</param>
        /// <param name="Token">API token, found on https://pinboard.in/settings/password </param>
        /// <param name="APIBaseURL">Base URL to use (defaults to Pinboard)</param>
        public API(string Token, string APIBaseURL = APIBaseURL, string Username = null, RestClient apiClient = null)
        {
            if (apiClient == null)
            {
                client = new RestClient(APIBaseURL);
            }

            else
            {
                client = apiClient;
            }

            token = Token;
            url = APIBaseURL;

            if (!string.IsNullOrEmpty(Username))
            {
                username = Username;
            }
        }

        /// <summary>
        /// Returns the most recent time a bookmark was added, updated or deleted.
        /// Use this before calling posts/all to see if the data has changed since the last fetch.
        /// <see cref="https://pinboard.in/api/#posts_update"/>
        /// </summary>
        /// <returns>UpdateResponse with Time property set to last update time</returns>
        public async Task<IRestResponse<UpdateResponse>> GetLastUpdateAsync()
        {
            RestRequest getUpdateRequest = new RestRequest("/posts/update");
            return await ExecuteAsync<UpdateResponse>(getUpdateRequest);
           
        }

        /// <summary>
        /// Adds a post based on the querystring we pass
        /// </summary>
        /// <param name="newPost">The post to add</param>
        /// <returns>PinboardResponse</returns>
        public async Task<IRestResponse<PinboardResponse>> AddPostAsync(Post newPost)
        {
            RestRequest AddPostRequest = new RestRequest("/posts/add");
            if (newPost.Href != null)
                AddPostRequest.AddParameter("url", newPost.Href);
            if (newPost.Description != null)
                AddPostRequest.AddParameter("description", newPost.Description);
            if (newPost.Extended != null)
                AddPostRequest.AddParameter("extended", newPost.Extended);
            if (newPost.Tag != null)
                AddPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("tags", newPost.Tag));
            if (newPost.CreationTime.HasValue)
                AddPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("dt", newPost.CreationTime));
            if (newPost.Replace != null)
                AddPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("replace", newPost.Replace));
            if (newPost.Shared != null)
                AddPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("shared", newPost.Shared));
            if (newPost.ToRead != null)
                AddPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("toread", newPost.ToRead));

            return await ExecuteAsync<PinboardResponse>(AddPostRequest);
        }

        /// <summary>
        /// Delete a post via URL
        /// </summary>
        /// <param name="URL">URL we want to match</param>
        /// <returns>PinboardResponse</returns>
        public async Task<IRestResponse<PinboardResponse>> DeletePost(string URL)
        {
            RestRequest DeletePostRequest = new RestRequest("/posts/delete");
            DeletePostRequest.AddParameter("url", URL);
            return await ExecuteAsync<PinboardResponse>(DeletePostRequest);

        }

        /// <summary>
        /// Delete a post
        /// </summary>
        /// <param name="deletePost">Post object with URL parameter</param>
        /// <returns>A Pinboard response - normally with a Code of "done"</returns>
        public async Task<IRestResponse<PinboardResponse>> DeletePostAsync(Post deletePost)
        {
            return await DeletePost(deletePost.Href);
        }

        /// <summary>
        /// Get posts - filtered by URL, tags or date
        /// </summary>
        /// <param name="Tags">A list of up to 3 tags</param>
        /// <param name="CreatedDate">Return posts created on this date</param>
        /// <param name="URL">Return the post with this URL</param>
        /// <param name="ChangedMeta">Include the "meta" attribute in the response</param>
        /// <returns>A list of Post objects</returns>
        public async Task<IRestResponse<List<Post>>> GetPostsAsync(List<Tag> Tags = null, DateTime? CreatedDate = null, string URL = null, bool ChangedMeta = false)
        {
            RestRequest GetPostsReq = new RestRequest("/posts/get");

            if (Tags != null)
                GetPostsReq.AddParameter(ParameterHelpers.ValueToGetArgument("tags", Tags));
            if (CreatedDate.HasValue)
                GetPostsReq.AddParameter(ParameterHelpers.ValueToGetArgument("dt", CreatedDate.Value));
            if (URL != null)
                GetPostsReq.AddParameter("url", URL);
            GetPostsReq.AddParameter(ParameterHelpers.ValueToGetArgument("meta", ChangedMeta));
            return await ExecuteAsync<List<Post>>(GetPostsReq);
        }

        /// <summary>
        /// Gets most recent posts
        /// </summary>
        /// <param name="Tags">Filter by up to three tags</param>
        /// <param name="Count">Number of posts to return - default 15</param>
        /// <returns>A list of Posts</returns>
        public async Task<IRestResponse<List<Post>>> GetRecentPostsAsync(List<Tag> Tags = null, int Count = 15)
        {
            RestRequest getPostsReq = new RestRequest("/posts/recent");
            getPostsReq.RequestFormat = DataFormat.Json;

            if (Tags != null)
                getPostsReq.AddParameter("tag", Tags.ToString());
            getPostsReq.AddParameter("count", Count);

            return await ExecuteAsync<List<Post>>(getPostsReq);
        }

        /// <summary>
        /// Gets number of posts by date
        /// </summary>
        /// <param name="Tags">Filter by up to three tags</param>
        /// <returns></returns>
        public async Task<IRestResponse<List<DateResponse>>> GetPostDatesAsync(List<Tag> Tags = null)
        {
            RestRequest getDatesReq = new RestRequest("/posts/dates");

            if (Tags != null)
                getDatesReq.AddParameter("tag", Tags);

            return await ExecuteAsync<List<DateResponse>>(getDatesReq);
        }

        /// <summary>
        /// Get all posts. This is rate-limited to once every 60 seconds and will return a 429 error if you exceed that
        /// </summary>
        /// <param name="Tags"></param>
        /// <param name="Start">Offset to start from - default is 0</param>
        /// <param name="Results">Number of posts to return - default is all</param>
        /// <param name="FromDate">Return only bookmarks created after this time</param>
        /// <param name="ToDate">Return only bookmarks created before this time</param>
        /// <param name="ChangedMeta">Include the "meta" attribute in the response</param>
        /// <returns>A list of Posts</returns>
        public async Task<IRestResponse<List<Post>>> GetAllPostsAsync(List<Tag> Tags = null, int? Start = null, int? Results = null, DateTime? FromDate = null, DateTime? ToDate = null, bool ChangedMeta = false)
        {
            RestRequest getPostsReq = new RestRequest("/posts/all");
            getPostsReq.RequestFormat = DataFormat.Json;

            if (Tags != null)
                getPostsReq.AddParameter(ParameterHelpers.ValueToGetArgument("tag", Tags));
            if (Start.HasValue)
                getPostsReq.AddParameter("start", Start);
            if (Results.HasValue)
                getPostsReq.AddParameter("results", Results);
            if (FromDate.HasValue)
                getPostsReq.AddParameter("fromdt", FromDate);
            if (ToDate.HasValue)
                getPostsReq.AddParameter("todt", ToDate);

            return await ExecuteAsync<List<Post>>(getPostsReq);
        }

        public async Task<SuggestedTagsResponse> GetSuggestedTagsAsync(string url)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a full list of the user's tags along with the number of times they were used.
        /// </summary>
        /// <returns>A list of Tag, <seealso cref="Tag" />/></returns>
        public async Task<IRestResponse<List<Tag>>> GetTags()
        {
            RestRequest getTagsRequest = new RestRequest("/tags/get");
            return await ExecuteAsync<List<Tag>>(getTagsRequest);
        }

        //public PinboardResponse DeleteTag()
        //{
        //    throw new NotImplementedException();
        //}

        //public PinboardResponse RenameTag(string oldTag, string newTag)
        //{
        //    throw new NotImplementedException();
        //}

        //public string GetSecretKey()
        //{
        //    //return PinboardResponse.Result
        //    throw new NotImplementedException();
        //}

        //public string GetAPIToken()
        //{
        //    //return PinboardResponse.Result
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Execute RestRequest, as suggested in the RestSharp documentation
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="request">RestRequest</param>
        /// <returns>Response object of type T</returns>
        private async Task<IRestResponse<T>> ExecuteAsync<T>(RestRequest request) where T : class
        {
            if (client == null)
            {
                throw new NullReferenceException("Client didn't instantiate a RestClient");
            }

            client.UserAgent = "SharpPinboard 0.05, voltagex@voltagex.org";
            client.BaseUrl = new Uri(url);

            if (APIBaseURL.Contains("pinboard")) //should this be done differently?
            {
                client.Authenticator = new TokenAuthenticator(token);
            }
            else
            {
                client.Authenticator = new HttpBasicAuthenticator(username, token);
            }

            return await client.ExecuteTaskAsync<T>(request);

            //if (response.StatusCode != HttpStatusCode.OK || response.ErrorException != null)
            //{
            //    throw new ApplicationException(response.StatusDescription != "OK" ? response.StatusDescription : response.ErrorMessage, response.ErrorException);
            //}

            //await response.Data;
        }
    }
}

