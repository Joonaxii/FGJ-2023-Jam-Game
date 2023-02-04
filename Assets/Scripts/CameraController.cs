using TMPro;
using UnityEngine;

[System.Serializable]
public class CameraController
{
    [SerializeField] private Transform _cameraRig;
    [SerializeField] private float _smoothTime = .15f;
    [SerializeField] private Vector3 _nearOffset = new Vector3(0, 10.0f, -10.0f);
    [SerializeField] private Vector3 _farOffset = new Vector3(0, 16.0f, -13.5f);
    [SerializeField] private Vector3 _rotationNearOffset = new Vector3(50, 0, 0);
    [SerializeField] private Vector3 _rotationFarOffset = new Vector3(65, 0, 0);
    [SerializeField, Range(0.0f, 1.0f)] private float _nearFarBlend = 1.0f;

    private Vector3 _targetPos;
    private Vector3 _velocity;
    private Vector3 _curPos;

    private Vector3 _curPosBlend;
    private Vector3 _curRotation;
    private Vector3 _veloRot;
    private Vector3 _veloPos;

    public void SetPosition(Vector3 pos, bool instant = false)
    {
        _targetPos = pos;
        if (instant)
        {
            _velocity = Vector3.zero;
            _curPos = _targetPos + Vector3.Lerp(_nearOffset, _farOffset, _nearFarBlend);

            if (_cameraRig != null)
            {
                _cameraRig.position = pos;
            }
        }
    }

    public void SetNearFarBlend(float blend, bool instant = false)
    {
        _nearFarBlend = Mathf.Clamp01(blend);
        if (instant)
        {

        }
    }

    public void Update(float delta)
    {
        Vector3 blend = Vector3.Lerp(_nearOffset, _farOffset, _nearFarBlend);
        Vector3 rot = Vector3.Lerp(_rotationNearOffset, _rotationFarOffset, _nearFarBlend);

        _curPosBlend = Vector3.SmoothDamp(_curPosBlend, _targetPos, ref _velocity, _smoothTime, float.MaxValue, delta);

        _curPos = Vector3.SmoothDamp(_curPosBlend, _targetPos, ref _velocity, _smoothTime, float.MaxValue, delta);
        Vector3 pos = _curPos + blend;

        if (_cameraRig != null)
        {
            _cameraRig.position = pos;
            _cameraRig.rotation = Quaternion.Euler(_curRotation);
        }
    }
}