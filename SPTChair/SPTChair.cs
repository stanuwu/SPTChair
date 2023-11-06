using BepInEx.Configuration;
using BepInEx.Logging;

namespace SPTChair;

public static class SPTChair
{
    public const string GUID = "com.stanuwu.cptchair";
    public const string NAME = "SPTChair";
    public const string VERSION = "1.0.0";
    public static ManualLogSource LOG;
    public static ConfigFile CONFIG;
    public static ChairManager MANAGER;
}