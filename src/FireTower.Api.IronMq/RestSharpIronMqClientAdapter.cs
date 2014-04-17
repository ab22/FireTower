using System;
using System.Net;
using FireTower.Domain;
using Newtonsoft.Json;
using RestSharp;

namespace FireTower.IronMq
{
    public class RestSharpIronMqClientAdapter : IIronMqPusher
    {
        const string DevToken = "Gi_V3JqWFJ6u4kghrrTs46sVAWk";
        const string ProjectId = "533b4de5669fbf000900008c";        
        readonly RestClient _client;
        readonly string _queueName;

        public RestSharpIronMqClientAdapter(string queueName)
        {
            _queueName = queueName;
            _client = new RestClient(string.Format("https://mq-aws-us-east-1.iron.io/1/projects/{0}", ProjectId));
        }

        #region IIronMqPusher Members

        public void Push(Guid userSessionToken, object command)
        {
            IRestRequest request = GetRequest(userSessionToken, command);
            IRestResponse response = _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Push into IronMQ failed. " + response.StatusCode);
            }
            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }
        }

        #endregion

        RestRequest GetRequest(Guid userSessionId, object command)
        {
            string resource = string.Format("/queues/{0}/messages", _queueName);
            var restRequest = new RestRequest(resource, Method.POST) {RequestFormat = DataFormat.Json};
            restRequest.AddHeader("Authorization", "OAuth " + DevToken);
            Type type = command.GetType();
            dynamic obj = command.ToDynamic();
            obj.Token = userSessionId;
            obj.Type = type.AssemblyQualifiedName;
            var serializeObject = (string) JsonConvert.SerializeObject(obj);
            var message = new
                              {
                                  messages = new[]
                                                 {
                                                     new {body = serializeObject}
                                                 }
                              };
            restRequest.AddBody(message);
            return restRequest;
        }
    }
}