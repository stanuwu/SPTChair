using System.Reflection;
using Aki.Reflection.Patching;
using EFT;
using SPTChair.modules;
using UnityEngine;

namespace SPTChair.patches;

public class PatchInitiateShot : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(Player.FirearmController).GetMethod("InitiateShot", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [PatchPrefix]
    public static void Prefix(Player.FirearmController __instance,
        GInterface273 weapon,
        BulletClass ammo,
        Vector3 shotPosition,
        ref Vector3 shotDirection,
        Vector3 fireportPosition,
        int chamberIndex,
        float overheat)
    {
        Aim.DoSilentAim(fireportPosition, ref shotDirection);
    }
}