using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMinigame : HackingBase
{
    [Header("Prefabs")]
    public GameObject playerRef;
    private GameObject _player;
    public GameObject goalRef;
    private GameObject _goal;
    [Space(10), Header("Spawn Locations")]
    public Transform[] spawnSpots;
    private Transform _playerSpawn;
    private Transform _goalSpawn;

    public bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        // Call this later from game manager when entering a building
        Initialize();
    }

    public override void Cancel()
    {
        throw new System.NotImplementedException();
    }
    public override void Finish()
    {
        isActive = false;
        Debug.Log("Maze Finish!");
    }
    public override void Initialize()
    {
        List<Transform> temp = new (spawnSpots);
        int id = Random.Range(0, temp.Count);
        _playerSpawn = temp[id];
        temp.RemoveAt(id);
        id = Random.Range(0, temp.Count);
        _goalSpawn = temp[id];

        _player = Instantiate(playerRef, transform);
        _player.transform.SetPositionAndRotation(_playerSpawn.position, Quaternion.identity);

        _goal = Instantiate(goalRef, transform);
        _goal.transform.SetPositionAndRotation(_goalSpawn.position, Quaternion.identity);

        isActive = true;
    }
}
