using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class EnemyState
{
    public string name;
    public bool isActive;
    public Vector3 position;
    public Vector3 eulerAngles;
    public string Type;
    public int waypointIndex;

    public EnemyState(string _name, bool _isActive, Vector3 _position, Vector3 _eulerAngles, string type, int waypointIndex)
    {
        name = _name;
        isActive = _isActive;
        position = _position;
        eulerAngles = _eulerAngles;
        Type = type;
        this.waypointIndex = waypointIndex;
    }
}