using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NMock2.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ImplementedAttribute : System.Attribute
    {
        public override string ToString() { return "Implemented method"; }

        public static bool IsDefinedOn(MemberInfo member)
        {
            foreach (object attribute in member.GetCustomAttributes(false))
            {
                if (attribute is ImplementedAttribute)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
