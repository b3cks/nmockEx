using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NMock2.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class VerificationRequiredAttribute : System.Attribute
    {
        public override string ToString() { return "Verification required method"; }

        public static bool IsDefinedOn(MemberInfo member)
        {
            foreach (object attribute in member.GetCustomAttributes(false))
            {
                if (attribute is VerificationRequiredAttribute)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
