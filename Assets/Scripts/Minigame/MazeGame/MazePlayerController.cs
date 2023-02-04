using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class MazePlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float turnSpeed;
    public float acceleration;
    public float maxVelocity;

    private Rigidbody2D _rb2D;
    private MazeMinigame _mazeManager;

    // Start is called before the first frame update
    void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _mazeManager = GetComponentInParent<MazeMinigame>();
    }

    // Update is called once per frame
    void Update()
    {
        var Inputs = GameManager.Instance.Inputs;

        if (Inputs.IsHeld(InputHandler.InputType.RotateLeft) && _mazeManager.isActive)
        {
            _rb2D.rotation += turnSpeed * GTime.GetDeltaTime(0);
        }
        if (Inputs.IsHeld(InputHandler.InputType.RotateRight) && _mazeManager.isActive)
        {
            _rb2D.rotation += -turnSpeed * GTime.GetDeltaTime(0);
        }

        if (Inputs.IsHeld(InputHandler.InputType.MoveUp) && _mazeManager.isActive)
        {
            _rb2D.AddForce(transform.up * acceleration);
            var vel = _rb2D.velocity;
            float mag = vel.magnitude;
            if (mag > maxVelocity)
            {
                vel /= mag;
                _rb2D.velocity = vel * maxVelocity;
            }
        }
    }
}
