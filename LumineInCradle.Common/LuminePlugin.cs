using BepInEx;
using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace LumineInCradle;

public abstract class LuminePlugin : BaseUnityPlugin
{

	static LuminePlugin()
	{
		DefaultTargetGameVersion = new GameVersion(KnownMajor, KnownMinor, KnownRevision);
	}

	public static GameVersion GetGameVersion()
	{
		if (_gameVersionCache == null)
		{
			var versionString = Application.version;
			_gameVersionCache = GameVersion.Parse(versionString);
		}

		return _gameVersionCache;
	}

	[NotNull]
	public ManualLogSource InternalLogger => Logger;

	public bool IsEnabled { get; protected set; }

	protected virtual bool TestAgainstCurrentGameVersion()
	{
		var gameVersion = GetGameVersion();

		if (gameVersion != DefaultTargetGameVersion)
		{
			Logger.LogWarning($"Not a known game version: {gameVersion}, ignoring.");
			return false;
		}

		return true;
	}

	[CanBeNull]
	private static GameVersion _gameVersionCache;

	protected static readonly GameVersion DefaultTargetGameVersion;

	private const int KnownMajor = 0;
	private const int KnownMinor = 20;
	private const string KnownRevision = "s";

}
