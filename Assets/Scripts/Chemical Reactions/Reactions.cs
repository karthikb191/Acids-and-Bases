using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// This class consists of a collection of all the reaction agents and their reaction information
/// </summary>

public enum Salt
{
    Null,
    NaCl
}

public enum Chemical
{
    HCl,
    NaOH
}

class Reactions
{
    //These reference the second reaction material and the resultant of the reaction

    //HCl Reaction Dictionary
    static Dictionary<Enum, Enum> hclDictionary = new Dictionary<Enum, Enum>()
    {
        { BasesList.NaOH, SaltsList.NaCl }
    };

    //NaOH Dictionary
    static Dictionary<Enum, Enum> naOHDictionary = new Dictionary<Enum, Enum>()
    {
        { AcidsList.HCl, SaltsList.NaCl }
    };

    //Hybiscus Dictionary
    static Dictionary<Enum, Enum> hybiscusDictionary = new Dictionary<Enum, Enum>()
    {
        { IndicatorsList.Bromythol_Blue, IndicatorsList.Turmeric }
    };


    //Parent Dictionary
    static Dictionary<Enum, Dictionary<Enum, Enum>> reactionDictionary =
        new Dictionary<Enum, Dictionary<Enum, Enum>>()
        {
            {AcidsList.HCl, hclDictionary},
            {BasesList.NaOH, naOHDictionary},
            {NormalItemList.Hybiscus, hybiscusDictionary }
        };


    public static Enum React(Enum reactant1, Enum reactant2)
    {
        if (reactionDictionary.ContainsKey(reactant1))
        {
            if (reactionDictionary[reactant1].ContainsKey(reactant2))
            {
                return reactionDictionary[reactant1][reactant2];
            }
        }
        if (reactionDictionary.ContainsKey(reactant2))
        {
            if (reactionDictionary[reactant2].ContainsKey(reactant1))
            {
                return reactionDictionary[reactant2][reactant1];
            }
        }
        return null;
    }

    //public static Dictionary<Chemical, Dictionary<Chemical, Salt>> reactionDictionary =
    //    new Dictionary<Chemical, Dictionary<Chemical, Salt>>()
    //    {
    //        {Chemical.HCl, hclDictionary},
    //        {Chemical.NaOH, naOHDictionary}
    //    };

}

class Extraction
{
    static Dictionary<Enum, Enum> extractionDictionary 
        = new Dictionary<Enum, Enum>()
    {
            {NormalItemList.Lemon, AcidsList.C6H8O7 }
    };

    public static UnityEngine.GameObject Extract(Enum item)
    {
        Enum i = extractionDictionary[item];

        //Get the prefab of the extracted item
        return ItemManager.instance.itemDictionary[i];
    }
}