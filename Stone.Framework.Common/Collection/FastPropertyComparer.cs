using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Stone.Framework.Common.Collection
{
    /// <summary>
    /// 快速的比较属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FastPropertyComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, object> _getPropertyValueFunc = null;

        public FastPropertyComparer(string propertyName)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName,
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);

            if (propertyInfo == null) { throw new ArgumentException(string.Format("{0} is not a property of type {1}.", propertyName, typeof(T))); }

            var expPara = Expression.Parameter(typeof(T), "obj");
            var me = Expression.Property(expPara, propertyInfo);
            _getPropertyValueFunc = Expression.Lambda<Func<T, object>>(me, expPara).Compile();
        }

        public bool Equals(T x, T y)
        {
            var xValue = _getPropertyValueFunc(x);
            var yValue = _getPropertyValueFunc(y);

            if (xValue == null) { return yValue == null; }

            return xValue.Equals(yValue);
        }

        public int GetHashCode(T obj)
        {
            var propertyValue = _getPropertyValueFunc(obj);
            return propertyValue == null ? 0 : propertyValue.GetHashCode();
        }
    }
}