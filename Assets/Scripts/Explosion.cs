using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Explo());
    }

    private void Update()
    {
        transform.position = new Vector2(transform.position.x - Time.deltaTime * 2f, transform.position.y);
    }

    private IEnumerator Explo()
    {
        GetComponent<Animator>().SetTrigger("explo");
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
