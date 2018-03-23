using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characther : MonoBehaviour
{
    public AnimationClip clip;

    SpriteRenderer Sprite;
    Animation Anim;
    // Use this for initialization
    void Start()
    {
        Sprite = GetComponentInChildren<SpriteRenderer>();
        Anim = gameObject.GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        Touch touch;
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            Anim.Play("CharactherHit");
            if (touch.position.x < Screen.width / 2)
            {
                gameObject.transform.position = new Vector3((float)-1.31, (float)-2.597116, (float)-1);
                Sprite.flipX = false;
            }
            else if (touch.position.x > Screen.width / 2)
            {
                gameObject.transform.position = new Vector3((float)1.31, (float)-2.597116, (float)-1);
                Sprite.flipX = true;
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Anim.clip = clip;
            Anim.Play();
            if (Input.mousePosition.x < Screen.width / 2)
            {
                gameObject.transform.position = new Vector3((float)-1.31, (float)-2.597116, (float)-1);
                Sprite.flipX = false;
            }
            else if (Input.mousePosition.x > Screen.width / 2)
            {
                gameObject.transform.position = new Vector3((float)1.31, (float)-2.597116, (float)-1);
                Sprite.flipX = true;
            }
        }
    }
}
