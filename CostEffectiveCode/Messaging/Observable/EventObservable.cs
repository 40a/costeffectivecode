﻿using System;
using CostEffectiveCode.Domain.Cqrs.Commands;

namespace CostEffectiveCode.Messaging.Observable
{
    /// <summary>
    /// Event-based implementation of IObservable&lt;T&gt;
    /// </summary>
    /// <typeparam name="T">type of message</typeparam>
    public class EventObservable<T> : IObservable<T>
    {
        private event Action<T> Event;

        // TODO: Need to check if Garbage Collector won't dispose handlers passed there. In other case we need to store handlers as instances
        public void AddHandler(ICommand<T> handler)
        {
            Event += handler.Execute;
        }

        public void AddHandler(Action<T> handler)
        {
            Event += handler;
        }

        public void RemoveHandler(ICommand<T> handler)
        {
            Event -= handler.Execute;
        }

        public void RemoveHandler(Action<T> handler)
        {
            Event -= handler;
        }

        public void HandleMessage(T message)
        {
            if (Event != null)
            {
                Event(message);
            }
        }
    }
}