using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ForumDb.Repositories;
using Newtonsoft.Json;

namespace ForumDb.IntegrationTests
{
    class InMemoryHttpServer<T>
    {
        private const string ApplicationJSON = "application/json";
        private const string ApplicationXML = "application/xml";

        private readonly HttpClient client;
        private string baseUrl;

        public InMemoryHttpServer(string baseUrl)
            //, IRepository<T> repository)
        {
            this.baseUrl = baseUrl;
            var config = new HttpConfiguration();
            this.AddHttpRoutes(config.Routes);
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            var resolver = new TestDependencyResolver();
            //resolver.Repository = repository;
            config.DependencyResolver = resolver;

            var server = new HttpServer(config);
            this.client = new HttpClient(server);
        }

        public HttpResponseMessage CreateGetRequest(string requestUrl, string mediaType = ApplicationJSON)
        {
            var url = requestUrl;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseUrl + url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Method = HttpMethod.Get;

            var response = this.client.SendAsync(request).Result;
            return response;
        }

        public Task<HttpResponseMessage> CreateGetRequestAsync(string requestUrl, string mediaType = ApplicationJSON)
        {
            var url = requestUrl;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseUrl + url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Method = HttpMethod.Get;

            var response = this.client.SendAsync(request);
            return response;
        }

        public HttpResponseMessage CreatePostRequest(string requestUrl, object data, string mediaType = ApplicationJSON)
        {
            var url = requestUrl;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseUrl + url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Method = HttpMethod.Post;

            if (mediaType == ApplicationJSON)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(data));
            }
            else if (mediaType == ApplicationXML)
            {
                request.Content = new StringContent(XmlConvert.SerializeObject(data));
            }
            
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

            var response = this.client.SendAsync(request).Result;
            return response;
        }

        public HttpResponseMessage CreatePutRequest(string requestUrl, object data, string mediaType = ApplicationJSON)
        {
            var url = requestUrl;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(baseUrl + url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Method = HttpMethod.Put;

            if (mediaType == ApplicationJSON)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(data));
            }
            else if (mediaType == ApplicationXML)
            {
                request.Content = new StringContent(XmlConvert.SerializeObject(data));
            }

            request.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

            var response = this.client.SendAsync(request).Result;
            return response;
        }

        private void AddHttpRoutes(HttpRouteCollection routeCollection)
        {
            var routes = GetRoutes();
            foreach (var route in routes)
            {
                routeCollection.MapHttpRoute(route.Name, route.Template, route.Defaults);
            }

            //routes.ForEach(route => routeCollection.MapHttpRoute(route.Name, route.Template, route.Defaults));
        }

        private List<Route> GetRoutes()
        {
            List<Route> routeList = new List<Route>();

            Route defaultApiRoute = new Route(
                "DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            Route PostsApiRoute = new Route(
                "PostsApi", "api/posts/{postId}/{action}", new { controller = "posts" });

            Route PostsCreateApi = new Route(
                "PostCreateApi", "api/posts/{action}", new { controller = "posts", action = "create" });

            Route ThreadsPostsApiRoute = new Route(
                "ThreadsPostsApi", "api/threads/{threadId}/{action}", new { controller = "threads", action = "posts" });

            Route usersApiRoute = new Route(
                "UsersApi", "api/users/{action}", new { controller = "users" });

            Route categoriesApiRoute = new Route(
                 "CategoriesApi", "api/categories/{action}", new { controller = "categories", action = "create" });

            routeList.Add(defaultApiRoute);
            routeList.Add(PostsApiRoute);
            routeList.Add(PostsCreateApi);
            routeList.Add(ThreadsPostsApiRoute);
            routeList.Add(usersApiRoute);
            routeList.Add(categoriesApiRoute);

            return routeList;
        }

        private class Route
        {
            public object Defaults { get; set; }

            public string Name { get; set; }

            public string Template { get; set; }

            public Route(string name, string template, object defaults)
            {
                this.Defaults = defaults;
                this.Name = name;
                this.Template = template;
            }
        }
    }
}
