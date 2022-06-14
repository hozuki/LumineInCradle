using BepInEx;
using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace LumineInCradle.VerticalSync
{
	[BepInPlugin("moe.mottomo.plugins.lumine_in_cradle.vertical_sync", "VertSync for Alice in Cradle", "1.0.0")]
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

			Logger.LogInfo("Plugin enabled");
		}

		private void Start()
		{
			if (Application.isMobilePlatform)
			{
				Application.targetFrameRate = HighEndMobileFrameRate;
			}
			else
			{
				QualitySettings.vSyncCount = VSyncCount;
			}
		}

		public static bool IsEnabled { get; set; }

		private static readonly GameVersion KnownGameVersion;

		private const int KnownMajor = 0;
		private const int KnownMinor = 20;
		private const string KnownRevision = "s";

		// Sync on every Nth frame
		private const int VSyncCount = 1;

		// Most mobile devices can't reach this value
		private const int HighEndMobileFrameRate = 144;

	}
}
