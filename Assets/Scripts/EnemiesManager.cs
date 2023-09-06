using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[System.Serializable]
public class PlayerAttackEvent : UnityEvent<PlayerController.ButtonColor>
{
}

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;
    
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField] private Transform spawnerParent;
    
    private List<Transform> spawners = new ();
    
    private List<GameObject> enemies = new ();

    public PlayerAttackEvent playerAttackEvent = new ();


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
        enemies.Add(newEnemy);

        var controller = newEnemy.GetComponent<EnemyController>();
        playerAttackEvent.AddListener(controller.Hurt);
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
        Destroy(enemyController.gameObject);
    }
}
