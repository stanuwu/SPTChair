using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSG.CameraEffects;
using EFT;
using EFT.Interactive;
using SPTChair.Util;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SPTChair.modules;

public static class Esp
{
    private static Dictionary<int, Player> _players = new Dictionary<int, Player>();

    public static Material Mat;

    public static Color AiColor = Color.yellow;
    public static Color PmcColor = Color.red;
    public static Color BossColor = Color.magenta;
    public static Color CrosshairColor = new Color(1, 1, 1, .5f);
    public static Color HudActive = Color.green;
    public static Color HudInactive = Color.gray;
    public static Color CorpseColor = Color.white;
    public static Color ExtractColor = Color.green;
    public static Color PowerColor = Color.blue;

    public static int DrawDistance = 600;

    private static Color GetPlayerColor(Player player)
    {
        if (!player.HealthController.IsAlive) return CorpseColor;
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
            if (_players.ContainsKey(p.Id)) continue;
            _players.Add(p.Id, p);
        }

        foreach (Player p in _players.Values)
        {
            if (p == null)
            {
                _players.Remove(p.Id);
                continue;
            }

            if (!PlayerUtil.IsOnScreen(camera, scopeCamera, p) || PlayerUtil.Distance(player, p) > DrawDistance || p.IsYourPlayer)
                continue;

            if (!SPTChair.MANAGER.CorpseEsp.Value && !p.HealthController.IsAlive) continue;

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
            Render.DrawString(new Vector2(offset, offset), "SPTChair", Color.cyan, false);
            Render.DrawString(new Vector2(offset, offset * 2), "Esp", SPTChair.MANAGER.Esp.Value ? HudActive : HudInactive,
                false);
            Render.DrawString(new Vector2(offset, offset * 3), "Crosshair",
                SPTChair.MANAGER.Crosshair.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 4), "Tracers",
                SPTChair.MANAGER.Tracers.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 5), "Silent Aim",
                SPTChair.MANAGER.SilentAim.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 6), "Stamina",
                SPTChair.MANAGER.Stamina.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 7), "Speed",
                SPTChair.MANAGER.Speed.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 8), "No Weight",
                SPTChair.MANAGER.NoWeight.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 9), "No Fall",
                SPTChair.MANAGER.NoFall.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 10), "Corpse Esp",
                SPTChair.MANAGER.CorpseEsp.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 11), "Extract Esp",
                SPTChair.MANAGER.ExtractEsp.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 12), "Radar",
                SPTChair.MANAGER.Radar.Value ? HudActive : HudInactive, false);
            Render.DrawString(new Vector2(offset, offset * 13), "Night Vision",
                SPTChair.MANAGER.NightVision.Value ? HudActive : HudInactive, false);
        }
    }

    private static void DrawExtract(GameWorld gameWorld, Player player, Camera camera, Camera scopeCamera)
    {
        if (!SPTChair.MANAGER.ExtractEsp.Value) return;
        foreach (ExfiltrationPoint exfil in gameWorld.ExfiltrationController.EligiblePoints(player.Profile))
        {
            Vector3 pos = exfil.transform.position;
            Vector3 onScreen = PlayerUtil.WorldToScreenPointScoped(camera, scopeCamera, pos);
            if (!PlayerUtil.IsVisible(onScreen)) continue;
            Render.DrawString(new Vector2(onScreen.x, onScreen.y), exfil.Settings.Name, ExtractColor);
        }
    }

    private static void DrawRadar(GameWorld gameWorld, Player player)
    {
        if (!SPTChair.MANAGER.Radar.Value) return;
        Vector2 ps = new Vector2(25, 25 * (SPTChair.MANAGER.Hud.Value ? 15 : 1));
        Vector2 size = new Vector2(200, 200);
        Render.DrawSquare(ps, size, Color.black);

        List<RadarPlayer> valid = new List<RadarPlayer>();
        foreach (Player p in gameWorld.AllAlivePlayersList)
        {
            if (p.Id.Equals(player.Id)) continue;
            RadarPlayer rp = new RadarPlayer(player, p);
            if (rp.Position.x < 100 && rp.Position.x > -100 && rp.Position.y < 100 && rp.Position.y > -100) valid.Add(rp);
        }

        Vector2 dotSize = new Vector2(4, 4);

        foreach (RadarPlayer rp in valid)
        {
            Render.DrawCenteredSquare(rp.Position + ps + (size / 2), dotSize, rp.Color);
        }

        Render.DrawCenteredSquare(ps + (size / 2), dotSize, Color.white);
        Render.DrawLine(new Vector2(ps.x + size.x / 2 - 0.5f, ps.y + size.y / 2), new Vector2(ps.x + size.x / 2 - 0.5f, ps.y), 1, Color.white);
    }

    private class RadarPlayer
    {
        public RadarPlayer(Player mainPlayer, Player player)
        {
            double yaw = (Math.PI / 180) * (mainPlayer.Rotation.x);
            Position = new Vector2(player.Position.x - mainPlayer.Position.x, player.Position.z - mainPlayer.Position.z);
            Position = new Vector2(
                (float)(Position.x * Math.Cos(yaw) - Position.y * Math.Sin(yaw)),
                (float)(Position.y * Math.Cos(yaw) + Position.x * Math.Sin(yaw))
            );
            Position = new Vector2(Position.x, Position.y * -1);
            Color = GetPlayerColor(player);
        }

        public Vector2 Position { get; }
        public Color Color { get; }
    }

    private static void DrawNightVision(Player player, Camera camera)
    {
        NightVision vis = camera.GetComponent<NightVision>();
        if (SPTChair.MANAGER.NightVision.Value)
        {
            vis.On = true;
        }
        else
        {
            vis.On = player.NightVisionObserver.Component != null && player.NightVisionObserver.Component.Togglable.On;
        }
    }

    public static void DoDraw(GameWorld gameWorld, Player player)
    {
        if (!Mat) InitMat();
        DrawHUD();
        Camera camera = Camera.current;
        Camera scopeCamera = Camera.allCameras.FirstOrDefault(c => c.name == "BaseOpticCamera(Clone)");
        DrawExtract(gameWorld, player, camera, scopeCamera);
        DrawAllEsp(gameWorld, player, camera, scopeCamera);
        DrawRadar(gameWorld, player);
        DrawCrosshair(scopeCamera);
        DrawNightVision(player, camera);
    }
}