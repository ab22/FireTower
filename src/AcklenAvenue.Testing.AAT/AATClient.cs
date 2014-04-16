using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using RestSharp;

namespace AcklenAvenue.Testing.AAT
{
    public class AATClient
    {
        readonly string _host;

        public AATClient(string host)
        {
            _host = host;
        }

        public IRestResponse Execute(string resource, Method method, object payload = null)
        {
            RestClient browser = GetRestClient();
            RestRequest request = BuildRestRequest(resource, method, payload);
            return browser.Execute(request);
        }

        public IRestResponse<T> Get<T>(string resource, object args = null) where T : new()
        {
            RestClient browser = GetRestClient();

            RestRequest request = BuildRestRequest(resource, Method.GET);
            if (args != null)
            {
                foreach (PropertyInfo property in args.GetType().GetProperties())
                {
                    request.AddParameter(property.Name, property.GetValue(args));
                }
            }

            return ExecuteRequest<T>(browser, request);
        }

        public IRestResponse Post(string resource, object payload, Guid? token = null)
        {
            RestClient browser = GetRestClient();
            RestRequest request = BuildRestRequest(resource, Method.POST, payload);
            if (token.HasValue)
            {
                browser.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token.Value.ToString());
            }
            return ExecuteRequest(browser, request);
        }

        private byte[] GetFile(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
            using (var ms = new MemoryStream())
            {
                Image.FromStream(stream).Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
        }

        public IRestResponse UploadFile(string resource, string url, Guid token, object payload = null)
        {
            RestClient browser = GetRestClient();
            var request = new RestRequest(resource, Method.POST)
                               {
                                   JsonSerializer = new RestSharpJsonNetSerializer(),
                                   RequestFormat = DataFormat.Json
                               };
            
            request.AddFile("image", GetFile(url), "someFilename");
            
            if (payload != null)
            {
                foreach(var property in payload.GetType().GetProperties())
                {
                    request.AddParameter(property.Name, property.GetValue(payload));
                }
            }
                

            browser.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token.ToString());
            return ExecuteRequest(browser, request);
        }

        RestClient GetRestClient(Guid? token = null)
        {
            var restClient = new RestClient(_host);
            if (token.HasValue)
                restClient.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token.ToString());
            return restClient;
        }

        public IRestResponse Delete(string resource, Guid token)
        {
            RestClient browser = GetRestClient(token);
            RestRequest request = BuildRestRequest(resource, Method.DELETE);
            return ExecuteRequest(browser, request);
        }

        public IRestResponse Get(string resource, object args = null)
        {
            RestClient browser = GetRestClient();

            RestRequest request = BuildRestRequest(resource, Method.GET);
            if (args != null)
            {
                foreach (PropertyInfo prop in args.GetType().GetProperties())
                {
                    request.AddParameter(prop.Name, prop.GetValue(args));
                }
            }

            return ExecuteRequest(browser, request);
        }

        static IRestResponse<T> ExecuteRequest<T>(RestClient browser, RestRequest request) where T : new()
        {
            IRestResponse<T> restResponse = browser.Execute<T>(request);
            IEnumerable<Parameter> corsHeader = restResponse.Headers.Where(x => x.Name == "Access-Control-Allow-Origin");
            if(restResponse.StatusCode==0)
            {
                throw new AATServerUnreachableException(browser, request);
            }
            if (!corsHeader.Any() && restResponse.ErrorException == null)
            {
                throw new AATCORSException();
            }
            if (restResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AATUnauthorizedException();
            }
            if (restResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new AATRestResponseException(restResponse, request);
            }
            if (restResponse.ErrorException != null)
            {
                throw restResponse.ErrorException;
            }
            if (restResponse.Data == null)
            {
                throw new Exception("Response data is null.");
            }
            return restResponse;
        }

        static IRestResponse ExecuteRequest(RestClient browser, RestRequest request)
        {
            IRestResponse restResponse = browser.Execute(request);
            if (restResponse.ErrorException != null)
            {
                throw restResponse.ErrorException;
            }
            return restResponse;
        }

        public IRestResponse<T> Execute<T>(string resource, Method method, object payload = null)
            where T : new()
        {
            RestClient browser = GetRestClient();
            RestRequest request = BuildRestRequest(resource, method, payload);
            return ExecuteRequest<T>(browser, request);
        }

        static RestRequest BuildRestRequest(string resource, Method method, object payload = null)
        {
            var request = new RestRequest(resource, method)
                              {
                                  JsonSerializer = new RestSharpJsonNetSerializer(),
                                  RequestFormat = DataFormat.Json
                              };
            //request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("Accept", "application/json");

            if (payload != null)
            {
                request.AddBody(payload);
            }
            return request;
        }
    }
}