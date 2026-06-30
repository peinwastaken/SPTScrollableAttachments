using AttachmentScrolling.Components;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace AttachmentScrolling.Patches;

public class ScrollTriggerDisablePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ScrollTrigger), nameof(ScrollTrigger.OnScroll));
    }

    [PatchPrefix]
    private static bool PatchPrefix()
    {
        return !AttachmentScrollComponent.Instance.HoveringDropdown;
    }
}
