using System.Diagnostics.CodeAnalysis;

namespace LumineInCradle;

[SuppressMessage("Class Declaration", "BepInEx001:Class inheriting from BaseUnityPlugin missing BepInPlugin attribute")]
public abstract class LuminePlugin<T> : LuminePlugin
	where T : LuminePlugin<T>
{

	public static T Instance => _instance;

	protected void SetThisAsInstance()
	{
		_instance = (T)this;
	}

	private static T _instance;

}
