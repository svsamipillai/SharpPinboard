using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Pinboard.Types;
using System.Net;
using Pinboard.Helpers;
using System.Web.Script.Serialization;
using ServiceStack.Text;
using System.IO;

namespace Pinboard
{
    public class API
    {
        private const string APIBaseURL = "https://api.pinboard.in/v1/";
        private readonly string token = null;
        private readonly string url = null;

        /// <summary>
        /// Instantiates a new Pinboard API client 
        /// </summary>
        /// <param name="Username">The username used to connect to Pinboard</param>
        /// <param name="Token">API token, found on https://pinboard.in/settings/password </param>
        /// <param name="APIBaseURL">Base URL to use (defaults to Pinboard)</param>
        public API(string Token, string APIBaseURL = APIBaseURL)
        {
            this.token = Token;
            this.url = APIBaseURL;
        }

        /// <summary>
        /// Gets posts/update, returns the most recent time a bookmark was added, updated or deleted.
        /// </summary>
        public DateTime LastUpdated()
        {
            RestRequest LastUpdatedRequest = new RestRequest("/posts/update");
            return Execute<UpdateResponse>(LastUpdatedRequest).Time;
        }

        /// <summary>
        /// Adds a post based on the querystring we pass
        /// </summary>
        /// <param name="newPost">The post to add</param>
        /// <returns>PinboardResponse</returns>
        public PinboardResponse AddPost(Post newPost)
        {
            RestRequest AddPostRequest = new RestRequest("/posts/add");
            if (newPost.URL != null)
                AddPostRequest.AddParameter("url", newPost.URL);
            if (newPost.Description != null)
                AddPostRequest.AddParameter("description", newPost.Description);
            if (newPost.Extended != null)
                AddPostRequest.AddParameter("extended", newPost.Extended);
            if (newPost.Tags != null)
                AddPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("tags", newPost.Tags));
            if (newPost.CreationTime.HasValue)
                AddPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("dt", newPost.CreationTime));
            if (newPost.Replace != null)
                AddPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("replace", newPost.Replace));
            if (newPost.Shared != null)
                AddPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("shared", newPost.Shared));
            if (newPost.ToRead != null)
                AddPostRequest.AddParameter(ParameterHelpers.ValueToGetArgument("toread", newPost.ToRead));

            var response = Execute<PinboardResponse>(AddPostRequest);
            return response;
        }

        /// <summary>
        /// Delete a post via URL
        /// </summary>
        /// <param name="URL">URL we want to match</param>
        /// <returns>PinboardResponse</returns>
        public PinboardResponse DeletePost(string URL)
        {
            RestRequest DeletePostRequest = new RestRequest("/posts/delete");
            DeletePostRequest.AddParameter("url", URL);
            return Execute<PinboardResponse>(DeletePostRequest);

        }

        /// <summary>
        /// Delete a post
        /// </summary>
        /// <param name="deletePost">Post object with URL parameter</param>
        /// <returns>A Pinboard response - normally with a Code of "done"</returns>
        public PinboardResponse DeletePost(Post deletePost)
        {
            return DeletePost(deletePost.URL);
        }

        /// <summary>
        /// Get posts - filtered by URL, tags or date
        /// </summary>
        /// <param name="Tags">A list of up to 3 tags</param>
        /// <param name="CreatedDate">Return posts created on this date</param>
        /// <param name="URL">Return the post with this URL</param>
        /// <param name="ChangedMeta">Include the "meta" attribute in the response</param>
        /// <returns>A list of Post objects</returns>
        public List<Post> GetPosts(Tags Tags = null, DateTime? CreatedDate = null, string URL = null, bool ChangedMeta = false)
        {
            RestRequest GetPostsReq = new RestRequest("/posts/get");
            GetPostsReq.RequestFormat = DataFormat.Json;

            if (Tags != null)
                GetPostsReq.AddParameter(ParameterHelpers.ValueToGetArgument("tags", Tags));
            if (CreatedDate.HasValue)
                GetPostsReq.AddParameter(ParameterHelpers.ValueToGetArgument("dt", CreatedDate.Value));
            if (URL != null)
                GetPostsReq.AddParameter("url", URL);
            GetPostsReq.AddParameter(ParameterHelpers.ValueToGetArgument("meta", ChangedMeta));
            return Execute<List<Post>>(GetPostsReq);
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
        public List<Post> GetAllPosts(Tags Tags = null, int? Start = null, int? Results = null, DateTime? FromDate = null, DateTime? ToDate = null, bool ChangedMeta = false)
        {
            RestRequest GetPostsReq = new RestRequest("/posts/all");
            GetPostsReq.RequestFormat = DataFormat.Json;

            if (Tags != null)
                GetPostsReq.AddParameter("tag",Tags.ToString());
            if (Start.HasValue)
                GetPostsReq.AddParameter("start",Start);
            if (Results.HasValue)
                GetPostsReq.AddParameter("results",Results);
            if (FromDate.HasValue)
                GetPostsReq.AddParameter("fromdt",FromDate);
            if (ToDate.HasValue)
                GetPostsReq.AddParameter("todt",ToDate);
            
            return Execute<List<Post>>(GetPostsReq);
        }

        /// <summary>
        /// Gets most recent posts
        /// </summary>
        /// <param name="Tags">Filter by up to three tags</param>
        /// <param name="Count">Number of posts to return - default 15</param>
        /// <returns>A list of Posts</returns>
        public List<Post> GetRecentPosts(Tags Tags = null, int Count = 15)
        {
            RestRequest GetPostsReq = new RestRequest("/posts/recent");
            GetPostsReq.RequestFormat = DataFormat.Json;

            if (Tags != null)
                GetPostsReq.AddParameter("tag", Tags.ToString());
            GetPostsReq.AddParameter("count", Count);

            return Execute<List<Post>>(GetPostsReq);
        }

        /// <summary>
        /// Gets number of posts by date
        /// </summary>
        /// <param name="Tags">Filter by up to three tags</param>
        /// <returns></returns>
        public List<DateResponse> GetPostDates(Tags Tags = null)
        {
            RestRequest GetDatesReq = new RestRequest("/posts/dates");
            GetDatesReq.RequestFormat = DataFormat.Json;

            if (Tags != null)
                GetDatesReq.AddParameter("tag", Tags.ToString());
            
            return Execute<List<DateResponse>>(GetDatesReq);
        }

        /// <summary>
        /// Execute RestRequest, as suggested in the RestSharp documentation
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="request">RestRequest</param>
        /// <returns></returns>
        public T Execute<T>(RestRequest request) where T : new()
        {
            RestClient client = new RestClient();
           //client.AddHandler("text/plain", new RestSharp.Deserializers.JsonDeserializer());

            client.UserAgent = "SharpPinboard 0.01, voltagex@voltagex.org";
            client.BaseUrl = this.url;
            client.Authenticator = new TokenAuthenticator(this.token);
            client.AddDefaultParameter("Accept", "text/json", ParameterType.HttpHeader);
            client.AddDefaultParameter("format", "json");
            
            IRestResponse response = client.Execute(request);

           if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(response.StatusDescription);
            ServiceStack.Text.JsonSerializer<T> serializer = new ServiceStack.Text.JsonSerializer<T>();

#if DEBUG
           
            JsonSerializer<string> responseSerializer = new JsonSerializer<string>(); 
            responseSerializer.SerializeToWriter(response.Content, new StreamWriter("Debug.json"));
#endif

            return serializer.DeserializeFromString(response.Content);
        }
    }
}
