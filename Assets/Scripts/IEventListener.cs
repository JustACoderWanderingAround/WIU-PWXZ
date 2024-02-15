using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum LISTENER_TYPE
{
    WORKER,
    GUARD
}
public interface IEventListener
{
    
    void RespondToSound(SoundWPosition sound);
}