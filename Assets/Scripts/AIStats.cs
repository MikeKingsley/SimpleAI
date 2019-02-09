using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/AI Stats")]
public class AIStats : ScriptableObject
{
    [Header("Movement Settings")]
    [Range(0f, 3f)] public float walkSpeed = 1.5f;
    [Range(3f, 6f)] public float runSpeed = 3f;
    public float fov = 120f;
    public float lookRange = 40f;
    public float wanderRadius = 200f;

    [Header("Ability Settings")]
    [Range(0f, 10000f)] public float Health = 100f;
    public float attackRange = 1f;
    public float attackRate = 1f;
    public float attackForce = 15f;
    public float attackDamage = 50f;

    [Header("Personality Settings")]
    public bool canFlee = true;
    [Range(0f, 20f)] public float bravery = 10f;
    public int teamID = 1;

    [Header("Effects")]
    public ParticleEffect impactEffect;
    public ParticleEffect deathEffect;
}