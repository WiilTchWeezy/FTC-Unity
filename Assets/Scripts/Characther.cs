﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Characther : MonoBehaviour
{
    public const int RIGHT = 1;
    public const int LEFT = 0;
    public int Side { get; set; }
    SpriteRenderer Sprite;
    Animator Anim;
    AudioSource Audio;

    public GameObject Barrel;
    public GameObject BarrelEnemyRight;
    public GameObject BarrelEnemyLeft;
    public Text LabelPoints;
    public AudioClip HitSoft;
    public AudioClip Success;
    public AudioClip BestScore;
    public Slider slider;

    private List<GameObject> Barrels;
    private bool _lastIsAEnemy = false;
    private int points = 0;
    private float lifeDownSpeed = 0.004f;
    private int bestScore = 16;

    // Use this for initialization
    void Start()
    {
        Sprite = GetComponentInChildren<SpriteRenderer>();
        Anim = GetComponentInChildren<Animator>();
        Audio = GetComponentInChildren<AudioSource>();
        Barrels = new List<GameObject>();
        InitiateBarrels();
        Audio.clip = HitSoft;
        slider.value = 1f;
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
        slider.value -= lifeDownSpeed;
    }

    public void GoLeft()
    {
        gameObject.transform.position = new Vector3((float)-1.31, (float)-2.597116, (float)-1);
        Sprite.flipX = false;
        Side = LEFT;
        if (CheckEnemy(Barrels[0]))
            Die();
    }

    public void GoRight()
    {
        gameObject.transform.position = new Vector3((float)1.31, (float)-2.597116, (float)-1);
        Sprite.flipX = true;
        Side = RIGHT;
        if (CheckEnemy(Barrels[0]))
            Die();
    }

    public void Hit(Touch touch)
    {
        Anim.SetFloat("Hitting", 1f);
        if (CheckEnemy(Barrels[0]))
            Die();
        if (touch.position.x < Screen.width / 2)
            GoLeft();
        else if (touch.position.x > Screen.width / 2)
            GoRight();
        AnimateBarrel();
    }

    public void Hit()
    {
        if (CheckEnemy(Barrels[0]))
            Die();
        Anim.SetFloat("Hitting", 1f);
        if (Input.mousePosition.x < Screen.width / 2)
            GoLeft();
        else if (Input.mousePosition.x > Screen.width / 2)
            GoRight();
        AnimateBarrel();
        AddNewBarrel();
        if (CheckEnemy(Barrels[0]))
            Die();
        else
        {
            points++;
            slider.value += 0.2f;
            switch (points)
            {
                case 10:
                    Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.006f;
                    break;
                case 30:
                    Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.008f;
                    break;
                case 50:
                    Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.010f;
                    break;
                case 100:
                    Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.013f;
                    break;
                case 150:
                    Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.016f;
                    break;
                case 200:
                    Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.020f;
                    break;
                default:
                    Audio.PlayOneShot(HitSoft);
                    break;
            }
            if (points == bestScore)
            {
                Audio.PlayOneShot(BestScore);
            }
            LabelPoints.text = points.ToString();
        }

    }

    private void InitiateBarrels()
    {
        float y = -2.597116f;
        for (int i = 0; i < 10; i++)
        {
            var barrel = SelectBarrel(i);
            Vector3 position;
            if (barrel.CompareTag("EnemyLeft"))
                position = new Vector3(-0.59f, y, (float)-1f);
            else
                position = new Vector3(0f, y, (float)-1f);

            Barrels.Add(Instantiate(barrel, position, Quaternion.identity));
            y = y + 1.28f;
        }
    }

    private void AddNewBarrel()
    {
        var barrel = SelectBarrel();
        Vector3 position;

        if (barrel.CompareTag("EnemyLeft"))
            position = new Vector3(-0.59f, (Barrels.Count - 2) * 1.28f, (float)-1f);
        else
            position = new Vector3(0f, (Barrels.Count - 2) * 1.28f, (float)-1f);

        Barrels.Add(Instantiate(barrel, position, Quaternion.identity));
    }

    private GameObject SelectBarrel(int index = -1)
    {
        if (index == 0 || index == 1)
            return Barrel;
        if (_lastIsAEnemy)
        {
            _lastIsAEnemy = false;
            return Barrel;
        }
        var ramdomNumber = Random.Range(0, 4);
        GameObject barrel = null;
        switch (ramdomNumber)
        {
            case 0:
                barrel = Barrel;
                _lastIsAEnemy = false;
                break;
            case 1:
                barrel = BarrelEnemyRight;
                _lastIsAEnemy = true;
                break;
            case 2:
                barrel = BarrelEnemyLeft;
                _lastIsAEnemy = true;
                break;
            case 3:
                barrel = BarrelEnemyLeft;
                _lastIsAEnemy = true;
                break;
            case 4:
                barrel = BarrelEnemyRight;
                _lastIsAEnemy = true;
                break;
            default:
                barrel = Barrel;
                _lastIsAEnemy = false;
                break;
        }
        return barrel;
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
            Barrels.Remove(lastBarrel);
            foreach (var item in Barrels)
                item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y - 1.28f);
        }
    }

    private bool CheckEnemy(GameObject gameObject)
    {
        if (gameObject.CompareTag("EnemyRight") && Side == RIGHT)
            return true;
        else if (gameObject.CompareTag("EnemyLeft") && Side == LEFT)
            return true;
        else
            return false;
    }

    private void Die()
    {
        SceneManager.LoadScene(0);
    }

    public void SliderChanged()
    {
        if (slider.value <= 0f)
        {
            Die();
        }
    }
}
