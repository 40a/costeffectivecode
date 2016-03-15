﻿using System;
using System.Linq;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Akka.TestKit.Xunit2;
using CostEffectiveCode.Akka.Actors;
using CostEffectiveCode.Akka.Messages;
using CostEffectiveCode.Common.Scope;
using CostEffectiveCode.Domain;
using CostEffectiveCode.Domain.Cqrs.Queries;
using CostEffectiveCode.Domain.Ddd.Specifications;
using CostEffectiveCode.Sample.Domain.Entities;
using Xunit;

namespace CostEffectiveCode.Akka.Tests.Tests
{
    public class QueryActorTests : TestKit
    {
        private readonly ContainerConfig _containerConfig;
        private const int MaxEntities = 6; // magic number assumed to present in db at least

        public QueryActorTests()
            : base(@"akka.loglevel = DEBUG")
        {
            _containerConfig = new ContainerConfig();
            _containerConfig.Configure();

            // ReSharper disable once ObjectCreationAsStatement
            new AutoFacDependencyResolver(_containerConfig.AutofacContainer, Sys);
        }

        [Fact]
        public void RequestedAll_ResponsedAllEntities()
        {
            GeneralCase(new FetchRequestMessageBase(), x => x.Entities.Count() >= MaxEntities);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(1)]
        [InlineData(0)]
        public void RequestedLimitedNumber_ToldLimitedNumber(int limit)
        {
            GeneralCase(new FetchRequestMessageBase(limit), x => x.Entities.Count() == limit);
        }

        [Fact]
        public void RequestedSingle_ResponsedFailure()
        {
            // arrange
            var queryActor = GetEmptyQueryActor();

            // act
            queryActor.Tell(new FetchRequestMessageBase(true, false));

            // assert
            var failureMessage = ExpectMsg<Failure>(new TimeSpan(0, 0, 10));
            Assert.NotNull(failureMessage.Exception);
            Assert.NotNull(failureMessage.Timestamp);
        }

        [Fact]
        public void RequestedFiltered_ResponsedFiltered()
        {
            GeneralCase(
                new FetchRequestMessage<Product>()
                    .Where(new ExpressionSpecification<Product>(x => x.Price >= 1000))
                    , x => x.Entities.Count() < MaxEntities && x.Entities.All(y => y.Price >= 1000));
        }

        [Fact]
        public void WithBaseQuery_RequestedAll_ResponsedAll()
        {
            // arrange
            var queryActor = GetBaseQueryActor();

            // act
            queryActor.Tell(new FetchRequestMessageBase());

            // assert
            GeneralAssert(x => x.Entities.Count() >= MaxEntities);
        }

        [Fact]
        public void WithBaseQuery_RequestedFiltered_ResponsedFiltered()
        {
            var queryActor = GetBaseQueryActor();

            queryActor.Tell(
                new FetchRequestMessage<Product>()
                    .Where(new ExpressionSpecification<Product>(x => x.Price >= 1000)));

            GeneralAssert(x => x.Entities.Count() < MaxEntities && x.Entities.All(y => y.Price >= 1000));
        }

        [Theory]
        [InlineData(3)]
        [InlineData(1)]
        [InlineData(0)]
        public void WithBaseQuery_RequestedLimited_ResponsedLimitedNumber(int limit)
        {
            // arrange
            var queryActor = Sys.ActorOf(Props.Create(() =>
                new QueryActor<Product>(
                    new DelegateScope<IQuery<Product, IExpressionSpecification<Product>>>(GetBaseQuery)
                )));

            // act
            queryActor.Tell(new FetchRequestMessageBase(3));

            // assert
            GeneralAssert(x => x.Entities.Count() == 3);
        }

        private void GeneralCase(FetchRequestMessageBase requestMessage, Func<FetchResponseMessage<Product>, bool> assertFunc)
        {
            // arrange
            var queryActor = GetEmptyQueryActor();

            // act
            queryActor.Tell(requestMessage);

            // assert
            GeneralAssert(assertFunc);
        }

        private IActorRef GetEmptyQueryActor()
        {
            var queryActorProps = Sys.DI().Props<QueryActor<Product>>();

            return Sys.ActorOf(queryActorProps, "testedQueryActor");
        }

        private IActorRef GetBaseQueryActor()
        {
            var queryActor = Sys.ActorOf(Props.Create(() =>
                new QueryActor<Product>(
                    new DelegateScope<IQuery<Product, IExpressionSpecification<Product>>>(GetBaseQuery)
                    )), "testedQueryActor");
            return queryActor;
        }

        private IQuery<Product, IExpressionSpecification<Product>> GetBaseQuery() =>
            _containerConfig
                .Container
                .Resolve<IQuery<Product, IExpressionSpecification<Product>>>()
                .Where(x => x.Active);

        //private static void GeneralAct(FetchRequestMessageBase request, IActorRef queryActor)
        //{
        //    queryActor.Tell(request);
        //}

        private void GeneralAssert(Func<FetchResponseMessage<Product>, bool> assertFunc)
        {
            var responseMessage = ExpectMsg<FetchResponseMessage<Product>>(new TimeSpan(0, 0, 10));

            Assert.NotNull(responseMessage.Entities);
            Assert.True(assertFunc(responseMessage));
        }
    }
}
