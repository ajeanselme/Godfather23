using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType
    {
        Melee,
        Range,
        Fast
    }
    
    
    public float moveSpeed;
    public float knockbackSpeed;
    public float knockbackDuration;
    public PlayerController.ButtonColor hurtColor;
    
    public EnemyType enemyType;
    
    private SpriteRenderer _spriteRenderer;

    private bool _knockbacking = false;
    private float _offsetPositioning;

    [HideInInspector]
    public float distanceToPlayer;

    public GameObject[] colors;
    public GameObject[] colorsMelee;

    public float attackCooldown;
    private float _nextAttackTime;

    public int colorAmount = 1;
    public int colorStreakNeeded = 1;
    
    private int _hurtAmount = 0;

    private Animator _animator;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        SetRandomColor();

        var melee = PlayerController.Instance.meleeRange;
        var distance = PlayerController.Instance.distanceRange;
        if (enemyType == EnemyType.Melee)
        {
            _offsetPositioning = 2;
        } else if (enemyType == EnemyType.Range)
        {
            _offsetPositioning = (distance - melee) / 4 + melee;
        } else if (enemyType == EnemyType.Fast)
        {
            _offsetPositioning = 2;
        }
    }

    void Update()
    {
        var playerPos = PlayerController.Instance.transform.position;
        var pos = transform.position;
        if (_knockbacking)
        {
            Vector3 oppositeVector = pos - playerPos;
            
            Vector2 newPos = pos + oppositeVector;
            
            var step =  knockbackSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(pos, newPos, step);
        }
        else
        {
            if (Vector2.Distance(playerPos, pos) > _offsetPositioning)
            {
                var step =  moveSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(pos, playerPos, step);
            }
            else
            {
                Attack();
            }
        }

        distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
        {
            if (distanceToPlayer <= PlayerController.Instance.meleeRange)
            {
                SetActive(true);
            }
        }
        else
        {
            if (distanceToPlayer <= PlayerController.Instance.distanceRange)
            {
                SetActive(true);
            }
        }
    }

    public void HurtMelee(PlayerController.ButtonColor buttonColor, int damage = 1)
    {
        if (buttonColor != hurtColor)
        {
            return;
        }

        var playerPos = PlayerController.Instance.transform.position;
        var pos = transform.position;
        if (Vector2.Distance(playerPos, pos) > PlayerController.Instance.meleeRange)
        {
            return;
        }
        Hurt(damage);
    }

    public void HurtDistance(PlayerController.ButtonColor buttonColor, int damage = 1)
    {
        if (buttonColor != hurtColor)
        {
            return;
        }

        var distance = Vector2.Distance(PlayerController.Instance.transform.position, transform.position);
        if (distance > PlayerController.Instance.distanceRange || distance <= PlayerController.Instance.meleeRange)
        {
            return;
        }
        Hurt(damage);
    }

    private void Hurt(int damage = 1)
    {
        StartCoroutine(Knockback());

        _hurtAmount += damage;

        if (_hurtAmount >= colorAmount + colorStreakNeeded)
        {
            EnemiesManager.Instance.KillEnemy(this);
            return;
        }

        if (_hurtAmount >= colorStreakNeeded)
        {
            SetRandomColor();
        }
    }

    private IEnumerator Knockback()
    {
        _knockbacking = true;
        yield return new WaitForSeconds(knockbackDuration);
        _knockbacking = false;
    }

    private void Attack()
    {
        if (Time.time <= _nextAttackTime) return;
        
        _nextAttackTime = Time.time + attackCooldown;
        PlayerController.Instance.TryHurt();
        
        _animator.SetTrigger("Attack");
    }

    private void SetRandomColor()
    {
        var random = Random.Range(0, 4);
        switch (random)
        {
            case 0:
                SetColor(PlayerController.ButtonColor.RED);
                break;
            case 1:
                SetColor(PlayerController.ButtonColor.GREEN);
                break;
            case 2:
                SetColor(PlayerController.ButtonColor.BLUE);
                break;
            case 3:
                SetColor(PlayerController.ButtonColor.YELLOW);
                break;
        }   
    }
    
    private void SetColor(PlayerController.ButtonColor buttonColor)
    {
        foreach (var color in colors)
        {
            color.SetActive(false);
        }
        foreach (var color in colorsMelee)
        {
            color.SetActive(false);
        }
        
        switch (buttonColor)
        {
            case PlayerController.ButtonColor.RED:
            {
                hurtColor = PlayerController.ButtonColor.RED;
                if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                {
                    colorsMelee[0].SetActive(true);
                }
                else
                {
                    colors[0].SetActive(true);
                }
                break;
            }
            case PlayerController.ButtonColor.GREEN:
            {
                hurtColor = PlayerController.ButtonColor.GREEN;
                if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                {
                    colorsMelee[2].SetActive(true);
                }
                else
                {
                    colors[2].SetActive(true);
                }
                break;
            }
            case PlayerController.ButtonColor.BLUE:
            {
                hurtColor = PlayerController.ButtonColor.BLUE;
                if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                {
                    colorsMelee[4].SetActive(true);
                }
                else
                {
                    colors[4].SetActive(true);
                }
                break;
            }
            case PlayerController.ButtonColor.YELLOW:
            {
                hurtColor = PlayerController.ButtonColor.YELLOW;
                if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                {
                    colorsMelee[6].SetActive(true);
                }
                else
                {
                    colors[6].SetActive(true);
                }
                break;
            }
        }
    }

    private void SetActive(bool value)
    {
        foreach (var color in colors)
        {
            color.SetActive(false);
        }
        if (value)
        {
            switch (hurtColor)
            {
                case PlayerController.ButtonColor.RED:
                {
                    if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                    {
                        colorsMelee[1].SetActive(true);
                    }
                    else
                    {
                        colors[1].SetActive(true);
                    }
                    break;
                }
                case PlayerController.ButtonColor.GREEN:
                {
                    if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                    {
                        colorsMelee[3].SetActive(true);
                    }
                    else
                    {
                        colors[3].SetActive(true);
                    }
                    break;
                }
                case PlayerController.ButtonColor.BLUE:
                {
                    if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                    {
                        colorsMelee[5].SetActive(true);
                    }
                    else
                    {
                        colors[5].SetActive(true);
                    }
                    break;
                }
                case PlayerController.ButtonColor.YELLOW:
                {
                    if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                    {
                        colorsMelee[7].SetActive(true);
                    }
                    else
                    {
                        colors[7].SetActive(true);
                    }
                    break;
                }
            }
        }
        else
        {
            switch (hurtColor)
            {
                case PlayerController.ButtonColor.RED:
                {
                    
                    if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                    {
                        colorsMelee[0].SetActive(true);
                    }
                    else
                    {
                        colors[0].SetActive(true);
                    }
                    break;
                }
                case PlayerController.ButtonColor.GREEN:
                {
                    if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                    {
                        colorsMelee[2].SetActive(true);
                    }
                    else
                    {
                        colors[2].SetActive(true);
                    }
                    break;
                }
                case PlayerController.ButtonColor.BLUE:
                {
                    if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                    {
                        colorsMelee[4].SetActive(true);
                    }
                    else
                    {
                        colors[4].SetActive(true);
                    }
                    break;
                }
                case PlayerController.ButtonColor.YELLOW:
                {
                    if (enemyType == EnemyType.Melee || enemyType == EnemyType.Fast)
                    {
                        colorsMelee[6].SetActive(true);
                    }
                    else
                    {
                        colors[6].SetActive(true);
                    }
                    break;
                }
            }
        }
    }
}
