using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using nel;

namespace LumineInCradle.EggHarvest;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public static class Patch
{

	private static Harmony _harmonyInstance;

	public static void PatchMethods()
	{
		if (_harmonyInstance != null)
		{
			return;
		}

		_harmonyInstance = new Harmony(LumineInCradle.PluginInfo.PLUGIN_GUID);

		_harmonyInstance.PatchAll(typeof(SCN_Patch));
	}

	private static class SCN_Patch
	{

		[HarmonyPatch(typeof(SCN), nameof(SCN.isBenchCmdEnable))]
		[HarmonyPrefix]
		public static bool isBenchCmdEnable_Prefix(ref bool __result, string /* key */ __0)
		{
			if (Plugin.IsEnabled)
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
