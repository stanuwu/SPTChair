using BepInEx;
using BepInEx.Configuration;
using SPTChair.patches;
using UnityEngine;

namespace SPTChair
{
    [BepInPlugin(SPTChair.GUID, SPTChair.NAME, SPTChair.VERSION)]
    [BepInProcess("EscapeFromTarkov")]
    public class SPTChairPlugin : BaseUnityPlugin
    {
        public GameObject ManagerHook;

        private void Awake()
        {
            SPTChair.LOG = Logger;
            SPTChair.CONFIG = Config;
            SPTChair.LOG.LogInfo($"SPTChair hooking...");
            ManagerHook = new GameObject("ManagerHook");
            ManagerHook.AddComponent<ChairManager>();
            SPTChair.MANAGER = ManagerHook.GetComponent<ChairManager>();
            DontDestroyOnLoad(ManagerHook);
            SPTChair.LOG.LogInfo($"SPTChair hooked!");
            SPTChair.LOG.LogInfo("SPTChair patching...");
            new PatchInitiateShot().Enable();
        }
    }
}