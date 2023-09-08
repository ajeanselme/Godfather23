using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[System.Serializable]
public class PlayerMeleeAttackEvent : UnityEvent<PlayerController.ButtonColor, int> { }
public class PlayerRangedAttackEvent : UnityEvent<PlayerController.ButtonColor, int> { }

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;
    
    [SerializeField]
    private GameObject enemyMeleePrefab;
    [SerializeField]
    private GameObject enemyRangedPrefab;

    [SerializeField] private Transform spawnerParent;
    
    private List<Transform> _spawners = new ();
    
    private List<EnemyController> _enemies = new ();

    public PlayerMeleeAttackEvent playerMeleeAttackEvent = new ();
    public PlayerRangedAttackEvent playerRangedAttackEvent = new ();

    public int killCount = 0;
    public int currentKillProgress = 0;

    private int _currentWave = 0;
    
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

        for (int i = 0; i < spawnerParent.childCount; i++)
        {
            _spawners.Add(spawnerParent.GetChild(i));
        }
    }


    private void Update()
    {
        if (_enemies.Count <= 0)
        {
            StartNewWave();
        }
    }
    
    
    private void StartNewWave()
    {
        _currentWave++;
        int enemyAmount = (int) (_currentWave * 1.1);
        for (int i = 0; i < enemyAmount; i++)
        {
            SpawnEnemy();
        }
    }
    
    private void SpawnEnemy()
    {
        var random = Random.Range(0, _spawners.Count);
        var spawner = _spawners[random];
        GameObject newEnemy = null;
        
        random = Random.Range(0, 2);
        if (random == 0)
        {
            newEnemy = Instantiate(enemyMeleePrefab, spawner);
        } else if (random == 1)
        {
            newEnemy = Instantiate(enemyRangedPrefab, spawner);
        }

        var controller = newEnemy!.GetComponent<EnemyController>();
        _enemies.Add(controller);

        random = Random.Range(1, 4);
        controller.colorAmount = (int) (random * _currentWave / 5f);

        random = Random.Range(1, 4);
        controller.colorStreakNeeded = (int) (random * _currentWave / 5f);
        
        playerMeleeAttackEvent.AddListener(controller.HurtMelee);
        playerRangedAttackEvent.AddListener(controller.HurtDistance);
    }

    public void KillEnemy(EnemyController enemyController)
    {
        _enemies.Remove(enemyController);
        Destroy(enemyController.gameObject);

        killCount++;
        currentKillProgress++;

        PlayerController.Instance.AddScore(100);
    }

    public EnemyController GetClosestRangedEnemy(PlayerController.ButtonColor buttonColor)
    {
        List<EnemyController> coloredEnemies = (from enemy in _enemies
            where enemy.hurtColor == buttonColor
            where enemy.enemyType == EnemyController.EnemyType.Range
            select enemy).ToList();
        
        coloredEnemies.Sort(SortByDistance);

        if (coloredEnemies.Count <= 0)
        {
            return null;
        }

        return coloredEnemies.First();
    }

    public EnemyController[] GetEnemiesAround(Vector2 position, PlayerController.ButtonColor buttonColor, float range)
    {
        List<EnemyController> coloredEnemies = (from enemy in _enemies
            where enemy.hurtColor == buttonColor
            where Vector2.Distance(enemy.transform.position, position) <= range
            select enemy).ToList();

        return coloredEnemies.ToArray();
    }
    
    private int SortByDistance(EnemyController p1, EnemyController p2)
    {
        return p1.distanceToPlayer.CompareTo(p2.distanceToPlayer);
    }
}
