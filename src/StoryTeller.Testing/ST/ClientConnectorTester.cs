﻿using System.Collections.Generic;
using System.Linq;
using Xunit;
using NSubstitute;
using Shouldly;
using ST.Client;
using StoryTeller.Commands;
using StoryTeller.Messages;
using StoryTeller.Remotes.Messaging;
using ST;

namespace StoryTeller.Testing.ST
{
    
    public class ClientConnectorTester
    {
        private readonly RecordingCommand<RunSpec> theCommand;
        private readonly IRemoteController theRemoteController;
        private readonly ClientConnector theConnector;

        public ClientConnectorTester()
        {
            theCommand = new RecordingCommand<RunSpec>();
            theRemoteController = Substitute.For<IRemoteController>();

            theConnector = new ClientConnector(new WebSocketsHandler(), theRemoteController, new ICommand[] { theCommand });
        }


        [Fact]
        public void calls_to_the_handler_if_one_matches_the_json()
        {
            var message = new RunSpec {id = "foo"};
            var json = JsonSerialization.ToCleanJson(message);

            theConnector.HandleJson(json);

            theCommand.Received.Single()
                .id.ShouldBe("foo");

            theRemoteController.DidNotReceive().SendJsonMessage(json);
        }

        [Fact]
        public void delegates_to_the_remote_controller_if_no_matching_handler()
        {
            var json = "{foo: 1}";

            theConnector.HandleJson(json);

            theRemoteController.Received().SendJsonMessage(json);
        }
    }

    public class RecordingCommand<T> : Command<T> where T : ClientMessage
    {
        public readonly IList<T> Received = new List<T>();

        public override void HandleMessage(T message)
        {
            Received.Add(message);
        }
    }
}