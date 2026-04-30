using BepInEx;
using BepInEx.Logging;
using SPT.Reflection.Patching;

namespace AttachmentScrolling;

[BepInPlugin("com.pein.attachmentscrolling", "AttachmentScrolling", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;

        var patchManager = new PatchManager(this, true);
        patchManager.EnablePatches();
        
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
