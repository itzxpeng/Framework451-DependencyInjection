using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Net451.Microsoft.Extensions.Internal
{
    internal static class ActivatorUtilities
    {
        private class ConstructorMatcher
        {
            private readonly ConstructorInfo _constructor;

            private readonly ParameterInfo[] _parameters;

            private readonly object[] _parameterValues;

            private readonly bool[] _parameterValuesSet;

            public ConstructorMatcher(ConstructorInfo constructor)
            {
                this._constructor = constructor;
                this._parameters = this._constructor.GetParameters();
                this._parameterValuesSet = new bool[this._parameters.Length];
                this._parameterValues = new object[this._parameters.Length];
            }

            public int Match(object[] givenParameters)
            {
                int num = 0;
                int result = 0;
                for (int i = 0; i != givenParameters.Length; i++)
                {
                    object obj = givenParameters[i];
                    TypeInfo typeInfo = (obj != null) ? obj.GetType().GetTypeInfo() : null;
                    bool flag = false;
                    int num2 = num;
                    while (!flag && num2 != this._parameters.Length)
                    {
                        if (!this._parameterValuesSet[num2] && this._parameters[num2].ParameterType.GetTypeInfo().IsAssignableFrom(typeInfo))
                        {
                            flag = true;
                            this._parameterValuesSet[num2] = true;
                            this._parameterValues[num2] = givenParameters[i];
                            if (num == num2)
                            {
                                num++;
                                if (num2 == i)
                                {
                                    result = num2;
                                }
                            }
                        }
                        num2++;
                    }
                    if (!flag)
                    {
                        return -1;
                    }
                }
                return result;
            }

            public object CreateInstance(IServiceProvider provider)
            {
                for (int i = 0; i != this._parameters.Length; i++)
                {
                    if (!this._parameterValuesSet[i])
                    {
                        object service = provider.GetService(this._parameters[i].ParameterType);
                        if (service == null)
                        {
                            object obj = default(object);
                            if (!ParameterDefaultValue.TryGetDefaultValue(this._parameters[i], out obj))
                            {
                                throw new InvalidOperationException(string.Format("Unable to resolve service for type '{0}' while attempting to activate '{1}'.", this._parameters[i].ParameterType, this._constructor.DeclaringType));
                            }
                            this._parameterValues[i] = obj;
                        }
                        else
                        {
                            this._parameterValues[i] = service;
                        }
                    }
                }
                try
                {
                    return this._constructor.Invoke(this._parameterValues);
                }
                catch (TargetInvocationException ex)
                {
                    ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                    throw;
                }
            }
        }

        private static readonly MethodInfo GetServiceInfo = ActivatorUtilities.GetMethodInfo((Expression<Func<IServiceProvider, Type, Type, bool, object>>)((IServiceProvider sp, Type t, Type r, bool c) => ActivatorUtilities.GetService(sp, t, r, c)));

        internal static object CreateInstance(IServiceProvider provider, Type instanceType, params object[] parameters)
        {
            int num = -1;
            ConstructorMatcher constructorMatcher = null;
            if (!instanceType.GetTypeInfo().IsAbstract)
            {
                foreach (ConstructorMatcher item in from constructor in instanceType.GetTypeInfo().DeclaredConstructors.Where(delegate (ConstructorInfo c)
                {
                    if (!c.IsStatic)
                    {
                        return c.IsPublic;
                    }
                    return false;
                })
                                                    select new ConstructorMatcher(constructor))
                {
                    int num2 = item.Match(parameters);
                    if (num2 != -1 && num < num2)
                    {
                        num = num2;
                        constructorMatcher = item;
                    }
                }
            }
            if (constructorMatcher == null)
            {
                throw new InvalidOperationException(string.Format("A suitable constructor for type '{0}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.", instanceType));
            }
            return constructorMatcher.CreateInstance(provider);
        }

        internal static ObjectFactory CreateFactory(Type instanceType, Type[] argumentTypes)
        {
            ConstructorInfo constructor = default(ConstructorInfo);
            int?[] parameterMap = default(int?[]);
            ActivatorUtilities.FindApplicableConstructor(instanceType, argumentTypes, out constructor, out parameterMap);
            ParameterExpression parameterExpression = Expression.Parameter(typeof(IServiceProvider), "provider");
            ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object[]), "argumentArray");
            return Expression.Lambda<Func<IServiceProvider, object[], object>>(ActivatorUtilities.BuildFactoryExpression(constructor, parameterMap, parameterExpression, parameterExpression2), new ParameterExpression[2]
            {
            parameterExpression,
            parameterExpression2
            }).Compile().Invoke;
        }

        public static T CreateInstance<T>(IServiceProvider provider, params object[] parameters)
        {
            return (T)ActivatorUtilities.CreateInstance(provider, typeof(T), parameters);
        }

        public static T GetServiceOrCreateInstance<T>(IServiceProvider provider)
        {
            return (T)ActivatorUtilities.GetServiceOrCreateInstance(provider, typeof(T));
        }

        public static object GetServiceOrCreateInstance(IServiceProvider provider, Type type)
        {
            return provider.GetService(type) ?? ActivatorUtilities.CreateInstance(provider, type, new List<object>());
        }

        private static MethodInfo GetMethodInfo<T>(Expression<T> expr)
        {
            return ((MethodCallExpression)expr.Body).Method;
        }

        private static object GetService(IServiceProvider sp, Type type, Type requiredBy, bool isDefaultParameterRequired)
        {
            object service = sp.GetService(type);
            if (service == null && !isDefaultParameterRequired)
            {
                throw new InvalidOperationException(string.Format("Unable to resolve service for type '{0}' while attempting to activate '{1}'.", type, requiredBy));
            }
            return service;
        }

        private static Expression BuildFactoryExpression(ConstructorInfo constructor, int?[] parameterMap, Expression serviceProvider, Expression factoryArgumentArray)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            Expression[] array = new Expression[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo obj = parameters[i];
                Type parameterType = obj.ParameterType;
                object value = default(object);
                bool flag = ParameterDefaultValue.TryGetDefaultValue(obj, out value);
                if (parameterMap[i].HasValue)
                {
                    array[i] = Expression.ArrayAccess(factoryArgumentArray, Expression.Constant(parameterMap[i]));
                }
                else
                {
                    Expression[] arguments = new Expression[4]
                    {
                    serviceProvider,
                    Expression.Constant(parameterType, typeof(Type)),
                    Expression.Constant(constructor.DeclaringType, typeof(Type)),
                    Expression.Constant(flag)
                    };
                    array[i] = Expression.Call(ActivatorUtilities.GetServiceInfo, arguments);
                }
                if (flag)
                {
                    ConstantExpression right = Expression.Constant(value);
                    array[i] = Expression.Coalesce(array[i], right);
                }
                array[i] = Expression.Convert(array[i], parameterType);
            }
            return Expression.New(constructor, array);
        }

        private static void FindApplicableConstructor(Type instanceType, Type[] argumentTypes, out ConstructorInfo matchingConstructor, out int?[] parameterMap)
        {
            matchingConstructor = null;
            parameterMap = null;
            foreach (ConstructorInfo declaredConstructor in instanceType.GetTypeInfo().DeclaredConstructors)
            {
                int?[] array = default(int?[]);
                if (!declaredConstructor.IsStatic && declaredConstructor.IsPublic && ActivatorUtilities.TryCreateParameterMap(declaredConstructor.GetParameters(), argumentTypes, out array))
                {
                    if (matchingConstructor != (ConstructorInfo)null)
                    {
                        throw new InvalidOperationException(string.Format("Multiple constructors accepting all given argument types have been found in type '{0}'. There should only be one applicable constructor.", instanceType));
                    }
                    matchingConstructor = declaredConstructor;
                    parameterMap = array;
                }
            }
            if (!(matchingConstructor == (ConstructorInfo)null))
            {
                return;
            }
            throw new InvalidOperationException(string.Format("A suitable constructor for type '{0}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.", instanceType));
        }

        private static bool TryCreateParameterMap(ParameterInfo[] constructorParameters, Type[] argumentTypes, out int?[] parameterMap)
        {
            parameterMap = new int?[constructorParameters.Length];
            for (int i = 0; i < argumentTypes.Length; i++)
            {
                bool flag = false;
                TypeInfo typeInfo = argumentTypes[i].GetTypeInfo();
                int num = 0;
                while (num < constructorParameters.Length)
                {
                    if (parameterMap[num].HasValue || !constructorParameters[num].ParameterType.GetTypeInfo().IsAssignableFrom(typeInfo))
                    {
                        num++;
                        continue;
                    }
                    flag = true;
                    parameterMap[num] = i;
                    break;
                }
                if (!flag)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
