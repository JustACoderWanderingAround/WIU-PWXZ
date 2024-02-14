using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementData")]
public class MovementData : ScriptableObject
{
    public float walkSpeed;
    public float crouchSpeed;
    public float sprintSpeed;

    public float baseJumpForce;
    public float jumpChargeMultiplier;

    public float airMultiplier;
    public float groundDrag;

    public float sprintStaminaCost;
    public float jumpStaminaCost;
    public float maxStamina;
    public float staminaRegenRate;
}