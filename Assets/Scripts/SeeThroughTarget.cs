using System.Collections.Generic;
using UnityEngine;

public class SeeThroughTarget : MonoBehaviour
{
    public static readonly List<SeeThroughTarget> All = new();

    void OnEnable() { All.Add(this); }
    void OnDisable() { All.Remove(this); }
}
