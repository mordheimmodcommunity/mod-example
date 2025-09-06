using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace ModExample
{
    public class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry mod;

        static bool Load(UnityModManager.ModEntry modEntry)
        {

            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            modEntry.Logger.Log("Mod Example has loaded!");
            mod = modEntry;
            enabled = modEntry.Enabled;
            modEntry.OnToggle = OnToggle;
            modEntry.OnUpdate = OnUpdate;

            return true; // If false the mod will show an error.
        }

        // Called when the mod is turned to on/off.
        // With this function you control an operation of the mod and inform users whether it is enabled or not.
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (value)
            {
                modEntry.Logger.Log("Mod Example is enabled!");
                // Perform all necessary steps to start mod.
            }
            else
            {
                modEntry.Logger.Log("Mod Example is disabled!");
                // Perform all necessary steps to stop mod.
            }

            enabled = value;
            return true; // If true, the mod will switch the state. If not, the state will not change.
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float deltaTime)
        {
            // Do something every frame
        }
    }

    [HarmonyPatch(typeof(MainMenuStartView))]
    [HarmonyPatch("StateEnter")]
    static class MainMenuStartView_StateEnter_Patch
    {
        static void Postfix(ref MainMenuStartView __instance)
        {
            if (!Main.enabled)
                return;

            try
            {
                __instance.StateMachine.ConfirmPopup.Show("menu_warning", "Mod Example", new Action<bool>(OnPopup), false, false);
            }
            catch (Exception e)
            {
                Main.mod.Logger.Error(e.ToString());
            }
        }

        static private void OnPopup(bool confirm)
        {
            if (confirm)
            {
                Application.Quit();
            }
        }
    }

    [HarmonyPatch(typeof(ConfirmationPopupView))]
    [HarmonyPatch("Show")]
    [HarmonyPatch(new Type[] { typeof(string), typeof(string), typeof(Action<bool>), typeof(bool), typeof(bool) })]
    static class ConfirmationPopupView_Show_Patch
    {
        static void Postfix(ref ConfirmationPopupView __instance, ref string titleId, ref string textId)
        {
            if (!Main.enabled)
                return;

            try
            {
                if (textId == "Mod Example")
                {
                    __instance.text.text = "This mod is an example!\nIf you click on Confirm it will close the game.";
                }
            }
            catch (Exception e)
            {
                Main.mod.Logger.Error(e.ToString());
            }
        }
    }
}
