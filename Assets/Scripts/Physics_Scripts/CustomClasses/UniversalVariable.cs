using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Filename : UniversalVariable
 * 
 * Goal : Provides a public single instance of universal physic constants, those constants will be used in every physics based calculations
 * 
 * Requirements : NaN
 */
public static class UniversalVariable
{
    private static float gravity = 9.8f;
    private static float airDrag = 1.0f;
    private static float time = 1.0f;
    private static float Bounciness = 1.0f;
    private static float SFriction = 1.0f;
    private static float DFriction = 1.0f;

    public static void SetTime(float xtime)
    {
        time = xtime;
    }

    public static float GetTime()
    {
        return time;
    }

    public static void SetBounciness(float xBounciness)
    {
        Bounciness = xBounciness;
    }

    public static float GetBounciness()
    {
        return Bounciness;
    }
    public static void SetSFriction(float xSFriction)
    {
        SFriction = xSFriction;
    }

    public static float GetSFriction()
    {
        return SFriction;
    }
    public static void SetDFriction(float xDFriction)
    {
        DFriction = xDFriction;
    }

    public static float GetDFriction()
    {
        return DFriction;
    }

    public static float GetGravity() 
    {
        return gravity;
    }

    public static void SetGravity(float xgravity)
    {
        gravity = xgravity;
    }

    public static float GetAirDrag() 
    {
        return airDrag;
    }

    public static void SetAirDrag(float xairDrag)
    {
        airDrag = xairDrag;
    }
}
