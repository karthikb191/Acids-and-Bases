using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates
{
    idle,
    running,
    jumping,
    falling,
    swimming,    //This is a new state that must be set when haldi falls into water
    stun
};

public enum Item
{
    none,
    parachute,
    magnet,
    light
}

public enum Liquids
{
    Neutral,
    Acid,
    Base
}

