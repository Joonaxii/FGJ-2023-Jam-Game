using UnityEngine;

[System.Serializable]
public class GameStats
{
    [SerializeField] private float _baseBitsPerSecond = 0.5f;
    [SerializeField] private float _startingBits      = 32;
    [SerializeField] private float _baseBitCapacity   = 64;

    private float _bits;
    private float _bitsPerSecond;
    private float _bitCapacity;

    public void Reset()
    {
        _bits = _startingBits;
        _bitsPerSecond = _baseBitsPerSecond;
        _bitCapacity = _baseBitCapacity;
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

    public void UpdateStats(float bitsPerSeconds, float bitCapacity)
    {
        _bitsPerSecond = _baseBitsPerSecond + bitsPerSeconds;
        _bitCapacity = _baseBitCapacity + bitCapacity;

    }

    public void Update(float delta)
    {
        _bits += delta * _bitsPerSecond;
        _bits = Mathf.Clamp(_bits, 0, _bitCapacity);
    }
}