using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    public PlayerController.ButtonColor _hurtColor;
    private SpriteRenderer _spriteRenderer;


    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        var random = Random.Range(0, 4);
        switch (random)
        {
            case 0:
            {
                _hurtColor = PlayerController.ButtonColor.RED;
                _spriteRenderer.color = Color.red;
                break;
            }
            case 1:
            {
                _hurtColor = PlayerController.ButtonColor.GREEN;
                _spriteRenderer.color = Color.green;
                break;
            }
            case 2:
            {
                _hurtColor = PlayerController.ButtonColor.BLUE;
                _spriteRenderer.color = Color.blue;
                break;
            }
            case 3:
            {
                _hurtColor = PlayerController.ButtonColor.YELLOW;
                _spriteRenderer.color = Color.yellow;
                break;
            }
        }
    }

    void Update()
    {
        var playerPos = PlayerController.Instance.transform.position;
        var pos = transform.position;
        if (Vector2.Distance(playerPos, pos) > 1)
        {
            var step =  moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(pos, playerPos, step);
        }
    }
}
