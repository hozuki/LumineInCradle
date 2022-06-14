using BepInEx;
using BepInEx.Logging;
using JetBrains.Annotations;

namespace LumineInCradle.EggHarvest
{
	[BepInPlugin("moe.mottomo.plugins.lumine_in_cradle.egg_harvest", "Unlock 'Lay Egg' and 'Reset SOX State'", "1.0.0")]
	[BepInDependency("moe.mottomo.plugins.lumine_in_cradle.common", BepInDependency.DependencyFlags.HardDependency)]
	public class Plugin : LuminePlugin
	{

		static Plugin()
		{
			KnownGameVersion = new GameVersion(KnownMajor, KnownMinor, KnownRevision);
		}

		private static Plugin _instance;

		public static Plugin Instance => _instance;

		[NotNull]
		public ManualLogSource InternalLogger => Logger;

		private void Awake()
		{
			_instance = this;

			var gameVersion = GetGameVersion();

			if (gameVersion != KnownGameVersion)
			{
				Logger.LogWarning($"Not a known game version: {gameVersion}, ignoring.");
				return;
			}

			IsEnabled = true;
			Patch.PatchMethods();

			Logger.LogInfo("Plugin enabled");
		}

		public static bool IsEnabled { get; set; }

		private static readonly GameVersion KnownGameVersion;

		private const int KnownMajor = 0;
		private const int KnownMinor = 20;
		private const string KnownRevision = "s";

	}
}
