﻿using System;
using StoryTeller.Commands;
using StoryTeller.Messages;

namespace ST.Client.Persistence
{
    public class AddSuiteCommand : Command<AddSuite>
    {
        private readonly Lazy<IPersistenceController> _controller;

        public AddSuiteCommand(Lazy<IPersistenceController> controller)
        {
            _controller = controller;
        }

        public override void HandleMessage(AddSuite message)
        {
            _controller.Value.AddSuite(message.parent, message.name);
        }
    }
}