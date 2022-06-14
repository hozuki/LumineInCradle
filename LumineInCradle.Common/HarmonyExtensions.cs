using System;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;

namespace LumineInCradle;

public static class HarmonyExtensions
{

	public static bool TryPatchAll(this Harmony harmony, [NotNull] Type type, [NotNull] ManualLogSource logger)
	{
		var staticMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		var desiredPatchMembers = staticMethods.Where(method =>
		{
			var attr = method.GetCustomAttribute<HarmonyPatch>();
			return attr != null;
		}).ToArray();

		var processor = harmony.CreateClassProcessor(type, true);
		var patched = processor.Patch();

		if (desiredPatchMembers.Length == patched.Count)
		{
			return true;
		}
		else
		{
			logger.LogWarning($"Detour not complete. {desiredPatchMembers.Length} static methods are annotated with [{nameof(HarmonyPatch)}] but only {patched.Count} are successfully patched.");
			return false;
		}
	}

}
