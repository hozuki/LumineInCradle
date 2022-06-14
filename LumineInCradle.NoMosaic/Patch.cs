using System.Diagnostics.CodeAnalysis;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using nel;
using XX;

namespace LumineInCradle.NoMosaic;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedParameter.Local")]
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

		var successful = true;

		successful = successful && _harmonyInstance.TryPatchAll(typeof(UIPictureBodyData_Patch), logger);
		successful = successful && _harmonyInstance.TryPatchAll(typeof(UIPictureBodySpine_Patch), logger);
		successful = successful && _harmonyInstance.TryPatchAll(typeof(MosaicShower_Patch), logger);

		return successful;
	}

	// See: https://github.com/BepInEx/HarmonyX/wiki/Patch-parameters

	private static class Implementations
	{

		public static bool getUseMosaic(IMosaicDescriptor descriptor)
		{
			return false;
		}

		public static int getSensitiveHiddenCount(IMosaicDescriptor descriptor)
		{
			return 0;
		}

		public static bool getSensitiveOrMosaicRect(IMosaicDescriptor descriptor, DRect bufferRect, int id)
		{
			return false;
		}

	}

	private static class UIPictureBodyData_Patch
	{

		[HarmonyPatch(typeof(UIPictureBodyData), nameof(IMosaicDescriptor.getUseMosaic))]
		[HarmonyPrefix]
		public static bool getUseMosaic_Prefix(UIPictureBodyData __instance, ref bool __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = Implementations.getUseMosaic(__instance);
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(UIPictureBodyData), nameof(IMosaicDescriptor.getSensitiveHiddenCount))]
		[HarmonyPrefix]
		public static bool getSensitiveHiddenCount_Prefix(UIPictureBodyData __instance, ref int __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = Implementations.getSensitiveHiddenCount(__instance);
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(UIPictureBodyData), nameof(IMosaicDescriptor.getSensitiveOrMosaicRect))]
		[HarmonyPrefix]
		public static bool getSensitiveOrMosaicRect_Prefix(UIPictureBodyData __instance, DRect /* BufRc */ __0, int /* id */ __1, ref bool __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = Implementations.getSensitiveOrMosaicRect(__instance, __0, __1);
				return false;
			}

			return true;
		}

	}

	private static class UIPictureBodySpine_Patch
	{

		[HarmonyPatch(typeof(UIPictureBodySpine), nameof(IMosaicDescriptor.getUseMosaic))]
		[HarmonyPrefix]
		public static bool getUseMosaic_Prefix(UIPictureBodySpine __instance, ref bool __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = Implementations.getUseMosaic(__instance);
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(UIPictureBodySpine), nameof(IMosaicDescriptor.getSensitiveHiddenCount))]
		[HarmonyPrefix]
		public static bool getSensitiveHiddenCount_Prefix(UIPictureBodySpine __instance, ref int __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = Implementations.getSensitiveHiddenCount(__instance);
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(UIPictureBodySpine), nameof(IMosaicDescriptor.getSensitiveOrMosaicRect))]
		[HarmonyPrefix]
		public static bool getSensitiveOrMosaicRect_Prefix(UIPictureBodySpine __instance, DRect /* BufRc */ __0, int /* id */ __1, ref bool __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = Implementations.getSensitiveOrMosaicRect(__instance, __0, __1);
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(UIPictureBodySpine), nameof(UIPictureBodySpine.getSensitiveVisibility))]
		[HarmonyPrefix]
		public static bool getSensitiveVisibility_Prefix(UIPictureBodySpine __instance, ref bool __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = false;
				return false;
			}

			return true;
		}

	}

	// Flags above don't seem to work, still have to use the approach below
	private static class MosaicShower_Patch
	{

		[HarmonyPatch(typeof(MosaicShower), nameof(MosaicShower.awakeMosaicShower))]
		[HarmonyPrefix]
		public static bool awakeMosaicShower_Prefix(MosaicShower __instance, int /* _render_layer */ __0, int /* _target_layer */ __1)
		{
			if (Plugin.Instance.IsEnabled)
			{
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(MosaicShower), nameof(MosaicShower.OnDestroy))]
		[HarmonyPrefix]
		public static bool OnDestroy_Prefix(MosaicShower __instance)
		{
			if (Plugin.Instance.IsEnabled)
			{
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(MosaicShower), nameof(MosaicShower.setTarget))]
		[HarmonyPrefix]
		public static bool setTarget_Prefix(MosaicShower __instance, IMosaicDescriptor /* _Targ */ __0, bool /* force */ __1)
		{
			if (Plugin.Instance.IsEnabled)
			{
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(MosaicShower), nameof(MosaicShower.runPost))]
		[HarmonyPrefix]
		public static bool runPost_Prefix(MosaicShower __instance)
		{
			if (Plugin.Instance.IsEnabled)
			{
				return false;
			}

			return true;
		}

	}

}
