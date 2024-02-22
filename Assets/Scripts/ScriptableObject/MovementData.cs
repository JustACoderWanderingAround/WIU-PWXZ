using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementData")]
public class MovementData : ScriptableObject
{
    public float walkSpeed;
    public float crouchSpeed;
    public float sprintSpeed;
    public float swimSpeed;
    public float swimBreathCost;

    public float baseJumpForce;
    public float jumpChargeMultiplier;

    public float airMultiplier;
    public float groundDrag;
    public float waterDrag;

    public float sprintStaminaCost;
    public float jumpStaminaCost;
    public float maxStamina;
    public float staminaRegenRate;

    public float walkNoiseRange;
    public float sprintNoiseRange;
    public float landNoiseRange;
}