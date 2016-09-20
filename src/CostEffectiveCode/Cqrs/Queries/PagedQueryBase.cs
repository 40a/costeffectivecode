﻿using System.Linq;
using CostEffectiveCode.Common;
using CostEffectiveCode.Ddd.Entities;
using CostEffectiveCode.Ddd.Specifications;
using CostEffectiveCode.Ddd.Specifications.UnitOfWork;

namespace CostEffectiveCode.Cqrs.Queries
{
    public class PagedQueryBase<TSpec, TEntity, TDto> : PagedEntityToDtoQuery<TSpec, TEntity, TDto>
        where TEntity : class, IEntity<int>
        where TDto : class, IEntity<int>
        where TSpec : IApplyable<TDto>, IPagedSpecification<TDto>
    {
        public PagedQueryBase(ILinqProvider linqProvier, IProjector projector)
            : base(linqProvier, projector)
        {
        }

        protected override IQueryable<TDto> GetQueryable(TSpec spec)
            => spec.Apply(Project(LinqProvider.GetQueryable<TEntity>())).OrderByDescending(x => x.Id);
    }
}