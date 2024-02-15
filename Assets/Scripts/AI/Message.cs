using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConcreteMessages
{
    public class Message
    {
        public Message(IEventListener sender)
        {
            this.sender = sender;
        }
        public IEventListener sender;

    }
    public class MessagePlayerHere : Message
    {
        public MessagePlayerHere(IEventListener sender, Vector3 pos, float threshHold) : base(sender)
        {
            location = pos;
            distThreshold = threshHold;
        }
        public Vector3 location;
        public float distThreshold;
    }
}
