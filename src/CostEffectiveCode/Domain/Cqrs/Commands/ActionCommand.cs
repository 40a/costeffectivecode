﻿using System;
using JetBrains.Annotations;

namespace CosteffectiveCode.Domain.Cqrs.Commands
{
    public class ActionCommand<T> : CommandBase<T>
    {
        private readonly Action<T> _action;

        public ActionCommand([NotNull] Action<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _action = action;
        }

        protected override void DoExecute(T input)
        {
            _action.Invoke(input);
        }
    }
}
