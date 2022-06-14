using System;
using JetBrains.Annotations;

namespace LumineInCradle;

public sealed class CachedClassObject<T>
	where T : class
{

	[NotNull]
	public T Value
	{
		get
		{
			if (_value == null)
			{
				throw new NullReferenceException();
			}

			return _value;
		}
		set => _value = value ?? throw new ArgumentNullException(nameof(value));
	}

	public bool HasValue => _value != null;

	public T GetValueOrSetDefault([CanBeNull] T defaultValue)
	{
		if (HasValue)
		{
			return Value;
		}

		if (defaultValue == null)
		{
			return null;
		}

		Value = defaultValue;

		return Value;
	}

	public T GetValueOrSetBy<TParam>(TParam arg, [NotNull] Func<TParam, T> getter)
	{
		if (HasValue)
		{
			return Value;
		}

		var value = getter(arg);

		if (value == null)
		{
			return null;
		}

		Value = value;

		return Value;
	}

	private T _value;

}
