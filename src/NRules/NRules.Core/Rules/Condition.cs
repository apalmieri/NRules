﻿using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;

namespace NRules.Core.Rules
{
    internal class Condition : ICondition, IEquatable<Condition>
    {
        private readonly Delegate _compiledExpression;
        private readonly List<Type> _factTypes = new List<Type>();

        public Condition(string key, Delegate compiledExpression, IEnumerable<Type> inputTypes)
        {
            Key = key;
            _compiledExpression = compiledExpression;
            _factTypes.AddRange(inputTypes);
        }

        public string Key { get; private set; }

        public IEnumerable<Type> FactTypes
        {
            get { return _factTypes; }
        }

        public bool IsSatisfiedBy(params Fact[] facts)
        {
            try
            {
                var objects = facts.Select(f => f.Object).ToArray();
                var result = (bool) _compiledExpression.DynamicInvoke(objects);
                return result;
            }
            catch (InvalidCastException e)
            {
                throw new InvalidOperationException("Fact type does not match condition type", e);
            }
        }

        public bool Equals(Condition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Key, Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Condition)) return false;
            return Equals((Condition) obj);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}