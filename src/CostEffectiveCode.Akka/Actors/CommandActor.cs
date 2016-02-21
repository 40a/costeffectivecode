﻿using System;
using Akka.Actor;
using CostEffectiveCode.Common;
using CostEffectiveCode.Common.Logger;
using CostEffectiveCode.Domain.Cqrs.Commands;
using JetBrains.Annotations;

namespace CostEffectiveCode.Akka.Actors
{

    public class CommandActor<TMessage> : ReceiveActor
    {
        private readonly ICommand<TMessage> _command;
        private readonly ILogger _logger;

        public CommandActor([NotNull] ICommand<TMessage> command, [CanBeNull] ILogger logger)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            _command = command;
            _logger = logger;

            Receive<TMessage>(x => Handle(x));
        }

        public void Handle(TMessage message)
        {
            _logger?.Debug($"CommandActor received message of type {message.GetType()} with value of {message}");
            _command.Execute(message);
        }
    }

}
