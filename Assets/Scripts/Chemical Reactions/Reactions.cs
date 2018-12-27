using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// This class consists of a collection of all the reaction agents and their reaction information
/// 
/// </summary>
/// 


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
    public static Dictionary<Chemical, Salt> hclDictionary = new Dictionary<Chemical, Salt>()
    {
        { Chemical.NaOH, Salt.NaCl }
    };

    //NaOH Dictionary
    public static Dictionary<Chemical, Salt> naOHDictionary = new Dictionary<Chemical, Salt>()
    {
        { Chemical.HCl, Salt.NaCl }
    };



    //Parent Dictionary
    public static Dictionary<Chemical, Dictionary<Chemical, Salt>> reactionDictionary =
        new Dictionary<Chemical, Dictionary<Chemical, Salt>>()
        {
            {Chemical.HCl, hclDictionary},
            {Chemical.NaOH, naOHDictionary}
        };


    
    

}