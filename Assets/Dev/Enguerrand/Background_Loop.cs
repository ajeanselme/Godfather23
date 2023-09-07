using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Loop : MonoBehaviour
{
    [SerializeField] Transform _pos1;
    [SerializeField] Transform _pos2;
    [SerializeField] AnimationCurve anim;
    [SerializeField] float duration = 2.0f; 

    // Update is called once per frame
    void Update()
    {
        float animTime = Mathf.Repeat(Time.time, duration);
        float t = animTime/duration;
        float t2 = anim.Evaluate(t);
        transform.position = Vector3.Lerp(_pos1.position, _pos2.position, t2);
    }
}
