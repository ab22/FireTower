using System;
using Blacksmith.Core;

namespace FireTower.IronMq
{
    public class IronMqSubscriber : IIronMqSubscriber
    {
        const string DevToken = "Gi_V3JqWFJ6u4kghrrTs46sVAWk";
        const string ProjectId = "533b4de5669fbf000900008c";
        readonly Client _client;

        public IronMqSubscriber()
        {
            _client = new Client(ProjectId, DevToken);
        }

        #region IIronMqSubscriber Members

        public void Subscribe(string queueName, string postUrl)
        {
            try
            {
                _client.Queue<object>(queueName).Subscribe(postUrl);
            }
            catch
            {                
            }
        }

        #endregion
    }
}