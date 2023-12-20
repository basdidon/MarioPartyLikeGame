using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionMapper : MonoBehaviour
{
    public static ActionMapper Instance { get; private set; }

    public enum EventName
    {
        Roll,
    }

    public event Action OnRolled;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public Action GetActionRaiser(EventName eventName)
    {
        return eventName switch
        {
            EventName.Roll => () => OnRolled?.Invoke(),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
