using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;
    
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField] private Transform spawner;
    
    private List<GameObject> enemies = new ();


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

    public void SpawnEnemy()
    {
        var newEnemy = Instantiate(enemyPrefab, spawner);
        enemies.Add(newEnemy);
        Debug.Log("Spawn enemy");
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
