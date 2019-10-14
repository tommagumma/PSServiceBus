﻿using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;

namespace PSServiceBus.Tests.Utils
{
    public class ServiceBusUtils
    {
        public string NamespaceConnectionString;

        private readonly ManagementClient managementClient;

        public ServiceBusUtils (string NamespaceConnectionString)
        {
            this.NamespaceConnectionString = NamespaceConnectionString;
            this.managementClient = new ManagementClient(NamespaceConnectionString);
        }

        public void CreateQueue(string QueueName)
        {
            this.managementClient.CreateQueueAsync(QueueName);
        }

        public void RemoveQueue(string QueueName)
        {
            this.managementClient.DeleteQueueAsync(QueueName);
        }
    }
}
