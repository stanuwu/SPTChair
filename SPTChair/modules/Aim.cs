using System;
using Comfort.Common;
using EFT;
using UnityEngine;

namespace SPTChair.modules;

public static class Aim
{
    private static float MaxRange = 200;
    private static float MaxFovMeters = 35;

    public static void DoSilentAim(Vector3 fireportPosition, ref Vector3 shotDirection)
    {
        Player player = Singleton<GameWorld>.Instance.MainPlayer;
        // Scuffed af way to check if this is the player since there is no access to player here
        if (fireportPosition != Singleton<GameWorld>.Instance.MainPlayer.Fireport.position) return;
        if (!SPTChair.MANAGER.SilentAim.Value) return;
        if (SPTChair.MANAGER.Debug.Value) SPTChair.LOG.LogInfo("SHOOTING");
        Player target = null;
        double distance = Double.MaxValue;
        foreach (Player p in Singleton<GameWorld>.Instance.AllAlivePlayersList)
        {
            if (p.IsYourPlayer) continue;
            if (!p.IsVisible) continue;

            Vector3 headPos = p.CameraPosition.position;

            if (Vector3.Distance(fireportPosition, headPos) > MaxRange) continue;

            Vector3 aimDirection = Vector3.Normalize(shotDirection);
            Vector3 v = headPos - fireportPosition;
            float d = Vector3.Dot(v, aimDirection);
            Vector3 closestPoint = fireportPosition + shotDirection * d;

            float newDist = Vector3.Distance(closestPoint, headPos);
            if (!(distance > newDist)) continue;
            distance = newDist;
            target = p;
        }

        if (SPTChair.MANAGER.Debug.Value)
        {
            SPTChair.LOG.LogInfo($"Closest Target: {distance}m FOV");
        }

        if (distance > MaxFovMeters) return;

        Vector3 aimPos = target.CameraPosition.position;

        if (target == null) return;
        if (SPTChair.MANAGER.Debug.Value)
        {
            SPTChair.LOG.LogInfo("AIMING");
            SPTChair.LOG.LogInfo($"Weapon: {fireportPosition.x}, {fireportPosition.y}, {fireportPosition.z}");
            SPTChair.LOG.LogInfo($"Target: {aimPos.x}, {aimPos.y}, {aimPos.z}");
        }

        shotDirection = Vector3.Normalize(aimPos - fireportPosition);
    }
}