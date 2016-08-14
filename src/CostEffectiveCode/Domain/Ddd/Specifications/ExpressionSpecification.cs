﻿using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace CosteffectiveCode.Domain.Ddd.Specifications
{
    [PublicAPI]
    public class ExpressionSpecification<T> : ISpecification<T>
    {
        public virtual Expression<Func<T, bool>> Expression { get; private set; }

        private Func<T, bool> _func;

        private Func<T, bool> Func => _func ?? (_func = Expression.Compile());

        public ExpressionSpecification([NotNull] Expression<Func<T, bool>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            Expression = expression;
        }

        public bool IsSatisfiedBy(T o)
        {
            return Func(o);
        }
    } 
}
