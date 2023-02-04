using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGoal : MonoBehaviour
{
    public float spinSpeed;
    private MazeMinigame _mazeManager;

    private void Start()
    {
        _mazeManager = GetComponentInParent<MazeMinigame>();
    }

    // Update is called once per frame
    void Update()
    {
        var rot = transform.eulerAngles;
        rot.z += spinSpeed * GTime.GetDeltaTime(0);
        transform.eulerAngles = rot;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerRig = collision.GetComponent<Rigidbody2D>();
            playerRig.velocity = Vector2.zero;
            _mazeManager.Finish();
        }
    }
}
