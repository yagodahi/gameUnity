using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

public static class StFunc
{
    public static void Delay(this MonoBehaviour mb, Action act, float dur)
    {
        mb.StartCoroutine(Del(act, dur));

        // Examples:
        // this.Delay(MyFunctionNoParams, 1f);
        // this.Delay(() => MyFunctionParams(0, false), 1f);
        // this.Delay(() => Debug.Log("Lambdas also work"), 1f);
    }

    static IEnumerator Del(Action act, float dur)
    {
        yield return new WaitForSeconds(dur);

        act();
    }

    //----------------------------------------------

    public static float Remap(this float value, float fromMin1, float fromMax1, float toMin2, float toMax2)
    {
        return (value - fromMin1) / (fromMax1 - fromMin1) * (toMax2 - fromMin1) + toMin2;
    }

    //----------------------------------------------

    public static bool RandomBool()
    {
        bool myRandomBool = new Random().Next(100) <= 50 ? true : false;

        return myRandomBool;
    }

    //----------------------------------------------

    public static int Bigger(this int x, int a, int b)
    {
        if (a > b)
        {
            return x = a;
        }
        else
        {
            return x = b;
        }
    }

    public static Vector3 Random(this ref Vector3 myVector, Vector3 min, Vector3 max)
    {
        return myVector = new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
    }
}
