using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour {

    Animator Anim;
	// Use this for initialization
	void Start () {
        Anim = GetComponentInChildren<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AnimRight()
    {
        Anim.SetBool("Right", true);
    }

    public void AnimLeft()
    {
        Anim.SetBool("Left", true);
    }
}
