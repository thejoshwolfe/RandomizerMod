using System;
using System.Diagnostics;

namespace RandomizerMod
{
    public static class LanguageHandler
    {
        public static string Get(string key, string sheet)
        {
            Randomizer randomizer = Randomizer.CurrentInstance;
            if (randomizer == null)
            {
                // vanilla
                return Language.Language.GetInternal(key, sheet);
            }

            //Experimental change, keeping the previous check commented here in case we need it back
            //string stack = new StackTrace().ToString();
            //if (!stack.Contains("at HutongGames.PlayMaker.Fsm.DoTransition(HutongGames.PlayMaker.FsmTransition transition, Boolean isGlobal)") && (!stack.Contains("at HutongGames.PlayMaker.Fsm.UpdateState(HutongGames.PlayMaker.FsmState state)") || key.Contains("CHARM_NAME_") || key.Contains("INV_NAME_TRINKET")))
            if (!Randomizer.InInventory())
            {
                //Switch locales based on loaded XML data
                RandomizerEntry pickup;
                if (RandomizerEntries.localeNameToEntry.TryGetValue(sheet + "." + key, out pickup))
                {
                    RandomizerEntry randomizedEntry;

                    if (randomizer.permutation.TryGetValue(pickup, out randomizedEntry))
                    {
                        //Check to make sure we're not showing the player shade cloak when we don't intend to give it to them
                        if (randomizedEntry == RandomizerEntries.ShadeCloak && !PlayerData.instance.hasDash)
                        {
                            randomizedEntry = RandomizerEntries.MothwingCloak;
                        }

                        //Similar checks for dream nail
                        if (randomizedEntry == RandomizerEntries.DreamGate && !PlayerData.instance.hasDreamNail)
                        {
                            randomizedEntry = RandomizerEntries.DreamNail;
                        }

                        if (randomizedEntry == RandomizerEntries.AwokenDreamNail && !PlayerData.instance.hasDreamNail)
                        {
                            randomizedEntry = RandomizerEntries.DreamNail;
                        }

                        //Similar checks for spells
                        if (randomizedEntry == RandomizerEntries.ShadeSoul && (randomizer._fireball1 + randomizer._fireball2) == 0)
                        {
                            randomizedEntry = RandomizerEntries.VengefulSpirit;
                        }

                        if (randomizedEntry == RandomizerEntries.DescendingDark && (randomizer._quake1 + randomizer._quake2) == 0)
                        {
                            randomizedEntry = RandomizerEntries.DesolateDive;
                        }

                        if (randomizedEntry == RandomizerEntries.AbyssShriek && (randomizer._scream1 + randomizer._scream2) == 0)
                        {
                            randomizedEntry = RandomizerEntries.HowlingWraiths;
                        }

                        string[] switchedLocale = randomizedEntry.localeNames[0].Split('.');
                        return Language.Language.GetInternal(switchedLocale[1], switchedLocale[0]);
                    }
                }
            }

            //Ruins1_05b is Lemm's room, checking to make sure we don't randomize his UI strings
            if (key.Contains("INV_NAME_TRINKET") && !Randomizer.InInventory() && GameManager.instance.GetSceneNameString() != "Ruins1_05b")
            {
                return Language.Language.GetInternal("INV_NAME_TRINKET" + randomizer.GetTrinketForScene(), sheet);
            }

            return Language.Language.GetInternal(key, sheet);
        }
    }
}
