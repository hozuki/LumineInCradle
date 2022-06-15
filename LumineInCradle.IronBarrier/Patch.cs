using System.Diagnostics.CodeAnalysis;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using m2d;
using nel;
using PixelLiner.PixelLinerLib;
using X = XX.X;

namespace LumineInCradle.IronBarrier;

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

		_harmonyInstance = new Harmony(PluginInfo.PLUGIN_GUID);

		return _harmonyInstance.TryPatchAll(typeof(EpManager_Patch), logger);
	}

	// Also see EpSuppressor
	private static class EpManager_Patch
	{

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.applyEpDamage))]
		[HarmonyPrefix]
		public static bool applyEpDamage_Prefix(EpManager __instance, ref bool __result, EpAtk /* Atk */ __0, M2Attackable /* AttackedBy */ __1, EPCATEG_BITS /* bits */ __2)
		{
			if (Plugin.Instance.IsEnabled)
			{
				if (__instance.Pr.isMasturbating())
				{
					// Masturbating, we allow that.
					return true;
				}

				// BLOCK ALL H DAMAGE FROM ENEMIES!!!!!!!!!

				__result = false;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.addMasturbateCountImmediate))]
		[HarmonyPostfix]
		public static void addMasturbateCountImmediate_Postfix(EpManager __instance)
		{
			if (!Plugin.Instance.IsEnabled)
			{
				return;
			}

			ClearCounters(__instance);
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.addEggLayCount))]
		[HarmonyPrefix]
		public static bool addEggLayCount_Prefix(EpManager __instance, PrEggManager.CATEG /* categ */ __0, int /* cnt */ __1)
		{
			if (Plugin.Instance.IsEnabled)
			{
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.addPeeCount))]
		[HarmonyPrefix]
		public static bool addPeeCount_Prefix(EpManager __instance)
		{
			if (Plugin.Instance.IsEnabled)
			{
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), "get_" + nameof(EpManager.masturbate_count))]
		[HarmonyPrefix]
		public static bool get_masturbate_count_Prefix(EpManager __instance, ref int __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = 0;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.getExperienceCount))]
		[HarmonyPrefix]
		public static bool getExperienceCount_Prefix(EpManager __instance, ref int __result, EPCATEG /* categ */ __0)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = 0;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.getExperienceLevel))]
		[HarmonyPrefix]
		public static bool getExperienceLevel_Prefix(EpManager __instance, ref float __result, EPCATEG /* categ */ __0)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = 0.0f;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.getOrgasmedIndividualTotal))]
		[HarmonyPrefix]
		public static bool getOrgasmedIndividualTotal_Prefix(EpManager __instance, ref int __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = 0;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.getOrgasmedTotal))]
		[HarmonyPrefix]
		public static bool getOrgasmedTotal_Prefix(EpManager __instance, ref int __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = 0;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.getLeadToOrgasmRatio))]
		[HarmonyPrefix]
		public static bool getLeadToOrgasmRatio_Prefix(EpManager __instance, ref float __result, EPCATEG /* target */ __0)
		{
			if (Plugin.Instance.IsEnabled)
			{
				__result = 0.0f;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.getOrgasmedTotal))]
		[HarmonyPrefix]
		[PatchingMayFail(PatchingMayFailAttribute.CommonReasons.MethodTooSmall)]
		public static bool recalcOrgasmable_Prefix(EpManager __instance)
		{
			if (!Plugin.Instance.IsEnabled)
			{
				return true;
			}

			X.ALL0(__instance.Ases_orgasmable);

			return false;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.quitOrasmSageTime))]
		[HarmonyPostfix]
		public static void quitOrasmSageTime_Postfix(EpManager __instance, bool /* cure_ser */ __0)
		{
			if (!Plugin.Instance.IsEnabled)
			{
				return;
			}

			// Clear sage time
			__instance.t_oazuke = 1000.0f;
			__instance.orgasm_oazuke = false;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.getNoelJuiceQualityAdd))]
		[HarmonyPrefix]
		public static bool getNoelJuiceQualityAdd_Prefix(EpManager __instance, ref int __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				// The Noel Juice (ew) quality modifier. This will be added to be grade base of item drop.
				// Item grade is in range [0, 4] (5 grades). Input is clamped to the range so we can safely specify a very high grade addition.
				__result = 99;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.getNoelEggQualityAdd))]
		[HarmonyPrefix]
		public static bool getNoelEggQualityAdd_Prefix(EpManager __instance, ref int __result)
		{
			if (Plugin.Instance.IsEnabled)
			{
				// The Noel Egg (ew) quality modifier. This will be added to be grade base of item drop.
				// Item grade is in range [0, 4] (5 grades). Input is clamped to the range so we can safely specify a very high grade addition.
				__result = 99;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.writeBinaryTo))]
		[HarmonyPrefix]
		public static bool writeBinaryTo_Prefix(EpManager __instance, ByteArray /* Ba */ __0)
		{
			if (Plugin.Instance.IsEnabled)
			{
				ClearCounters(__instance);
			}

			// Go on
			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.readBinaryFrom))]
		[HarmonyPostfix]
		public static void readBinaryFrom_Postfix(EpManager __instance, ByteArray /* Ba */ __0)
		{
			if (!Plugin.Instance.IsEnabled)
			{
				return;
			}

			ClearCounters(__instance);

			__instance.Pr.ep = X.IntC(__instance.ep);

			__instance.flushCurrentBattle();
		}

		private static void ClearCounters(EpManager instance)
		{
			X.ALL0(instance.Atotal_exp);
			X.ALL0(instance.Atotal_orgasmed);
			X.ALL0(instance.Ases_orgasmable);

			instance.Osituation_orgasmed.Clear();
			instance.Oegg_layed.Clear();

			instance.last_ex_count = 0;
			instance.last_ex_count_temp = 0;
			instance.last_ex_multi_count = 0;
			instance.last_ex_multi_count_temp = 0;
			instance.pee_count = 0;
			instance.orgasm_individual_count = 0;
			instance.bt_exp_added = 0;
			instance.bt_orgasm = 0;
			instance.bt_applied = 0;
			instance.orgasm_oazuke = false;
			instance.lead_to_orgasm = 0;
			instance.cure_ep_after_orgasm = 0.0f;
			instance.cure_ep_after_orgasm_one = 0.0f;
			instance.t_oazuke = -1.0f;
			instance.ep = 0.0f;
			instance.t_crack_cure = 0.0f;
			instance.multiple_orgasm = 0;
			instance.crack_cure_count = 0;
			instance.crack_cure_once = 0;
			instance.t_sage = 0.0f;
			instance.t_lock = 0.0f;
		}

	}

}
