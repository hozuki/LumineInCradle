using BepInEx;

namespace LumineInCradle.NoMosaic
{
	[BepInPlugin("moe.mottomo.plugins.lumine_in_cradle.no_mosaic", "No Mosaic for Alice in Cradle", "1.0.0")]
	[BepInDependency("moe.mottomo.plugins.lumine_in_cradle.common", BepInDependency.DependencyFlags.HardDependency)]
	public class Plugin : MethodPatcherPlugin<Plugin>
	{

		protected override bool PatchMethods()
		{
			return Patch.PatchMethods(Logger);
		}

	}
}
