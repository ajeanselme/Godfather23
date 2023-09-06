using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;

    public enum ButtonColor
    {
        RED,
        GREEN,
        BLUE,
        YELLOW
    }
    
    public float meleeRange;
    public float distanceRange;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            var key = InputController.Instance.GetKey(Input.inputString);
            if (key?.Type == InputController.ButtonKey.AttackType.Melee)
            {
                MeleeAttack(key.Color);
            } else if (key?.Type == InputController.ButtonKey.AttackType.Ranged)
            {
                RangedAttack(key.Color);
            }
        }
    }

    private void MeleeAttack(ButtonColor color)
    {
        EnemiesManager.Instance.playerMeleeAttackEvent.Invoke(color);
    }
    private void RangedAttack(ButtonColor color)
    {
        EnemiesManager.Instance.GetClosestEnemy(color)?.HurtDistance(color);
    }
    
}
