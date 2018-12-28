using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level{
    public float bestTime = Mathf.Infinity;
    public List<Salt> targetSalts;
    public List<Salt> saltsAchieved = new List<Salt>();
    public int stars = 10;   //Max: stars
    public int starsCollected = 0;
}


public class LevelObjectives
{

    class Level_1 : Level
    {
        public Level_1()
        {
            Achievements();
        }

        public Level Achievements()
        {
            targetSalts = new List<Salt>()
            {
                Salt.Null
            };
            
            return this;
        }
    }

    class Level_2 : Level
    {
        public Level_2()
        {
            Achievements();
        }

        public Level Achievements()
        {
            targetSalts = new List<Salt>()
            {
                Salt.NaCl
            };

            return this;
        }
    }

    public static List<Level> levels = new List<Level>()
    {
        new Level_1(), new Level_2()
    };
}