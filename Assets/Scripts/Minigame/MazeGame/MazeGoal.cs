using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGoal : MonoBehaviour
{
    public ParticleSystem explEffect;
    private MazeMinigame _mazeManager;

    private void Start()
    {
        _mazeManager = GetComponentInParent<MazeMinigame>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var ps = Instantiate(explEffect, transform.position, Quaternion.identity);
            ps.gameObject.layer = LayerMask.NameToLayer("Hacking");

            transform.GetChild(0).gameObject.SetActive(false);

            var playerRig = collision.GetComponent<Rigidbody2D>();
            playerRig.velocity = Vector2.zero;
            _mazeManager.Finish();

            Destroy(gameObject);
        }
    }
}
