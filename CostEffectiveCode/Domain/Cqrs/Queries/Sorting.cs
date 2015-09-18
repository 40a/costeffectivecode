﻿using System;
using System.Linq.Expressions;
using CostEffectiveCode.Domain.Ddd.Entities;
using JetBrains.Annotations;

namespace CostEffectiveCode.Domain.Cqrs.Queries
{
    /// <summary>
    ///     Sort order enumeration
    /// </summary>
    [PublicAPI]
    public enum SortOrder
    {
        [PublicAPI] Asc = 1,
        [PublicAPI] Desc = 2
    }

    [PublicAPI]
    public class Sorting<TEntity, TKey>
        where TEntity: class, IEntity
    {
        public Expression<Func<TEntity, TKey>> Expression { get; private set; }

        public SortOrder SortOrder { get; private set; }

        public Sorting(
            [NotNull] Expression<Func<TEntity, TKey>> expression,
            SortOrder sortOrder = SortOrder.Asc)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            Expression = expression;
            SortOrder = sortOrder;
        }
    }

}
