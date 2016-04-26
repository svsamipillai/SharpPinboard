using Pinboard.Helpers;
using Pinboard.Types;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pinboard
{
    public class Api
    {
        private const string ApiBaseUrl = "https://api.pinboard.in/v1/";
        private static RestClient _client;
        private readonly string _token;
        private readonly string _url;
        private readonly string _username;

        /// <summary>
        ///     Instantiates a new Pinboard API client
        /// </summary>
        public Api(string token, string apiBaseUrl = ApiBaseUrl, string username = null, RestClient apiClient = null)
        {
            _client = apiClient ?? new RestClient(apiBaseUrl);

            _token = token;
            _url = apiBaseUrl;

            if (!string.IsNullOrEmpty(username))
            {
                _username = username;
            }
        }

        /// <summary>
        ///     Returns the most recent time a bookmark was added, updated or deleted.
        ///     Use this before calling posts/all to see if the data has changed since the last fetch.
        /// </summary>
        public async Task<IRestResponse<UpdateResponse>> GetLastUpdateAsync()
        {
            var getUpdateRequest = new RestRequest("/posts/update");
            return await ExecuteAsync<UpdateResponse>(getUpdateRequest);
        }

        /// <summary>
        ///     Adds a post based on the querystring we pass
        /// </summary>
        public async Task<IRestResponse<PinboardResponse>> AddPostAsync(Post newPost)
        {
            var addPostRequest = new RestRequest("/posts/add");
            if (newPost.Href != null)
                addPostRequest.AddParameter("url", newPost.Href);
            if (newPost.Description != null)
                addPostRequest.AddParameter("description", newPost.Description);
            if (newPost.Extended != null)
                addPostRequest.AddParameter("extended", newPost.Extended);
            if (newPost.Tag != null)
                addPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("tags", newPost.Tag));
            if (newPost.CreationTime.HasValue)
                addPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("dt", newPost.CreationTime));
            if (newPost.Replace != null)
                addPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("replace", newPost.Replace));
            if (newPost.Shared != null)
                addPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("shared", newPost.Shared));
            if (newPost.ToRead != null)
                addPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("toread", newPost.ToRead));

            return await ExecuteAsync<PinboardResponse>(addPostRequest);
        }

        /// <summary>
        ///     Delete a post via URL
        /// </summary>
        public async Task<IRestResponse<PinboardResponse>> DeletePost(string url)
        {
            var deletePostRequest = new RestRequest("/posts/delete");
            deletePostRequest.AddParameter("url", url);
            return await ExecuteAsync<PinboardResponse>(deletePostRequest);
        }

        /// <summary>
        ///     Delete a post
        /// </summary>
        public async Task<IRestResponse<PinboardResponse>> DeletePostAsync(Post deletePost)
        {
            return await DeletePost(deletePost.Href);
        }

        /// <summary>
        ///     Get posts - filtered by URL, tags or date
        /// </summary>
        public async Task<IRestResponse<List<Post>>> GetPostsAsync(List<Tag> tags = null, DateTime? createdDate = null,
            string url = null, bool changedMeta = false)
        {
            var getPostsReq = new RestRequest("/posts/get");

            if (tags != null)
                getPostsReq.AddParameter(ParameterHelpers.ValueToGetArgument("tags", tags));
            if (createdDate.HasValue)
                getPostsReq.AddParameter(ParameterHelpers.ValueToGetArgument("dt", createdDate.Value));
            if (url != null)
                getPostsReq.AddParameter("url", url);
            getPostsReq.AddParameter(ParameterHelpers.ValueToGetArgument("meta", changedMeta));

            return await ExecuteAsync<List<Post>>(getPostsReq);
        }

        /// <summary>
        ///     Gets most recent posts
        /// </summary>
        public async Task<IRestResponse<List<Post>>> GetRecentPostsAsync(List<Tag> tags = null, int count = 15)
        {
            var getPostsReq = new RestRequest("/posts/recent") { RequestFormat = DataFormat.Json };

            if (tags != null)
                getPostsReq.AddParameter("Name", tags.ToString());

            getPostsReq.AddParameter("count", count);

            return await ExecuteAsync<List<Post>>(getPostsReq);
        }

        /// <summary>
        ///     Gets number of posts by date
        /// </summary>
        public async Task<IRestResponse<List<DateResponse>>> GetPostDatesAsync(List<Tag> tags = null)
        {
            var getDatesReq = new RestRequest("/posts/dates");

            if (tags != null)
                getDatesReq.AddParameter("Name", tags);

            return await ExecuteAsync<List<DateResponse>>(getDatesReq);
        }

        /// <summary>
        ///     Get all posts. This is rate-limited to once every 60 seconds and will return a 429 error if you exceed that
        /// </summary>
        public async Task<IRestResponse<List<Post>>> GetAllPostsAsync(List<Tag> tags = null, int? start = null,
            int? results = null, DateTime? fromDate = null, DateTime? toDate = null, bool changedMeta = false)
        {
            var getPostsReq = new RestRequest("/posts/all") { RequestFormat = DataFormat.Json };

            if (tags != null)
                getPostsReq.AddParameter(ParameterHelpers.ValueToGetArgument("Name", tags));
            if (start.HasValue)
                getPostsReq.AddParameter("start", start);
            if (results.HasValue)
                getPostsReq.AddParameter("results", results);
            if (fromDate.HasValue)
                getPostsReq.AddParameter("fromdt", fromDate);
            if (toDate.HasValue)
                getPostsReq.AddParameter("todt", toDate);

            return await ExecuteAsync<List<Post>>(getPostsReq);
        }

        /// <summary>
        ///     Returns a full list of the user's tags along with the number of times they were used.
        /// </summary>
        public async Task<IRestResponse<List<Tag>>> GetTags()
        {
            var getTagsRequest = new RestRequest("/tags/get");
            return await ExecuteAsync<List<Tag>>(getTagsRequest);
        }

        /// <summary>
        ///     Execute RestRequest, as suggested in the RestSharp documentation
        /// </summary>
        private async Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request) where T : class
        {
            if (_client == null)
            {
                throw new NullReferenceException("Client didn't instantiate a RestClient");
            }

            _client.UserAgent = "SharpPinboard 0.05, voltagex@voltagex.org";
            _client.BaseUrl = new Uri(_url);

            if (ApiBaseUrl.Contains("pinboard"))
            {
                _client.Authenticator = new TokenAuthenticator(_token);
            }
            else
            {
                _client.Authenticator = new HttpBasicAuthenticator(_username, _token);
            }

            return await _client.ExecuteTaskAsync<T>(request);
        }
    }
}