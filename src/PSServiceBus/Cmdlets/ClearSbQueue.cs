﻿using System.Collections.Generic;
using System.Management.Automation;
using PSServiceBus.Helpers;
using PSServiceBus.Outputs;
using PSServiceBus.Enums;

namespace PSServiceBus.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Clears all messages from an Azure Service Bus queue or subscription.</para>
    /// <para type="description">This cmdlet clears all messages from an Azure Service Bus queue or subscription.</para>
    /// <para type="description">The messages will be destroyed and not outputted to the console.</para>
    /// </summary>
    /// <example>
    /// <code>Clear-SbQueue -NamespaceConnectionString $namespaceConnectionString -QueueName 'example-queue'</code>
    /// <para>Clears all messages from the queue 'example-queue'</para>
    /// <para></para>
    /// </example>
    /// <example>
    /// <code>Clear-SbQueue -NamespaceConnectionString $namespaceConnectionString -TopicName 'example-topic' -SubscriptionName 'example-subscription'</code>
    /// <para>Clears all messages from the subscription called 'example-subscription' in the topic 'example-topic'</para>
    /// <para></para>
    /// </example>
    /// <example>
    /// <code>Clear-SbQueue -NamespaceConnectionString $namespaceConnectionString -QueueName 'example-queue' -ReceiveBatchQty 200</code>
    /// <para>Clears all messages from the queue 'example-queue'</para>
    /// <para>If you have insights into the message size in the queue, you can leverage that knowledge with the -ReceiveBatchQty parameter and increase the speed of the overall purge processs.</para>
    /// </example>
    /// <example>
    /// <code>Clear-SbQueue -NamespaceConnectionString $namespaceConnectionString -QueueName 'example-queue' -PrefetchQty 200</code>
    /// <para>Clears all messages from the queue 'example-queue'</para>
    /// <para>If you have insights into the number of messages in the queue, you can leverage that knowledge with the -PrefetchQty parameter and increase the speed of the overall purge processs.</para>
    /// </example>
    /// <example>
    /// <code>Clear-SbQueue -NamespaceConnectionString $namespaceConnectionString -QueueName 'example-queue' -ReceiveBatchQty 200 -PrefetchQty 200</code>
    /// <para>Clears all messages from the queue 'example-queue'</para>
    /// <para>If you have insights into the number of messages and the message size in the queue, you can leverage that knowledge with the -PrefetchQty and -ReceiveBatchQty parameters and increase the speed of the overall purge processs.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Clear, "SbQueue")]
    public class ClearSbQueue : PSCmdlet
    {
        /// <summary>
        /// <para type="description">A connection string with 'Manage' rights for the Azure Service Bus Namespace.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string NamespaceConnectionString { get; set; }

        /// <summary>
        /// <para type="description">The name of the queue to retrieve messages from.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            ParameterSetName = "ClearQueue"
        )]
        public string QueueName { get; set; }

        /// <summary>
        /// <para type="description">The name of the topic containing the subscription to retrieve messages from.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            ParameterSetName = "ClearSubscription"
        )]
        public string TopicName { get; set; }

        /// <summary>
        /// <para type="description">The name of the subscription to retrieve messages from.</para>
        /// </summary>
        [Parameter(
            Mandatory = true,
            ParameterSetName = "ClearSubscription"
        )]
        public string SubscriptionName { get; set; }

        /// <summary>
        /// <para type="description">The number of messages to retrieve in a single batch - defaults to 1. Increasing this number will increase the efficiency for the cmdlet per round trip to the service bus queue / topic and makes the purge operation faster.</para>
        /// </summary>
        [Parameter]
        public int ReceiveBatchQty { get; set; } = 1;

        /// <summary>
        /// <para type="description">The number of messages to prefetch - defaults to 1. While the cmdlet is working on a batch of messages, it can utilize some caching mechanisms that will prefetch messages in a background thread. Increasing this number will increase the efficiency for the cmdlet and makes the purge operation faster.</para>
        /// </summary>
        [Parameter]
        public int PrefetchQty { get; set; } = 1;

        /// <summary>
        /// <para type="description">Retrieves messages from the entity's dead letter queue.</para>
        /// </summary>
        [Parameter]
        public SwitchParameter DeadLetterQueue { get; set; }

        /// <summary>
        /// Main cmdlet method.
        /// </summary>
        protected override void ProcessRecord()
        {
            SbReceiver sbReceiver;
            SbManager sbManager = new SbManager(NamespaceConnectionString);

            if (this.ParameterSetName == "ClearQueue")
            {
                sbReceiver = new SbReceiver(NamespaceConnectionString, QueueName, DeadLetterQueue, sbManager, ReceiveBatchQty, PrefetchQty, true);
            }
            else
            {
                sbReceiver = new SbReceiver(NamespaceConnectionString, TopicName, SubscriptionName, DeadLetterQueue, sbManager, ReceiveBatchQty, PrefetchQty, true);
            }
            
            sbReceiver.PurgeMessages(this);
            sbReceiver.Dispose();
        }
    }
}
