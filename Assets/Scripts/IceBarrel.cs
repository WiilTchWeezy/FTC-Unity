using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBarrel : MonoBehaviour
{
    public Sprite IceBrokeSprite;
    public Sprite BarrelSprite;

    private int hit = 0;
    private bool _isLastHit = false;

    private SpriteRenderer _spriteRenderer;
    private Animator _anim;
    // Use this for initialization
    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool Hit(int direction)
    {
        hit++;
        switch (hit)
        {
            case 1:
                _spriteRenderer.sprite = IceBrokeSprite;
                _isLastHit = false;
                break;
            case 2:
                _spriteRenderer.sprite = BarrelSprite;
                _isLastHit = false;
                break;
            case 3:
                if (direction == 1)
                    AnimLeft();
                else
                    AnimRight();
                _isLastHit = true;
                break;
            default:
                break;
        }
        return _isLastHit;
    }

    public void AnimRight()
    {
        if (_anim.gameObject.activeSelf)
        {
            Debug.Log("AnimRight");
            _anim.SetBool("Right", true);
            StartCoroutine(DestroyObj());
        }
    }

    public void AnimLeft()
    {
        if (_anim.gameObject.activeSelf)
        {
            Debug.Log("AnimLeft");
            _anim.SetBool("Left", true);
            StartCoroutine(DestroyObj());
        }
    }

    public IEnumerator DestroyObj()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
        Debug.Log("DestroyCalled");
    }

    public bool IceIsBreaked()
    {
        return hit == 2;
    }


    public bool HaveNoIce()
    {
        return hit == 3;
    }
}
