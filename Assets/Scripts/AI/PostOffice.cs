using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConcreteMessages;


public class PostOffice : MonoBehaviour
{
    public static PostOffice instance;

    List<IEventListener> recipients = new List<IEventListener>();  
    private PostOffice()
    {
    }
    public static PostOffice GetInstance()
    {
        if (!instance)
        {
            GameObject postOffice = new GameObject();
            postOffice.name = "Post Office";
            postOffice.AddComponent<PostOffice>();
            instance = postOffice.GetComponent<PostOffice>();
        }
        return instance;
    }
    public void SendEvent(Message message)
    {
        MessagePlayerHere messagePlayerHere = message as MessagePlayerHere; 
    }
    public void Subscribe(IEventListener newListener)
    {
        recipients.Add(newListener);
    }
}
