using BepInEx;
using UnityEngine;

namespace LumineInCradle.VerticalSync
{
	[BepInPlugin("moe.mottomo.plugins.lumine_in_cradle.vertical_sync", "VertSync for Alice in Cradle", "1.0.0")]
	[BepInDependency("moe.mottomo.plugins.lumine_in_cradle.common", BepInDependency.DependencyFlags.HardDependency)]
	public class Plugin : LuminePlugin<Plugin>
	{

		private void Awake()
		{
			SetThisAsInstance();

			if (!TestAgainstCurrentGameVersion())
			{
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

		// Sync on every Nth frame
		private const int VSyncCount = 1;

		// Most mobile devices can't reach this value
		private const int HighEndMobileFrameRate = 144;

	}
}
