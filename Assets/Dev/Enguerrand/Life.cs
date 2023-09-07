using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    [SerializeField] Slider sliderLife;
    [SerializeField] int lifepoint = 3;
    [SerializeField] int maxLifePoint = 3;
    [SerializeField] int currentLifePoint;
    [SerializeField] bool isDead = false;


    // Start is called before the first frame update
    void Start()
    {
        currentLifePoint = lifepoint;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            takeDamage(1);
        }
    }

    private void takeDamage(int damage)
    {
        if (isDead == false)
        {
            currentLifePoint--;
            sliderLife.value = currentLifePoint;
        }
    }
}
