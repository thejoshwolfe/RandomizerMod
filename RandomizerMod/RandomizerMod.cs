using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace RandomizerMod
{
    public class RandomizerMod : Mod
    {
        //Attach to the modded dll
        public override void Initialize()
        {
            Log("initializing!");
            ModHooks.Instance.GetPlayerIntHook += GetPlayerDataInt;
            ModHooks.Instance.SetPlayerIntHook += SetPlayerDataInt;
            ModHooks.Instance.GetPlayerBoolHook += GetPlayerDataBool;
            ModHooks.Instance.SetPlayerBoolHook += SetPlayerDataBool;
            ModHooks.Instance.SavegameLoadHook += LoadGame;
            ModHooks.Instance.SavegameSaveHook += SaveGame;
            ModHooks.Instance.SavegameClearHook += DeleteGame;
            ModHooks.Instance.NewGameHook += NewGame;

            //ModHooks.Instance.SceneChanged += SceneHandler.CheckForChanges;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneHandler.CheckForChanges;

            ModHooks.Instance.LanguageGetHook += LanguageHandler.Get;
            ModHooks.Instance.BeforeSceneLoadHook += RoomChanger.ChangeRoom;
            UnityEngine.GameObject UIObj = new UnityEngine.GameObject();
            UIObj.AddComponent<GUIController>();
            UnityEngine.GameObject.DontDestroyOnLoad(UIObj);

            RandomizerEntries.Initialize();

            Log("initialized!");
        }

        public override string GetVersion()
        {
            return "2.0.0";
        }

        public static int GetPlayerDataInt(string name)
        {
            if (Randomizer.CurrentInstance != null)
            {
                // randomizer
                return Randomizer.CurrentInstance.GetPlayerDataInt(name);
            }
            else
            {
                // vanilla
                // TODO: why is this _ prefix stripping here?
                if (name.StartsWith("_")) name = name.Substring(1);
                return PlayerData.instance.GetIntInternal(name);
            }
        }
        public static void SetPlayerDataInt(string name, int value)
        {
            if (Randomizer.CurrentInstance != null)
            {
                // randomizer
                Randomizer.CurrentInstance.SetPlayerDataInt(name, value);
            }
            else
            {
                // vanilla
                PlayerData.instance.SetIntInternal(name, value);
            }
        }
        public static bool GetPlayerDataBool(string name)
        {
            if (Randomizer.CurrentInstance != null)
            {
                // randomizer
                return Randomizer.CurrentInstance.GetPlayerDataBool(name);
            }
            else
            {
                // vanilla
                return PlayerData.instance.GetBoolInternal(name);
            }
        }
        public static void SetPlayerDataBool(string name, bool value)
        {
            if (Randomizer.CurrentInstance != null)
            {
                // randomizer
                Randomizer.CurrentInstance.SetPlayerDataBool(name, value);
            }
            else
            {
                // vanilla
                PlayerData.instance.SetBoolInternal(name, value);
            }
        }
        public static void LoadGame(int profileId)
        {
            Log("LoadGame()");
            bool permadeath = PlayerData.instance.permadeathMode > 0;
            GUIController.loadedSave = true;

            try
            {
                if (!File.Exists(saveFileLocation(profileId))) return;
                {
                    using (StreamReader streamReader = new StreamReader(saveFileLocation(profileId)))
                    {
                        Log("Loading from: " + saveFileLocation(profileId));
                        Randomizer.CurrentInstance = Randomizer.LoadFrom(streamReader, permadeath);
                    }
                }
            }
            catch (Exception e)
            {
                Warning("failed to load randomizer data", e);
            }
        }
        public static void SaveGame(int profileId)
        {
            Log("SaveGame()");
            if (Randomizer.CurrentInstance != null)
            {
                using (StreamWriter streamWriter = new StreamWriter(saveFileLocation(profileId)))
                {
                    Log("Saving to: " + saveFileLocation(profileId));
                    Randomizer.CurrentInstance.SaveGame(streamWriter);
                }
            }
        }
        public static void NewGame()
        {
            Log("NewGame()");
            bool permadeath = PlayerData.instance.permadeathMode > 0;
            Randomizer.CurrentInstance = Randomizer.Randomize(GUIController.seed, GUIController.hardMode, permadeath);
        }
        public static void DeleteGame(int profileId)
        {
            Log("DeleteGame()");
            if (File.Exists(saveFileLocation(profileId)))
            {
                Log("Deleting: " + saveFileLocation(profileId));
                File.Delete(saveFileLocation(profileId));
            }
        }
        private static string saveFileLocation(int profileId)
        {
            return Application.persistentDataPath + @"\user" + profileId + ".rnd";
        }

        public static void Warning(string message, Exception e)
        {
            Warning(message + ":\n" + e.ToString());
        }
        public static void Warning(string message)
        {
            Log("WARNING: " + message);
        }
        public static void Log(string message)
        {
            Modding.ModHooks.ModLog("[RANDOMIZER] " + message);
        }
    }
}
