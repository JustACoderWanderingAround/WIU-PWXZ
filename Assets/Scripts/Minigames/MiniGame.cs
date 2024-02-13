using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MiniGame : MonoBehaviour
{
    bool hasWon;
    public float maxTimer;
    float timer;
    public EventTrigger.TriggerEvent OnWinCallBack;
    protected virtual void OnStart() { }
    protected void OnWin() 
    {
        BaseEventData eventData = new BaseEventData(EventSystem.current);
        eventData.selectedObject = this.gameObject;
        OnWinCallBack.Invoke(eventData);
    }
}
