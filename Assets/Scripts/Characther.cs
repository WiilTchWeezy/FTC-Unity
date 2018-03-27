using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characther : MonoBehaviour
{
    public const int RIGHT = 1;
    public const int LEFT = 0;
    public int Side { get; set; }
    SpriteRenderer Sprite;
    Animator Anim;
    // Use this for initialization
    void Start()
    {
        Sprite = GetComponentInChildren<SpriteRenderer>();
        Anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Touch touch;
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            Anim.SetFloat("Hitting", 1f);
            if (touch.position.x < Screen.width / 2)
                GoLeft();
            else if (touch.position.x > Screen.width / 2)
                GoRight();
        }
        else
        {
            Anim.SetFloat("Hitting", -1f);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Anim.SetFloat("Hitting", 1f);
            if (Input.mousePosition.x < Screen.width / 2)
                GoLeft();
            else if (Input.mousePosition.x > Screen.width / 2)
                GoRight();
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
}
