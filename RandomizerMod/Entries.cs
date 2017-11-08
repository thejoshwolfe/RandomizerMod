using System;
using System.Collections.Generic;
using System.Reflection;

namespace RandomizerMod
{
    public static class RandomizerEntries
    {
        public static List<RandomizerEntry> randomizedEntries = new List<RandomizerEntry>();
        public static Dictionary<string, RandomizerEntry> varNameToEntry = new Dictionary<string, RandomizerEntry>();
        public static Dictionary<string, RandomizerEntry> localeNameToEntry = new Dictionary<string, RandomizerEntry>();

        public static RandomizerEntry HARD = new PseudoEntry("HARD");
        public static RandomizerEntry CLASSIC = new PseudoEntry("CLASSIC");

        public static RandomizerEntry FIREBALL = new EntryGroup((have) => have(VengefulSpirit) || have(ShadeSoul));
        public static RandomizerEntry QUAKE = new EntryGroup((have) => have(DesolateDive) || have(DescendingDark));
        public static RandomizerEntry DASH_CLOAK = new EntryGroup((have) => have(MothwingCloak) || have(ShadeCloak));
        public static RandomizerEntry SHADE_CLOAK = new EntryGroup((have) => have(MothwingCloak) && have(ShadeCloak));
        public static RandomizerEntry SCREAM = new EntryGroup((have) => have(HowlingWraiths) || have(AbyssShriek));
        public static RandomizerEntry DREAM_NAIL = new EntryGroup((have) => have(DreamNail) || have(DreamGate) || have(AwokenDreamNail));

        public static RandomizerEntry WIN = new EntryGroup((have) =>
            // Herra
            have(DREAM_NAIL) && have(MantisClaw) &&
            // Monomon
            (
                have(IsmasTear) ||
                (have(SHADE_CLOAK) && have(CrystalHeart)) ||
                (have(HARD) && have(CrystalHeart))
            ) &&
            // Lurrien
            (
                have(MonarchWings) || have(HARD)
            )
        );
        public static RandomizerEntry REACH_EVERYTHING = new EntryGroup((have) =>
            have(SHADE_CLOAK) &&
            have(MantisClaw) &&
            have(MonarchWings) &&
            have(IsmasTear) &&
            have(CrystalHeart) &&
            have(DREAM_NAIL) &&
            have(SCREAM) &&
            have(QUAKE) &&
            have(FIREBALL)
        );

        public static RandomizerEntry PASS_BALDURS = new EntryGroup((have) =>
            have(FIREBALL) ||
            have(GrubberflysElegy) ||
            have(GlowingWomb) ||
            have(SporeShroom) ||
            have(QUAKE) ||
            have(Weaversong) ||
            (have(HARD) && have(MarkofPride))
        );
        public static RandomizerEntry REACH_SALUBRA = new EntryGroup((have) =>
            have(MantisClaw) ||
            have(DASH_CLOAK) ||
            have(MonarchWings) ||
            have(CrystalHeart) ||
            (have(HARD) && have(FIREBALL)) ||
            (have(HARD) && have(CLASSIC))
        );
        public static RandomizerEntry REACH_LEG_EATER = new EntryGroup((have) =>
            have(MantisClaw) ||
            have(MonarchWings) ||
            have(DASH_CLOAK) ||
            have(IsmasTear) ||
            (have(HARD) && have(FIREBALL)) ||
            (have(CrystalHeart) && have(PASS_BALDURS))
        );

        public static RandomizerEntry MothwingCloak = new RandomizerEntry(
            "MothwingCloak", true,
            new RandomizerVar[] { new RandomizerVar("hasDash", typeof(bool), null), new RandomizerVar("canDash", typeof(bool), null) },
            (have) => have(FIREBALL) || have(IsmasTear) || have(GrubberflysElegy) || have(DASH_CLOAK) || have(MantisClaw) || have(MonarchWings) || have(GlowingWomb) || have(SporeShroom) || have(Weaversong) || (have(MarkofPride) && have(HARD)),
            RandomizerType.ABILITY,
            new string[] { "UI.INV_NAME_DASH", "Prompts.GET_ITEM_INTRO1", "Prompts.BUTTON_DESC_PRESS", "Prompts.GET_DASH_1", "Prompts.GET_DASH_2" }
        );
        public static RandomizerEntry MantisClaw = new RandomizerEntry(
            "MantisClaw", true,
            new RandomizerVar[] { new RandomizerVar("hasWalljump", typeof(bool), null), new RandomizerVar("canWallJump", typeof(bool), null) },
            (have) => have(DASH_CLOAK) || have(MonarchWings) || have(MantisClaw) || have(IsmasTear) || (have(FIREBALL) && have(HARD)) || (have(CrystalHeart) && (have(FIREBALL) || have(GrubberflysElegy) || have(GlowingWomb) || have(SporeShroom) || have(QUAKE) || have(Weaversong))),
            RandomizerType.ABILITY,
            new string[] { "UI.INV_NAME_WALLJUMP", "Prompts.GET_ITEM_INTRO1", "Prompts.BUTTON_DESC_PRESS", "Prompts.GET_WALLJUMP_1", "Prompts.GET_WALLJUMP_2" }
        );
        public static RandomizerEntry CrystalHeart = new RandomizerEntry(
            "CrystalHeart", true,
            new RandomizerVar[] { new RandomizerVar("hasSuperDash", typeof(bool), null), new RandomizerVar("canSuperDash", typeof(bool), null) },
            (have) => (have(MantisClaw) && (have(DASH_CLOAK) || have(CrystalHeart) || have(MonarchWings) || (have(FIREBALL) && have(HARD)))) || (have(MonarchWings) && (have(DASH_CLOAK) || have(FIREBALL)) && have(HARD) && have(CLASSIC)),
            RandomizerType.ABILITY,
            new string[] { "UI.INV_NAME_SUPERDASH", "Prompts.GET_ITEM_INTRO2", "Prompts.BUTTON_DESC_HOLD", "Prompts.GET_SUPERDASH_1", "Prompts.GET_SUPERDASH_2" }
        );
        public static RandomizerEntry MonarchWings = new RandomizerEntry(
            "MonarchWings", true,
            new RandomizerVar[] { new RandomizerVar("hasDoubleJump", typeof(bool), null) },
            (have) => (have(MonarchWings) && have(HARD)) || (have(MantisClaw) && (have(CrystalHeart) || have(MonarchWings))),
            RandomizerType.ABILITY,
            new string[] { "UI.INV_NAME_DOUBLEJUMP", "Prompts.GET_ITEM_INTRO2", "Prompts.BUTTON_DESC_PRESS", "Prompts.GET_DOUBLEJUMP_1", "Prompts.GET_DOUBLEJUMP_2" }
        );
        public static RandomizerEntry ShadeCloak = new RandomizerEntry(
            "ShadeCloak", true,
            new RandomizerVar[] { new RandomizerVar("hasShadowDash", typeof(bool), null), new RandomizerVar("canShadowDash", typeof(bool), null) },
            (have) =>
                (have(MantisClaw) && (have(MonarchWings) || (have(HARD) && (have(DASH_CLOAK) || have(CrystalHeart))))) ||
                (have(HARD) && have(SHADE_CLOAK) && have(MonarchWings)),
            RandomizerType.ABILITY,
            new string[] { "UI.INV_NAME_SHADOWDASH", "Prompts.GET_ITEM_INTRO7", "Prompts.BUTTON_DESC_PRESS", "Prompts.GET_SHADOWDASH_1", "Prompts.GET_SHADOWDASH_2" }
        );
        public static RandomizerEntry IsmasTear = new RandomizerEntry(
            "IsmasTear", true,
            new RandomizerVar[] { new RandomizerVar("hasAcidArmour", typeof(bool), null) },
            (have) => have(CrystalHeart) && (have(MantisClaw) || (have(MonarchWings) && have(IsmasTear))),
            RandomizerType.ABILITY,
            new string[] { "UI.INV_NAME_ACIDARMOUR" }
        );
        public static RandomizerEntry DreamNail = new RandomizerEntry(
            "DreamNail", true,
            new RandomizerVar[] { new RandomizerVar("hasDreamNail", typeof(bool), null) },
            (have) => have(MantisClaw) || have(HARD),
            RandomizerType.ABILITY,
            new string[] { "UI.INV_NAME_DREAMNAIL_A" }
        );
        public static RandomizerEntry DreamGate = new RandomizerEntry(
            "DreamGate", true,
            new RandomizerVar[] { new RandomizerVar("hasDreamGate", typeof(bool), null) },
            (have) => have(WIN),
            RandomizerType.ABILITY,
            new string[] { "UI.INV_NAME_DREAMGATE" }
        );
        public static RandomizerEntry AwokenDreamNail = new RandomizerEntry(
            "AwokenDreamNail", true,
            new RandomizerVar[] { new RandomizerVar("dreamNailUpgraded", typeof(bool), null) },
            (have) => have(WIN),
            RandomizerType.ABILITY,
            new string[] { "UI.INV_NAME_DREAMNAIL_B" }
        );
        public static RandomizerEntry VengefulSpirit = new RandomizerEntry(
            "VengefulSpirit", true,
            new RandomizerVar[] { new RandomizerVar("fireballLevel", typeof(int), 1) },
            (have) => true,
            RandomizerType.SPELL,
            new string[] { "UI.INV_NAME_SPELL_FIREBALL1" }
        );
        public static RandomizerEntry ShadeSoul = new RandomizerEntry(
            "ShadeSoul", true,
            new RandomizerVar[] { new RandomizerVar("fireballLevel", typeof(int), 2) },
            (have) => ((have(MantisClaw) || have(MonarchWings)) && have(HARD) && have(CLASSIC)) || (have(MantisClaw) && (have(DASH_CLOAK) || have(CrystalHeart) || have(MonarchWings) || have(IsmasTear))),
            RandomizerType.SPELL,
            new string[] { "UI.INV_NAME_SPELL_FIREBALL2" }
        );
        public static RandomizerEntry DesolateDive = new RandomizerEntry(
            "DesolateDive", true,
            new RandomizerVar[] { new RandomizerVar("quakeLevel", typeof(int), 1) },
            (have) => ((have(MantisClaw) || have(MonarchWings)) && have(HARD) && have(CLASSIC)) || (have(MantisClaw) && (have(DASH_CLOAK) || have(CrystalHeart) || have(MonarchWings) || have(IsmasTear))),
            RandomizerType.SPELL,
            new string[] { "UI.INV_NAME_SPELL_QUAKE1" }
        );
        public static RandomizerEntry DescendingDark = new RandomizerEntry(
            "DescendingDark", true,
            new RandomizerVar[] { new RandomizerVar("quakeLevel", typeof(int), 2) },
            (have) => have(QUAKE) && ((have(MantisClaw) && (have(CrystalHeart) || ((have(DASH_CLOAK) && have(FIREBALL)) && have(HARD)))) || (have(MonarchWings) && (have(CrystalHeart) || have(DASH_CLOAK) || (have(FIREBALL) && have(HARD))))),
            RandomizerType.SPELL,
            new string[] { "UI.INV_NAME_SPELL_QUAKE2" }
        );
        public static RandomizerEntry HowlingWraiths = new RandomizerEntry(
            "HowlingWraiths", true,
            new RandomizerVar[] { new RandomizerVar("screamLevel", typeof(int), 1) },
            (have) => have(MantisClaw) || have(MonarchWings),
            RandomizerType.SPELL,
            new string[] { "UI.INV_NAME_SPELL_SCREAM1" }
        );
        public static RandomizerEntry AbyssShriek = new RandomizerEntry(
            "AbyssShriek", true,
            new RandomizerVar[] { new RandomizerVar("screamLevel", typeof(int), 2) },
            (have) => (have(SCREAM)) && (((have(MantisClaw) || have(MonarchWings)) && have(HARD)) || (have(MantisClaw) && have(MonarchWings))),
            RandomizerType.SPELL,
            new string[] { "UI.INV_NAME_SPELL_SCREAM2" }
        );
        public static RandomizerEntry GatheringSwarm = new RandomizerEntry(
            "GatheringSwarm", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_1", typeof(bool), null) },
            (have) => true,
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_1" }
        );
        public static RandomizerEntry WaywardCompass = new RandomizerEntry(
            "WaywardCompass", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_2", typeof(bool), null) },
            (have) => true,
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_2" }
        );
        public static RandomizerEntry Grubsong = new RandomizerEntry(
            "Grubsong", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_3", typeof(bool), null) },
            (have) => have(WIN),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_3" }
        );
        public static RandomizerEntry StalwartShell = new RandomizerEntry(
            "StalwartShell", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_4", typeof(bool), null) },
            (have) => true,
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_4" }
        );
        public static RandomizerEntry BaldurShell = new RandomizerEntry(
            "BaldurShell", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_5", typeof(bool), null) },
            (have) => (have(MantisClaw) || have(DASH_CLOAK) || have(MonarchWings) || have(CrystalHeart)) && (have(FIREBALL) || have(QUAKE) || have(GrubberflysElegy) || have(GlowingWomb) || have(SporeShroom) || have(Weaversong) || (have(MarkofPride) && have(HARD))),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_5" }
        );
        public static RandomizerEntry FuryoftheFallen = new RandomizerEntry(
            "FuryoftheFallen", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_6", typeof(bool), null) },
            (have) => have(MantisClaw),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_6" }
        );
        public static RandomizerEntry QuickFocus = new RandomizerEntry(
            "QuickFocus", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_7", typeof(bool), null) },
            (have) => have(WIN),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_7" }
        );
        public static RandomizerEntry LifebloodHeart = new RandomizerEntry(
            "LifebloodHeart", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_8", typeof(bool), null) },
            (have) => have(REACH_SALUBRA),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_8" }
        );
        public static RandomizerEntry LifebloodCore = new RandomizerEntry(
            "LifebloodCore", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_9", typeof(bool), null) },
            (have) => have(REACH_EVERYTHING),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_9" }
        );
        public static RandomizerEntry DefendersCrest = new RandomizerEntry(
            "DefendersCrest", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_10", typeof(bool), null) },
            (have) => (have(MantisClaw) && (have(CrystalHeart) || have(DASH_CLOAK) || have(MonarchWings) || have(HARD))) || (have(MonarchWings) && have(HARD)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_10" }
        );
        public static RandomizerEntry Flukenest = new RandomizerEntry(
            "Flukenest", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_11", typeof(bool), null) },
            (have) => have(QUAKE) && (have(MantisClaw) || have(HARD)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_11" }
        );
        public static RandomizerEntry ThornsofAgony = new RandomizerEntry(
            "ThornsofAgony", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_12", typeof(bool), null) },
            (have) => have(DASH_CLOAK) || (have(MantisClaw) && have(CrystalHeart)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_12" }
        );
        public static RandomizerEntry MarkofPride = new RandomizerEntry(
            "MarkofPride", true,
            new RandomizerVar[] { new RandomizerVar("gotCharm_13", typeof(bool), null) },
            (have) => have(MantisClaw) || (have(MonarchWings) && have(HARD)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_13" }
        );
        public static RandomizerEntry SteadyBody = new RandomizerEntry(
            "SteadyBody", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_14", typeof(bool), null) },
            (have) => have(REACH_SALUBRA),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_14" }
        );
        public static RandomizerEntry HeavyBlow = new RandomizerEntry(
            "HeavyBlow", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_15", typeof(bool), null) },
            (have) => have(MantisClaw) || (have(MonarchWings) && (have(FIREBALL) || have(DASH_CLOAK)) && have(HARD)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_15" }
        );
        public static RandomizerEntry SharpShadow = new RandomizerEntry(
            "SharpShadow", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_16", typeof(bool), null) },
            (have) => have(SHADE_CLOAK) && (have(MantisClaw) || have(IsmasTear) || (have(MonarchWings) && have(HARD) && have(CLASSIC))),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_16" }
        );
        public static RandomizerEntry SporeShroom = new RandomizerEntry(
            "SporeShroom", true,
            new RandomizerVar[] { new RandomizerVar("gotCharm_17", typeof(bool), null) },
            (have) => (have(MantisClaw) && (have(DASH_CLOAK) || have(MonarchWings) || have(CrystalHeart) || have(IsmasTear))) || (have(MonarchWings) && (have(DASH_CLOAK) || have(IsmasTear)) && have(HARD) && have(CLASSIC)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_17" }
        );
        public static RandomizerEntry Longnail = new RandomizerEntry(
            "Longnail", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_18", typeof(bool), null) },
            (have) => have(REACH_SALUBRA),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_18" }
        );
        public static RandomizerEntry ShamanStone = new RandomizerEntry(
            "ShamanStone", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_19", typeof(bool), null) },
            (have) => have(REACH_SALUBRA),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_19" }
        );
        public static RandomizerEntry SoulCatcher = new RandomizerEntry(
            "SoulCatcher", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_20", typeof(bool), null) },
            (have) => true,
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_20" }
        );
        public static RandomizerEntry SoulEater = new RandomizerEntry(
            "SoulEater", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_21", typeof(bool), null) },
            (have) => have(QUAKE) && (have(MantisClaw) || have(MonarchWings)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_21" }
        );
        public static RandomizerEntry GlowingWomb = new RandomizerEntry(
            "GlowingWomb", true,
            new RandomizerVar[] { new RandomizerVar("gotCharm_22", typeof(bool), null) },
            (have) => have(CrystalHeart) && (have(MantisClaw) || have(MonarchWings)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_22" }
        );
        public static RandomizerEntry FragileHeart = new RandomizerEntry(
            "FragileHeart", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_23", typeof(bool), null) },
            (have) => have(REACH_LEG_EATER),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_23" }
        );
        public static RandomizerEntry FragileGreed = new RandomizerEntry(
            "FragileGreed", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_24", typeof(bool), null) },
            (have) => have(REACH_LEG_EATER),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_24" }
        );
        public static RandomizerEntry FragileStrength = new RandomizerEntry(
            "FragileStrength", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_25", typeof(bool), null) },
            (have) => have(REACH_LEG_EATER),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_25" }
        );
        public static RandomizerEntry JonisBlessing = new RandomizerEntry(
            "JonisBlessing", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_27", typeof(bool), null) },
            (have) => have(MantisClaw),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_27" }
        );
        public static RandomizerEntry ShapeofUnn = new RandomizerEntry(
            "ShapeofUnn", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_28", typeof(bool), null) },
            (have) => have(IsmasTear) && (have(MonarchWings) || have(MantisClaw)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_28" }
        );
        public static RandomizerEntry Hiveblood = new RandomizerEntry(
            "Hiveblood", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_29", typeof(bool), null) },
            (have) => (have(MantisClaw) && (have(MonarchWings) || have(HARD))) || (have(MonarchWings) && have(DASH_CLOAK) && have(HARD)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_29" }
        );
        public static RandomizerEntry DreamWielder = new RandomizerEntry(
            "DreamWielder", true,
            new RandomizerVar[] { new RandomizerVar("gotCharm_30", typeof(bool), null) },
            (have) => have(WIN),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_30" }
        );
        public static RandomizerEntry Dashmaster = new RandomizerEntry(
            "Dashmaster", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_31", typeof(bool), null) },
            (have) => have(DASH_CLOAK) || have(MonarchWings) || have(MantisClaw) || have(IsmasTear) || (have(FIREBALL) && have(HARD)) || (have(CrystalHeart) && (have(FIREBALL) || have(GrubberflysElegy) || have(GlowingWomb) || have(SporeShroom) || have(QUAKE) || have(Weaversong))),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_31" }
        );
        public static RandomizerEntry QuickSlash = new RandomizerEntry(
            "QuickSlash", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_32", typeof(bool), null) },
            (have) => have(DASH_CLOAK) && have(QUAKE) && (have(MantisClaw) || (have(MonarchWings) && have(HARD))),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_32" }
        );
        public static RandomizerEntry SpellTwister = new RandomizerEntry(
            "SpellTwister", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_33", typeof(bool), null) },
            (have) => ((have(MantisClaw) || have(MonarchWings)) && have(HARD) && have(CLASSIC)) || (have(MantisClaw) && (have(DASH_CLOAK) || have(CrystalHeart) || have(MonarchWings) || have(IsmasTear))),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_33" }
        );
        public static RandomizerEntry DeepFocus = new RandomizerEntry(
            "DeepFocus", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_34", typeof(bool), null) },
            (have) => (have(CrystalHeart) && (have(MantisClaw) || (have(MonarchWings) && have(FIREBALL) && have(HARD)))) || (have(MonarchWings) && have(DASH_CLOAK) && have(HARD)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_34" }
        );
        public static RandomizerEntry GrubberflysElegy = new RandomizerEntry(
            "GrubberflysElegy", true,
            new RandomizerVar[] { new RandomizerVar("gotCharm_35", typeof(bool), null) },
            (have) => have(REACH_EVERYTHING),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_35" }
        );
        public static RandomizerEntry Sprintmaster = new RandomizerEntry(
            "Sprintmaster", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_37", typeof(bool), null) },
            (have) => have(MantisClaw) || (have(MonarchWings) && (have(FIREBALL) || have(DASH_CLOAK)) && have(HARD)),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_37" }
        );
        public static RandomizerEntry Dreamshield = new RandomizerEntry(
            "Dreamshield", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_38", typeof(bool), null) },
            (have) => have(MantisClaw) || have(HARD),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_38" }
        );
        public static RandomizerEntry Weaversong = new RandomizerEntry(
            "Weaversong", true,
            new RandomizerVar[] { new RandomizerVar("gotCharm_39", typeof(bool), null) },
            (have) => have(MantisClaw),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_39" }
        );
        public static RandomizerEntry Grimmchild = new RandomizerEntry(
            "Grimmchild", false,
            new RandomizerVar[] { new RandomizerVar("gotCharm_40", typeof(bool), null) },
            (have) => have(MantisClaw) && have(DREAM_NAIL),
            RandomizerType.CHARM,
            new string[] { "UI.CHARM_NAME_40" }
        );

        public static void Initialize()
        {
            AddEntry(MothwingCloak);
            AddEntry(MantisClaw);
            AddEntry(CrystalHeart);
            AddEntry(MonarchWings);
            AddEntry(ShadeCloak);
            AddEntry(IsmasTear);
            AddEntry(DreamNail);
            AddEntry(DreamGate);
            AddEntry(AwokenDreamNail);
            AddEntry(VengefulSpirit);
            AddEntry(ShadeSoul);
            AddEntry(DesolateDive);
            AddEntry(DescendingDark);
            AddEntry(HowlingWraiths);
            AddEntry(AbyssShriek);
            AddEntry(GatheringSwarm);
            AddEntry(WaywardCompass);
            AddEntry(Grubsong);
            AddEntry(StalwartShell);
            AddEntry(BaldurShell);
            AddEntry(FuryoftheFallen);
            AddEntry(QuickFocus);
            AddEntry(LifebloodHeart);
            AddEntry(LifebloodCore);
            AddEntry(DefendersCrest);
            AddEntry(Flukenest);
            AddEntry(ThornsofAgony);
            AddEntry(MarkofPride);
            AddEntry(SteadyBody);
            AddEntry(HeavyBlow);
            AddEntry(SharpShadow);
            AddEntry(SporeShroom);
            AddEntry(Longnail);
            AddEntry(ShamanStone);
            AddEntry(SoulCatcher);
            AddEntry(SoulEater);
            AddEntry(GlowingWomb);
            AddEntry(FragileHeart);
            AddEntry(FragileGreed);
            AddEntry(FragileStrength);
            AddEntry(JonisBlessing);
            AddEntry(ShapeofUnn);
            AddEntry(Hiveblood);
            AddEntry(DreamWielder);
            AddEntry(Dashmaster);
            AddEntry(QuickSlash);
            AddEntry(SpellTwister);
            AddEntry(DeepFocus);
            AddEntry(GrubberflysElegy);
            AddEntry(Sprintmaster);
            AddEntry(Dreamshield);
            AddEntry(Weaversong);
            AddEntry(Grimmchild);
        }

        public static void AddEntry(RandomizerEntry entry)
        {
            foreach (RandomizerVar var in entry.varNames)
            {
                if (typeof(PlayerData).GetField(var.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static) == null)
                {
                    RandomizerMod.Warning("var name not found: " + var.name);
                    return;
                }
            }

            randomizedEntries.Add(entry);
            foreach (RandomizerVar varName in entry.varNames)
            {
                string val = (varName.value ?? "").ToString();
                varNameToEntry.Add(varName.name + val, entry);
            }
            for (int i = 0; i < entry.localeNames.Length; i++)
            {
                // TODO: why are we ignoring the second and third locale names?
                if (i != 1 && i != 2)
                {
                    localeNameToEntry.Add(entry.localeNames[i], entry);
                }
            }
        }
    }
}
