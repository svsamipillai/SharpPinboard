using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pinboard.Helpers;
using Pinboard.Types;
using RestSharp;

namespace Pinboard
{
    public class Api
    {
        private const string ApiBaseUrl = "http://api.pinboard.in/v1/";
        private static RestClient _client;
        private readonly string _token;
        private readonly string _url;

        public Api(string token)
        {
            _token = token;
            _url = ApiBaseUrl;
            _client = new RestClient(_url);
        }

        public async Task<IRestResponse<UpdateResponse>> GetLastUpdateAsync()
        {
            const string resource = "/posts/update";
            var getUpdateRequest = new RestRequest(resource);
            return await ExecuteAsync<UpdateResponse>(getUpdateRequest);
        }

        public async Task<IRestResponse<PinboardResponse>> AddPostAsync(Post newPost)
        {
            var addPostRequest = new RestRequest("/posts/add");

            if (newPost.Href != null)
            {
                addPostRequest.AddParameter("url", newPost.Href);
            }
            if (newPost.Description != null)
            {
                addPostRequest.AddParameter("description", newPost.Description);
            }
            if (newPost.Extended != null)
            {
                addPostRequest.AddParameter("extended", newPost.Extended);
            }
            if (newPost.Tag != null)
            {
                addPostRequest.AddParameter(
                    ParameterHelpers.ValueToGetArgument("tags", newPost.Tag));
            }
            if (newPost.CreationTime.HasValue)
            {
                addPostRequest.AddParameter(
                    ParameterHelpers.ValueToGetArgument("dt", newPost.CreationTime));
            }
            if (newPost.Replace != null)
            {
                addPostRequest.AddParameter(
                    ParameterHelpers.ValueToGetArgument("replace", newPost.Replace));
            }
            if (newPost.Shared != null)
            {
                addPostRequest.AddParameter(
                    ParameterHelpers.ValueToGetArgument("shared", newPost.Shared));
            }
            if (newPost.ToRead != null)
            {
                addPostRequest.AddParameter(
                    ParameterHelpers.ValueToGetArgument("toread", newPost.ToRead));
            }

            return await ExecuteAsync<PinboardResponse>(addPostRequest);
        }

        public async Task<IRestResponse<PinboardResponse>> DeletePost(string url)
        {
            var deletePostRequest = new RestRequest("/posts/delete");
            deletePostRequest.AddParameter("url", url);
            return await ExecuteAsync<PinboardResponse>(deletePostRequest);
        }

        public async Task<IRestResponse<PinboardResponse>> DeletePostAsync(Post deletePost)
        {
            return await DeletePost(deletePost.Href);
        }

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

        public async Task<IRestResponse<List<Post>>> GetRecentPostsAsync(List<Tag> tags = null, int count = 15)
        {
            var getPostsReq = new RestRequest("/posts/recent") {RequestFormat = DataFormat.Json};

            if (tags != null)
                getPostsReq.AddParameter("Name", tags.ToString());

            getPostsReq.AddParameter("count", count);

            return await ExecuteAsync<List<Post>>(getPostsReq);
        }

        public async Task<IRestResponse<List<DateResponse>>> GetPostDatesAsync(List<Tag> tags = null)
        {
            var getDatesReq = new RestRequest("/posts/dates");

            if (tags != null)
                getDatesReq.AddParameter("Name", tags);

            return await ExecuteAsync<List<DateResponse>>(getDatesReq);
        }

        public async Task<IRestResponse<List<Post>>> GetAllPostsAsync(List<Tag> tags = null, int? start = null,
            int? results = null, DateTime? fromDate = null, DateTime? toDate = null, bool changedMeta = false)
        {
            var getPostsReq = new RestRequest("/posts/all") {RequestFormat = DataFormat.Json};

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

        public async Task<IRestResponse<List<Tag>>> GetTags(string format)
        {
            var getTagsRequest = new RestRequest("/tags/get");
            getTagsRequest.AddHeader("Content-Type", "application/json");
            getTagsRequest.AddQueryParameter("format", format);
            return await ExecuteAsync<List<Tag>>(getTagsRequest);
        }

        private async Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request) where T : class
        {
            if (_client == null)
            {
                throw new NullReferenceException("Client didn't instantiate a RestClient");
            }

            _client.UserAgent = "Rest Sharp Client";
            _client.BaseUrl = new Uri(_url);
            _client.Authenticator = new TokenAuthenticator(_token);

            return await _client.ExecuteTaskAsync<T>(request);
        }
    }
}