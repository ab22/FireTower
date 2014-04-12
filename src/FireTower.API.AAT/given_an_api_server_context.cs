using System;
using AcklenAvenue.Testing.AAT;
using FireTower.Presentation.Requests;
using FireTower.Presentation.Responses;
using Machine.Specifications;
using MongoDB.Driver;
using RestSharp;

namespace FireTower.API.AAT
{
    public class given_an_api_server_context<T> where T : IAATState
    {
        protected static AATClient Client;

        Establish master_context =
            () => { Client = new AATClient(GetHost()); };

        static string GetHost()
        {
            if (typeof (T) == typeof (DeployedToQA))
            {
                return "https://firetowerapidev.apphb.com";
            }
            if (typeof (T) == typeof (DeployedToQaWorker))
            {
                throw new NotImplementedException("Please add thr url for the qa worker server.");
            }
            else
            {
                return "http://localhost:38397/";
            }
        }

        protected static SuccessfulLoginResponse<Guid> Login()
        {
            IRestResponse<SuccessfulLoginResponse<Guid>> restResponse =
                Client.Execute<SuccessfulLoginResponse<Guid>>("/login", Method.POST,
                                                              new BasicLoginRequest
                                                              {
                                                                  Email = "test@test.com",
                                                                  Password = "password"
                                                              });
            return restResponse.Data;
        }

        protected static long RegisterUser()
        {
            var newUserRequest = new NewUserRequest
                                     {
                                         FirstName = "Byron", LastName = "Sommardahl", Name = "Byron Sommardahl", FacebookId = 123456, Locale = "es_ES", Username = "bsommardahl", Verified = true
                                     };
            Client.Execute("/user/facebook", Method.POST,
                                                               newUserRequest);
            return newUserRequest.FacebookId;
        }


        protected static MongoDatabase MongoDatabase()
        {
            var uri =
                new MongoUrl(
                    @"mongodb://client:password@ds045137.mongolab.com:45137/appharbor_ab50c767-930d-4b7d-9571-dd2a0b62d5a9");

            MongoServer server = new MongoClient(uri).GetServer();

            MongoDatabase db = server.GetDatabase(uri.DatabaseName);
            return db;
        }
    }
}