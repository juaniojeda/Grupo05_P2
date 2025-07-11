using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LifeSystemAccess
{
    private static LifeSystem _lifeSystem;

    public static void Register(LifeSystem lifeSystem)
    {
        _lifeSystem = lifeSystem;
    }

    public static void LoseLife()
    {
        _lifeSystem?.LoseLife();
    }

    public static void Clear()
    {
        _lifeSystem = null;
    }
}
