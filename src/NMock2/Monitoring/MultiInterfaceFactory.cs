using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;


namespace NMock2.Monitoring
{
	public class MultiInterfaceFactory
	{
		private ModuleBuilder moduleBuilder;
		private static Hashtable createdTypes = new Hashtable();
        
		public MultiInterfaceFactory(string name)
		{
        	AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = name;
			
			AssemblyBuilder assemblyBuilder =
				AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			moduleBuilder = assemblyBuilder.DefineDynamicModule(name);
		}
		
		public Type GetType(Type baseInterfaces, Type mockedType)
		{
			
            TypeId id = Id(baseInterfaces);
			if (createdTypes.ContainsKey(id))
			{
				return (Type) createdTypes[id];
			}
			else
			{
				string typeName = "MultiInterface" + (createdTypes.Count+1);
				Type newType = CreateType(typeName, baseInterfaces, mockedType);
				createdTypes[id] = newType;
				return newType;
			}
		}

        private Type CreateType(string typeName, Type baseInterfaces, Type mockedType)
		{
            TypeBuilder typeBuilder = null;
            if (!mockedType.IsClass)
            {
                Type[] interfaces = new Type[] { baseInterfaces, mockedType };
                typeBuilder = moduleBuilder.DefineType(
                    typeName,
                    TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Interface,
                    null,
                    interfaces);
            }
            else
            {
                Type[] interfaces = new Type[] { baseInterfaces };
                typeBuilder = moduleBuilder.DefineType(
                    typeName,
                    TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Interface,
                    null,
                    interfaces);
            }
			return typeBuilder.CreateType();
		}

		private TypeId Id(params Type[] types)
		{
			return new TypeId(types);
		}

		private class TypeId
		{
			Type[] types;
			
			public TypeId(params Type[] types)
			{
				this.types = types;
			}

			public override int GetHashCode()
			{
				int hashCode = 0;
				foreach (Type type in types)
				{
					hashCode ^= type.GetHashCode();
				}
				return hashCode;
			}
			
			public override bool Equals(object obj)
			{
				return obj is TypeId
					&& ContainsSameTypesAs( (TypeId)obj );
			}

			private bool ContainsSameTypesAs(TypeId other)
			{
				if (other.types.Length != types.Length) return false;

				for (int i = 0; i < types.Length; i++)
				{
					if (Array.IndexOf(other.types, types[i]) < 0) return false;
				}

				return true;
			}
		}
	}
}
