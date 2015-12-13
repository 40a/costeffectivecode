﻿using System;
using CostEffectiveCode.Domain.Cqrs.Queries;
using CostEffectiveCode.Domain.Ddd.Entities;
using CostEffectiveCode.Domain.Ddd.Specifications;
using CostEffectiveCode.Domain.Ddd.UnitOfWork;

namespace CostEffectiveCode.EntityFramework
{
    [Obsolete("Was used as a naive implementation in tests", true)]
    public class ExpressionQueryFactory : IQueryFactory
    {
        private readonly ILinqProvider _linqProvider;

        public ExpressionQueryFactory(ILinqProvider linqProvider)
        {
            _linqProvider = linqProvider;
        }

        public IQuery<TEntity, IExpressionSpecification<TEntity>> GetQuery<TEntity>()
            where TEntity : class, IEntity
        {
            return new ExpressionQuery<TEntity>(_linqProvider);
        }

        public IQuery<TEntity, TSpecification> GetQuery<TEntity, TSpecification>()
            where TEntity : class, IEntity where TSpecification : ISpecification<TEntity>
        {
            throw new NotSupportedException();
        }

        public TQuery GetQuery<TEntity, TSpecification, TQuery>()
            where TEntity : class, IEntity
            where TSpecification : ISpecification<TEntity>
            where TQuery : IQuery<TEntity, TSpecification>
        {
            throw new NotSupportedException();
        }
    }
}
