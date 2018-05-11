using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Characther : MonoBehaviour
{
    public const int RIGHT = 1;
    public const int LEFT = 0;
    public int Side { get; set; }
    SpriteRenderer Sprite;
    Animator Anim;

    public GameObject Barrel;
    public GameObject BarrelEnemyRight;
    public GameObject BarrelEnemyLeft;

    private List<GameObject> Barrels;
    private bool _lastIsAEnemy = false;

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
        AddNewBarrel();
        if (CheckEnemy(Barrels[0]))
            Die();
    }

    private void InitiateBarrels()
    {
        float y = -2.597116f;
        for (int i = 0; i < 10; i++)
        {
            Barrels.Add(Instantiate(SelectBarrel(i), new Vector3(0f, y, (float)-1), Quaternion.identity));
            y = y + 1.28f;
        }
    }

    private void AddNewBarrel()
    {
        Barrels.Add(Instantiate(SelectBarrel(), new Vector3(0f, (Barrels.Count - 2) * 1.28f, (float)-1f), Quaternion.identity));
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
}
