using BepInEx;
using JetBrains.Annotations;
using UnityEngine;

namespace LumineInCradle;

public abstract class LuminePlugin : BaseUnityPlugin
{

	public static GameVersion GetGameVersion()
	{
		if (_gameVersionCache == null)
		{
			var versionString = Application.version;
			_gameVersionCache = GameVersion.Parse(versionString);
		}

		return _gameVersionCache;
	}

	[CanBeNull]
	private static GameVersion _gameVersionCache;

}
