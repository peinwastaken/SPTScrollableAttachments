using AttachmentScrolling.Config;
using BepInEx;
using BepInEx.Logging;
using SPT.Reflection.Patching;

namespace AttachmentScrolling;

[BepInPlugin("com.pein.attachmentscrolling", "ScrollableAttachments", "1.1.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;

        var patchManager = new PatchManager(this, true);
        patchManager.EnablePatches();

        GeneralConfig.Initialize(Config);
    }
}
