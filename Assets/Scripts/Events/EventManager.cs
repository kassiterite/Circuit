using System;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static Action onGateStateChange;
    public static Action onEndReached;
    public static Action onTooltipDisplayed;
    public static void SendGateStateChange()
    { 
        onGateStateChange?.Invoke();  
    } 

    public static void SendEndReached()
    { 
        onEndReached?.Invoke();
    }

    public static void SendTooltipDisplayed()
    { 
        onTooltipDisplayed?.Invoke();
    }
}
