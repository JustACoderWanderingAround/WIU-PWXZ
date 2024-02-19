using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeartbeatBeater : MiniGame
{
    [SerializeField] GameObject leftHalf;
    [SerializeField] GameObject rightHalf;

    [SerializeField] GameObject leftSpawner;
    [SerializeField] GameObject rightSpawner;
    [SerializeField] GameObject target;
    float spawnTimer = 0;
    public float maxSpawnTimer;
    bool spawnLeft;
    List<GameObject> halfQueue = new List<GameObject>();
    float totalScore;
    [SerializeField] Slider timerSlider;
    private void OnEnable()
    {
        timer = 10;
        totalScore = 0;
        //mainScore.text = "Score: " + totalScore;
        maxSpawnTimer = Random.Range(0.5f, 1f);
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;
        if (timer < 0)
        {
            OnWin();
        }
        if (spawnTimer < 0 && timer > 0)
        {
            maxSpawnTimer = Random.Range(0.5f, 1f);
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
        timerSlider.value = timer * 0.1f;
    }
    private void FixedUpdate()
    {
        Collider2D[] halfsToDestroy = Physics2D.OverlapCircleAll(gameObject.transform.position, 10);
        if (halfsToDestroy.Length > 1)
            OnLose();
    }
    void SpawnBeat()
    {
        if (spawnLeft)
        {
            spawnLeft = false;
            GameObject newHalf = Instantiate(leftHalf, leftSpawner.transform.position, Quaternion.identity, gameObject.transform);
            newHalf.GetComponent<HeartHalf>().Init(target, 2 + Random.Range(0, 3));
        } else
        {
            spawnLeft = true;
            GameObject newHalf = Instantiate(rightHalf, rightSpawner.transform.position, Quaternion.identity, gameObject.transform);
            newHalf.GetComponent<HeartHalf>().Init(target, 2 + Random.Range(0, 3));
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
    protected override void OnLose()
    {
        Collider2D[] halfsToDestroy = Physics2D.OverlapCircleAll(gameObject.transform.position, 1920);
        foreach (Collider2D col in halfsToDestroy)
        {
            if (col.gameObject.CompareTag("LeftHalf") || col.gameObject.CompareTag("RightHalf"))
                Destroy(col.gameObject);
        }
        BaseEventData eventData = new BaseEventData(EventSystem.current);
        eventData.selectedObject = this.gameObject;
        OnLoseCallBack.Invoke(eventData);
    }
    void EvaluateScore(GameObject go)
    {
        float score = 500 - Vector3.Distance(go.transform.position, gameObject.transform.position);
        totalScore += score;
        Destroy(go);
        
    }
}
