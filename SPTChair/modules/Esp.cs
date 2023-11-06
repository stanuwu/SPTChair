using System.Linq;
using EFT;
using EFT.Interactive;
using SPTChair.Util;
using UnityEngine;

namespace SPTChair.modules;

public static class Esp
{
    public static Material Mat;

    public static Color AiColor = Color.yellow;
    public static Color PmcColor = Color.red;
    public static Color BossColor = Color.magenta;
    public static Color CrosshairColor = new Color(1, 1, 1, .5f);
    public static Color HudActive = Color.green;
    public static Color HudInactive = Color.gray;

    public static int DrawDistance = 600;

    private static Color GetPlayerColor(Player player)
    {
        return player.Side is EPlayerSide.Bear or EPlayerSide.Usec ? PmcColor : player.AIData.IAmBoss ? BossColor : AiColor;
    }

    private static void InitMat()
    {
        Material mat = new Material(Shader.Find("Hidden/Internal-Colored"));
        mat.hideFlags = HideFlags.HideAndDontSave;
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        mat.SetInt("_ZWrite", 0);
        Mat = mat;
        GUI.skin.label.fontSize = 24;
    }

    private static void DrawAllEsp(GameWorld gameWorld, Player player, Camera camera, Camera scopeCamera)
    {
        foreach (Player p in gameWorld.AllAlivePlayersList)
        {
            if (!PlayerUtil.IsOnScreen(camera, scopeCamera, p) || PlayerUtil.Distance(player, p) > DrawDistance || p.IsYourPlayer)
                continue;

            Color playerColor = GetPlayerColor(p);

            Vector3 headScreenPos = PlayerUtil.HeadScreenPos(camera, scopeCamera, p);

            if (SPTChair.MANAGER.Esp.Value)
            {
                Vector3 headBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.Head);
                Vector3 neckBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.Neck);
                Vector3 pelvisBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.Pelvis);
                Vector3 shoulderLeftBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.Shoulders[0]);
                Vector3 shoulderRightBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.Shoulders[1]);
                Vector3 armLeftBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.Forearms[0]);
                Vector3 armRightBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.Forearms[1]);
                Vector3 handLeftBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.LeftPalm);
                Vector3 handRightBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.RightPalm);
                Vector3 legLeftBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.LeftThigh1);
                Vector3 legRightBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.RightThigh1);
                Vector3 kneeLeftBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.LeftThigh2);
                Vector3 kneeRightBone = PlayerUtil.BoneScreenPos(camera, scopeCamera, p.PlayerBones.RightThigh2);
                Vector3 footLeftBone = PlayerUtil.WorldToScreenPointScoped(camera, scopeCamera, p.PlayerBones.LeftThigh2.position - new Vector3(0, .5f, 0));
                Vector3 footRightBone = PlayerUtil.WorldToScreenPointScoped(camera, scopeCamera, p.PlayerBones.RightThigh2.position - new Vector3(0, .5f, 0));

                // body
                DrawBone(headBone, neckBone, playerColor);
                DrawBone(neckBone, pelvisBone, playerColor);

                // left arm
                DrawBone(neckBone, shoulderLeftBone, playerColor);
                DrawBone(shoulderLeftBone, armLeftBone, playerColor);
                DrawBone(armLeftBone, handLeftBone, playerColor);

                // right arm
                DrawBone(neckBone, shoulderRightBone, playerColor);
                DrawBone(shoulderRightBone, armRightBone, playerColor);
                DrawBone(armRightBone, handRightBone, playerColor);

                // left leg
                DrawBone(pelvisBone, legLeftBone, playerColor);
                DrawBone(legLeftBone, kneeLeftBone, playerColor);
                DrawBone(kneeLeftBone, footLeftBone, playerColor);

                // right leg
                DrawBone(pelvisBone, legRightBone, playerColor);
                DrawBone(legRightBone, kneeRightBone, playerColor);
                DrawBone(kneeRightBone, footRightBone, playerColor);
            }

            if (SPTChair.MANAGER.Tracers.Value)
            {
                Render.DrawLine(new Vector2(Screen.width / 2f, Screen.height / 2f), new Vector2(headScreenPos.x, headScreenPos.y), 0.5f, playerColor);
            }
        }
    }

    private static void DrawBone(Vector2 bone1, Vector2 bone2, Color playerColor)
    {
        Render.DrawLine(bone1, bone2, 1f, playerColor);
    }

    private static void DrawCrosshair(Camera scopeCamera)
    {
        if (SPTChair.MANAGER.Crosshair.Value && scopeCamera == null)
        {
            float x = Screen.width / 2f;
            float y = Screen.height / 2f;
            Render.DrawLine(new Vector2(x - 6, y), new Vector2(x + 5, y), 1f, CrosshairColor);
            Render.DrawLine(new Vector2(x, y - 6), new Vector2(x, y + 5), 1f, CrosshairColor);
        }
    }

    private static void DrawHUD()
    {
        if (SPTChair.MANAGER.Hud.Value)
        {
            int offset = 25;
            Render.DrawString(new Vector2(offset, offset), "SPTChair", HudActive, false);
            Render.DrawString(new Vector2(offset, offset * 2), "Esp", SPTChair.MANAGER.Esp.Value ? HudActive : HudInactive,
                false);
            Render.DrawString(new Vector2(offset, offset * 3), "Crosshair",
                SPTChair.MANAGER.Crosshair.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 4), "Tracers",
                SPTChair.MANAGER.Tracers.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 5), "Silent Aim",
                SPTChair.MANAGER.SilentAim.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 6), "Radar (WIP)", SPTChair.MANAGER.Radar.Value ? HudActive : HudInactive,
                false);
        }
    }

    private static void DrawRadar()
    {
    }

    public static void DoDraw(GameWorld gameWorld, Player player)
    {
        if (!Mat) InitMat();
        Camera camera = Camera.current;
        Camera scopeCamera = Camera.allCameras.FirstOrDefault(c => c.name == "BaseOpticCamera(Clone)");
        DrawAllEsp(gameWorld, player, camera, scopeCamera);
        DrawHUD();
        DrawRadar();
        DrawCrosshair(scopeCamera);
    }
}