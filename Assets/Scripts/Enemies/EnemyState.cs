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
    public float rotationX;
    public float rotationY;
    public float rotationZ;

    public EnemyState(string _name, bool _isActive, Vector3 _position, float _rotationX, float _rotationY, float _rotationZ)
    {
        name = _name;
        isActive = _isActive;
        position = _position;
        rotationX = _rotationX;
        rotationY = _rotationY;
        rotationZ = _rotationZ;
    }
}
