using BepInEx;

namespace LumineInCradle.EggHarvest
{
	[BepInPlugin("moe.mottomo.plugins.lumine_in_cradle.egg_harvest", "Unlock 'Lay Egg' and 'Reset SOX State'", "1.0.0")]
	[BepInDependency("moe.mottomo.plugins.lumine_in_cradle.common", BepInDependency.DependencyFlags.HardDependency)]
	public class Plugin : MethodPatcherPlugin<Plugin>
	{

		protected override bool PatchMethods()
		{
			return Patch.PatchMethods(Logger);
		}

	}
}
