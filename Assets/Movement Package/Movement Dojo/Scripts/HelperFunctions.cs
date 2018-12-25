using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{
    public static float CounterAnimation(float counter, float min, float max, float increment, int sign, out int outSign)
    {
        outSign = sign;
        if (counter >= max)
            outSign = -1;
        if (counter <= min)
            outSign = 1;

        counter += outSign * increment;

        return counter;
    }

}
