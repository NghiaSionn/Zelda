using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameTimeData : ScriptableObject
{
    public int dayCount = 1;
    public string currentTimeString = "00:00";
}
