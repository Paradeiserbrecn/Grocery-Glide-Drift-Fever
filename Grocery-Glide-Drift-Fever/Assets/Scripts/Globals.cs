using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public static class Globals
{
    public static TimeSpan raceTime = TimeSpan.Zero;
    public static List<Item> excessItems = new List<Item>();
    public static float DriftScore, AirtimeScore = 0;

    public static LevelData currentLevel;
}
