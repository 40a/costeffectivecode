﻿using CostEffectiveCode.Common;
using CostEffectiveCode.Domain.Ddd.Entities;
using CostEffectiveCode.Domain.Ddd.UnitOfWork;
using JetBrains.Annotations;

namespace CostEffectiveCode.Domain.Cqrs.Commands
{
    public class CreateEntityCommand<T> : UnitOfWorkScopeCommand<T>
        where T: class, IEntity
    {
        public override void Execute(T context)
        {
            UnitOfWorkScope.Instance.Save(context);
            UnitOfWorkScope.Instance.Commit();
        }

        public CreateEntityCommand([NotNull] IScope<IUnitOfWork> unitOfWorkScope)
            : base(unitOfWorkScope)
        {
        }
    }
}
