using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<EventManager>();
            return _instance;
        }
    }

    private void Awake() => _instance = this;
    

}
