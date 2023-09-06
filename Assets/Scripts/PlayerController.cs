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

    [SerializeField]
    private float meleeRange;
    
    [SerializeField]
    private GameObject meleeController;

    [SerializeField]
    private float distanceRange;
    
    [SerializeField]
    private GameObject distanceController;


    private List<EnemyController> _meleeEnemiesInRange;
    private List<EnemyController> _distanceEnemiesInRange;

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

        meleeController.GetComponent<CircleCollider2D>().radius = meleeRange;
        distanceController.GetComponent<CircleCollider2D>().radius = distanceRange;
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
                // TODO Distance attack
            }
        }
    }

    public void ColliderTrigger(Collider2D other)
    {
        Debug.Log(other);
        if (other.GetInstanceID() == meleeController.GetInstanceID())
        {
            Debug.Log("New enemy in melee range");
            _meleeEnemiesInRange.Add(other.gameObject.GetComponent<EnemyController>());
        } else if (other.GetInstanceID() == distanceController.GetInstanceID())
        {
            _distanceEnemiesInRange.Add(other.gameObject.GetComponent<EnemyController>());
        }
    }

    private void MeleeAttack(ButtonColor color)
    {
        Debug.Log("Melee " + color);
        foreach (var enemy in _meleeEnemiesInRange)
        {
            if (enemy._hurtColor == color)
            {
                EnemiesManager.Instance.KillEnemy(enemy);
            }
        }
    }
    
}
