using AttachmentScrolling.Components;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine.EventSystems;

namespace AttachmentScrolling.Patches;

public class ScrollTriggerDisablePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ScrollTrigger), nameof(ScrollTrigger.OnScroll));
    }

    [PatchPrefix]
    private static bool PatchPrefix(ScrollTrigger __instance, PointerEventData eventData)
    {
        return !AttachmentScrollComponent.IsScrollSuppressed(__instance, eventData);
    }
}
