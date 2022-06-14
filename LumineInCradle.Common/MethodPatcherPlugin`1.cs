using System.Diagnostics.CodeAnalysis;

namespace LumineInCradle;

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class MethodPatcherPlugin<T> : LuminePlugin<T>
	where T : MethodPatcherPlugin<T>
{

	protected virtual void Awake()
	{
		SetThisAsInstance();

		if (!TestAgainstCurrentGameVersion())
		{
			return;
		}

		if (PatchMethods())
		{
			IsEnabled = true;
			Logger.LogInfo("Plugin enabled");
		}
		else
		{
			Logger.LogInfo("Plugin is not enabled because of incomplete patching");
		}
	}

	protected abstract bool PatchMethods();

}
