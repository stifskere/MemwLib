using System.Collections;
using System.Reflection;

namespace MemwLib.CoreUtils;

internal delegate string? PropertyConditionPredicate(PropertyInfo property, object? typeCondition);

internal delegate object? TypeConditionPredicate(Type type);

internal static class TypeUtils
{
    public static object? InferTypeOf(string obj)
    {
        if (obj.Equals("null", StringComparison.OrdinalIgnoreCase))
            return null;
        
        if (int.TryParse(obj, out int intObj))
            return intObj;
        
        if (double.TryParse(obj, out double doubleObj))
            return doubleObj;

        if (bool.TryParse(obj, out bool boolObj))
            return boolObj;

        return obj;
    }

    // Примечание: Словарь будет содержать другой Dictionary<string, object?> для не примитивных типов/списков.
    public static object FillType(Type targetType, Dictionary<string, object?> values, 
        TypeConditionPredicate typePredicate, PropertyConditionPredicate propertyPredicate)
    {
        object instance = Activator.CreateInstance(targetType)!;

        foreach (PropertyInfo property in targetType.GetProperties())
        {
            string? propertyName = propertyPredicate(property, typePredicate(targetType));

            if (propertyName is null || !values.TryGetValue(propertyName, out object? value))
                continue;

            if (value is null)
            {
                if (Nullable.GetUnderlyingType(property.PropertyType) is not null)
                    throw new NullReferenceException("Can not assign null to a non nullable property.");
                
                property.SetValue(instance, null);
                continue;
            }
            
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                if (property.PropertyType.IsGenericType && property.PropertyType.IsAssignableTo(typeof(IEnumerable)) 
                    && !property.PropertyType.IsAssignableTo(typeof(IDictionary)) || property.PropertyType.IsArray)
                {
                    Type[] genericArgs = property.PropertyType.GetGenericArguments();
                    Type elementType = genericArgs.Length > 0 ? genericArgs[0] : property.PropertyType.GetElementType()!;
                    IList listInstance = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;

                    foreach (object listValue in (IEnumerable)value)
                    {
                        listInstance.Add(listValue is Dictionary<string, object?> subDict 
                            ? FillType(elementType, subDict, typePredicate, propertyPredicate)
                            : Convert.ChangeType(listValue, elementType)
                        );
                    }
                    
                    if (property.PropertyType.IsArray)
                    {
                        Array arrayInstance = Array.CreateInstance(elementType, listInstance.Count);
                        listInstance.CopyTo(arrayInstance, 0);
                        property.SetValue(instance, arrayInstance);
                        continue;
                    }
                    
                    property.SetValue(instance, listInstance);
                    continue;
                }
                
                property.SetValue(instance, FillType(property.PropertyType, (Dictionary<string, object?>)value, typePredicate, propertyPredicate));
                continue;
            }
            
            property.SetValue(instance, Convert.ChangeType(value, property.PropertyType));
        }

        return instance;
    }
}