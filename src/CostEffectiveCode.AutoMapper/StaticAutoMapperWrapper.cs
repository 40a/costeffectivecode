﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using AutoMapper.QueryableExtensions;
using CostEffectiveCode.Common;
using AM = AutoMapper;
using IMapper = CostEffectiveCode.Common.IMapper;

namespace CostEffectiveCode.AutoMapper
{
    public class StaticAutoMapperWrapper : IMapper, IProjector
    {
        public TReturn Map<TReturn>(object src) => AM.Mapper.Map<TReturn>(src);

        public TReturn Map<TReturn>(object src, TReturn dest) => AM.Mapper.Map(src, dest);

        public IQueryable<TReturn> Project<TSource, TReturn>(IQueryable<TSource> queryable)
            => queryable.ProjectTo<TReturn>();

        internal static ConcurrentDictionary<Type, ConcurrentDictionary<Type, Action<AM.IMapperConfigurationExpression>>> TypeMap
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Action<AM.IMapperConfigurationExpression>>>();

        public static void Init()
        {
            AM.Mapper.Initialize(InitMappings);
            AM.Mapper.AssertConfigurationIsValid();
        }


        private static void InitMappings(AM.IMapperConfigurationExpression c)
        {
            foreach (var sourceType in TypeMap)
            {
                foreach (var destType in sourceType.Value)
                {
                    destType.Value.Invoke(c);
                }                  
            }

            c.CreateMissingTypeMaps = true;
        }
    }
}
