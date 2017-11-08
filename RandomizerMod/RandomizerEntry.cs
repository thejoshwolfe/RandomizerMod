using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace RandomizerMod
{
    //Type of pickup
    //Not currently used much; mostly for future proofing
    public enum RandomizerType
    {
        ABILITY,
        CHARM,
        SPELL,
        RELIC,
        PSEUDO
    };

    public delegate bool ReachableWithFunc(Predicate<RandomizerEntry> have);

    public class RandomizerEntry
    {
        public string name;
        public bool isSignificant;
        public RandomizerVar[] varNames;
        public ReachableWithFunc ReachableWith;
        public RandomizerType type;
        public string[] localeNames;
        public RandomizerEntry(string name, bool isSignificant, RandomizerVar[] varNames, ReachableWithFunc ReachableWith, RandomizerType type, string[] localeNames)
        {
            this.name = name;
            this.isSignificant = isSignificant;
            this.varNames = varNames;
            this.ReachableWith = ReachableWith;
            this.type = type;
            this.localeNames = localeNames;
        }

        //Get index of entry so we can change the matching index on the swapped item
        public int GetVarNameIndex(string s)
        {
            for (int i = 0; i < this.varNames.Length; i++)
            {
                if (s == this.varNames[i].name) return i;
            }
            return -1;
        }
        public override string ToString()
        {
            return name.ToString();
        }
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
    public class PseudoEntry : RandomizerEntry
    {
        public PseudoEntry(string name) : base(name, false, null, null, RandomizerType.PSEUDO, null)
        {
        }
    }
    public class EntryGroup : RandomizerEntry
    {
        public EntryGroup(ReachableWithFunc ReachableWith) : base("<group>", false, null, ReachableWith, RandomizerType.PSEUDO, null)
        {
        }
    }

    public struct RandomizerVar
    {
        public string name;
        public Type type;
        public object value;

        public RandomizerVar(string name, Type type, object value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }
    }
}
