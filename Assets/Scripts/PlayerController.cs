using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    public GameObject[] skins;
    
    public GameObject diePanel;
    public GameObject couchePrefab;

    public Image furyBar;

    public float rangedAttackCooldown = 1f;
    public int furyThreshold = 5;
    public int furyMultiplicator = 2;
    public float furyTime = 10;

    private float _furyTimer = 0f;
    private int _lastKillProgress = 0;
    
    private int _currentHealth;
    private bool _invincible;
    private bool _fury;

    private int _activeSkinIndex = 0;
    private float _nextRangedAttack;

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
        furyBar.fillAmount = 0;
        _currentHealth = maxHealth;

        skins[0].SetActive(true);
        skins[1].SetActive(false);
        skins[2].SetActive(false);
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
        
        UpdateFury(Time.deltaTime);
    }

    private void MeleeAttack(ButtonColor color)
    {
        var random = Random.Range(1, 4);
        skins[_activeSkinIndex].transform.GetChild(0).GetComponent<Animator>().SetTrigger("Melee" + random);
        if (_fury)
        {
            EnemiesManager.Instance.playerMeleeAttackEvent.Invoke(color, furyMultiplicator);
        }
        else
        {
            EnemiesManager.Instance.playerMeleeAttackEvent.Invoke(color, 1);
        }

        UpdateFury(Time.deltaTime);
    }
    
    private void RangedAttack(ButtonColor color)
    {
        if (Time.time <= _nextRangedAttack)
            return;


        var closest = EnemiesManager.Instance.GetClosestEnemy(color);
        if(closest == null) return;

        var targetPosition = closest.transform.position;
        
        if(Vector2.Distance(transform.position, targetPosition) > distanceRange)
            return;
        
        _nextRangedAttack = Time.time + rangedAttackCooldown;
        if (_fury)
        {
            StartCoroutine(LaunchCouche(targetPosition, color, furyMultiplicator));
        }
        else
        {
            StartCoroutine(LaunchCouche(targetPosition, color));
        }
    }

    private void UpdateFury(float deltaTime)
    {
        var currentKillProgress = EnemiesManager.Instance.currentKillProgress;
        if (currentKillProgress == _lastKillProgress)
        {
            _furyTimer += deltaTime * 0.1f;
            furyBar.fillAmount = Mathf.Lerp(furyBar.fillAmount, (float) EnemiesManager.Instance.currentKillProgress / furyThreshold, _furyTimer);
        }
        else
        {
            StartCoroutine(FillFuryAnimation());

            if (currentKillProgress >= furyThreshold)
            {
                EnemiesManager.Instance.currentKillProgress = 0;
                StartCoroutine(LaunchFury());
            }

            _furyTimer = 0f;
        }

        _lastKillProgress = currentKillProgress;
    }

    private IEnumerator FillFuryAnimation()
    {
        var parent = furyBar.transform.parent;
        var wait = new WaitForSeconds(.0001f);
        for (int i = 0; i < 50; i++)
        {
            var rotation = Mathf.Lerp(0, -2, i / 50f);
            parent.rotation = Quaternion.AngleAxis(rotation, furyBar.transform.parent.forward);
            yield return wait;
        }
        for (int i = 0; i < 50; i++)
        {
            var rotation = Mathf.Lerp(-2, 0, i / 50f);
            parent.eulerAngles = new Vector3(0, 0, rotation);
            yield return wait;
        }
        for (int i = 0; i < 50; i++)
        {
            var rotation = Mathf.Lerp(0, 2, i / 50f);
            parent.eulerAngles = new Vector3(0, 0, rotation);
            yield return wait;
        }
        for (int i = 0; i < 50; i++)
        {
            var rotation = Mathf.Lerp(2, 0, i / 50f);
            parent.eulerAngles = new Vector3(0, 0, rotation);
            yield return wait;
        }
    }

    private IEnumerator LaunchFury()
    {
        _invincible = true;
        _fury = true;

        StartCoroutine(StarMode());
        
        yield return new WaitForSeconds(furyTime);

        _fury = false;
        _invincible = false;
    }

    private IEnumerator StarMode()
    {
        var wait = new WaitForSeconds(.2f);
        while (_fury)
        {
            SetColor(new Color(Random.Range(0f, 1f),  Random.Range(0f, 1f),  Random.Range(0f, 1f), 1f));
            yield return wait;
        }
        SetColor(new Color(1f,1f, 1f, 1f));
    }

    public void TryHurt()
    {
        if(_invincible) return;

        _currentHealth -= 1;

        if (_currentHealth <= 0)
        {
            Die();
            return;
        }
        
        StartCoroutine(HurtState());

        var step = maxHealth / skins.Length;
        if (_currentHealth >= maxHealth / 1.5f)
        {
            skins[0].SetActive(true);
            skins[1].SetActive(false);
            skins[2].SetActive(false);
            _activeSkinIndex = 0;
        } else if (_currentHealth >= maxHealth / 3f)
        {
            skins[0].SetActive(false);
            skins[1].SetActive(true);
            skins[2].SetActive(false);
            _activeSkinIndex = 1;
        }
        else
        {
            skins[0].SetActive(false);
            skins[1].SetActive(false);
            skins[2].SetActive(true);
            _activeSkinIndex = 2;
        }
        Debug.Log("Hurt " + _currentHealth);
    }

    private IEnumerator HurtState()
    {
        _invincible = true;
        var wait = new WaitForSeconds(.1f);
        
        for (int i = 0; i < 5; i++)
        {
            SetAlpha(.5f);
            yield return wait;
            
            SetAlpha(.8f);
            yield return wait;
        }
        
        yield return new WaitForSeconds(.5f);
        _invincible = false;
    }

    private IEnumerator LaunchCouche(Vector2 target, ButtonColor color, int damage = 1)
    {
        skins[_activeSkinIndex].transform.GetChild(0).GetComponent<Animator>().SetTrigger("Ranged");
        yield return new WaitForSeconds(.8f);
        var newCouche = Instantiate(couchePrefab);
        var pos = transform.position;
        pos.y += 1f;
        newCouche.transform.position = pos;
        
        newCouche.GetComponent<Couche>().origin = pos;
        newCouche.GetComponent<Couche>().target = target;
        newCouche.GetComponent<Couche>().color = color;
        newCouche.GetComponent<Couche>().damage = damage;
    }
    
    private void SetAlpha(float alpha)
    {
        SpriteRenderer[] children = skins[_activeSkinIndex].GetComponentsInChildren<SpriteRenderer>();
        Color newColor;
        foreach(SpriteRenderer child in children) {
            newColor = child.color;
            newColor.a = alpha;
            child.color = newColor;
        }
    }
    
    private void SetColor(Color color)
    {
        SpriteRenderer[] children = skins[_activeSkinIndex].GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer child in children) {
            child.color = color;
        }
    }

    private void Die()
    {
        diePanel.SetActive(true);
        Time.timeScale = 0;
    }
}
