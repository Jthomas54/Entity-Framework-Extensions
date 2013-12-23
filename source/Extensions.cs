using System;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFrameworkExtensions
{
    public static class Extensions
    {
        /// <summary>
        /// Returns the first element in a sequence that matches the given criteria.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="IQueryable{T}" /> to return the first element of.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="fallback">A fallback function to get a default record if the first query didn't return any records.</param>
        /// <returns><see cref="TSource"/></returns>
        /// <remarks>
        ///     If the first query doesn't find any matches, the second query will be executed to get the default.
        ///     This method joins the two predicates together so the all results are returned in a single statement.
        /// </remarks>
        public static TSource FirstOrDefault<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, Expression<Func<TSource, bool>> fallback)
        {
            if (source == null) throw new ArgumentNullException("source");

            var query = predicate.Or(fallback);
            
            var materializedSource = source.Where(query).ToList();
            var result = materializedSource.FirstOrDefault(predicate.Compile());


            if (result != null) return result;

            return materializedSource.FirstOrDefault(fallback.Compile());
        }
    }
}