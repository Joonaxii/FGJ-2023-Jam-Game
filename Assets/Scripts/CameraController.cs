using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[System.Serializable]
public class CameraController
{
    [SerializeField] private Transform _cameraRig;
    [SerializeField] private float _smoothTime = .15f;
    [SerializeField] private Vector3 _nearOffset = new Vector3(0, 10.0f, -10.0f);
    [SerializeField] private Vector3 _farOffset = new Vector3(0, 16.0f, -13.5f);
    [SerializeField] private Vector3 _rotationNearOffset = new Vector3(50, 0, 0);
    [SerializeField] private Vector3 _rotationFarOffset = new Vector3(65, 0, 0);

    private Vector3 _targetPos;
    private Vector3 _velocity;
    private Vector3 _curPos;

    private float _targetBlend;
    private float _curBlend;
    private float _blendVelo;
  
    public void SetPosition(Vector3 pos, bool instant = false)
    {
        _targetPos = pos;
        if (instant)
        {
            _velocity = Vector3.zero;
            _curPos = _targetPos + Vector3.Lerp(_nearOffset, _farOffset, _curBlend);

            if (_cameraRig != null)
            {
                _cameraRig.position = pos;
            }
        }
    }

    public void SetNearFarBlend(float blend, bool instant = false)
    {
        _targetBlend = Mathf.Clamp01(blend);
        if (instant)
        {
            _curBlend = _targetBlend;
            _blendVelo = 0;
            _velocity = Vector3.zero;
            _curPos = _targetPos;

            if (_cameraRig != null)
            {
                Vector3 curRot = Vector3.Lerp(_rotationNearOffset, _rotationFarOffset, _curBlend);

                _cameraRig.position = _curPos + Vector3.Lerp(_nearOffset, _farOffset, _curBlend);
                _cameraRig.rotation = Quaternion.Euler(curRot);
            }
        }
    }

    public void Update(float delta)
    {
        _curBlend = Mathf.SmoothDamp(_curBlend, _targetBlend, ref _blendVelo, _smoothTime, float.MaxValue, delta);
        Vector3 blend = Vector3.Lerp(_nearOffset, _farOffset, _curBlend);
        Vector3 rot = Vector3.Lerp(_rotationNearOffset, _rotationFarOffset, _curBlend);

        _curPos = Vector3.SmoothDamp(_curPos, _targetPos, ref _velocity, _smoothTime, float.MaxValue, delta);
        Vector3 pos = _curPos + blend;

        if (_cameraRig != null)
        {
            _cameraRig.position = pos;
            _cameraRig.rotation = Quaternion.Euler(rot);
        }
    }
}