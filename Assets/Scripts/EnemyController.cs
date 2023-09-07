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

    public float attackCooldown;
    private float _nextAttackTime;

    public int colorAmount = 1;
    public int colorStreakNeeded = 1;
    
    private int _hurtAmount = 0;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        SetRandomColor();

        var melee = PlayerController.Instance.meleeRange;
        var distance = PlayerController.Instance.distanceRange;
        if (enemyType == EnemyType.Melee)
        {
            _offsetPositioning = 1;
        } else if (enemyType == EnemyType.Range)
        {
            _offsetPositioning = (distance - melee) / 2 + melee;
        } else if (enemyType == EnemyType.Fast)
        {
            _offsetPositioning = 1;
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
        
        //TODO Play attack animation
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
        switch (buttonColor)
        {
            case PlayerController.ButtonColor.RED:
            {
                hurtColor = PlayerController.ButtonColor.RED;
                _spriteRenderer.color = Color.red;
                break;
            }
            case PlayerController.ButtonColor.GREEN:
            {
                hurtColor = PlayerController.ButtonColor.GREEN;
                _spriteRenderer.color = Color.green;
                break;
            }
            case PlayerController.ButtonColor.BLUE:
            {
                hurtColor = PlayerController.ButtonColor.BLUE;
                _spriteRenderer.color = Color.blue;
                break;
            }
            case PlayerController.ButtonColor.YELLOW:
            {
                hurtColor = PlayerController.ButtonColor.YELLOW;
                _spriteRenderer.color = Color.yellow;
                break;
            }
        }
    }
}
