using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    
    public float moveSpeed;
    public float knockbackSpeed;
    public float knockbackDuration;
    public int maxHealth = 2;
    public PlayerController.ButtonColor _hurtColor;

    private SpriteRenderer _spriteRenderer;

    private int _currentHealth;
    private bool _knockbacking = false;

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

        _currentHealth = maxHealth;
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
            if (Vector2.Distance(playerPos, pos) > 1)
            {
                var step =  moveSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(pos, playerPos, step);
            }
        }
    }

    public void Hurt(PlayerController.ButtonColor buttonColor)
    {
        if (buttonColor != _hurtColor)
        {
            return;
        }

        var playerPos = PlayerController.Instance.transform.position;
        var pos = transform.position;
        if (Vector2.Distance(playerPos, pos) > PlayerController.Instance.meleeRange)
        {
            return;
        }
        
        StartCoroutine(Knockback());
        _currentHealth -= 1;

        if (_currentHealth <= 0)
        {
            EnemiesManager.Instance.KillEnemy(this);
        }
    }

    private IEnumerator Knockback()
    {
        _knockbacking = true;
        yield return new WaitForSeconds(knockbackDuration);
        _knockbacking = false;
    }
}
