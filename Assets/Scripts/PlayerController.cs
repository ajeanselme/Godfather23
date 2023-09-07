using System;
using System.Collections;
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
    public int maxHealth;

    public GameObject diePanel;

    private int _currentHealth;
    private bool _invincible;

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

    private void Start()
    {
        diePanel.SetActive(false);
        _currentHealth = maxHealth;
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

    public void TryHurt()
    {
        if(_invincible) return;

        //TODO ANIMATION
        
        _currentHealth -= 1;

        if (_currentHealth <= 0)
        {
            Die();
            return;
        }
        
        StartCoroutine(HurtState());
        
        Debug.Log("Hurt " + _currentHealth);
    }

    private IEnumerator HurtState()
    {
        _invincible = true;
        
        //TODO Play animation

        yield return new WaitForSeconds(1);
        
        _invincible = false;
    }


    private void Die()
    {
        diePanel.SetActive(true);
        Time.timeScale = 0;
    }
}
