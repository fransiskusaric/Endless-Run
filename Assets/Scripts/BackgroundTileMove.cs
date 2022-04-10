using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTileMove : MonoBehaviour
{
    private static float maxSpeed = 15f;
    public float speed = 8f;
    private float acceleration = 0.1f;

    private int counter = 0;
    private bool isFalling = false;

    private void Start()
    {
        //InvokeRepeating("AddSpeed", 5f, 5f);

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = Instantiate(transform.GetChild(i).gameObject, ControlPoint.main.bgPool);
            obj.SetActive(false);
        }
    }

    private void Update()
    {
        // Running
        if (ControlPoint.main.characterModel.state == CharacterModel.State.Running)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform tile = transform.GetChild(i);
                tile.position += Vector3.back * speed * Time.deltaTime;
                if (tile.position.z < -10)
                {
                    // Not random
                    //tile.position += new Vector3(0, 0, 60);
                    //tile.SetAsLastSibling();

                    //ClearObjects(tile);
                    //SpawnObjects(tile);
                    

                    // Random (use pooling system)
                    // 1. Return the last tile to the pool
                    tile.parent = ControlPoint.main.bgPool;
                    tile.gameObject.SetActive(false);
                    ClearObjects(tile);

                    // 2. Take tile from the pool randomly
                    int childIndex = Random.Range(0, ControlPoint.main.bgPool.childCount);
                    tile = ControlPoint.main.bgPool.GetChild(childIndex);
                    tile.position = transform.GetChild(transform.childCount - 1).position + 10 * Vector3.forward;
                    tile.parent = transform;
                    tile.gameObject.SetActive(true);
                    SpawnObjects(tile);

                    counter++;
                }
            }
            if (isFalling)
                isFalling = false;
        }

        // Backward running
        else if (ControlPoint.main.characterModel.state == CharacterModel.State.Backward)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform tile = transform.GetChild(i);
                tile.position += Vector3.forward * (speed / 2f) * Time.deltaTime;
                if (tile.position.z > 50)
                {
                    tile.position += new Vector3(0, 0, -60);
                    tile.SetAsFirstSibling();

                    ClearObjects(tile);
                }
            }
        }

        // Add speed every 6 tiles
        if (counter >= 6)
        {
            AlterSpeed(acceleration);
            if (acceleration < 0.5f)
                acceleration += 0.1f;
            counter = 0;
        }

        // Reduce speed when falling
        if (ControlPoint.main.characterModel.state == CharacterModel.State.Falling)
        {
            if (!isFalling)
            {
                isFalling = true;
                AlterSpeed(speed / -8);
                acceleration = 0.1f;
            }
        }
    }

    private void AlterSpeed(float accelerate)
    {
        speed += accelerate;
        speed = Mathf.Round(speed * 10.0f) * 0.1f;
        //print("speed: " + speed);
        if (speed > maxSpeed)
            speed = maxSpeed;
    }

    
    public void SpawnObjects(Transform tile)
    {
        // Spawn obstacles
        int obstaclePath = Random.Range(-1, 2);
        if (ControlPoint.main.obstaclePool.childCount > 0)
        {
            int childIndex = Random.Range(0, ControlPoint.main.obstaclePool.childCount);
            Transform obj = ControlPoint.main.obstaclePool.GetChild(childIndex); // Get random obstacle
            obj.parent = tile.GetChild(0); // Refer to ObstacleContainer
            obj.localPosition = new Vector3(ControlPoint.main.characterModel.pathSpacing * obstaclePath, 0.2f, 0);
            obj.gameObject.SetActive(true);
        }

        // Spawn coins
        bool doubleSpawn = Random.Range(0, 100) < 25;
        if (doubleSpawn)
        {
            for (int i = -1; i <= 1; i++)
            {
                if (i != obstaclePath && ControlPoint.main.coinPool.childCount > 0)
                {
                    Transform obj = ControlPoint.main.coinPool.GetChild(0); // Get coin
                    obj.parent = tile.GetChild(1); // Refer to CoinContainer
                    obj.localPosition = new Vector3(ControlPoint.main.characterModel.pathSpacing * i, 0, 0);
                    obj.gameObject.SetActive(true);
                }
            }
        }
        else if (ControlPoint.main.coinPool.childCount > 0)
        {
            int coinPath = 0;
            if (obstaclePath == -1)
            {
                if (Random.Range(0, 100) < 50)
                    coinPath = 1;
            }
            else if (obstaclePath == 0)
            {
                if (Random.Range(0, 100) < 50)
                    coinPath = -1;
                else
                    coinPath = 1;
            }
            else if (obstaclePath == 1)
            {
                if (Random.Range(0, 100) < 50)
                    coinPath = -1;
            }
            Transform obj = ControlPoint.main.coinPool.GetChild(0);
            obj.parent = tile.GetChild(1);
            obj.localPosition = new Vector3(ControlPoint.main.characterModel.pathSpacing * coinPath, 0, 0);
            obj.gameObject.SetActive(true);
        }
    }
    
    public void ClearObjects(Transform tile)
    {
        // Clear obstacles
        while (tile.GetChild(0).childCount > 0)
        {
            Transform obj = tile.GetChild(0).GetChild(0);
            obj.parent = ControlPoint.main.obstaclePool;
            obj.gameObject.SetActive(false);
        }

        // Clear coins
        while (tile.GetChild(1).childCount > 0)
        {
            Transform obj = tile.GetChild(1).GetChild(0);
            obj.parent = ControlPoint.main.coinPool;
            obj.gameObject.SetActive(false);

            // Reactivate individual coin
            for (int i = 0; i < obj.childCount; i++)
            {
                Transform coin = obj.GetChild(i);
                coin.gameObject.SetActive(true);
            }
        }
    }
}
