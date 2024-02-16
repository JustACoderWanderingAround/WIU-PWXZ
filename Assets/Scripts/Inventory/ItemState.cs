using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class ItemState
{
    public string name;
    public int id;
    public bool isActive;
    public Vector3 position;
    public float rotationX;
    public float rotationY;
    public float rotationZ;

    public ItemState(string _name,int _id,bool _isActive, Vector3 _position, float _rotationX, float _rotationY, float _rotationZ)
    {
        name = _name;
        id = _id;
       isActive = _isActive;
       position = _position;
       rotationX = _rotationX;
        rotationY = _rotationY;
        rotationZ = _rotationZ;
    }
}
