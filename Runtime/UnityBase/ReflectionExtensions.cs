using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ALib;

namespace UnityUtilities.UnityBase
{
    public static class ReflectionExtensions
    {
        public static (PropertyInfo, object) GetDeepPropertyInfo(this object instance, string path)
        {
            var pp = path.Split('.');
            Type t = instance.GetType();
            foreach (var prop in pp.SkipLast(1))
            {
                PropertyInfo propInfo = t.GetProperty(prop);
                if (propInfo != null)
                {
                    instance = propInfo.GetValue(instance, null);
                    t = propInfo.PropertyType;
                }
                else return (null,null);
            }
            return (t.GetProperty(pp.LastOrDefault() ?? ""), instance);
        }
        public static (EventInfo, object) GetDeepEventInfo(this object instance, string path)
        {
            var pp = path.Split('.');
            Type t = instance.GetType();
            foreach (var prop in pp.SkipLast(1))
            {
                PropertyInfo propInfo = t.GetProperty(prop);
                if (propInfo != null)
                {
                    instance = propInfo.GetValue(instance, null);
                    t = propInfo.PropertyType;
                }
                else return (null,null);
            }
            return (t.GetEvent(pp.LastOrDefault() ?? ""), instance);
        }
    }
}
