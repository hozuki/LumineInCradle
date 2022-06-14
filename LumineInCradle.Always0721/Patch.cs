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

		static UiBenchMenu_Patch()
		{
			BenchCmdTypeCache = new CachedClassObject<Type>();
			BenchCmdCtorCache = new CachedClassObject<ConstructorInfo>();
			ACmdFieldCache = new CachedClassObject<FieldInfo>();
		}

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
			var classInfo = typeof(UiBenchMenu);
			var acmdField = ACmdFieldCache.GetValueOrSetBy(classInfo, ci => ci.GetField("ACmd", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));

			var benchCmds = acmdField.GetValue(null) as object[];

			// cure_ep (masturbate)
			benchCmds[3] = CreateNewBenchCmd("cure_ep", pr => !pr.isMasturbating(), false, false);
			// cure_egged (lay egg)
			benchCmds[4] = CreateNewBenchCmd("cure_egged", pr => true, false, false);

			acmdField.SetValue(null, benchCmds);
		}

		private static object CreateNewBenchCmd([NotNull] string key, [NotNull] Func<PR, bool> canUsePredicate, bool canSetAuto, bool onlyInSafeArea)
		{
			var assembly = Assembly.GetAssembly(typeof(UiBenchMenu));
			var classInfo = BenchCmdTypeCache.GetValueOrSetBy(assembly, asm => asm.GetType("nel.UiBenchMenu+BenchCmd"));

			// Alternative:
			// notice the (partial) assembly qualified name here:
			// var classInfo = Type.GetType("nel.UiBenchMenu+BenchCmd, Assembly-CSharp");
			var ctor = BenchCmdCtorCache.GetValueOrSetBy(classInfo, ci => ci.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, BenchCmdCtorParamTypes, Array.Empty<ParameterModifier>()));

			var obj = ctor.Invoke(new object[] { key, canUsePredicate, canSetAuto, onlyInSafeArea });

			return obj;
		}

		[NotNull]
		private static readonly Type[] BenchCmdCtorParamTypes =
		{
			typeof(string), typeof(Func<PR, bool>), typeof(bool), typeof(bool),
		};

		private static readonly CachedClassObject<Type> BenchCmdTypeCache;
		private static readonly CachedClassObject<ConstructorInfo> BenchCmdCtorCache;
		private static readonly CachedClassObject<FieldInfo> ACmdFieldCache;

	}

}
