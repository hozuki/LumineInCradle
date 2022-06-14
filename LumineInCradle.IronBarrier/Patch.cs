using System.Diagnostics.CodeAnalysis;
using Better;
using HarmonyLib;
using m2d;
using nel;
using PixelLiner.PixelLinerLib;
using static LumineInCradle.FieldHelper;
using X = XX.X;

namespace LumineInCradle.IronBarrier;

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

		_harmonyInstance = new Harmony(PluginInfo.PLUGIN_GUID);

		_harmonyInstance.PatchAll(typeof(EpManager_Patch));
	}

	// Also see EpSuppressor
	private static class EpManager_Patch
	{

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.applyEpDamage))]
		[HarmonyPrefix]
		public static bool applyEpDamage_Prefix(EpManager __instance, ref bool __result, EpAtk /* Atk */ __0, M2Attackable /* AttackedBy */ __1, EPCATEG_BITS /* bits */ __2)
		{
			if (Plugin.IsEnabled)
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
			if (!Plugin.IsEnabled)
			{
				return;
			}

			ClearCounters(__instance);
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.addEggLayCount))]
		[HarmonyPrefix]
		public static bool addEggLayCount_Prefix(EpManager __instance, PrEggManager.CATEG /* categ */ __0, int /* cnt */ __1)
		{
			if (Plugin.IsEnabled)
			{
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.addPeeCount))]
		[HarmonyPrefix]
		public static bool addPeeCount_Prefix(EpManager __instance)
		{
			if (Plugin.IsEnabled)
			{
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), "get_" + nameof(EpManager.masturbate_count))]
		[HarmonyPrefix]
		public static bool get_masturbate_count_Prefix(EpManager __instance, ref int __result)
		{
			if (Plugin.IsEnabled)
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
			if (Plugin.IsEnabled)
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
			if (Plugin.IsEnabled)
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
			if (Plugin.IsEnabled)
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
			if (Plugin.IsEnabled)
			{
				__result = 0;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), "getLeadToOrgasmRatio")]
		[HarmonyPrefix]
		public static bool getLeadToOrgasmRatio_Prefix(EpManager __instance, ref float __result, EPCATEG /* target */ __0)
		{
			if (Plugin.IsEnabled)
			{
				__result = 0.0f;
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.getOrgasmedTotal))]
		[HarmonyPrefix]
		public static bool recalcOrgasmable_Prefix(EpManager __instance)
		{
			if (!Plugin.IsEnabled)
			{
				return true;
			}

			Ases_orgasmable ??= AccessTools.FieldRefAccess<float[]>(typeof(EpManager), nameof(Ases_orgasmable));
			X.ALL0(Ases_orgasmable(__instance));

			return false;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.quitOrasmSageTime))]
		[HarmonyPostfix]
		public static void quitOrasmSageTime_Postfix(EpManager __instance, bool /* cure_ser */ __0)
		{
			if (!Plugin.IsEnabled)
			{
				return;
			}

			// Clear sage time
			t_oazuke ??= AccessTools.FieldRefAccess<float>(typeof(EpManager), nameof(t_oazuke));
			t_oazuke(__instance) = 1000.0f;
			orgasm_oazuke ??= AccessTools.FieldRefAccess<bool>(typeof(EpManager), nameof(orgasm_oazuke));
			orgasm_oazuke(__instance) = false;
		}

		[HarmonyPatch(typeof(EpManager), nameof(EpManager.getNoelJuiceQualityAdd))]
		[HarmonyPrefix]
		public static bool getNoelJuiceQualityAdd_Prefix(EpManager __instance, ref int __result)
		{
			if (Plugin.IsEnabled)
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
			if (Plugin.IsEnabled)
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
			if (Plugin.IsEnabled)
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
			if (!Plugin.IsEnabled)
			{
				return;
			}

			ClearCounters(__instance);

			__instance.Pr.ep = X.IntC(ep(__instance));

			__instance.flushCurrentBattle();
		}

		private static void ClearCounters(EpManager instance)
		{
			FastFieldValueClear(instance, nameof(Atotal_exp), ref Atotal_exp);
			FastFieldValueClear(instance, nameof(Atotal_orgasmed), ref Atotal_orgasmed);
			FastFieldValueClear(instance, nameof(Ases_orgasmable), ref Ases_orgasmable);
			FastFieldValueClear(instance, nameof(Osituation_orgasmed), ref Osituation_orgasmed);
			FastFieldValueClear(instance, nameof(Oegg_layed), ref Oegg_layed);

			FastFieldValueClear(instance, nameof(last_ex_count), ref last_ex_count);
			FastFieldValueClear(instance, nameof(last_ex_count_temp), ref last_ex_count_temp);
			FastFieldValueClear(instance, nameof(last_ex_multi_count), ref last_ex_multi_count);
			FastFieldValueClear(instance, nameof(last_ex_multi_count_temp), ref last_ex_multi_count_temp);
			FastFieldValueClear(instance, nameof(pee_count), ref pee_count);
			FastFieldValueClear(instance, nameof(orgasm_individual_count), ref orgasm_individual_count);
			FastFieldValueClear(instance, nameof(bt_exp_added), ref bt_exp_added);
			FastFieldValueClear(instance, nameof(bt_orgasm), ref bt_orgasm);
			FastFieldValueClear(instance, nameof(bt_applied), ref bt_applied);
			FastFieldValueClear(instance, nameof(orgasm_oazuke), ref orgasm_oazuke);
			FastFieldValueClear(instance, nameof(lead_to_orgasm), ref lead_to_orgasm);
			FastFieldValueClear(instance, nameof(cure_ep_after_orgasm), ref cure_ep_after_orgasm);
			FastFieldValueClear(instance, nameof(cure_ep_after_orgasm_one), ref cure_ep_after_orgasm_one);
			FastFieldValueClear(instance, nameof(t_oazuke), ref t_oazuke, -1.0f);
			FastFieldValueClear(instance, nameof(ep), ref ep);
			FastFieldValueClear(instance, nameof(t_crack_cure), ref t_crack_cure);
			FastFieldValueClear(instance, nameof(multiple_orgasm), ref multiple_orgasm);
			FastFieldValueClear(instance, nameof(crack_cure_count), ref crack_cure_count);
			FastFieldValueClear(instance, nameof(crack_cure_once), ref crack_cure_once);
			FastFieldValueClear(instance, nameof(t_sage), ref t_sage);
			FastFieldValueClear(instance, nameof(t_lock), ref t_lock);
		}

		private static AccessTools.FieldRef<object, byte[]> Atotal_exp;
		private static AccessTools.FieldRef<object, int[]> Atotal_orgasmed;
		private static AccessTools.FieldRef<object, float[]> Ases_orgasmable;
		private static AccessTools.FieldRef<object, BDic<string, int>> Osituation_orgasmed;
		private static AccessTools.FieldRef<object, BDic<PrEggManager.CATEG, int>> Oegg_layed;
		private static AccessTools.FieldRef<object, int> last_ex_count;
		private static AccessTools.FieldRef<object, int> last_ex_count_temp;
		private static AccessTools.FieldRef<object, int> last_ex_multi_count;
		private static AccessTools.FieldRef<object, int> last_ex_multi_count_temp;
		private static AccessTools.FieldRef<object, int> pee_count;
		private static AccessTools.FieldRef<object, int> orgasm_individual_count;
		private static AccessTools.FieldRef<object, EPCATEG_BITS> bt_exp_added;
		private static AccessTools.FieldRef<object, EPCATEG_BITS> bt_orgasm;
		private static AccessTools.FieldRef<object, EPCATEG_BITS> bt_applied;
		private static AccessTools.FieldRef<object, bool> orgasm_oazuke;
		private static AccessTools.FieldRef<object, EPCATEG_BITS> lead_to_orgasm;
		private static AccessTools.FieldRef<object, float> cure_ep_after_orgasm;
		private static AccessTools.FieldRef<object, float> cure_ep_after_orgasm_one;
		private static AccessTools.FieldRef<object, float> t_oazuke;
		private static AccessTools.FieldRef<object, float> ep;
		private static AccessTools.FieldRef<object, float> t_crack_cure;
		private static AccessTools.FieldRef<object, int> multiple_orgasm;
		private static AccessTools.FieldRef<object, int> crack_cure_count;
		private static AccessTools.FieldRef<object, int> crack_cure_once;
		private static AccessTools.FieldRef<object, float> t_sage;
		private static AccessTools.FieldRef<object, float> t_lock;

	}

}
