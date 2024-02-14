using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MiniGame : MonoBehaviour
{
    protected bool hasWon;
    public float maxTimer;
    protected float timer;
    public GameObject gameCanvas;
    public EventTrigger.TriggerEvent OnWinCallBack;
    protected void OnEnable()
    {
        timer = maxTimer;
        gameCanvas.SetActive(true);
    }
    protected virtual void OnStart() { }
    protected virtual void OnWin() 
    {
        gameCanvas.SetActive(false);
        BaseEventData eventData = new BaseEventData(EventSystem.current);
        eventData.selectedObject = this.gameObject;
        OnWinCallBack.Invoke(eventData);
    }
}
