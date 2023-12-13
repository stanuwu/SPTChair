using EFT;
using UnityEngine;

namespace SPTChair.Util;

public static class PlayerUtil
{
    public static Vector3 WorldToScreenPointScoped(Camera camera, Camera scopeCamera, Vector3 worldPoint)
    {
        if (scopeCamera == null)
        {
            return WorldToScreenPoint(camera, worldPoint);
        }

        float scale = Screen.height / (float)camera.scaledPixelHeight;
        Vector3 cameraOffset = new Vector3(
            camera.pixelWidth / 2 - scopeCamera.pixelWidth / 2,
            camera.pixelHeight / 2 - scopeCamera.pixelHeight / 2,
            0f
        );
        Vector3 screenPoint = scopeCamera.WorldToScreenPoint(worldPoint) + cameraOffset;
        screenPoint.y = Screen.height - screenPoint.y * scale;
        screenPoint.x *= scale;
        return screenPoint;
    }

    private static Vector3 WorldToScreenPoint(Camera camera, Vector3 worldPoint)
    {
        Vector3 screenPoint = camera.WorldToScreenPoint(worldPoint);
        float scale = Screen.height / (float)camera.scaledPixelHeight;
        screenPoint.y = Screen.height - screenPoint.y * scale;
        screenPoint.x *= scale;
        return screenPoint;
    }

    public static Vector3 ScreenPos(Camera camera, Camera scopeCamera, Player player)
    {
        return WorldToScreenPointScoped(camera, scopeCamera, player.Transform.position);
    }

    public static Vector3 HeadScreenPos(Camera camera, Camera scopeCamera, Player player)
    {
        return WorldToScreenPointScoped(camera, scopeCamera, player.PlayerBones.Head.position);
    }

    public static Vector3 BoneScreenPos(Camera camera, Camera scopeCamera, BifacialTransform bone)
    {
        return WorldToScreenPointScoped(camera, scopeCamera, bone.position);
    }

    public static Vector3 BoneScreenPos(Camera camera, Camera scopeCamera, Transform bone)
    {
        return WorldToScreenPointScoped(camera, scopeCamera, bone.position);
    }

    public static bool IsOnScreen(Camera camera, Camera scopeCamera, Player player)
    {
        return IsVisible(ScreenPos(camera, scopeCamera, player));
    }

    public static bool IsVisible(Vector3 screenPoint)
    {
        return screenPoint.z > 0.01f && screenPoint.x > -100f && screenPoint.y > -100f && screenPoint.x < Screen.width + 100 &&
               screenPoint.y < Screen.height + 100;
    }

    public static float Distance(Player player, Player player2)
    {
        return Vector3.Distance(player.CameraPosition.position, player2.Transform.position);
    }
}