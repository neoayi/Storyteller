﻿using System;
using System.Threading;
using StoryTeller.Engine;
using StoryTeller.Messages;
using StoryTeller.Remotes.Messaging;

namespace StoryTeller
{
    /// <summary>
    /// Runs inside a remote process
    /// </summary>
    public class StorytellerAgent : IListener<StartProject>, IListener<Shutdown>
    {
        private readonly EngineAgent _engine;
        private readonly ManualResetEvent _completion = new ManualResetEvent(false);

        public StorytellerAgent(int port, ISystem system)
        {
            Console.WriteLine($"AGENT: Running the StorytellerAgent at port {port} with system {system.GetType().Name}");

            EventAggregator.Messaging.AddListener(this);

            _engine = new EngineAgent(port, system);
        }

        public void Receive(StartProject message)
        {
            _engine.Start(message.Project);
        }

        public void Receive(Shutdown message)
        {
            Console.WriteLine("Shutdown requested...");

            try
            {
                _engine.Dispose();

                Console.WriteLine("Shut down gracefully.");
            }
            catch (Exception e)
            {
                ConsoleWriter.Write(ConsoleColor.Red, e.ToString());
                throw;
            }
            finally
            {
                _completion.Set();
            }
        }

        public static void Run(string[] args)
        {
            Run(args, new NulloSystem());
        }

        public static void Run(string[] args, ISystem system)
        {
            var agent = new StorytellerAgent(int.Parse(args[0]), system);
            agent._completion.WaitOne();
        }
    }

    public class Shutdown : ClientMessage
    {
        public Shutdown() : base("shutdown")
        {
        }
    }
}