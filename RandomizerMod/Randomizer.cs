using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Web.Extensions;
using UnityEngine;
using HutongGames.PlayMaker;
using GlobalEnums;

namespace RandomizerMod
{
    public class Randomizer
    {
        public static Randomizer CurrentInstance = null;

        public Dictionary<RandomizerEntry, RandomizerEntry> permutation = new Dictionary<RandomizerEntry, RandomizerEntry>();

        private bool swappedCloak = false;
        private bool swappedGate = false;
        private bool swappedAwoken = false;
        private bool swappedFireball = false;
        private bool swappedQuake = false;
        private bool swappedScream = false;

        public int _fireball1 = 0;
        public int _fireball2 = 0;
        public int _quake1 = 0;
        public int _quake2 = 0;
        public int _scream1 = 0;
        public int _scream2 = 0;

        private int seed;
        private bool hardMode;
        private bool permadeath;

        private Randomizer(int seed, bool hardMode, bool permadeath)
        {
            this.seed = seed;
            this.hardMode = hardMode;
            this.permadeath = permadeath;
            RandomizerMod.Log("Randomizer settings: seed:" + seed + (hardMode ? " hard" : "") + (permadeath ? " permadeath" : ""));
        }

        public static Randomizer Randomize(int seed, bool hardMode, bool permadeath)
        {
            Randomizer randomizer = new Randomizer(seed, hardMode, permadeath);
            randomizer.Randomize();
            return randomizer;
        }
        public void Randomize()
        {
            System.Random rand = new System.Random(seed);

            HashSet<RandomizerEntry> haveItems = new HashSet<RandomizerEntry>();
            if (hardMode) haveItems.Add(RandomizerEntries.HARD);
            if (!permadeath) haveItems.Add(RandomizerEntries.CLASSIC);

            List<RandomizerEntry> yetToPlaceItems = new List<RandomizerEntry>(RandomizerEntries.randomizedEntries);
            List<RandomizerEntry> yetToFindLocations = new List<RandomizerEntry>(RandomizerEntries.randomizedEntries);

            RandomizerMod.Log("=============================");
            RandomizerMod.Log("==== Randomization start ====");
            RandomizerMod.Log("=============================");

            while (yetToPlaceItems.Count > 0)
            {
                // find new locations
                List<RandomizerEntry> newlyReachableLocations = new List<RandomizerEntry>();
                for (int i = 0; i < yetToFindLocations.Count; i++)
                {
                    RandomizerEntry location = yetToFindLocations[i];
                    if (location.ReachableWith(haveFuncWithItems(haveItems)))
                    {
                        RandomizerMod.Log("reachable: " + location);
                        newlyReachableLocations.Add(location);
                        swapAndPop(yetToFindLocations, i);
                        i--;
                    }
                }
                int groupSize = newlyReachableLocations.Count;
                if (groupSize == 0) throw randomizationFailed(yetToFindLocations, yetToPlaceItems);

                // Choose random items to place in these locations.
                List<RandomizerEntry> newItems = new List<RandomizerEntry>();
                for (int i = 0; i < groupSize; i++)
                {
                    int index;
                    if (i == groupSize - 1 && yetToFindLocations.Count > 0)
                    {
                        // We'd better have chosen some items that let us progress.
                        HashSet<RandomizerEntry> wouldBeItems = new HashSet<RandomizerEntry>(haveItems);
                        wouldBeItems.UnionWith(newItems);

                        bool foundNewLocations = false;
                        foreach (RandomizerEntry location in yetToFindLocations)
                        {
                            if (location.ReachableWith(haveFuncWithItems(wouldBeItems)))
                            {
                                foundNewLocations = true;
                                break;
                            }
                        }
                        if (!foundNewLocations)
                        {
                            // Uh oh. We have to choose a progression item to place into this group.
                            RandomizerMod.Log("forcing progression item into this group");

                            List<int> availableProgressionItemIndexes = new List<int>();
                            for (int j = 0; j < yetToPlaceItems.Count; j++)
                            {
                                RandomizerEntry item = yetToPlaceItems[j];
                                wouldBeItems.Add(item);
                                foreach (RandomizerEntry location in yetToFindLocations)
                                {
                                    if (location.ReachableWith(haveFuncWithItems(wouldBeItems)))
                                    {
                                        RandomizerMod.Log("available progression item: " + item);
                                        availableProgressionItemIndexes.Add(j);
                                        break;
                                    }
                                }
                                wouldBeItems.Remove(item);
                            }
                            // Theoretically possible to fail, but probably not with the current requirements database.
                            if (availableProgressionItemIndexes.Count == 0) throw randomizationFailed(yetToFindLocations, yetToPlaceItems);

                            index = availableProgressionItemIndexes[rand.Next(availableProgressionItemIndexes.Count)];
                            newItems.Add(swapAndPop(yetToPlaceItems, index));
                            continue;
                        }
                    }

                    // Select some random item.
                    index = rand.Next(yetToPlaceItems.Count);
                    newItems.Add(swapAndPop(yetToPlaceItems, index));
                }

                // Now randomly assign each of these new items a location
                for (int i = 0; i < groupSize; i++)
                {
                    RandomizerEntry location = newlyReachableLocations[i];
                    int randomIndex = rand.Next(newItems.Count);
                    RandomizerEntry item = swapAndPop(newItems, randomIndex);
                    RandomizerMod.Log("Assigning location: " + location + " => " + item + (item.isSignificant ? "    <------------------" : ""));
                    permutation.Add(location, item);
                    haveItems.Add(item);
                }
            }
            RandomizerMod.Log("================================");
            RandomizerMod.Log("==== Randomization complete ====");
            RandomizerMod.Log("================================");
        }
        private static Predicate<RandomizerEntry> haveFuncWithItems(HashSet<RandomizerEntry> items)
        {
            Predicate<RandomizerEntry> haveFunc = null;
            haveFunc = (entry) =>
            {
                if (entry is EntryGroup) return entry.ReachableWith(haveFunc);
                return items.Contains(entry);
            };
            return haveFunc;
        }
        private static T swapAndPop<T>(List<T> list, int index)
        {
            T result = list[index];
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return result;
        }
        private Exception randomizationFailed(List<RandomizerEntry> yetToFindLocations, List<RandomizerEntry> yetToPlaceItems)
        {
            RandomizerMod.Warning("Randomization failed with " + yetToFindLocations.Count + "," + yetToPlaceItems.Count + " outstanding locations,items");
            foreach (RandomizerEntry location in yetToFindLocations)
            {
                RandomizerMod.Log("Never reached location: " + location);
            }
            foreach (RandomizerEntry item in yetToPlaceItems)
            {
                RandomizerMod.Log("Never placed item: " + item);
            }
            return new Exception("Randomization failed");
        }

        public int GetPlayerDataInt(string name)
        {
            if (string.IsNullOrEmpty(name)) return 0;
            if (name == "_true") return 2;
            if (name == "_fireballLevel") return _fireball1 + _fireball2;
            if (name == "_quakeLevel") return _quake1 + _quake2;
            if (name == "_screamLevel") return _scream1 + _scream2;

            RandomizerEntry requestedEntry;
            RandomizerEntry randomizedEntry;

            if (RandomizerEntries.varNameToEntry.TryGetValue(name, out requestedEntry) && permutation.TryGetValue(requestedEntry, out randomizedEntry))
            {
                // randomized

                int index = requestedEntry.GetVarNameIndex(name);

                //Return the matching var or the first one if there is no matching index
                RandomizerVar var = randomizedEntry.varNames[index < randomizedEntry.varNames.Length ? index : 0];

                if (var.type == typeof(bool)) return PlayerData.instance.GetBoolInternal(var.name) ? 2 : 0;

                if (randomizedEntry == RandomizerEntries.VengefulSpirit) return _fireball1 * 2;
                if (randomizedEntry == RandomizerEntries.ShadeSoul) return _fireball2 * 2;
                if (randomizedEntry == RandomizerEntries.DesolateDive) return _quake1 * 2;
                if (randomizedEntry == RandomizerEntries.DescendingDark) return _quake2 * 2;
                if (randomizedEntry == RandomizerEntries.HowlingWraiths) return _scream1 * 2;
                if (randomizedEntry == RandomizerEntries.AbyssShriek) return _scream2 * 2;

                return PlayerData.instance.GetIntInternal(var.name) >= (int)var.value ? 2 : 0;
            }
            else
            {
                // some vanilla thing
                return PlayerData.instance.GetIntInternal(name);
            }
        }

        public void SetPlayerDataInt(string name, int value)
        {
            if (string.IsNullOrEmpty(name)) return;

            string nameVal = name + value;

            if (name == "trinket1" || name == "trinket2" || name == "trinket3" || name == "trinket4")
            {
                //It would be cleaner to override this in SetPlayerDataBool, but this works just as well
                PlayerData.instance.SetBoolInternal("foundTrinket1", true);
                PlayerData.instance.SetBoolInternal("foundTrinket2", true);
                PlayerData.instance.SetBoolInternal("foundTrinket3", true);
                PlayerData.instance.SetBoolInternal("foundTrinket4", true);
                PlayerData.instance.SetBoolInternal("noTrinket1", false);
                PlayerData.instance.SetBoolInternal("noTrinket2", false);
                PlayerData.instance.SetBoolInternal("noTrinket3", false);
                PlayerData.instance.SetBoolInternal("noTrinket4", false);

                //Make sure the change is +1 so we don't randomize selling trinkets to Lemm
                int change = value - PlayerData.instance.GetIntInternal(name);

                if (change != 1)
                {
                    PlayerData.instance.SetIntInternal(name, value);
                    return;
                }

                int trinketNum = GetTrinketForScene();

                PlayerData.instance.SetIntInternal("trinket" + trinketNum, PlayerData.instance.GetIntInternal("trinket" + trinketNum) + 1);
                return;
            }

            RandomizerEntry requestedEntry;
            RandomizerEntry randomizedEntry;

            //Check if var is in data before running randomizer code
            if (RandomizerEntries.varNameToEntry.TryGetValue(nameVal, out requestedEntry) && permutation.TryGetValue(requestedEntry, out randomizedEntry))
            {
                //Randomizer breaks progression, so we need to ensure the player never gets shade cloak before mothwing cloak
                if (randomizedEntry == RandomizerEntries.ShadeCloak && !PlayerData.instance.hasDash && !PlayerData.instance.canDash)
                {
                    Swap(RandomizerEntries.ShadeCloak, RandomizerEntries.MothwingCloak);
                    randomizedEntry = RandomizerEntries.MothwingCloak;
                    swappedCloak = true;
                }

                //Similar checks for dream nail
                if (randomizedEntry == RandomizerEntries.DreamGate && !PlayerData.instance.hasDreamNail)
                {
                    Swap(RandomizerEntries.DreamNail, RandomizerEntries.DreamGate);
                    randomizedEntry = RandomizerEntries.DreamNail;
                    swappedGate = true;
                }

                if (randomizedEntry == RandomizerEntries.AwokenDreamNail && !PlayerData.instance.hasDreamNail)
                {
                    Swap(RandomizerEntries.DreamNail, RandomizerEntries.AwokenDreamNail);
                    randomizedEntry = RandomizerEntries.DreamNail;
                    swappedAwoken = true;
                }

                //Similar checks for spells
                if (randomizedEntry == RandomizerEntries.ShadeSoul && (_fireball1 + _fireball2) == 0)
                {
                    Swap(RandomizerEntries.VengefulSpirit, RandomizerEntries.ShadeSoul);
                    randomizedEntry = RandomizerEntries.VengefulSpirit;
                    swappedFireball = true;
                }

                if (randomizedEntry == RandomizerEntries.DescendingDark && (_quake1 + _quake2) == 0)
                {
                    Swap(RandomizerEntries.DesolateDive, RandomizerEntries.DescendingDark);
                    randomizedEntry = RandomizerEntries.DesolateDive;
                    swappedQuake = true;
                }

                if (randomizedEntry == RandomizerEntries.AbyssShriek && (_scream1 + _scream2) == 0)
                {
                    Swap(RandomizerEntries.HowlingWraiths, RandomizerEntries.AbyssShriek);
                    randomizedEntry = RandomizerEntries.HowlingWraiths;
                    swappedScream = true;
                }

                //FSM variable is probably tracked separately, need to make sure it's accurate
                if (name == "hasDreamGate" && !PlayerData.instance.hasDreamGate)
                {
                    FSMUtility.LocateFSM(HeroController.instance.gameObject, "Dream Nail").FsmVariables.GetFsmBool("Dream Warp Allowed").Value = false;
                }

                //Set all bools relating to the given entry
                for (int i = 0; i < randomizedEntry.varNames.Length; i++)
                {
                    RandomizerVar var = randomizedEntry.varNames[i];

                    if (var.type == typeof(bool))
                    {
                        PlayerData.instance.SetBoolInternal(var.name, value > 0);
                    }
                    else
                    {
                        if (randomizedEntry == RandomizerEntries.VengefulSpirit) _fireball1 = value > 0 ? 1 : 0;
                        else if (randomizedEntry == RandomizerEntries.ShadeSoul) _fireball2 = value > 0 ? 1 : 0;
                        else if (randomizedEntry == RandomizerEntries.DesolateDive) _quake1 = value > 0 ? 1 : 0;
                        else if (randomizedEntry == RandomizerEntries.DescendingDark) _quake2 = value > 0 ? 1 : 0;
                        else if (randomizedEntry == RandomizerEntries.HowlingWraiths) _scream1 = value > 0 ? 1 : 0;
                        else if (randomizedEntry == RandomizerEntries.AbyssShriek) _scream2 = value > 0 ? 1 : 0;
                    }

                    //FSM variable is probably tracked separately, need to make sure it's accurate
                    if (randomizedEntry.varNames[i].name == "hasDreamGate")
                    {
                        FSMUtility.LocateFSM(HeroController.instance.gameObject, "Dream Nail").FsmVariables.GetFsmBool("Dream Warp Allowed").Value = true;
                    }

                    //Need to make the charms page accessible if the player gets their first charm from a non-charm pickup
                    if (randomizedEntry.type == RandomizerType.CHARM && requestedEntry.type != RandomizerType.CHARM) PlayerData.instance.hasCharm = true;
                }
                return;
            }

            PlayerData.instance.SetIntInternal(name, value);
        }

        //Randomize trinkets based on scene name and seed
        public int GetTrinketForScene()
        {
            //Adding all chars from scene name to seed works well enough because there's only two places with multiple trinkets in a scene
            char[] sceneCharArray = GameManager.instance.GetSceneNameString().ToCharArray();
            int[] sceneNumbers = sceneCharArray.Select(c => Convert.ToInt32(c)).ToArray();

            int modifiedSeed = seed;

            for (int i = 0; i < sceneNumbers.Length; i++)
            {
                modifiedSeed += sceneNumbers[i];
            }

            //Total trinket count: 14 / 16 / 7 / 4, using those values to get mostly accurate randomization, rather than truely moving them around
            int trinketNum = new System.Random(modifiedSeed).Next(1, 42);

            if (trinketNum <= 14) return 1;
            if (trinketNum <= 14 + 16) return 2;
            if (trinketNum <= 14 + 16 + 7) return 3;
            return 4;
        }

        //Override for PlayerData.GetBool
        public bool GetPlayerDataBool(string name)
        {
            if (GameManager.instance.GetSceneNameString() != "RestingGrounds_07" && GameManager.instance.GetSceneNameString() != "RestingGrounds_04")
            {
                if (name == "hasDreamGate" || name == "dreamNailUpgraded" || name == "hasDreamNail")
                {
                    return PlayerData.instance.GetBoolInternal(name);
                }
            }

            if (string.IsNullOrEmpty(name)) return false;

            if (name == "_true")
            {
                return true;
            }
            else if (name == "_false")
            {
                return false;
            }
            else if (name == "hasAcidArmour")
            {
                return GameManager.instance.GetSceneNameString() == "Waterways_13" ? false : PlayerData.instance.hasAcidArmour;
            }

            //Check stack trace to see if player is in a menu
            string stack = new StackTrace().ToString();

            //Split into multiple ifs because this looks horrible otherwise
            //TODO: Cleaner way of checking than stack trace
            if (!stack.Contains("at ShopMenuStock.BuildItemList()"))
            {
                if (stack.Contains("at HutongGames.PlayMaker.Fsm.Start()"))
                {
                    return PlayerData.instance.GetBoolInternal(name);
                }

                if (name.Contains("gotCharm_") && (stack.Contains("at HutongGames.PlayMaker.Fsm.DoTransition(HutongGames.PlayMaker.FsmTransition transition, Boolean isGlobal)") || InInventory()))
                {
                    return PlayerData.instance.GetBoolInternal(name);
                }
            }


            RandomizerEntry requestedEntry;
            RandomizerEntry randomizedEntry;

            //Don't run randomizer if bool is not in the loaded data
            if (RandomizerEntries.varNameToEntry.TryGetValue(name, out requestedEntry) && permutation.TryGetValue(requestedEntry, out randomizedEntry))
            {
                int index = requestedEntry.GetVarNameIndex(name);

                RandomizerVar var;

                //Return the matching bool or the first one if there is no matching index
                if (randomizedEntry.varNames.Length > index)
                {
                    var = randomizedEntry.varNames[index];
                }
                else
                {
                    var = randomizedEntry.varNames[0];
                }

                if (var.type == typeof(bool))
                {
                    return PlayerData.instance.GetBoolInternal(var.name);
                }
                else
                {
                    if (randomizedEntry == RandomizerEntries.VengefulSpirit) return _fireball1 > 0;
                    if (randomizedEntry == RandomizerEntries.ShadeSoul) return _fireball2 > 0;
                    if (randomizedEntry == RandomizerEntries.DesolateDive) return _quake1 > 0;
                    if (randomizedEntry == RandomizerEntries.DescendingDark) return _quake2 > 0;
                    if (randomizedEntry == RandomizerEntries.HowlingWraiths) return _scream1 > 0;
                    if (randomizedEntry == RandomizerEntries.AbyssShriek) return _scream2 > 0;

                    return PlayerData.instance.GetIntInternal(var.name) >= (int)var.value;
                }
            }
            else
            {
                return PlayerData.instance.GetBoolInternal(name);
            }
        }

        //Override for PlayerData.SetBool
        public void SetPlayerDataBool(string name, bool val)
        {
            if (string.IsNullOrEmpty(name)) return;

            RandomizerEntry requestedEntry;
            RandomizerEntry randomizedEntry;

            //Check if bool is in data before running randomizer code
            if (RandomizerEntries.varNameToEntry.TryGetValue(name, out requestedEntry) && permutation.TryGetValue(requestedEntry, out randomizedEntry))
            {
                //Randomizer breaks progression, so we need to ensure the player never gets shade cloak before mothwing cloak
                if (randomizedEntry == RandomizerEntries.ShadeCloak && !PlayerData.instance.hasDash && !PlayerData.instance.canDash)
                {
                    Swap(RandomizerEntries.ShadeCloak, RandomizerEntries.MothwingCloak);
                    randomizedEntry = RandomizerEntries.MothwingCloak;
                    swappedCloak = true;
                }

                //Similar checks for dream nail
                if (randomizedEntry == RandomizerEntries.DreamGate && !PlayerData.instance.hasDreamNail)
                {
                    Swap(RandomizerEntries.DreamNail, RandomizerEntries.DreamGate);
                    randomizedEntry = RandomizerEntries.DreamNail;
                    swappedGate = true;
                }

                if (randomizedEntry == RandomizerEntries.AwokenDreamNail && !PlayerData.instance.hasDreamNail)
                {
                    Swap(RandomizerEntries.DreamNail, RandomizerEntries.AwokenDreamNail);
                    randomizedEntry = RandomizerEntries.DreamNail;
                    swappedAwoken = true;
                }

                //Similar checks for spells
                if (randomizedEntry == RandomizerEntries.ShadeSoul && (_fireball1 + _fireball2) == 0)
                {
                    Swap(RandomizerEntries.VengefulSpirit, RandomizerEntries.ShadeSoul);
                    randomizedEntry = RandomizerEntries.VengefulSpirit;
                    swappedFireball = true;
                }

                if (randomizedEntry == RandomizerEntries.DescendingDark && (_quake1 + _quake2) == 0)
                {
                    Swap(RandomizerEntries.DesolateDive, RandomizerEntries.DescendingDark);
                    randomizedEntry = RandomizerEntries.DesolateDive;
                    swappedQuake = true;
                }

                if (randomizedEntry == RandomizerEntries.AbyssShriek && (_scream1 + _scream2) == 0)
                {
                    Swap(RandomizerEntries.HowlingWraiths, RandomizerEntries.AbyssShriek);
                    randomizedEntry = RandomizerEntries.HowlingWraiths;
                    swappedScream = true;
                }

                //FSM variable is probably tracked separately, need to make sure it's accurate
                if (name == "hasDreamGate" && !PlayerData.instance.hasDreamGate)
                {
                    FSMUtility.LocateFSM(HeroController.instance.gameObject, "Dream Nail").FsmVariables.GetFsmBool("Dream Warp Allowed").Value = false;
                }

                //Set all bools relating to the given entry
                for (int i = 0; i < randomizedEntry.varNames.Length; i++)
                {
                    RandomizerVar var = randomizedEntry.varNames[i];

                    if (var.type == typeof(bool))
                    {
                        PlayerData.instance.SetBoolInternal(var.name, val);
                    }
                    else
                    {
                        if (randomizedEntry == RandomizerEntries.VengefulSpirit) _fireball1 = val ? 1 : 0;
                        else if (randomizedEntry == RandomizerEntries.ShadeSoul) _fireball2 = val ? 1 : 0;
                        else if (randomizedEntry == RandomizerEntries.DesolateDive) _quake1 = val ? 1 : 0;
                        else if (randomizedEntry == RandomizerEntries.DescendingDark) _quake2 = val ? 1 : 0;
                        else if (randomizedEntry == RandomizerEntries.HowlingWraiths) _scream1 = val ? 1 : 0;
                        else if (randomizedEntry == RandomizerEntries.AbyssShriek) _scream2 = val ? 1 : 0;
                    }

                    //FSM variable is probably tracked separately, need to make sure it's accurate
                    if (randomizedEntry.varNames[i].name == "hasDreamGate")
                    {
                        FSMUtility.LocateFSM(HeroController.instance.gameObject, "Dream Nail").FsmVariables.GetFsmBool("Dream Warp Allowed").Value = true;
                    }

                    //Need to make the charms page accessible if the player gets their first charm from a non-charm pickup
                    if (randomizedEntry.type == RandomizerType.CHARM && requestedEntry.type != RandomizerType.CHARM) PlayerData.instance.hasCharm = true;
                }
            }
            else
            {
                PlayerData.instance.SetBoolInternal(name, val);
            }
        }

        //Swap two given entries
        private void Swap(RandomizerEntry entry1, RandomizerEntry entry2)
        {
            try
            {
                RandomizerEntry key = permutation.FirstOrDefault((x) => x.Value == entry1).Key;
                RandomizerEntry key2 = permutation.FirstOrDefault((x) => x.Value == entry2).Key;
                permutation[key] = entry2;
                permutation[key2] = entry1;
            }
            catch (Exception e)
            {
                RandomizerMod.Warning("Could not swap entries " + entry1 + " and " + entry2, e);
            }
        }

        //TODO: Hook GameManager.SaveGame to write save to the same file as everything else
        public void SaveGame(StreamWriter streamWriter)
        {
            streamWriter.WriteLine(seed);
            streamWriter.WriteLine(hardMode);
            streamWriter.WriteLine(swappedCloak);
            streamWriter.WriteLine(swappedGate);
            streamWriter.WriteLine(swappedAwoken);
            streamWriter.WriteLine(swappedFireball);
            streamWriter.WriteLine(swappedQuake);
            streamWriter.WriteLine(swappedScream);
            streamWriter.WriteLine(_fireball1);
            streamWriter.WriteLine(_fireball2);
            streamWriter.WriteLine(_quake1);
            streamWriter.WriteLine(_quake2);
            streamWriter.WriteLine(_scream1);
            streamWriter.WriteLine(_scream2);
        }

        //Load randomizer save from file if applicable
        public static Randomizer LoadFrom(StreamReader streamReader, bool permadeath)
        {
            int seed = Convert.ToInt32(streamReader.ReadLine());
            bool hardMode = Convert.ToBoolean(streamReader.ReadLine());
            Randomizer randomizer = new Randomizer(seed, hardMode, permadeath);
            randomizer.Randomize();

            randomizer.swappedCloak = Convert.ToBoolean(streamReader.ReadLine());
            randomizer.swappedGate = Convert.ToBoolean(streamReader.ReadLine());
            randomizer.swappedAwoken = Convert.ToBoolean(streamReader.ReadLine());
            randomizer.swappedFireball = Convert.ToBoolean(streamReader.ReadLine());
            randomizer.swappedQuake = Convert.ToBoolean(streamReader.ReadLine());
            randomizer.swappedScream = Convert.ToBoolean(streamReader.ReadLine());
            randomizer._fireball1 = Convert.ToInt32(streamReader.ReadLine());
            randomizer._fireball2 = Convert.ToInt32(streamReader.ReadLine());
            randomizer._quake1 = Convert.ToInt32(streamReader.ReadLine());
            randomizer._quake2 = Convert.ToInt32(streamReader.ReadLine());
            randomizer._scream1 = Convert.ToInt32(streamReader.ReadLine());
            randomizer._scream2 = Convert.ToInt32(streamReader.ReadLine());

            // the player may have already gotten some progressive items and triggered swaps.
            if (randomizer.swappedCloak) randomizer.Swap(RandomizerEntries.MothwingCloak, RandomizerEntries.ShadeCloak);
            if (randomizer.swappedGate) randomizer.Swap(RandomizerEntries.DreamNail, RandomizerEntries.DreamGate);
            if (randomizer.swappedAwoken) randomizer.Swap(RandomizerEntries.DreamNail, RandomizerEntries.AwokenDreamNail);
            if (randomizer.swappedFireball) randomizer.Swap(RandomizerEntries.VengefulSpirit, RandomizerEntries.ShadeSoul);
            if (randomizer.swappedQuake) randomizer.Swap(RandomizerEntries.DesolateDive, RandomizerEntries.DescendingDark);
            if (randomizer.swappedScream) randomizer.Swap(RandomizerEntries.HowlingWraiths, RandomizerEntries.AbyssShriek);

            return randomizer;
        }

        //Checks if player is in inventory
        //None of this should ever be null, but checking just in case
        public static bool InInventory()
        {
            GameObject invTop = GameObject.FindGameObjectWithTag("Inventory Top");

            if (invTop != null)
            {
                PlayMakerFSM invFSM = invTop.GetComponent<PlayMakerFSM>();

                if (invFSM != null)
                {
                    FsmBool invOpen = invFSM.FsmVariables.GetFsmBool("Open");

                    if (invOpen != null)
                    {
                        return invOpen.Value;
                    }
                }
            }
            return false;
        }
    }
}
