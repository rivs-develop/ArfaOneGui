using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RIVS.ASAK.Core.Tools
{
    /// <summary>
    /// Класс для поддержки модульности системы
    /// </summary>
    public static class ModuleEnvironment
    {
        /// <summary>
        /// Зарегистрировать все обработчики из указанных библиотек
        /// </summary>
        /// <param name="assemblies"></param>
        public static void Register(List<Assembly> assemblies)
        {
            //var logger = LogManager.GetCurrentClassLogger();
            var types = assemblies.SelectMany(x => x.GetTypes()).ToList();
            var assembliesList = assemblies.Aggregate("",
                                                      (seed, ass) =>
                                                      seed + (seed == "" ? ass.GetName().Name : ", " + ass.GetName().Name));
            //logger.Info("ModuleEnvironment.Register assemblies: " + assembliesList);
            foreach (var type in types)
            {
                var publicStaticFields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                foreach (var field in publicStaticFields)
                {
                    if (field.FieldType.BaseType != typeof (MulticastDelegate))
                    {
                        continue;
                    }
                    var moduleExportAttribute = Attribute.GetCustomAttribute(field.FieldType,
                                                                             typeof (EnvironmentExportAttribute));
                    if (moduleExportAttribute == null)
                    {
                        continue;
                    }
                    var handler = (Delegate)field.GetValue(null);
                    Register(handler);
                    var methodName = string.IsNullOrEmpty(type.Namespace)
                        ? type.Name + "." + handler.Method.Name
                        : type.Namespace + "." + type.Name + "." + handler.Method.Name;
                    //logger.Info("ModuleEnvironment.Register registered delegate: " + handler + " method: " + methodName);
                }
            }
        }

        internal static void Register(Delegate handler)
        {
            if (handler == null)
            {
                throw new ArgumentException();
            }
            var registerMethodGeneric = typeof(ModuleEnvironment)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Single(x => x.Name == "RegisterGeneric" && x.IsGenericMethod);
            var registerMethodConcrete = registerMethodGeneric.MakeGenericMethod(handler.GetType());
            registerMethodConcrete.Invoke(null, new object[] { handler });
        }

        internal static void RegisterGeneric<T>(T handler)
        {
            var h = handler as Delegate;
            if (h == null)
            {
                throw new ArgumentException();
            }
            var existing = ModuleEnvironment<T>.Run as Delegate;
            var result = existing != null ? Delegate.Combine(existing, h) : h;
            var resObj = (object)result;
            ModuleEnvironment<T>.Run = (T)resObj;
        }
    }

    /// <summary>
    /// Класс для поддержки модульности системы
    /// </summary>
    /// <typeparam name="T">Тип вызываемого метода</typeparam>
    public static class ModuleEnvironment<T>
    {
        private static T _run;

        /// <summary>
        /// Выполнить указанный метод
        /// </summary>
        public static T Run
        {
            get
            {
                if (typeof (T).BaseType != typeof (MulticastDelegate))
                {
                    throw new ArgumentException();
                }
                if (_run == null)
                {
                    _run = CreateVoidDelegate();
                }
                return _run;
            }
            set
            {
                _run = value;
            }
        }

        /// <summary>
        /// Создать пустой делегат типа T
        /// </summary>
        /// <returns>Пустой делегат</returns>
        private static T CreateVoidDelegate()
        {
            var delegateType = typeof(T);
            var method = delegateType.GetMethod("Invoke");
            var parameters = new List<ParameterExpression>();
            foreach (var param in method.GetParameters())
            {
                parameters.Add(Expression.Parameter(param.ParameterType, param.Name));
            }
            var res = Expression.Lambda<T>(Expression.Empty(), parameters).Compile();
            return res;
        }
    }

    /// <summary>
    /// Говорит, что помеченный атрибутом делегат является экспортируемым для модулей и реализующие 
    /// его методы будут включены во взаимодействий с модулями системы через класс ModuleEnvironment
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate)]
    public class EnvironmentExportAttribute : Attribute
    {
    }
    
}
