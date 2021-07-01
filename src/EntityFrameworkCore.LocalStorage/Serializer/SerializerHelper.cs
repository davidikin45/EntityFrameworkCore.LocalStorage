using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace EntityFrameworkCore.LocalStorage.Serializer
{
	static class SerializerHelper
	{
		private static readonly Dictionary<Type, object> _commonTypeDictionary = new Dictionary<Type, object>
		{
#pragma warning disable IDE0034 // Simplify 'default' expression - default causes default(object)
			{ typeof(int), default(int) },
			{ typeof(Guid), default(Guid) },
			{ typeof(DateTime), default(DateTime) },
			{ typeof(DateTimeOffset), default(DateTimeOffset) },
			{ typeof(long), default(long) },
			{ typeof(bool), default(bool) },
			{ typeof(double), default(double) },
			{ typeof(short), default(short) },
			{ typeof(float), default(float) },
			{ typeof(byte), default(byte) },
			{ typeof(char), default(char) },
			{ typeof(uint), default(uint) },
			{ typeof(ushort), default(ushort) },
			{ typeof(ulong), default(ulong) },
			{ typeof(sbyte), default(sbyte) }
#pragma warning restore IDE0034 // Simplify 'default' expression
		};

		private static object GetDefaultValue(Type type)
		{
			if (!type.GetTypeInfo().IsValueType)
			{
				return null;
			}

			// A bit of perf code to avoid calling Activator.CreateInstance for common types and
			// to avoid boxing on every call. This is about 50% faster than just calling CreateInstance
			// for all value types.
			return _commonTypeDictionary.TryGetValue(type, out var value)
				? value
				: Activator.CreateInstance(type);
		}
		public static object Deserialize(this string input, Type type)
		{
			type = Nullable.GetUnderlyingType(type) ?? type;

			if (string.IsNullOrEmpty(input))
			{
				return GetDefaultValue(type);
			}

			if (type == typeof(DateTimeOffset))
			{
				return DateTimeOffset.Parse(input, CultureInfo.InvariantCulture);
			}

			if (type == typeof(TimeSpan))
			{
				return TimeSpan.Parse(input, CultureInfo.InvariantCulture);
			}
			
			if (type == typeof(Guid))
			{
				return Guid.Parse(input);
			}
			
			if (type.IsArray)
			{
				Type arrType = type.GetElementType();
				List<object> arr = new List<object>();

				foreach (string s in input.Split(','))
				{
					arr.Add(s.Deserialize(arrType));
				}

				return arr.ToArray();
			}

			if(type.IsEnum)
			{
				return Enum.Parse(type, input, true);
			}
			

			return Convert.ChangeType(input, type, CultureInfo.InvariantCulture);
		}

		public static string Serialize(this object input)
		{
			if (input != null)
			{
				if (input.GetType().IsArray)
				{
					string result = "";

					object[] arr = (object[])input;

					for (int i = 0; i < arr.Length; i++)
					{
						result += arr[i].Serialize();

						if (i + 1 < arr.Length)
						{
							result += ",";
						}
					}

					return result;
				}

				return input is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture) : input.ToString();
			}

			return "";
		}

		public static TKey GetKey<TKey, T>(IPrincipalKeyValueFactory<T> keyValueFactory, IEntityType entityType, Func<string, string> valueSelector)
		{
			return (TKey)keyValueFactory.CreateFromKeyValues(
				entityType.FindPrimaryKey().Properties
					.Select(p =>
						valueSelector(p.GetColumnName())
							.Deserialize(p.GetValueConverter()?.ProviderClrType ?? p.ClrType)).ToArray());
		}
	}
}
