// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlatPocoFactory.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Defines the FlatPocoFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Startitecture.Orm.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;

    using JetBrains.Annotations;

    using Startitecture.Core;
    using Startitecture.Orm.Model;
    using Startitecture.Resources;

    /// <summary>
    /// Creates flattened POCOs for POCO data requests.
    /// </summary>
    public class FlatPocoFactory
    {
        /// <summary>
        /// The converters.
        /// </summary>
        private static readonly List<Func<object, object>> Converters = new List<Func<object, object>>();

        /// <summary>
        /// The Converters field.
        /// </summary>
        private static readonly FieldInfo ConvertersField = typeof(FlatPocoFactory).GetField(
            "Converters", 
            BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic);

        /// <summary>
        /// The <see cref="IDataRecord.GetValue"/> method.
        /// </summary>
        private static readonly MethodInfo GetValueMethod = typeof(IDataRecord).GetMethod("GetValue", new[] { typeof(int) });

        /// <summary>
        /// The Invoke method for a method delegate.
        /// </summary>
        private static readonly MethodInfo InvokeMethod = typeof(Func<object, object>).GetMethod("Invoke");

        /// <summary>
        /// The <see cref="IDataRecord.IsDBNull"/> method.
        /// </summary>
        private static readonly MethodInfo IsDbNullMethod = typeof(IDataRecord).GetMethod("IsDBNull");

        /// <summary>
        /// The entity Get method for lists.
        /// </summary>
        private static readonly MethodInfo ListItemMethod = typeof(List<Func<object, object>>).GetProperty("Item")?.GetGetMethod();

        /// <summary>
        /// Gets a <see cref="FlatPocoFactory"/> that populates all returnable attributes of a POCO.
        /// </summary>
        public static FlatPocoFactory ReturnableFactory { get; } = new FlatPocoFactory();

        /// <summary>
        /// Creates a delegate that returns a POCO from the provided data reader.
        /// </summary>
        /// <param name="dataRequest">
        /// The data request to create a POCO for.
        /// </param>
        /// <param name="type">
        /// The type of the POCO to create.
        /// </param>
        /// <param name="attributeDefinitions">
        /// The attribute definitions.
        /// </param>
        /// <returns>
        /// A <see cref="Delegate"/> that creates a POCO from the reader.
        /// </returns>
        public static PocoDelegateInfo CreateDelegate(
            [NotNull] PocoDataRequest dataRequest,
            [NotNull] Type type,
            [NotNull] IEnumerable<EntityAttributeDefinition> attributeDefinitions)
        {
            if (dataRequest == null)
            {
                throw new ArgumentNullException(nameof(dataRequest));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (attributeDefinitions == null)
            {
                throw new ArgumentNullException(nameof(attributeDefinitions));
            }

            var definitions = attributeDefinitions.ToList();

            // Create the method
            var name = $"poco_factory_{Guid.NewGuid()}";
            var method = new DynamicMethod(name, type, new[] { typeof(IDataReader) }, true);
            var generator = method.GetILGenerator();
            var mapper = Mappers.GetMapper(type);
            var reader = dataRequest.DataReader;
            
            if (type == typeof(object))
            {
                // var poco=new T()
                var constructorInfo = typeof(ExpandoObject).GetConstructor(Type.EmptyTypes);

                if (constructorInfo == null)
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.CurrentCulture, ErrorMessages.ConstructorInfoDoesNotExist, typeof(ExpandoObject)));
                }

                generator.Emit(OpCodes.Newobj, constructorInfo); // obj
                var addMethod = typeof(IDictionary<string, object>).GetMethod("Add");

                // Enumerate all fields generating a set assignment for the column
                for (var i = dataRequest.FirstColumn; i < dataRequest.FirstColumn + dataRequest.FieldCount; i++)
                {
                    var sourceType = reader.GetFieldType(i);

                    #if NET472
                    var fieldName = reader.GetName(i).Replace(".", string.Empty); // Remove period from dot-qualified names.
                    #else
                    var fieldName = reader.GetName(i).Replace(".", string.Empty, true, CultureInfo.InvariantCulture); // Remove period from dot-qualified names.
                    #endif

                    generator.Emit(OpCodes.Dup); // obj, obj
                    generator.Emit(OpCodes.Ldstr, fieldName); // obj, obj, fieldname

                    // Get the converter
                    var converter = mapper.GetFromDbConverter(null, sourceType);

                    // Setup stack for call to converter
                    AddConverterToStack(generator, converter);

                    // reader[i]
                    generator.Emit(OpCodes.Ldarg_0); // obj, obj, fieldname, converter?,    rdr
                    generator.Emit(OpCodes.Ldc_I4, i); // obj, obj, fieldname, converter?,  rdr,i
                    generator.Emit(OpCodes.Callvirt, GetValueMethod); // obj, obj, fieldname, converter?,  value

                    // Convert DBNull to null
                    generator.Emit(OpCodes.Dup); // obj, obj, fieldname, converter?,  value, value
                    generator.Emit(OpCodes.Isinst, typeof(DBNull)); // obj, obj, fieldname, converter?,  value, (value or null)
                    var lblNotNull = generator.DefineLabel();
                    generator.Emit(OpCodes.Brfalse_S, lblNotNull); // obj, obj, fieldname, converter?,  value
                    generator.Emit(OpCodes.Pop); // obj, obj, fieldname, converter?

                    if (converter != null)
                    {
                        generator.Emit(OpCodes.Pop); // obj, obj, fieldname, 
                    }

                    generator.Emit(OpCodes.Ldnull); // obj, obj, fieldname, null

                    if (converter != null)
                    {
                        var lblReady = generator.DefineLabel();
                        generator.Emit(OpCodes.Br_S, lblReady);
                        generator.MarkLabel(lblNotNull);
                        generator.Emit(OpCodes.Callvirt, InvokeMethod);
                        generator.MarkLabel(lblReady);
                    }
                    else
                    {
                        generator.MarkLabel(lblNotNull);
                    }

                    generator.Emit(OpCodes.Callvirt, addMethod);
                }

                generator.Emit(OpCodes.Ret);
            }
            else if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
            {
                // Do we need to install a converter?
                var sourceType = reader.GetFieldType(0);
                var converter = GetConverter(mapper, null, sourceType, type);

                // "if (!rdr.IsDBNull(i))"
                generator.Emit(OpCodes.Ldarg_0); // rdr
                generator.Emit(OpCodes.Ldc_I4_0); // rdr,0
                generator.Emit(OpCodes.Callvirt, IsDbNullMethod); // bool
                var continueLabel = generator.DefineLabel();
                generator.Emit(OpCodes.Brfalse_S, continueLabel);
                generator.Emit(OpCodes.Ldnull); // null
                var finishLabel = generator.DefineLabel();
                generator.Emit(OpCodes.Br_S, finishLabel);

                generator.MarkLabel(continueLabel);

                // Setup stack for call to converter
                AddConverterToStack(generator, converter);

                generator.Emit(OpCodes.Ldarg_0); // rdr
                generator.Emit(OpCodes.Ldc_I4_0); // rdr,0
                generator.Emit(OpCodes.Callvirt, GetValueMethod); // value

                // Call the converter
                if (converter != null)
                {
                    generator.Emit(OpCodes.Callvirt, InvokeMethod);
                }

                generator.MarkLabel(finishLabel);
                generator.Emit(OpCodes.Unbox_Any, type); // value converted
                generator.Emit(OpCodes.Ret);
            }
            else
            {
                // Set this when a column has a value.
                var hasValue = generator.DeclareLocal(typeof(bool));
                generator.Emit(OpCodes.Ldc_I4, 0);
                generator.Emit(OpCodes.Stloc, hasValue);

                // var poco=new T()
                const BindingFlags InstanceConstructors = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                generator.Emit(OpCodes.Newobj, type.GetConstructor(InstanceConstructors, null, Array.Empty<Type>(), null));

                // Enumerate all fields generating a set assignment for the column
                for (var i = dataRequest.FirstColumn; i < dataRequest.FirstColumn + dataRequest.FieldCount; i++)
                {
                    // Get the PocoColumn for this db column, ignore if not known
                    var key = reader.GetName(i);

                    // We may need to set multiple attributes based on a single reader column when there are multiple paths in a POCO to 
                    // the same object reference.
                    var attribute = definitions.FirstOrDefault(x => x.ReferenceName == key);

                    if (attribute == EntityAttributeDefinition.Empty)
                    {
                        continue;
                    }

                    // Get the source type for this column
                    var sourceType = reader.GetFieldType(i);
                    var destinationType = attribute.PropertyInfo.PropertyType;

                    // "if (!rdr.IsDBNull(i))"
                    generator.Emit(OpCodes.Ldarg_0); // poco,rdr
                    generator.Emit(OpCodes.Ldc_I4, i); // poco,rdr,i
                    generator.Emit(OpCodes.Callvirt, IsDbNullMethod); // poco,bool
                    var nextLabel = generator.DefineLabel();
                    generator.Emit(OpCodes.Brtrue_S, nextLabel); // poco

                    // There's a value here so we set our local variable as such.
                    var setLabel = generator.DefineLabel();
                    generator.Emit(OpCodes.Ldloc, hasValue);
                    generator.Emit(OpCodes.Ldc_I4, 1);
                    generator.Emit(OpCodes.Beq, setLabel);
                    generator.Emit(OpCodes.Ldc_I4, 1);
                    generator.Emit(OpCodes.Stloc, hasValue);
                    generator.MarkLabel(setLabel);

                    // Back to our poco.
                    generator.Emit(OpCodes.Dup); // poco,poco

                    // Do we need to install a converter?
                    var converter = GetConverter(mapper, attribute.PropertyInfo, sourceType, destinationType);

                    // Fast
                    var handled = false;

                    if (converter == null)
                    {
                        var valueGetter = typeof(IDataRecord).GetMethod(string.Concat("Get", sourceType.Name), new[] { typeof(int) });

                        var nullableType = Nullable.GetUnderlyingType(destinationType);

                        var valueGetterMatchesType = valueGetter != null && valueGetter.ReturnType == sourceType
                                                     && (valueGetter.ReturnType == destinationType || valueGetter.ReturnType == nullableType);

                        if (valueGetterMatchesType)
                        {
                            generator.Emit(OpCodes.Ldarg_0); // *,rdr
                            generator.Emit(OpCodes.Ldc_I4, i); // *,rdr,i
                            generator.Emit(OpCodes.Callvirt, valueGetter); // *,value

                            // Convert to Nullable
                            if (nullableType != null)
                            {
                                var constructorInfo = destinationType.GetConstructor(new[] { nullableType });

                                if (constructorInfo == null)
                                {
                                    throw new InvalidOperationException(
                                        string.Format(CultureInfo.CurrentCulture, ErrorMessages.ConstructorInfoDoesNotExist, nullableType));
                                }

                                generator.Emit(OpCodes.Newobj, constructorInfo);
                            }

                            generator.Emit(OpCodes.Callvirt, attribute.PropertyInfo.GetSetMethod(true)); // poco
                            handled = true;
                        }
                    }

                    // Not so fast
                    if (handled == false)
                    {
                        // Setup stack for call to converter
                        AddConverterToStack(generator, converter);

                        // "value = rdr.GetValue(i)"
                        generator.Emit(OpCodes.Ldarg_0); // *,rdr
                        generator.Emit(OpCodes.Ldc_I4, i); // *,rdr,i
                        generator.Emit(OpCodes.Callvirt, GetValueMethod); // *,value

                        // Call the converter
                        if (converter != null)
                        {
                            generator.Emit(OpCodes.Callvirt, InvokeMethod);
                        }

                        // Assign it
                        generator.Emit(OpCodes.Unbox_Any, attribute.PropertyInfo.PropertyType); // poco,poco,value
                        generator.Emit(OpCodes.Callvirt, attribute.SetValueMethod); // poco
                    }

                    ////columnsMapped--;
                    generator.MarkLabel(nextLabel);
                }

                var onLoadedMethod = RecurseInheritedTypes(type, x => x.GetMethod("OnLoaded", InstanceConstructors, null, Array.Empty<Type>(), null));

                if (onLoadedMethod != null)
                {
                    generator.Emit(OpCodes.Dup);
                    generator.Emit(OpCodes.Callvirt, onLoadedMethod);
                }

                // Check whether anything was set.
                var isSetLabel = generator.DefineLabel();
                generator.Emit(OpCodes.Ldloc, hasValue);
                generator.Emit(OpCodes.Ldc_I4, 1);
                generator.Emit(OpCodes.Beq, isSetLabel);

                // Remove the poco and load a null onto the stack.
                generator.Emit(OpCodes.Pop);
                generator.Emit(OpCodes.Ldnull);
                generator.Emit(OpCodes.Ret);

                // Jump here if something was set.
                generator.MarkLabel(isSetLabel);
                generator.Emit(OpCodes.Ret);
            }

            // Cache it, return it
            var mappingDelegate = method.CreateDelegate(Expression.GetFuncType(typeof(IDataReader), type));
            return new PocoDelegateInfo(mappingDelegate);
        }

        /// <summary>
        /// Creates a delegate that returns a POCO from the provided data reader.
        /// </summary>
        /// <typeparam name="T">
        /// The type of POCO expected by the return statement.
        /// </typeparam>
        /// <param name="dataRequest">
        /// The data request to create a POCO for.
        /// </param>
        /// <returns>
        /// A <see cref="Delegate"/> that creates a POCO from the reader.
        /// </returns>
        public PocoDelegateInfo CreateDelegate<T>([NotNull] PocoDataRequest dataRequest)
        {
            if (dataRequest == null)
            {
                throw new ArgumentNullException(nameof(dataRequest));
            }

            return this.CreateDelegate(dataRequest, typeof(T));
        }

        /// <summary>
        /// Creates a delegate that returns a POCO from the provided data reader.
        /// </summary>
        /// <param name="dataRequest">
        /// The data request to create a POCO for.
        /// </param>
        /// <param name="type">
        /// The type of the POCO to create.
        /// </param>
        /// <returns>
        /// A <see cref="Delegate"/> that creates a POCO from the reader.
        /// </returns>
        public PocoDelegateInfo CreateDelegate([NotNull] PocoDataRequest dataRequest, [NotNull] Type type)
        {
            if (dataRequest == null)
            {
                throw new ArgumentNullException(nameof(dataRequest));
            }

            return CreateDelegate(dataRequest, type, dataRequest.AttributeDefinitions);
        }

        /// <summary>
        /// Adds a converter to the stack.
        /// </summary>
        /// <param name="generator">
        /// The intermediate language generator.
        /// </param>
        /// <param name="converter">
        /// The converter to add.
        /// </param>
        private static void AddConverterToStack(ILGenerator generator, Func<object, object> converter)
        {
            if (converter == null)
            {
                return;
            }

            // Add the converter
            var converterIndex = Converters.Count;
            Converters.Add(converter);

            // Generate IL to push the converter onto the stack
            generator.Emit(OpCodes.Ldsfld, ConvertersField);
            generator.Emit(OpCodes.Ldc_I4, converterIndex);
            generator.Emit(OpCodes.Callvirt, ListItemMethod); // Converter
        }

        /// <summary>
        /// Gets a converter for the specified column.
        /// </summary>
        /// <param name="mapper">
        /// The data mapper to apply.
        /// </param>
        /// <param name="column">
        /// The column to convert.
        /// </param>
        /// <param name="sourceType">
        /// The source type.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// A function that converts the source type to the destination type.
        /// </returns>
        private static Func<object, object> GetConverter(IMapper mapper, PropertyInfo column, Type sourceType, Type destinationType)
        {
            // Get converter from the mapper
            if (column != null)
            {
                var converter = mapper.GetFromDbConverter(column, sourceType);

                if (converter != null)
                {
                    return converter;
                }
            }

            // Forced type conversion including integral types -> enum
            if (destinationType.IsEnum && IsIntegralType(sourceType))
            {
                if (sourceType != typeof(int))
                {
                    return value => Convert.ChangeType(value, typeof(int), null);
                }
            }
            else if (!destinationType.IsAssignableFrom(sourceType))
            {
                if (destinationType.IsEnum && sourceType == typeof(string))
                {
                    return stringValue => EnumMapper.EnumFromString(destinationType, (string)stringValue);
                }

                return value => Convert.ChangeType(value, destinationType, null);
            }

            return null;
        }

        /// <summary>
        /// Determines whether the type is an integral type.
        /// </summary>
        /// <param name="type">
        /// The type to evaluate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the type is an integral type; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsIntegralType(Type type)
        {
            var tc = Type.GetTypeCode(type);
            return tc >= TypeCode.SByte && tc <= TypeCode.UInt64;
        }

        /// <summary>
        /// Recursively evaluates the inherited types of the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to evaluate.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <typeparam name="T">
        /// The expected return type.
        /// </typeparam>
        /// <returns>
        /// An instance of type <typeparamref name="T"/>.
        /// </returns>
        private static T RecurseInheritedTypes<T>(Type type, Func<Type, T> callback)
        {
            while (type != null)
            {
                var info = callback(type);

                if (Evaluate.IsSet(info))
                {
                    return info;
                }

                type = type.BaseType;
            }

            return default(T);
        }
    }
}
