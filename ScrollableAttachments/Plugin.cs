using AttachmentScrolling.Config;
using AttachmentScrolling.Patches;
using BepInEx;
using BepInEx.Logging;

namespace AttachmentScrolling;

[BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
[BepInDependency("com.arys.unitytoolkit", "2.0.2")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;

        GeneralConfig.Initialize(Config);

        new EditBuildAwakePatch().Enable();
        new WeaponModdingWakePatch().Enable();
        new ScrollTriggerDisablePatch().Enable();
    }
}
