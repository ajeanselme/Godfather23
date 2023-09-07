using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Couche : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float damageRange = 2f;
    
    [HideInInspector]
    public Vector2 origin;
    
    [HideInInspector]
    public PlayerController.ButtonColor color;
    
    [HideInInspector]
    public Vector2 target;
    
    [HideInInspector]
    public int damage;

    private void Update()
    {
        var currentPos = transform.position;
        
        if (Vector2.Distance(currentPos, target) > .3f)
        {
            var step =  moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(currentPos, target, step);
        }
        else
        {
            Explode();
        }
    }

    private void Explode()
    {
        Destroy(gameObject);
        
        foreach (var enemyController in EnemiesManager.Instance.GetEnemiesAround(target, color, damageRange))
        {
            enemyController.HurtDistance(color, damage);
        }
    }
}
