using System;
using System.Linq.Expressions;

namespace RabbitMQ.Client.Core.DependencyInjection.Specifications
{
    internal abstract class Specification<T>
    {
        protected abstract Expression<Func<T , bool>> ToExpression();

        internal bool IsSatisfiedBy(T entity)
        {
            Func<T , bool> predicate = ToExpression().Compile();
            return predicate(entity);
        }
    }
}