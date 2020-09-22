using System;
using System.Collections.Generic;
using NHibernate.Expression;

namespace BusiBlocks
{
    /// <summary>
    /// A filter to specify a list of values using a JunctionOperator (AND, OR) and a ValueOperator (Equal, NotEqual, ...).
    /// This class can generate a NHibernate ICriterion to be used inside queries.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Filter<T>
    {
        private readonly JunctionOperator _junctionOperator;

        private readonly ValueOperator _valueOperator;

        private readonly List<T> _values = new List<T>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pJunctionOperator"></param>
        /// <param name="pValueOperator"></param>
        /// <param name="pValues"></param>
        public Filter(JunctionOperator pJunctionOperator,
                      ValueOperator pValueOperator,
                      params T[] pValues)
        {
            _junctionOperator = pJunctionOperator;
            _valueOperator = pValueOperator;

            if (pValues != null)
            {
                Values.AddRange(pValues);
            }
        }

        public JunctionOperator JunctionOperator
        {
            get { return _junctionOperator; }
        }

        public ValueOperator ValueOperator
        {
            get { return _valueOperator; }
        }

        public List<T> Values
        {
            get { return _values; }
        }

        /// <summary>
        /// Convert the filter to a ICriterion that can be used with NHibernate
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual ICriterion ToCriterion(string propertyName)
        {
            Junction junction;
            if (JunctionOperator == JunctionOperator.And)
                junction = new Conjunction();
            else if (JunctionOperator == JunctionOperator.Or)
                junction = new Disjunction();
            else
                throw new ArgumentException("Invalid value", "JunctionOperator");

            //This code is used when the values are empty because otherwise 
            // some version of NHibernate returns false if the Conjunction or Disjunction is empty
            if (Values.Count == 0)
                junction.Add(Expression.Or(Expression.IsNull(propertyName), Expression.IsNotNull(propertyName)));

            foreach (object val in Values)
            {
                if (ValueOperator == ValueOperator.Equal)
                    junction.Add(Expression.Eq(propertyName, val));
                else if (ValueOperator == ValueOperator.NotEqual)
                    junction.Add(Expression.Not(Expression.Eq(propertyName, val)));
                else if (ValueOperator == ValueOperator.Contains)
                {
                    if (val is string == false)
                        throw new ArgumentException("For Contains operator the value must be a string");
                    junction.Add(Expression.Like(propertyName, (string) val, MatchMode.Anywhere));
                }
                else if (ValueOperator == ValueOperator.StartWith)
                {
                    if (val is string == false)
                        throw new ArgumentException("For StartWith operator the value must be a string");
                    junction.Add(Expression.Like(propertyName, (string) val, MatchMode.Start));
                }
                else if (ValueOperator == ValueOperator.EndWith)
                {
                    if (val is string == false)
                        throw new ArgumentException("For EndWith operator the value must be a string");
                    junction.Add(Expression.Like(propertyName, (string) val, MatchMode.End));
                }
                else
                    throw new ArgumentException("Invalid value", "ValueOperator");
            }

            return junction;
        }
    }

    public static class Filter
    {
        #region Factory methods

        /// <summary>
        /// Create a filter for string values using an AND junction and a LIKE operator.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Filter<string> ContainsAll(params string[] values)
        {
            return new Filter<string>(JunctionOperator.And, ValueOperator.Contains, values);
        }

        /// <summary>
        /// Create a filter for string values using an OR junction and a LIKE operator.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Filter<string> ContainsOne(params string[] values)
        {
            return new Filter<string>(JunctionOperator.Or, ValueOperator.Contains, values);
        }

        /// <summary>
        /// Create a filter using an OR junction and an EQUAL operator
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Filter<T> MatchOne<T>(params T[] values)
        {
            return new Filter<T>(JunctionOperator.Or, ValueOperator.Equal, values);
        }

        #endregion
    }
}