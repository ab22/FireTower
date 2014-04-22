using System;
using AcklenAvenue.Testing.AAT;
using FireTower.Domain;
using FireTower.Domain.Commands;
using Machine.Specifications;
using RestSharp;

namespace FireTower.API.AAT
{
    
    public class when_posting_a_command_from_the_queue : given_an_api_server_context<CurrentlyDeveloping>
    {
        static IRestResponse _result;
        static dynamic _commandFromQueue;

        Establish context = () =>
                                {
                                    RegisterUser();

                                    Guid token = Login().Token;
                                    var command = new NewUserCommand
                                                      {
                                                          FirstName = "Byron",
                                                          LastName = "Sommardahl",
                                                          Name = "Byron Sommardahl",
                                                          FacebookId = "123456",
                                                          Locale = "es_ES",
                                                          Username = "bsommardahl",                                                          
                                                      };

                                    _commandFromQueue = command.ToDynamic();
                                    _commandFromQueue.Token = token;
                                    _commandFromQueue.Type = command.GetType().AssemblyQualifiedName;
                                };

        Because of =
            () => _result = Client.Post("/work", new NewUserCommand
                                                     {
                                                         FirstName = "Byron",
                                                         LastName = "Sommardahl",
                                                         Name = "Byron Sommardahl",
                                                         FacebookId = "123456",
                                                         Locale = "es_ES",
                                                         Username = "bsommardahl",
                                                     });

        //It should_process_the_command =
        //    () => _result.ShouldBeOk();
    }
}