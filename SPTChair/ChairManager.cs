using BepInEx.Configuration;
using UnityEngine;
using EFT;
using Comfort.Common;

namespace SPTChair;

public class ChairManager : MonoBehaviour
{
    public ConfigEntry<bool> Debug { get; set; }

    public ConfigEntry<bool> Hud { get; set; }
    public ConfigEntry<bool> Esp { get; set; }
    public ConfigEntry<bool> Crosshair { get; set; }
    public ConfigEntry<bool> Tracers { get; set; }
    public ConfigEntry<bool> SilentAim { get; set; }
    public ConfigEntry<bool> Radar { get; set; }

    private GameWorld GameWorld;

    private bool GameStarted()
    {
        return true;
    }

    private bool MatchStarted()
    {
        return Singleton<GameWorld>.Instantiated;
    }

    private void Start()
    {
        const string cat = "Modules";
        Hud = SPTChair.CONFIG.Bind(cat, "Hud", true);
        Esp = SPTChair.CONFIG.Bind(cat, "Esp", true);
        Crosshair = SPTChair.CONFIG.Bind(cat, "Crosshair", true);
        Tracers = SPTChair.CONFIG.Bind(cat, "Tracers", true);
        SilentAim = SPTChair.CONFIG.Bind(cat, "Silent Aim", true);
        Radar = SPTChair.CONFIG.Bind(cat, "Radar", true);

        Debug = SPTChair.CONFIG.Bind("Debug", "Debug Out", false);
    }

    private void Update()
    {
        if (!MatchStarted()) return;
        GameWorld = Singleton<GameWorld>.Instance;
    }

    private void OnGUI()
    {
        if (!MatchStarted()) return;
        GameWorld = Singleton<GameWorld>.Instance;
        Player player = GameWorld.MainPlayer;
        modules.Esp.DoDraw(GameWorld, player);
    }
}