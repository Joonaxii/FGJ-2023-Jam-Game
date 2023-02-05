using System.Runtime.ConstrainedExecution;
using UnityEngine;

[System.Serializable]
public class GameStats
{
    public float BitsPerSecond => _bitsPerSecond * _cpuPower;
    public float CPUPower => _cpuPower;
    public int BitCapacity => Mathf.FloorToInt(_bitCapacity);
    public int Bits => Mathf.FloorToInt(_bits);
    public float Corruption => _percentCorrupted;

    public int Detections => _lives;
    public int MaxDetections => _maxLives;

    public float ScanTime => _scanTimer;
    public float ScanInterval => _scanInterval;

    public float FullScanTime => _fullScan;
    public float FullScanDuration => _fullScanTime;

    [SerializeField] private float _baseBitsPerSecond = 0.5f;
    [SerializeField] private float _baseCPUPower = 1.0f;
    [SerializeField] private float _startingBits = 32;
    [SerializeField] private float _baseBitCapacity = 64;
    [SerializeField] private float _baseScanInterval = 180.0f;
    [SerializeField] private float _fullScanTime = 900;

    [SerializeField] private int _maxLives = 3;

    private float _bits;
    private float _bitsPerSecond;
    private float _bitCapacity;
    private float _cpuPower;
    private float _scanTimer;
    private float _scanInterval;
    private int _lives;

    private float _addBPS;
    private float _addBC;
    private float _addCPU;
    private float _multScan;
    private float _fullScan;
    private float _percentCorrupted;

    public void Reset()
    {
        _bits = _startingBits;
        _bitsPerSecond = _baseBitsPerSecond;
        _bitCapacity = _baseBitCapacity;
        _cpuPower = _baseCPUPower;
        _scanInterval = _baseScanInterval;
        _lives = 0;

        _addBPS = 0;
        _addBC = 0;
        _addCPU = 0;

        _percentCorrupted = 0;

        _multScan = 1;
    }

    public bool HasEnoughBits(int bits) => bits <= Bits;

    public void SetCorruption(float corrupt)
    {
        _percentCorrupted = corrupt;
        UpdateStats();
    }

    public bool TakeDamage()
    {
        if (_lives > _maxLives) { return true; }
        _lives++;
        GameManager.Instance.GameUI.UpdateDetections(_lives, _maxLives);

        if(_lives > _maxLives) 
        {
            GameManager.Instance.StartFinalScan();
            return true;
        }

        GameManager.Instance.AudioManager.PlaySFX("UnitConvert");
        return false;
    }

    public bool UseBits(int bits)
    {
        int curBits = Bits;
        if (curBits < bits) { return false; }
        _bits -= bits;

        GameManager.Instance.GameUI.UpdateBitAmount(Bits, BitCapacity, _bitsPerSecond, GameManager.Instance.Player.AccumulatedCost);
        return true;
    }

    public void AddCPU(float add)
    {
        _addBPS += add;
    }

    public void AddBPS(float add)
    {
        _addBPS += add;
    }

    public void AddCapacity(float add)
    {
        _addBC += add;
    }

    public void AddScanMult(float add)
    {
        _multScan += add;
    }

    public void UpdateStats()
    {
        _bitsPerSecond = _baseBitsPerSecond + _addBPS;
        _bitCapacity = _baseBitCapacity + _addBC;
        _cpuPower = _baseCPUPower + _addCPU;

        _scanInterval = (_baseScanInterval * _multScan) * Mathf.Lerp(1.0f, 0.15f, _percentCorrupted);
        
        
        GameManager.Instance.GameUI.UpdateBitAmount(Bits, BitCapacity, _bitsPerSecond, GameManager.Instance.Player.AccumulatedCost);
        GameManager.Instance.GameUI.UpdateScanTime(GameManager.Instance.IsScanInProgress ? 0 : _scanInterval - _scanTimer, _percentCorrupted);
        GameManager.Instance.GameUI.UpdateFullScanTime(GameManager.Instance.YoureBoned ? 0 : _fullScanTime - _fullScan);
    }

    public void Update(float delta)
    {
        int prev = Bits;
        _bits += delta * _bitsPerSecond;
        _bits = Mathf.Clamp(_bits, 0, _bitCapacity);

        int cur = Bits;
        if (cur != prev)
        {
            GameManager.Instance.GameUI.UpdateBitAmount(cur, BitCapacity, _bitsPerSecond, GameManager.Instance.Player.AccumulatedCost);
        }
    }

    public int UpdateScan(float delta)
    {
        _scanTimer += delta;
        _fullScan += delta;

        if(_fullScan >= _fullScanTime)
        {
            return 2;
        }

        if (_scanTimer >= _scanInterval)
        {
            _scanTimer = 0;
            return 1;
        }
        GameManager.Instance.GameUI.UpdateScanTime(_scanInterval - _scanTimer, _percentCorrupted);
        GameManager.Instance.GameUI.UpdateFullScanTime(_fullScanTime - _fullScan);
        return 0;
    }
}