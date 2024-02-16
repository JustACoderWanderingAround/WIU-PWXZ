using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartbeatBeater : MiniGame
{
    [SerializeField] GameObject leftHalf;
    [SerializeField] GameObject rightHalf;

    [SerializeField] GameObject leftSpawner;
    [SerializeField] GameObject rightSpawner;
    [SerializeField] GameObject target;
    float spawnTimer = 0;
    public float maxSpawnTimer = 1f;
    bool spawnLeft;
    List<GameObject> halfQueue = new List<GameObject>();
    float totalScore;

    private void OnEnable()
    {
        timer = 0;
        totalScore = 1000f;
    }
    private void Update()
    {
        if (totalScore < 0)
            //OnLose();
        if (timer < 0)
            OnWin();
        totalScore -= Time.deltaTime;
        timer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;
        if (spawnTimer < 0)
        {
            spawnTimer = maxSpawnTimer;
            SpawnBeat();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            DestroyHalf(true);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            DestroyHalf(false);
        }
    }
    void SpawnBeat()
    {
        if (spawnLeft)
        {
            spawnLeft = false;
            GameObject newHalf = Instantiate(leftHalf, leftSpawner.transform.position, Quaternion.identity, gameObject.transform);
            newHalf.GetComponent<HeartHalf>().Init(target, 3.0f);
        } else
        {
            spawnLeft = true;
            GameObject newHalf = Instantiate(rightHalf, rightSpawner.transform.position, Quaternion.identity, gameObject.transform);
            newHalf.GetComponent<HeartHalf>().Init(target, 3.0f);
        }
    }
    void DestroyHalf(bool isLeft)
    {

        Collider2D[] halfsToDestroy = Physics2D.OverlapCircleAll(gameObject.transform.position, 175);
        foreach (Collider2D col in halfsToDestroy)
        {
            if (isLeft && col.gameObject.CompareTag("LeftHalf"))
                EvaluateScore(col.gameObject);
            else if (!isLeft && col.gameObject.CompareTag("RightHalf"))
                EvaluateScore(col.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        totalScore -= 100f;
        Destroy(collision.gameObject);
    }
    void EvaluateScore(GameObject go)
    {
        float score = Vector3.Distance(go.transform.position, gameObject.transform.position);
        totalScore += score;
    }
}
