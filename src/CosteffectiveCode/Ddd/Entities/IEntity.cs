﻿using JetBrains.Annotations;

namespace CostEffectiveCode.Ddd.Entities
{
    [PublicAPI]
    public interface IEntity
    {
        /// <summary>
        /// Basic read-only method to get the string value of primary key.
        /// Examples: "0", "5", "aaa", "5,6" (composite key), "aaa,bbb,7" (tripple composite key)
        /// 
        /// Hint: Please avoid using for calculations!
        /// </summary>
        /// <returns>Representation of entity's primary key</returns>
        object Id { get; }
    }

    public interface IEntity<out T> : IEntity
    {
        new T Id { get; }
    }

}