using System;
using System.Collections.Generic;
using System.Text;
using NMock2.Internal;
using System.Collections;
using System.Reflection;
using NMock2.Matchers;
using System.Runtime.Remoting.Messaging;
using NMock2.Actions;

namespace NMock2
{
    public class ContractVerify
    {
        public static void Check(object realObj, object mock)
        {
            IMockObject mockObj = (IMockObject) mock;
            ArrayList stubs = mockObj.GetStubExpectation();
            Type realType = realObj.GetType();
            foreach (BuildableExpectation expectation in stubs)
            {
                ArgumentsMatcher argumentsMatcher = (ArgumentsMatcher)expectation.ArgumentsMatcher;
                MethodNameMatcher methodNameMatcher = (MethodNameMatcher)expectation.MethodMatcher;
                string methodName = methodNameMatcher.GetMethodName();
                ArrayList actions = expectation.Actions;
                if (actions[0] is ReturnAction)
                {
                    ReturnAction action = (ReturnAction)actions[0];
                    object expectedReturn = action.GetResult();
                    object[] args = argumentsMatcher.GetExpectedArguments();
                    object realReturn = realType.InvokeMember(methodName, BindingFlags.InvokeMethod, null, realObj, args);
                    if (expectedReturn.Equals(realReturn))
                    {
                        Console.WriteLine(methodName + " called with " + printArgs(args) + ": passed");
                    }
                    else
                    {
                        Console.WriteLine(methodName + " called with " + printArgs(args) + ": failed");
                    }
                }
            }
        }

        public static string printArgs(object[] args)
        {
            string result = "(";
            for (int i = 0; i < args.Length; i++)
            {
                Type realType = args[i].GetType();
                MethodInfo toString = realType.GetMethod("ToString", new Type[0]);
                result += toString.Invoke(args[i], null);
                if (i == args.Length - 1) { break; }
                result += ",";
            }
            result += ")";
            return result;
        }
    }
}
