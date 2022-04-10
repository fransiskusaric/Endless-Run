using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("General References")]
    public CharacterModel characterModel;
    private GameObject characterPrefab;
    private Transform mainCamera;
    public BackgroundTileMove bgTile;
    public Transform bgPool;
    private int index = 0;

    [Header("Obstacle References")]
    public Transform obstaclePool;
    public GameObject[] obstaclePrefabs;
    public int obstaclePreloadCount = 5;

    [Header("Coin References")]
    public Transform coinPool;
    public GameObject coinPrefab;
    public int coinPreloadCount = 20;

    [Header("User Interface")]
    public Text scoreTxt;
    public TextMeshProUGUI cdText;

    public static PlayerController main;

    private void Awake()
    {
        main = this;
        
        // Preload obstacles
        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            for (int counter = 0; counter < obstaclePreloadCount; counter++)
            {
                GameObject obj = Instantiate(obstaclePrefabs[i], obstaclePool);
                obj.SetActive(false);
            }
        }

        // Preload coin
        for (int i = 0; i < coinPreloadCount; i++)
        {
            GameObject obj = Instantiate(coinPrefab, coinPool);
            obj.SetActive(false);
        }
    }

    private void Start()
    {
        mainCamera = transform.GetChild(0).GetComponent<Transform>();
        index = PlayerPrefs.GetInt("characterIndex");
        var loadPrefab = new Object();
        if (index == 0)
        {
            loadPrefab = Resources.Load("Models/Character/Prefabs/Boy");
        }
        else if (index == 1)
        {
            loadPrefab = Resources.Load("Models/Character/Prefabs/Rin");
            mainCamera.position += mainCamera.forward * -1.5f;
        }
        else if (index == 2)
        {
            loadPrefab = Resources.Load("Models/Character/Prefabs/Satomi");
            mainCamera.position += mainCamera.forward * -1.5f;
        }
        print("index:" + index);
        print("prefab" + loadPrefab);
        InstantiateCharacter(loadPrefab);
        if (scoreTxt == null)
            scoreTxt = GameObject.Find("Canvas").transform.Find("ScoreText").GetComponent<Text>();
    }

    private void InstantiateCharacter(Object prefab)
    {
        //resources/models/character/
        characterPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity, gameObject.transform) as GameObject;
        characterModel = characterPrefab.GetComponent<CharacterModel>();
        characterModel.SetAnimator();
        characterModel.TriggerAnimation("runTrigger");
    }
}
