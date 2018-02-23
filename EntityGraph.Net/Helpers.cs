using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenSoftware.EntityGraph.Net
{
    internal static class Helpers
    {
        /// <summary>
        /// Returns an array of PropertyInfo objects for properties which have the "DataMemberAttribute"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static IEnumerable<PropertyInfo> GetDataMembers(object obj)
        {
            const BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in obj.GetType().GetProperties(bindingAttr)
                where
                    !IsAssociation(p.PropertyType) && p.CanWrite
                select p;
            return qry;
        }

        internal static bool IsAssociation(Type type)
        {
            while (true)
            {
                if (type == typeof(string)) return false;

                if (type.IsArray)
                {
                    type = type.GetElementType();
                    continue;
                }

                if (type.IsGenericType && type.GetInterface(nameof(IEnumerable)) != null)
                {
                    var elementType = type.GetGenericArguments()[0];
                    type = elementType;
                    continue;
                }

                return type.IsClass || type.IsInterface;
            }
        }

        public static bool MemberwiseCompare(object o1, object o2)
        {
            var type1 = o1.GetType();
            var type2 = o2.GetType();
            if (type1 != type2) return false;

            foreach (var prop in GetDataMembers(o1))
            {
                var v1 = prop.GetValue(o1);
                var v2 = prop.GetValue(o2);
                if (v1 == null)
                {
                    if (v2 != null) return false;
                }
                else
                {
                    if (v1.Equals(v2) == false) return false;
                }
            }

            return true;
        }
    }
}