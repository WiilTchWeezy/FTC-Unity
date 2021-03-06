﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{

    Animator Anim = null;
    // Use this for initialization
    void Start()
    {
        Anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AnimRight()
    {
        if (Anim.gameObject.activeSelf)
        {
            Debug.Log("AnimRight");
            Anim.SetBool("Right", true);
            StartCoroutine(DestroyObj());
        }
    }

    public void AnimLeft()
    {
        if (Anim.gameObject.activeSelf)
        {
            Debug.Log("AnimLeft");
            Anim.SetBool("Left", true);
            StartCoroutine(DestroyObj());
        }
    }

    public IEnumerator DestroyObj()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
        Debug.Log("DestroyCalled");
    }
}
