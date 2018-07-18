using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBarrel : Barrel
{

    Animator Anim = null;

    public Sprite IceBrokeSprite;
    public Sprite BarrelSprite;

    private int hit = 0;
    private bool _isLastHit = false;

    private SpriteRenderer _spriteRenderer;
    // Use this for initialization
    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
                    AnimRight();
                else
                    AnimLeft();
                _isLastHit = true;
                break;
            default:
                break;
        }
        return _isLastHit;
    }
}
