using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using nel;

namespace LumineInCradle.Always0721;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class Patch
{

	private static Harmony _harmonyInstance;

	public static bool PatchMethods([NotNull] ManualLogSource logger)
	{
		if (_harmonyInstance != null)
		{
			return true;
		}

		_harmonyInstance = new Harmony(PluginInfo.PLUGIN_GUID);

		return _harmonyInstance.TryPatchAll(typeof(UiBenchMenu_Patch), logger);
	}

	private static class UiBenchMenu_Patch
	{

		[HarmonyPatch(typeof(UiBenchMenu), nameof(UiBenchMenu.initBenchMenu))]
		[HarmonyPostfix]
		public static void initBenchMenu_Postfix()
		{
			if (!Plugin.Instance.IsEnabled)
			{
				return;
			}

			ModifyCommands();

			// Reinitialize
			UiBenchMenu.newGame();
		}

		[HarmonyPatch(typeof(UiBenchMenu), "get_" + nameof(UiBenchMenu.alloc_masturb))]
		[HarmonyPrefix]
		public static bool get_alloc_masturb_Prefix(ref bool __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = true;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(UiBenchMenu), "set_" + nameof(UiBenchMenu.alloc_masturb))]
		[HarmonyPrefix]
		public static bool set_alloc_masturb_Prefix()
		{
			if (Plugin.Instance.IsEnabled)
			{
				var field = AccessTools.Field(typeof(UiBenchMenu), "several_flags");
				var value = (int)field.GetValue(null);
				value &= 0x10;
				field.SetValue(null, value);

				return false;
			}

			return true;
		}

		// Interesting:
		// https://www.strathweb.com/2018/10/no-internalvisibleto-no-problem-bypassing-c-visibility-rules-with-roslyn/
		private static void ModifyCommands()
		{
			// cure_ep (masturbate)
			UiBenchMenu.ACmd[3] = CreateNewBenchCmd("cure_ep", pr => !pr.isMasturbating(), false, false);
			// cure_egged (lay egg)
			UiBenchMenu.ACmd[4] = CreateNewBenchCmd("cure_egged", pr => true, false, false);
		}

		private static UiBenchMenu.BenchCmd CreateNewBenchCmd([NotNull] string key, [NotNull] Func<PR, bool> canUsePredicate, bool canSetAuto, bool onlyInSafeArea)
		{
			return new UiBenchMenu.BenchCmd(key, canUsePredicate, canSetAuto, onlyInSafeArea);
		}

	}

}
