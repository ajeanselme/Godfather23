using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[System.Serializable]
public class PlayerMeleeAttackEvent : UnityEvent<PlayerController.ButtonColor> { }
public class PlayerRangedAttackEvent : UnityEvent<PlayerController.ButtonColor> { }

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;
    
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField] private Transform spawnerParent;
    
    private List<Transform> spawners = new ();
    
    private List<EnemyController> enemies = new ();

    public PlayerMeleeAttackEvent playerMeleeAttackEvent = new ();
    public PlayerRangedAttackEvent playerRangedAttackEvent = new ();


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
            spawners.Add(spawnerParent.GetChild(i));
        }
    }

    public void SpawnEnemy()
    {
        var random = Random.Range(0, spawners.Count);
        var spawner = spawners[random];
        var newEnemy = Instantiate(enemyPrefab, spawner);

        var controller = newEnemy.GetComponent<EnemyController>();
        enemies.Add(controller);
        
        playerMeleeAttackEvent.AddListener(controller.HurtMelee);
        playerRangedAttackEvent.AddListener(controller.HurtDistance);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnEnemy();
        }
    }

    public void KillEnemy(EnemyController enemyController)
    {
        enemies.Remove(enemyController);
        Destroy(enemyController.gameObject);
    }

    public EnemyController GetClosestEnemy(PlayerController.ButtonColor buttonColor)
    {
        List<EnemyController> coloredEnemies = (from enemy in enemies
            where enemy.hurtColor == buttonColor
            select enemy).ToList();
        
        coloredEnemies.Sort(SortByDistance);

        if (coloredEnemies.Count <= 0)
        {
            return null;
        }

        return coloredEnemies.First();
    }
    
    private int SortByDistance(EnemyController p1, EnemyController p2)
    {
        return p1.distanceToPlayer.CompareTo(p2.distanceToPlayer);
    }
}
