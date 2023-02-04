using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

[System.Serializable]
public class GameBoard
{
    private const float PADDING = 0.5f;

    public int Width => _width;
    public int Height => _height;

    [System.Flags]
    public enum TileFlags : byte
    {
        None        = 0x00,
        Hacked      = 0x01,

        Overclocked = 0x2,

        PermaHack   = 0x80,
    }               

    public enum TileType
    {
        Empty,
        MainBase,

        BitGenerator,
        BitCapacitor,

        SystemClock,
        CPU,
        Firewall,

        System32,
    }

    public struct BoardTile
    {
        public TileType type;
        public TileFlags flags;

        public Vector3 worldPos;

        public Color currentColor;

        public bool IsHacked() => (flags & (TileFlags.PermaHack | TileFlags.Hacked)) != 0;
    }

    [ColorUsage(true, true)] public Color hackedColor = Color.red;
    [ColorUsage(true, true)] public Color normalColor = Color.green;
    [ColorUsage(true, true)] public Color overClockedColor = Color.cyan;
    [ColorUsage(true, true)] public Color scanColor = Color.blue;

    public Sprite gridGlow;

    public float scanSweep = 0.5f;

    private BoardTile[] _board;

    private Transform _gridRoot;
    private SpriteRenderer[] _glowRends;
    private int _width;
    private int _height;

    private IEnumerator[] _scans;

    public static float TileTypeToBitCost(TileType type, float difficultyMult = 1.0f)
    {
        switch (type)
        {
            case TileType.Empty:        return 1    * difficultyMult;
            case TileType.BitGenerator: return 16   * difficultyMult;
            case TileType.BitCapacitor: return 16   * difficultyMult;
            case TileType.SystemClock:  return 48   * difficultyMult;
            case TileType.CPU:          return 64   * difficultyMult;
            case TileType.Firewall:     return 256  * difficultyMult;
            case TileType.System32:     return 1024 * difficultyMult;
            default:                    return 0;
        }
    }

    public bool IsInBounds(Vector2Int point) { return point.x >= 0 && point.x < _width && point.y >= 0 && point.y < _height; }

    public Color TileToColor(ref BoardTile tile)
    {
        if (tile.IsHacked()) { return hackedColor; }
        return tile.flags.HasFlag(TileFlags.Overclocked) ? overClockedColor : normalColor;
    }

    public void Generate(int width, int height)
    {
        if (_gridRoot != null)
        {
            GameObject.Destroy(_gridRoot.gameObject);
            _gridRoot = null;
        }

        _gridRoot = new GameObject("Grid Root").transform;
        _gridRoot.position = Vector3.zero;

        var grp = _gridRoot.AddComponent<SortingGroup>();
        grp.sortingOrder = -100;

        _glowRends = null;

        _width = Mathf.Clamp(width, 1, 255);
        _height = Mathf.Clamp(height, 1, 255);

        _width  += (_width  & 0x1) == 0 ? 1 : 0;
        _height += (_height & 0x1) == 0 ? 1 : 0; 

        int center = (_height >> 1) * _width + (_width >> 1);
        _board = new BoardTile[_width * _height];

        _scans = new IEnumerator[_width];

        ref BoardTile mainTile = ref _board[center];

        mainTile.type = TileType.MainBase;
        mainTile.flags = TileFlags.PermaHack;

        float wSize = (((_width  -1.0f)  * (1.0f + (PADDING * 1f))) * 0.5f) + 0.5f;
        float hSize = (((_height  -1.0f) * (1.0f + (PADDING * 1f))) * 0.5f) + 0.5f;
         
        _glowRends = new SpriteRenderer[_board.Length];

        float baseY = -hSize;
        for (int y = 0; y < _height; y++)
        {
            int yP = y * _width;
            float baseX = -wSize;
            for (int x = 0; x < _width; x++)
            {
                int ind = yP + x;
                ref BoardTile tile = ref _board[ind];

                _glowRends[ind] = new GameObject($"Glow #{ind}").AddComponent<SpriteRenderer>();
                var trGlow = _glowRends[ind].transform;
                _glowRends[ind].sprite = gridGlow;
                _glowRends[ind].color = tile.currentColor = TileToColor(ref tile);
                trGlow.SetParent(_gridRoot);

                tile.worldPos = new Vector3(baseX + 0.5f, 0, baseY + 0.5f);
                trGlow.eulerAngles = new Vector3(-90, 0, 0);
                trGlow.position = tile.worldPos;

                baseX += 1.0f + PADDING;
            }
            baseY += 1.0f + PADDING;
        }
    }

    public IEnumerator BeginScan(int column)
    {
        yield return GameManager.Instance.StartCoroutine(BeginScanColumn(column));
    }

    public void EndScan(int column)
    {
        if (_scans[column] != null)
        {
            GameManager.Instance.StopCoroutine(_scans[column]);
        }
        GameManager.Instance.StartCoroutine(_scans[column] = EndScanColumn(column));
    }

    public int GetGridPointMoveCost(Vector2Int pos)
    {
        int ind = pos.y * _width + pos.x;
        if (ind < 0 || ind >= _board.Length) { return 0; }

        ref var tile = ref _board[ind];
        if (tile.IsHacked()) { return 0; }
        return Mathf.FloorToInt(GameBoard.TileTypeToBitCost(tile.type));
    }

    public void RefreshStats(out float addBPS, out float addBCap, out float speedMod)
    {
        addBPS = 0;
        addBCap = 0;
        speedMod = 1.0f;
        for (int i = 0; i < _board.Length; i++)
        {
            ref BoardTile tile = ref _board[i];

            float mult = tile.flags.HasFlag(TileFlags.Overclocked) ? 3.0f : 1.0f;
            if(tile.IsHacked())
            {
                switch (tile.type)
                {
                    case TileType.BitCapacitor:
                        addBCap += 16 * mult;
                        break;
                    case TileType.BitGenerator:
                        addBPS += 0.5f * mult;
                        break;
                    case TileType.CPU:
                        speedMod += 0.25f * mult;
                        break;
                }
            }
        }
    }

    public Vector3 GridToWorld(Vector2Int coord)
    {
        Vector3 vec = default;
        vec.x = _width * PADDING * -0.5f + coord.x + coord.x * PADDING - 0.5f;
        vec.y = _height * PADDING * -0.5f + coord.y + coord.y * PADDING - 0.5f;
        return vec;
    }

    private IEnumerator BeginScanColumn(int column)
    {
        Color[] scanBuf = new Color[_height];

        int xPos = column;
        int scan = _width;
        for (int y = 0; y < scanBuf.Length; y++)
        {
            scanBuf[y] = TileToColor(ref _board[xPos]);
            xPos += scan;
        }

        int heightH = _height >> 1;
        float[] times = new float[heightH + 1];

        float offset = 0;
        float step = 0.1f;

        for (int i = 0; i < heightH + 1; i++)
        {
            times[i] = offset;
            offset -= step;
        }

        while (true)
        {
            bool done = true;
            xPos = column;
            int xPosB = _height * _width - _width + column;
            for (int i = 0, j = _height - 1; i < heightH + 1; i++, j--)
            {
                float time = times[i];
                if (time >= scanSweep)
                {
                    xPos += scan;
                    xPosB -= scan;
                    continue; 
                }

                time += GTime.GetDeltaTime(0);
                times[i] = time;
                float n = time / scanSweep;

                if (time >= scanSweep) 
                {
                    time = scanSweep;
                    _glowRends[xPos].color = _board[xPos].currentColor = scanColor;
                    if (i != heightH)
                    {
                        _glowRends[xPosB].color = _board[xPosB].currentColor = scanColor;
                    }
                }
                else
                {
                    _glowRends[xPos].color  = _board[xPos].currentColor  = Color.Lerp(scanBuf[i], scanColor, n);
                    if (i != heightH)
                    {
                        _glowRends[xPosB].color = _board[xPosB].currentColor = Color.Lerp(scanBuf[j], scanColor, n);
                    }
                    done = false;
                }

                xPos += scan;
                xPosB -= scan;
            }
            if (done) { break; }
            yield return null;
        }
        EndScan(column);
    }

    private IEnumerator EndScanColumn(int column)
    {
        Color[] scanBuf = new Color[_height];

        int xPos = column;
        int scan = _width;
        for (int y = 0; y < scanBuf.Length; y++)
        {
            scanBuf[y] = TileToColor(ref _board[xPos]);
            xPos += scan;
        }

        int heightH = _height >> 1;
        float[] times = new float[heightH + 1];

        float offset = 0;
        float step = 0.05f;

        for (int i = 0; i < heightH + 1; i++)
        {
            times[i] = offset;
            offset -= step;
        }

        float fastSweep = scanSweep * 0.5f;
        while (true)
        {
            bool done = true;
            xPos = column;
            int xPosB = _height * _width - _width + column;
            for (int i = 0, j = _height - 1; i < heightH + 1; i++, j--)
            {
                float time = times[i];
                if (time >= fastSweep)
                {
                    xPos += scan;
                    xPosB -= scan;
                    continue;
                }

                time += GTime.GetDeltaTime(0);
                times[i] = time;
                float n = time / fastSweep;

                if (time >= fastSweep)
                {
                    time = fastSweep;

                    _glowRends[xPos].color = _board[xPos].currentColor = scanBuf[i];
                    if (i != heightH)
                    {
                        _glowRends[xPosB].color = _board[xPosB].currentColor = scanBuf[j];
                    }
                }
                else
                {
                    _glowRends[xPos].color = _board[xPos].currentColor = Color.Lerp(scanColor, scanBuf[i], n);
                    if (i != heightH)
                    {
                        _glowRends[xPosB].color = _board[xPosB].currentColor = Color.Lerp(scanColor, scanBuf[j], n);
                    }
                    done = false;
                }

                xPos += scan;
                xPosB -= scan;
            }
            if (done) { break; }
            yield return null;
        }
      
    }
}
