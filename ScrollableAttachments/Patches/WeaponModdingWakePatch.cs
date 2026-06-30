using AttachmentScrolling.Components;
using EFT.UI;
using EFT.UI.WeaponModding;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace AttachmentScrolling.Patches;

public class WeaponModdingWakePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(WeaponModdingScreen), nameof(WeaponModdingScreen.Awake));
    }

    [PatchPostfix]
    private static void PatchPostfix(WeaponModdingScreen __instance)
    {
        __instance.RectTransform.gameObject.AddComponent<AttachmentScrollComponent>();
    }
}
