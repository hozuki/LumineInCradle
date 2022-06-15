using System;
using JetBrains.Annotations;

namespace LumineInCradle;

public sealed class PatchingMayFailAttribute : Attribute
{

	public static class CommonReasons
	{

		public const string MethodTooSmall = "Method too small";

	}

	public PatchingMayFailAttribute()
		: this("unknown")
	{
	}

	public PatchingMayFailAttribute([NotNull] string reason)
	{
		Reason = reason;
	}

	[NotNull]
	public string Reason { get; }

}
