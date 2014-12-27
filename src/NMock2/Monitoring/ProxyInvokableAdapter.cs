using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using NMock2.Attribute;
using System.Runtime.Remoting;

namespace NMock2.Monitoring
{
	public class ProxyInvokableAdapter : RealProxy
	{
		private readonly IInvokable invokable;
        private readonly Type facadeType;
        private readonly Type objectClass;
        private readonly object proxyObject;

        public ProxyInvokableAdapter(Type facadeType, Type objectClass, object[] constructorArguments, IInvokable invokable) :
            base(facadeType)
		{
			this.invokable = invokable;
            this.facadeType = facadeType;
            this.objectClass = objectClass;
            if (objectClass != null)
            {
                proxyObject = Activator.CreateInstance(objectClass, constructorArguments);
            }
		}
		
		public override IMessage Invoke(IMessage msg)
		{
			MethodCall call = new MethodCall(msg);
			ParameterInfo[] parameters = call.MethodBase.GetParameters();
			Invocation invocation = new Invocation(GetTransparentProxy(),
												   (MethodInfo)call.MethodBase,
											       call.Args);
            // Invoke the real implemented method
            if (ImplementedAttribute.IsDefinedOn(call.MethodBase) && objectClass != null)
            {
                if (VerificationRequiredAttribute.IsDefinedOn(call.MethodBase))
                {
                    try
                    {
                        invokable.Invoke(invocation);
                    }
                    catch
                    {

                    }
                }
                
                object returnValue = objectClass.InvokeMember(
                    call.MethodName, 
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, 
                    null, proxyObject, call.Args);
                
                return new ReturnMessage(returnValue, call.Args, call.Args.Length, call.LogicalCallContext, call);
            }
            else
            {
                invokable.Invoke(invocation);

                if (invocation.IsThrowing)
                {
                    //TODO: it is impossible to set output parameters and throw an exception,
                    //      even though this is allowed by .NET method call semantics.
                    return new ReturnMessage(invocation.Exception, call);
                }
                else
                {
                    object[] outArgs = CollectOutputArguments(invocation, call, parameters);
                    return new ReturnMessage(invocation.Result, outArgs, outArgs.Length,
                                              call.LogicalCallContext, call);
                }
            }
		}
		
		private static object[] CollectOutputArguments(Invocation invocation,
			                                           MethodCall call,
			                                           ParameterInfo[] parameters)
		{
			ArrayList outArgs = new ArrayList(call.ArgCount);
			
			for (int i = 0; i < call.ArgCount; i++)
			{
				if (!parameters[i].IsIn) outArgs.Add(invocation.Parameters[i]);
			}
			
			return outArgs.ToArray();
		}
	}
}
