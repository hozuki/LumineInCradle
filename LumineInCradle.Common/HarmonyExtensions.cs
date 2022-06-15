using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;

namespace LumineInCradle;

public static class HarmonyExtensions
{

	private sealed class PatchResultHandler
	{

		static PatchResultHandler()
		{
			// e.g.: DMD<nel.EpManager::applyEpDamage>
			DynamicMethodNameRegex = new Regex(@"^DMD\<[\w\-_.]+::([\w\-_.]+)\>$");
		}

		public PatchResultHandler([NotNull] Harmony harmony, [NotNull] Type type, [NotNull] ManualLogSource logger)
		{
			_harmony = harmony;
			_type = type;
			_logger = logger;
		}

		public bool HandlePatch()
		{
			var harmony = _harmony;
			var type = _type;
			var logger = _logger;

			var allStaticMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			var allMembersToPatch = allStaticMethods.Where(method => method.GetCustomAttribute<HarmonyPatch>() != null).ToArray();

			var processor = harmony.CreateClassProcessor(type, true);
			var patched = processor.Patch();

			if (allMembersToPatch.Length == patched.Count)
			{
				return true;
			}

			logger.LogWarning($"Detour not complete. {allMembersToPatch.Length} static methods are annotated with [{nameof(HarmonyPatch)}] but only {patched.Count} are successfully patched.");

			return HandleFailure(allMembersToPatch, patched);
		}

		private bool HandleFailure(MethodInfo[] allMembersToPatch, List<MethodInfo> patched)
		{
			var logger = _logger;

			GetFailedPatchings(allMembersToPatch, patched, out var recoverable, out var unrecoverable);

			if (unrecoverable.Length == 0)
			{
				if (recoverable.Length > 0)
				{
					logger.LogWarning("Some methods failed to patch but they have " + nameof(PatchingMayFailAttribute) + ":");

					foreach (var method in recoverable)
					{
						logger.LogWarning($"  * {method.Name}");
					}
				}

				return true;
			}

			// Cannot recover

			logger.LogWarning("Methods failed to patch but don't have " + nameof(PatchingMayFailAttribute) + ":");

			foreach (var method in recoverable)
			{
				logger.LogWarning($"  * recoverable: {method.Name}");
			}

			foreach (var method in unrecoverable)
			{
				logger.LogWarning($"  * unrecoverable: {method.Name}");
			}

			return false;
		}

		private static void GetFailedPatchings(MethodInfo[] allMembersToPatch, List<MethodInfo> patched, out MethodInfo[] recoverable, out MethodInfo[] unrecoverable)
		{
			var membersToPatchDict = new Dictionary<string, MethodInfo>();

			foreach (var method in allMembersToPatch)
			{
				var key = GetPatchMethodTargetName(method.Name);
				membersToPatchDict.Add(key, method);
			}

			var patchedNames = new HashSet<string>();

			foreach (var method in patched)
			{
				var key = GetDynamicMethodTargetName(method.Name);
				patchedNames.Add(key);
			}

			var recoverableList = new List<MethodInfo>();
			var unrecoverableList = new List<MethodInfo>();

			foreach (var kv in membersToPatchDict)
			{
				if (!patchedNames.Contains(kv.Key))
				{
					// Oops, it's not marked with PatchingMayFailAttribute
					if (kv.Value.GetCustomAttribute<PatchingMayFailAttribute>() == null)
					{
						unrecoverableList.Add(kv.Value);
					}
					else
					{
						recoverableList.Add(kv.Value);
					}
				}
			}

			recoverable = recoverableList.Count == 0 ? Array.Empty<MethodInfo>() : recoverableList.ToArray();
			unrecoverable = unrecoverableList.Count == 0 ? Array.Empty<MethodInfo>() : unrecoverableList.ToArray();
		}

		private static string GetPatchMethodTargetName(string symbolName)
		{
			// a_method_name_Prefix or a_method_name_Postfix, we retrieve the "a_method_name" part.

			if (symbolName.EndsWith("_Prefix"))
			{
				return symbolName.Substring(0, symbolName.Length - 7);
			}
			else if (symbolName.EndsWith("_Postfix"))
			{
				return symbolName.Substring(0, symbolName.Length - 8);
			}
			else
			{
				return symbolName;
			}
		}

		private static string GetDynamicMethodTargetName(string symbolName)
		{
			// DMD<nel.EpManager::applyEpDamage> => applyEpDamage
			// note: subject to changes

			var match = DynamicMethodNameRegex.Match(symbolName);

			if (!match.Success)
			{
				return symbolName;
			}

			return match.Groups[1].Value;
		}

		private static readonly Regex DynamicMethodNameRegex;

		private readonly Harmony _harmony;
		private readonly Type _type;
		private readonly ManualLogSource _logger;

	}

	public static bool TryPatchAll([NotNull] this Harmony harmony, [NotNull] Type type, [NotNull] ManualLogSource logger)
	{
		var handler = new PatchResultHandler(harmony, type, logger);
		return handler.HandlePatch();
	}

}
