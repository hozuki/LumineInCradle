using System.Diagnostics.CodeAnalysis;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using nel;

namespace LumineInCradle.EggHarvest;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public static class Patch
{

	private static Harmony _harmonyInstance;

	public static bool PatchMethods([NotNull] ManualLogSource logger)
	{
		if (_harmonyInstance != null)
		{
			return true;
		}

		_harmonyInstance = new Harmony(LumineInCradle.PluginInfo.PLUGIN_GUID);

		return _harmonyInstance.TryPatchAll(typeof(SCN_Patch), logger);
	}

	private static class SCN_Patch
	{

		[HarmonyPatch(typeof(SCN), nameof(SCN.isBenchCmdEnable))]
		[HarmonyPrefix]
		public static bool isBenchCmdEnable_Prefix(ref bool __result, string /* key */ __0)
		{
			if (Plugin.Instance.IsEnabled)
			{
				switch (__0)
				{
					case "cure_ep_sensitivity": // totally not implemented at the time of writing
					case "cure_egged": // not working at the time of writing
						__result = true;
						return false;
				}
			}

			return true;
		}

	}

}
