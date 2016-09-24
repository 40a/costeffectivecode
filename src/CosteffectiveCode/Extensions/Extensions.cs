﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using CostEffectiveCode.Common;
using CostEffectiveCode.Cqrs.Queries;
using CostEffectiveCode.Ddd.Entities;
using CostEffectiveCode.Ddd.Specifications;
using CostEffectiveCode.Ddd.Specifications.UnitOfWork;
using JetBrains.Annotations;

namespace CostEffectiveCode.Extensions
{
    [PublicAPI]
    public static class Extensions
    {
        #region Dynamic Expression Compilation

        private static readonly ConcurrentDictionary<Expression, object> Cache
            = new ConcurrentDictionary<Expression, object>();

        public static Func<TEntity, bool> AsFunc<TEntity>(this TEntity entity, Expression<Func<TEntity, bool>> expr)
            where TEntity : class, IHasId
        {
            //@see http://sergeyteplyakov.blogspot.ru/2015/06/lazy-trick-with-concurrentdictionary.html
            return ((Lazy<Func<TEntity, bool>>)Cache.GetOrAdd(expr, id => new Lazy<object>(
                    () => Cache.GetOrAdd(id, expr.Compile())))).Value;
        }

        public static bool Is<TEntity>(this TEntity entity, Expression<Func<TEntity, bool>> expr)
            where TEntity : class, IHasId
        {
            return AsFunc(entity, expr).Invoke(entity);
        }

        #endregion

        #region Composite Specifications

        public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
             where T : class, IHasId
        {
            return new AndSpecification<T>(left, right);
        }
        public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
             where T : class, IHasId
        {
            return new OrSpecification<T>(left, right);
        }
        public static ISpecification<T> Not<T>(this ISpecification<T> left)
             where T : class, IHasId
        {
            return new NotSpecification<T>(left);
        }

        #endregion

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, bool cnd, Expression<Func<T, bool>>  expr)
            => cnd
                ? queryable.Where(expr)
                : queryable;

        public static IQueryable<T> Match<T, TPattern>(this IQueryable<T> source, object pattern,
            Func<IQueryable<T>, TPattern, IQueryable<T>> evaluator) where TPattern : class
            => pattern is TPattern
                ? evaluator.Invoke(source, (TPattern) pattern)
                : source;

        public static IQueryable<T> Match<T, TSpecifcation>(this IQueryable<T> source, object pattern)
            where T : class
            where TSpecifcation : ILinqSpecification<T>
            => pattern is TSpecifcation
                ? ((ILinqSpecification<T>)pattern).Apply(source)
                : source;

        public static TEntity ById<TEntity>(this ILinqProvider linqProvider, int id)
            where TEntity : class, IHasId<int>
            => linqProvider.GetQueryable<TEntity>().ById(id);

        public static TEntity ById<TEntity>(this IQueryable<TEntity> queryable, int id)
            where TEntity : class, IHasId<int>
            => queryable.SingleOrDefault(x => x.Id == id);
        public static TProjection ById<TEntity, TProjection>(this IQueryable<TEntity> queryable, int id, IProjector projector)
            where TEntity : class, IHasId<int>
            => projector
                .Project<TEntity, TProjection>(queryable.Where(x => x.Id == id))
                .SingleOrDefault();
    }
}
