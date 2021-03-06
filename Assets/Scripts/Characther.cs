﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GoogleMobileAds.Api;


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
    public GameObject IceBarrel;
    public Text LabelPoints;
    public Text LabelPointsFinal;
    public Text LabelBestScore;
    public AudioClip HitSoft;
    public AudioClip Success;
    public AudioClip BestScore;
    public AudioClip Fail;
    public AudioClip IceHit;
    public AudioClip IceBreak;
    public Slider slider;
    public GameObject Panel;
    public Button ButtonSound;
    public Sprite SoundOn;
    public Sprite SoundOff;

    private List<GameObject> Barrels;
    private bool _lastIsAEnemy = false;
    private bool _lastIsIce = false;
    private int points = 0;
    private float lifeDownSpeed = 0.004f;
    private bool isAlive;
    private int _bestScore = 0;
    private BannerView bannerView;


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
        Panel.SetActive(false);
        isAlive = true;
        InitPlayerPrefs();
        _bestScore = LoadGameBestScore();
        InitAudioState();
#if UNITY_ANDROID
        string appId = "ca-app-pub-1586874665810792~8191328678";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-3940256099942544~1458002511";
#else
            string appId = "unexpected_platform";
#endif
        MobileAds.Initialize(appId);
        this.RequestBanner();

    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-1586874665810792/1617702534";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    private void InitPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("bestScore") == false)
        {
            PlayerPrefs.SetInt("bestScore", 0);
            PlayerPrefs.Save();
        }
        else
        {
            if (PlayerPrefs.GetInt("bestScore") == 64)
            {
                PlayerPrefs.SetInt("bestScore", 0);
                PlayerPrefs.Save();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            Touch touch;
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        touch = Input.GetTouch(i);
                        Hit(touch);
                    }
                }

            }
            else
            {
                Anim.SetFloat("Hitting", -1f);
            }

            //if (Input.GetButtonDown("Fire1"))
            //{
            //    Hit();
            //}
            //else
            //{
            //    Anim.SetFloat("Hitting", -1f);
            //}
            slider.value -= lifeDownSpeed;
        }
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
        if (CheckEnemy(Barrels[0]))
            Die();
        Anim.SetFloat("Hitting", 1f);
        if (touch.position.x < Screen.width / 2)
            GoLeft();
        else if (touch.position.x > Screen.width / 2)
            GoRight();
        var barrel = AnimateBarrel();
        if (CheckEnemy(Barrels[0]))
            Die();
        else
        {
            var pointsAdded = AddPoints(barrel);
            AddLifePoints(barrel);
            switch (points)
            {
                case 10:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.006f;
                    break;
                case 30:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.008f;
                    break;
                case 50:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.010f;
                    break;
                case 100:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.013f;
                    break;
                case 150:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.016f;
                    break;
                case 200:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.020f;
                    break;
                default:
                    if (barrel != null)
                    {
                        if (barrel.CompareTag("IceBarrel"))
                        {
                            var iceBarrelScript = barrel.GetComponent<IceBarrel>();
                            if (iceBarrelScript.IceIsBreaked())
                            {
                                Audio.PlayOneShot(IceBreak);
                                StartCoroutine(FrozeLife());
                            }
                            else if (iceBarrelScript.HaveNoIce())
                                Audio.PlayOneShot(HitSoft);
                            else
                                Audio.PlayOneShot(IceHit);
                        }
                        else
                        {
                            if (pointsAdded)
                                Audio.PlayOneShot(HitSoft);
                        }
                    }
                    else
                    {
                        if (pointsAdded)
                            Audio.PlayOneShot(HitSoft);
                    }
                    break;
            }
            if (points == _bestScore)
            {
                Audio.PlayOneShot(BestScore);
            }
            LabelPoints.text = points.ToString();
        }

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
        var barrel = AnimateBarrel();
        if (CheckEnemy(Barrels[0]))
            Die();
        else
        {
            var pointsAdded = AddPoints(barrel);
            AddLifePoints(barrel);
            switch (points)
            {
                case 10:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.006f;
                    break;
                case 30:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.008f;
                    break;
                case 50:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.010f;
                    break;
                case 100:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.013f;
                    break;
                case 150:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.016f;
                    break;
                case 200:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.020f;
                    break;
                case 500:
                    if (pointsAdded)
                        Audio.PlayOneShot(Success);
                    lifeDownSpeed = 0.025f;
                    break;
                default:
                    if (barrel != null)
                    {
                        if (barrel.CompareTag("IceBarrel"))
                        {
                            var iceBarrelScript = barrel.GetComponent<IceBarrel>();
                            if (iceBarrelScript.IceIsBreaked())
                            {
                                Audio.PlayOneShot(IceBreak);
                                StartCoroutine(FrozeLife());
                            }
                            else if (iceBarrelScript.HaveNoIce())
                                Audio.PlayOneShot(HitSoft);
                            else
                                Audio.PlayOneShot(IceHit);
                        }
                        else
                        {
                            if (pointsAdded)
                                Audio.PlayOneShot(HitSoft);
                        }
                    }
                    else
                    {
                        if (pointsAdded)
                            Audio.PlayOneShot(HitSoft);
                    }
                    break;
            }
            if (points == _bestScore)
            {
                Audio.PlayOneShot(BestScore);
            }
            LabelPoints.text = points.ToString();
        }

    }

    private void AddLifePoints(GameObject barrel)
    {
        if (barrel != null)
        {
            slider.value += 0.2f;
        }
    }

    private bool AddPoints(GameObject barrel)
    {
        if (barrel != null)
        {
            if (barrel.CompareTag("IceBarrel"))
            {
                var iceBarrelScript = barrel.GetComponent<IceBarrel>();
                if (iceBarrelScript.HaveNoIce())
                {
                    points++;
                    return true;
                }
                else
                    return false;
            }
            else
            {
                points++;
                return true;
            }
        }
        return false;
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

        if (points > 50 && points < 100)
        {
            var luckyNumber = Random.Range(0, 20);
            if (luckyNumber == 8 && _lastIsIce == false)
            {
                barrel = IceBarrel;
                _lastIsIce = true;
            }
            else
            {
                _lastIsIce = false;
            }
        }
        if (points > 100 && points < 150)
        {
            var luckyNumber = Random.Range(0, 30);
            if (luckyNumber == 8 && _lastIsIce == false)
            {
                barrel = IceBarrel;
                _lastIsIce = true;
            }
            else
            {
                _lastIsIce = false;
            }
        }

        if (points > 250 && points < 300)
        {
            var luckyNumber = Random.Range(0, 40);
            if (luckyNumber == 8 && _lastIsIce == false)
            {
                barrel = IceBarrel;
                _lastIsIce = true;
            }
            else
            {
                _lastIsIce = false;
            }
        }
        if (points > 500)
        {
            var luckyNumber = Random.Range(0, 60);
            if (luckyNumber == 8 && _lastIsIce == false)
            {
                barrel = IceBarrel;
                _lastIsIce = true;
            }
            else
            {
                _lastIsIce = false;
            }
        }

        return barrel;
    }

    private GameObject AnimateBarrel()
    {
        if (Barrels.Count > 0)
        {
            var lastBarrel = Barrels[0];
            if (lastBarrel.CompareTag("IceBarrel"))
            {
                var iceBarrelScrpit = lastBarrel.GetComponentInChildren<IceBarrel>();
                if (iceBarrelScrpit.Hit(Side))
                {
                    Barrels.Remove(lastBarrel);
                    foreach (var item in Barrels)
                        item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y - 1.28f);
                    AddNewBarrel();
                }
            }
            else
            {
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

                AddNewBarrel();
            }
            return lastBarrel;
        }
        return null;
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
        Audio.PlayOneShot(Fail);
        if (points == 64)
            points++;
        LabelPointsFinal.text = points.ToString();
        Panel.SetActive(true);
        isAlive = false;
        Anim.SetFloat("Hitting", -1f);
        if (points > _bestScore)
        {
            _bestScore = points;
            SaveGameBestScore(_bestScore);
        }
        LabelBestScore.text = _bestScore.ToString();
        slider.value = 0;
    }

    public void SliderChanged()
    {
        if (slider.value <= 0f)
        {
            Die();
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(1);
    }

    private void SaveGameBestScore(int score)
    {
        PlayerPrefs.SetInt("bestScore", score);
        PlayerPrefs.Save();
    }

    private int LoadGameBestScore()
    {
        if (PlayerPrefs.HasKey("bestScore"))
            return PlayerPrefs.GetInt("bestScore", 0);
        else
            return 0;
    }

    public void MuteSound()
    {
        if (ButtonSound.image.sprite == SoundOn)
        {
            Audio.volume = 0f;
            ButtonSound.image.sprite = SoundOff;
            SaveSoundState(false);
        }
        else
        {
            Audio.volume = 1f;
            ButtonSound.image.sprite = SoundOn;
            SaveSoundState(true);
        }
    }

    private void SaveSoundState(bool state)
    {
        if (state)
            PlayerPrefs.SetInt("AudioState", 1);
        else
            PlayerPrefs.SetInt("AudioState", 0);

        PlayerPrefs.Save();
    }

    private bool GetSoundState()
    {
        if (PlayerPrefs.HasKey("AudioState"))
        {
            var audioState = PlayerPrefs.GetInt("AudioState");
            if (audioState == 0)
                return false;
            else
                return true;
        }
        else
        {
            SaveSoundState(true);
            return true;
        }
    }

    private void InitAudioState()
    {
        if (GetSoundState())
        {
            Audio.volume = 1f;
            ButtonSound.image.sprite = SoundOn;
        }
        else
        {
            Audio.volume = 0f;
            ButtonSound.image.sprite = SoundOff;
        }
    }

    private IEnumerator FrozeLife()
    {
        lifeDownSpeed = 0;
        var anim = slider.GetComponent<Animator>();
        anim.SetBool("IsFrozed", true);
        yield return new WaitForSeconds(3);
        anim.SetBool("IsFrozed", false);
        if (points < 10)
            lifeDownSpeed = 0.006f;
        else if (points >= 30)
            lifeDownSpeed = 0.008f;
        else if (points >= 50)
            lifeDownSpeed = 0.010f;
        else if (points >= 100)
            lifeDownSpeed = 0.013f;
        else if (points >= 150)
            lifeDownSpeed = 0.016f;
        else if (points >= 200)
            lifeDownSpeed = 0.020f;
        else if (points >= 500)
            lifeDownSpeed = 0.025f;
        else
            lifeDownSpeed = 0.006f;

    }
}
