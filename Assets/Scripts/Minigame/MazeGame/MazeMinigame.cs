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

    public void CleanUp()
    {
        isActive = false;
        Destroy(_player);
    }

    public override void Cancel() => CleanUp();
    public override void Finish() => CleanUp();
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
