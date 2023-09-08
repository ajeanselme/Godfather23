using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Loop : MonoBehaviour
{
    private SpriteRenderer _renderer;


    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _renderer.material.mainTextureOffset = new Vector2(Time.fixedTime * .2f, 0f);
    }
}
