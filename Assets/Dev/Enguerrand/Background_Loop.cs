using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Loop : MonoBehaviour
{
    [SerializeField] Transform _pos1;
    [SerializeField] Transform _pos2;
    [SerializeField] AnimationCurve anim;
    [SerializeField] float duration = 3.0f;
    [SerializeField] float backgroundSpeedupOverTime = 0.1f;

    // Update is called once per frame
    void Update()
    {
        float animTime = Mathf.Repeat(Time.time, duration);
        float t = animTime/duration;
        float t2 = anim.Evaluate(t);
        transform.position = Vector3.Lerp(_pos1.position, _pos2.position, t2);

        if(duration > 1)
        {
            duration -= backgroundSpeedupOverTime * Time.deltaTime;
            float animTime2 = Mathf.Repeat(Time.time, duration);
            float t1 = animTime2 / duration;
            float tbis = anim.Evaluate(t1);
            transform.position = Vector3.Lerp(_pos1.position, _pos2.position, tbis);
        }

    }
}
