﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characther : MonoBehaviour
{
    public const int RIGHT = 1;
    public const int LEFT = 0;
    public int Side { get; set; }
    SpriteRenderer Sprite;
    Animator Anim;

    public GameObject Barrel;

    private List<GameObject> Barrels;

    // Use this for initialization
    void Start()
    {
        Sprite = GetComponentInChildren<SpriteRenderer>();
        Anim = GetComponentInChildren<Animator>();
        Barrels = new List<GameObject>();
        InitiateBarrels();
    }

    // Update is called once per frame
    void Update()
    {
        Touch touch;
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            Hit(touch);
        }
        else
        {
            Anim.SetFloat("Hitting", -1f);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Hit();
        }
        else
        {
            Anim.SetFloat("Hitting", -1f);
        }
    }

    public void GoLeft()
    {
        gameObject.transform.position = new Vector3((float)-1.31, (float)-2.597116, (float)-1);
        Sprite.flipX = false;
        Side = LEFT;
    }

    public void GoRight()
    {
        gameObject.transform.position = new Vector3((float)1.31, (float)-2.597116, (float)-1);
        Sprite.flipX = true;
        Side = RIGHT;
    }

    public void Hit(Touch touch)
    {
        Anim.SetFloat("Hitting", 1f);
        if (touch.position.x < Screen.width / 2)
            GoLeft();
        else if (touch.position.x > Screen.width / 2)
            GoRight();
        AnimateBarrel();
    }

    public void Hit()
    {
        Anim.SetFloat("Hitting", 1f);
        if (Input.mousePosition.x < Screen.width / 2)
            GoLeft();
        else if (Input.mousePosition.x > Screen.width / 2)
            GoRight();
        AnimateBarrel();
    }

    private void InitiateBarrels()
    {
        float y = -2.597116f;
        for (int i = 0; i < 20; i++)
        {
            Barrels.Add(Instantiate(Barrel, new Vector3(0f, y, (float)-1), Quaternion.identity));
            y = y + 1.28f;
        }
    }

    private void AnimateBarrel()
    {
        if (Barrels.Count > 0)
        {
            var lastBarrel = Barrels[0];
            var barrelScrpit = lastBarrel.GetComponentInChildren<Barrel>();
            if (Side == RIGHT)
            {
                barrelScrpit.AnimLeft();
            }
            else if (Side == LEFT)
            {
                barrelScrpit.AnimRight();
            }
        }
    }
}
