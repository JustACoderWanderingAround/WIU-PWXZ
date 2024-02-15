using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConcreteMessages;


public class PostOffice : MonoBehaviour
{
    public static PostOffice Instance;

    List<IEventListener> recipients = new List<IEventListener>();  
    private PostOffice()
    {
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
