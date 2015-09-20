﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CostEffectiveCode.Domain;
using CostEffectiveCode.Domain.Ddd;
using JetBrains.Annotations;

namespace CostEffectiveCode.Tests.Entities
{
    public class Product : NamedEntity
    {
        [BusinessRule]
        public static readonly Expression<Func<Product, bool>> ActiveRule =
            p => p.Active && p.Price > 0;
     
        private Category _category;
     
        [Required, NotNull]
        public virtual Category Category
        {
            get { return _category; }
            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException();
                }

                _category = value;
            }
        }

        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }

        public bool Active { get; set; }

        public bool IsActive()
        {
            return this.Is(ActiveRule);
        }

        internal Product()
        {
            
        }

        public Product(string name, [NotNull] Category category, decimal price, bool active = true)
            :base(name)
        {
            if (category == null) throw new ArgumentNullException("category");
            Price = price;
            Active = active;
            _category = category;
            this.ValidateSelf();
        }
    }
}
