using AttachmentScrolling.Components;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace AttachmentScrolling.Patches;

public class UIAwakePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(EditBuildScreen), nameof(EditBuildScreen.Awake));
    }

    [PatchPostfix]
    private static void PatchPostfix(EditBuildScreen __instance)
    {
        __instance.RectTransform.gameObject.AddComponent<AttachmentScrollComponent>();
    }
}
