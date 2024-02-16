using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConcreteMessages;


public class PostOffice : MonoBehaviour
{
    public static PostOffice instance;

    List<GameObject> recipients = new List<GameObject>();  
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
    public void SendToPostOffice(Message message)
    {
        MessagePlayerHere messagePlayerHere = message as MessagePlayerHere; 
        if (messagePlayerHere != null)
        {
            foreach (GameObject go in recipients)
            {
                if (go.GetComponent<IEventListener>().GetListenerType() == LISTENER_TYPE.GUARD)
                {
                    if (Vector3.Distance(go.transform.position, messagePlayerHere.location) < messagePlayerHere.distThreshold)
                    {
                        SoundWPosition newSound = new SoundWPosition(null, messagePlayerHere.soundType, messagePlayerHere.location, 1000);
                        newSound.soundType = SoundWPosition.SoundType.MOVEMENT;
                        go.GetComponent<IEventListener>().RespondToSound(newSound);
                    }
                }
            }
        }
    }
    public void Subscribe(GameObject newListener)
    {
        recipients.Add(newListener);
    }
}
