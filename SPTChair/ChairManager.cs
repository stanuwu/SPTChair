using System;
using System.Reflection;
using BepInEx.Configuration;
using BSG.CameraEffects;
using UnityEngine;
using EFT;
using Comfort.Common;
using EFT.HealthSystem;
using EFT.Interactive;

namespace SPTChair;

public class ChairManager : MonoBehaviour
{
    public ConfigEntry<bool> Debug { get; set; }

    public ConfigEntry<bool> Hud { get; set; }
    public ConfigEntry<bool> Esp { get; set; }
    public ConfigEntry<bool> Crosshair { get; set; }
    public ConfigEntry<bool> Tracers { get; set; }
    public ConfigEntry<bool> SilentAim { get; set; }
    public ConfigEntry<bool> Stamina { get; set; }
    public ConfigEntry<bool> Speed { get; set; }
    public ConfigEntry<bool> NoWeight { get; set; }
    public ConfigEntry<bool> NoFall { get; set; }
    public ConfigEntry<bool> CorpseEsp { get; set; }
    public ConfigEntry<bool> ExtractEsp { get; set; }
    public ConfigEntry<bool> NightVision { get; set; }
    public ConfigEntry<bool> Radar { get; set; }

    private GameWorld GameWorld;

    private bool MatchStarted()
    {
        return Singleton<GameWorld>.Instantiated && Singleton<GameWorld>.Instance.MainPlayer != null && Camera.current.GetComponent<NightVision>() != null;
    }

    private void Start()
    {
        const string cat = "Modules";
        Hud = SPTChair.CONFIG.Bind(cat, "Hud", true);
        Esp = SPTChair.CONFIG.Bind(cat, "Esp", true);
        Crosshair = SPTChair.CONFIG.Bind(cat, "Crosshair", true);
        Tracers = SPTChair.CONFIG.Bind(cat, "Tracers", true);
        SilentAim = SPTChair.CONFIG.Bind(cat, "Silent Aim", true);
        Stamina = SPTChair.CONFIG.Bind(cat, "Stamina", true);
        Speed = SPTChair.CONFIG.Bind(cat, "Speed", true);
        NoWeight = SPTChair.CONFIG.Bind(cat, "No Weight", true);
        NoFall = SPTChair.CONFIG.Bind(cat, "No Fall", true);
        CorpseEsp = SPTChair.CONFIG.Bind(cat, "Corpse Esp", true);
        ExtractEsp = SPTChair.CONFIG.Bind(cat, "Extract Esp", true);
        NightVision = SPTChair.CONFIG.Bind(cat, "Night Vision", false);
        Radar = SPTChair.CONFIG.Bind(cat, "Radar", true);

        Debug = SPTChair.CONFIG.Bind("Debug", "Debug Out", false);
    }

    private void FixedUpdate()
    {
        if (!MatchStarted()) return;
        GameWorld = Singleton<GameWorld>.Instance;
        Player player = GameWorld.MainPlayer;
        if (NoFall.Value)
        {
            player.Physical.FallDamageMultiplier = 0;
        }

        if (Speed.Value)
        {
            player._characterController.Move(player._characterController.velocity / 100, 100);
        }
    }

    private void Update()
    {
        if (!MatchStarted()) return;
        GameWorld = Singleton<GameWorld>.Instance;
        Player player = GameWorld.MainPlayer;
        if (Stamina.Value)
        {
            player.Physical.Stamina.Current = 100;
            player.Physical.HandsStamina.Current = 100;
            player.Physical.Oxygen.Current = 100;
        }

        if (NoWeight.Value)
        {
            player.HealthController.SetEncumbered(false);
            player.HealthController.SetOverEncumbered(false);
        }
    }

    private void OnGUI()
    {
        if (!MatchStarted()) return;
        GameWorld = Singleton<GameWorld>.Instance;
        Player player = GameWorld.MainPlayer;
        modules.Esp.DoDraw(GameWorld, player);
    }
}