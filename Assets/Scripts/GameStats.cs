using UnityEngine;

[System.Serializable]
public class GameStats
{
    public float BitsPerSecond => _bitsPerSecond;
    public float CPUPower => _cpuPower;
    public int BitCapacity => Mathf.FloorToInt(_bitCapacity);

    [SerializeField] private float _baseBitsPerSecond = 0.5f;
    [SerializeField] private float _baseCPUPower      = 1.0f;
    [SerializeField] private float _startingBits      = 32;
    [SerializeField] private float _baseBitCapacity   = 64;
    [SerializeField] private float _baseScanInterval  = 180.0f;

    private float _bits;
    private float _bitsPerSecond;
    private float _bitCapacity;
    private float _cpuPower;
    private float _scanTimer;


    public void Reset()
    {
        _bits = _startingBits;
        _bitsPerSecond = _baseBitsPerSecond;
        _bitCapacity = _baseBitCapacity;
        _cpuPower = _baseCPUPower;
    }

    public void ResetScanTimer()
    {
        _scanTimer = _baseScanInterval;
    }

    public int GetBits() => Mathf.FloorToInt(_bits);
    public bool HasEnoughBits(int bits) => bits <= GetBits();

    public bool UseBits(int bits)
    {
        int curBits = GetBits();
        if(curBits < bits) { return false; }

        _bits -= curBits;
        return true;
    }

    public void UpdateStats(float bitsPerSeconds, float bitCapacity, float speedMod)
    {
        _bitsPerSecond = _baseBitsPerSecond + bitsPerSeconds;
        _bitCapacity = _baseBitCapacity + bitCapacity;
        _cpuPower = _baseCPUPower * speedMod;
    }

    public void Update(float delta)
    {
        _bits += delta * _bitsPerSecond;
        _bits = Mathf.Clamp(_bits, 0, _bitCapacity);
    }

    public bool UpdateScan(float delta)
    {
        _scanTimer += delta;
        if(_scanTimer >= _baseScanInterval)
        {
            _scanTimer = 0;
            return true;
        }
        return false;
    }
}