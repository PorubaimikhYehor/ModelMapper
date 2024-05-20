namespace ModelMappingClass;
using System;

public class ModelMapper
{
	public TOut Map<TOut>(object source) 
	{
		return (TOut)MapItem(source, typeof(TOut));
	}

	private object MapItem(object source, Type destinationType)
	{
        if (source == null)
        {
            return null;   
        }
		if (IsList(source.GetType()) && IsList(destinationType))
		{
			return MapList(source, destinationType);
		}
		else if (IsDictionary(source.GetType()) && IsDictionary(destinationType))
		{	
			return MapDictionary(source, destinationType);
		}
		else if (source.GetType().IsEnum || destinationType.IsEnum)
		{
			return MapEnum(source, destinationType);	
		}
		else if (!IsPrimitive(destinationType) && !IsUnsupportedType(destinationType))
		{
			return MapObject(source, destinationType);
		}
		else if (source.GetType() == destinationType)
		{
			return source;
		}
		else
		{
			if (destinationType == typeof(string))
			{
				return string.Empty;
			}
			else
			{
				return Activator.CreateInstance(destinationType);
			}
		}
    }
	private System.Collections.IList MapList(object source, Type destinationType)
	{
		var sourceList = source as System.Collections.IList;
		var destinationList = Activator.CreateInstance(destinationType) as System.Collections.IList;
		Type destinationListType = destinationType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));
		Type destinationListItemType = destinationListType != null ? destinationListType.GetGenericArguments()[0] : null;
		if(destinationListItemType != null && sourceList != null)
		{
			foreach (var item in sourceList)
			{
				destinationList.Add(MapItem(item, destinationListItemType));
			}
		}		
		return destinationList;		
	}

	private System.Collections.IDictionary MapDictionary(object source, Type destinationType)
	{	
		var sourceDict = source as System.Collections.IDictionary;
		var destinationDict = Activator.CreateInstance(destinationType) as System.Collections.IDictionary;
		if (sourceDict != null)
		{
			var destKeyType = destinationType.GetGenericArguments()[0];
			var destValueType = destinationType.GetGenericArguments()[1];
			bool canUsePrimitiveKey = !IsPrimitive(destKeyType) || sourceDict.GetType().GetGenericArguments()[0] == destKeyType;
			bool canUsePrimitiveValue = !IsPrimitive(destValueType) || sourceDict.GetType().GetGenericArguments()[1] == destValueType;
			if (canUsePrimitiveKey && canUsePrimitiveValue)
			{
				foreach (System.Collections.DictionaryEntry entry in sourceDict)
				{	
					destinationDict.Add(MapItem(entry.Key, destKeyType), MapItem(entry.Value, destValueType));
				}
			}
		}
		return destinationDict;
	}

	private object MapEnum(object source, Type destinationType)
	{
		// Console.WriteLine(Enum.GetValuesAsUnderlyingType(destinationType).Cast<object>().FirstOrDefault());
		string enumString = source.ToString();
        if (destinationType.IsEnum)
        {
            if (Enum.IsDefined(destinationType, enumString))
            {
                return Enum.Parse(destinationType, enumString);
            } 
            else
            {
                return Enum.GetValues(destinationType).Cast<object>().FirstOrDefault();
            }
        }
        else if (typeof(string) == destinationType)
        {
            return enumString;
        }
        else
        {
            return Activator.CreateInstance(destinationType);
        }
	}

	private object MapObject(object source, Type destinationType)
	{
		var destObject = Activator.CreateInstance(destinationType);
		if (source != null)
		{
			foreach (var sourceProperty in source.GetType().GetProperties())
			{
				var destinationProperty = destinationType.GetProperty(sourceProperty.Name);
				if (destinationProperty != null && destinationProperty.CanWrite)
				{
					var nestedSourceValue = sourceProperty.GetValue(source);
					if (nestedSourceValue != null)
					{
						var nestedDestinationValue = MapItem(nestedSourceValue, destinationProperty.PropertyType);
						destinationProperty.SetValue(destObject, nestedDestinationValue);
					}
				}
			}
		}
		return destObject;
	}

	private bool IsList(Type type)
	{
		return type.IsGenericType && typeof(System.Collections.IList).IsAssignableFrom(type);
	}

	private bool IsPrimitive(Type type)
	{
		return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);
	}

	private bool IsUnsupportedType(Type type)
	{
		return type.IsInterface || type.IsAbstract || type.IsPointer || type.IsArray;
	}

	private bool IsDictionary(Type type)
	{
		return type.IsGenericType && typeof(System.Collections.IDictionary).IsAssignableFrom(type);
	}
}