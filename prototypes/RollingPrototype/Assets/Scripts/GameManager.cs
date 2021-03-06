﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GameState
{
    Start = 0,
    Playing = 1,
    Ending = 2
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Components")]
    [SerializeField]
    GameObject leaderboard;
    [SerializeField]
    GameObject timer;
    [SerializeField]
    GameObject instruction;
    [SerializeField]
    GameObject minimap;
    [SerializeField]
    GetSpawningPoints spawningPoints;
    [SerializeField]
    ObjectSpawner objectSpawner;
    [SerializeField]
    GameObject recognition;
    [SerializeField]
    Goal goal;

    [Header("Game Components")]
    [SerializeField]
    Transform ship;
    [SerializeField]
    Transform player;
    [SerializeField]
    GameObject playerCamera;
    [SerializeField]
    GameObject endingCamera;

    [Header("Setting")]
    [SerializeField]
    [Range(0, 1)]
    float spawnThreshold;
    [SerializeField]
    [Range(0.1f, 0.5f)]
    float goalThresholdOutdoor;
    [SerializeField]
    [Range(0.1f, 0.5f)]
    float goalThresholdIndoor;

    [HideInInspector]
    public Dictionary<string, int> userLeaderboard; // most collected user leaderboard
    [HideInInspector]
    public Dictionary<string, int> objectLeaderboard;  // most collected object leaderboard
    [HideInInspector]
    public int objectsInScene;
    [HideInInspector]
    public int objectsIndoor;
    [HideInInspector]
    public int objectsOutdoor;
    [HideInInspector]
    public int totalNumObjects;
    [HideInInspector]
    public int totalNumPoints;
    [HideInInspector]
    public List<GameObject> allObjectsInScene;
    [HideInInspector]
    public TaskType currentTask;
    [HideInInspector]
    public HashSet<string> playerCollected;
    [HideInInspector]
    public GameState gameState;
    [HideInInspector]
    public int totalItemCollected;
    [HideInInspector]
    public int taskCompleted;
    [HideInInspector]
    public HashSet<string> uniqueCollection;

    void Awake()
    {
        if (instance == null)
            instance = this;

        spawningPoints.InitSpawningPoints();

        userLeaderboard = new Dictionary<string, int>();
        objectLeaderboard = new Dictionary<string, int>();
        allObjectsInScene = new List<GameObject>();
        playerCollected = new HashSet<string>();
        uniqueCollection = new HashSet<string>();

        gameState = GameState.Start;
        objectsInScene = 0;
        totalNumObjects = 0;

        totalNumPoints += spawningPoints.indoorList.Count;
        totalNumPoints += spawningPoints.outdoorList.Count;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    switch (gameState)
        //    {
        //        case GameState.Start:
        //            StartGame();
        //            break;
        //    }
        //}
    }

    public void SortAndDisplayLeaderboard()
    {
        SortAndDisplayUsers();
        SortAndDisplayObjects();
    }

    void SortAndDisplayUsers()
    {
        List<KeyValuePair<string, int>> sortedBoard = userLeaderboard.ToList();
        sortedBoard.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
        string user1 = sortedBoard.Count > 0 ? sortedBoard[0].Key : "";
        int count1 = sortedBoard.Count > 0 ? sortedBoard[0].Value : 0;
        string user2 = sortedBoard.Count > 1 ? sortedBoard[1].Key : "";
        int count2 = sortedBoard.Count > 1 ? sortedBoard[1].Value : 0;
        string user3 = sortedBoard.Count > 2 ? sortedBoard[2].Key : "";
        int count3 = sortedBoard.Count > 2 ? sortedBoard[2].Value : 0;
        leaderboard.GetComponent<Leaderboard>().UpdateSoundBoard(user1, count1,
            user2, count2, user3, count3);
    }

    void SortAndDisplayObjects()
    {
        // Sort the recording leaderboard by value
        List<KeyValuePair<string, int>> sortedBoard = objectLeaderboard.ToList();       
        sortedBoard.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
        string obj1 = sortedBoard.Count > 0 ? sortedBoard[0].Key : "";
        int count1 = sortedBoard.Count > 0 ? sortedBoard[0].Value : 0;
        string obj2 = sortedBoard.Count > 1 ? sortedBoard[1].Key : "";
        int count2 = sortedBoard.Count > 1 ? sortedBoard[1].Value : 0;
        string obj3 = sortedBoard.Count > 2 ? sortedBoard[2].Key : "";
        int count3 = sortedBoard.Count > 2 ? sortedBoard[2].Value : 0;
        leaderboard.GetComponent<Leaderboard>().UpdateItemBoard(obj1, count1,
            obj2, count2, obj3, count3, totalItemCollected, taskCompleted);
    }

    public void EmptyPoint(Vector3 point, bool isOutdoor)
    {
        if (isOutdoor)
            spawningPoints.outdoorList.Add(point);
        else
            spawningPoints.indoorList.Add(point);
    }

    public void CheckEnoughSpace()
    {
        int emptyPoints = totalNumPoints - objectsInScene;
        if (emptyPoints >= objectSpawner.delayedFiles.Count)
        {
            StartCoroutine(objectSpawner.SpawnDelayedFiles(objectSpawner.delayedFiles.Count));
        }
        else if (objectsInScene < spawnThreshold * totalNumPoints)
        {
            StartCoroutine(objectSpawner.SpawnDelayedFiles(emptyPoints));
        }
    }

    public void SetGoal()
    {
        goal.SetGoal();
    }

    public void UpdateGoal(int num)
    {
        goal.UpdateGoal(num);
    }

    public void GameEndWrapper()
    {
        timer.SetActive(false);
        minimap.SetActive(false);
        recognition.SetActive(false);
        goal.gameObject.SetActive(false);
        leaderboard.SetActive(true);
        gameState = GameState.Ending;
        //StartCoroutine(GameEnd());
    }

    public IEnumerator GameEnd()
    {
        playerCamera.SetActive(false);
        endingCamera.SetActive(true);
        player.SetParent(ship);
        float elapsedTime = 0f;
        while (elapsedTime < 10f)
        {
            if (elapsedTime > 3f)
                leaderboard.SetActive(true);

            ship.Translate(10 * Vector3.right * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
    }

    public int NumTypesInScene()
    {
        HashSet<string> num = new HashSet<string>();
        foreach(GameObject obj in allObjectsInScene)
        {
            num.Add(obj.name);
        }

        return num.Count;
    }

    public void StartGame()
    {
        instruction.SetActive(false);
        timer.SetActive(true);
        minimap.SetActive(true);
        recognition.SetActive(true);
        goal.gameObject.SetActive(true);
        gameState = GameState.Playing;
        SetGoal();
    }
}
