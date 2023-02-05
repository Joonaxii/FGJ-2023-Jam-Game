
using System.Collections;
using System.Collections.Generic;
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
        None = 0x00,
        Hacked = 0x01,

        Overclocked = 0x2,
        BeingScanned = 0x4,

        PermaHack = 0x80,
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
        public bool linker;
        public int linkTile;

        public TileType type;
        public TileFlags flags;

        public Vector2Int gridPos;
        public Vector3 worldPos;
        public Color currentColor;
        public bool beingUpdated;

        public Unit unit;

        public bool IsHacked() => (flags & (TileFlags.PermaHack | TileFlags.Hacked)) != 0;
        public bool IsBeingScanned() => (flags & TileFlags.BeingScanned) != 0;
    }

    [ColorUsage(true, true)] public Color hackedColor = Color.red;
    [ColorUsage(true, true)] public Color normalColor = Color.green;
    [ColorUsage(true, true)] public Color unitColor = Color.green;
    [ColorUsage(true, true)] public Color overClockedColor = Color.cyan;
    [ColorUsage(true, true)] public Color overClockedColorHacked = Color.magenta;
    [ColorUsage(true, true)] public Color scanColor = Color.blue;

    public Sprite gridGlow;

    public float scanSweep = 0.5f;

    public Material materialGrid;

    public Unit[] units;

    private BoardTile[] _board;

    private Transform _gridRoot;
    private SpriteRenderer[] _glowRends;
    private int _width;
    private int _height;

    private IEnumerator[] _scans;

    private int[] _virusMask = new int[8];

    public static float TileTypeToBitCost(TileType type, float difficultyMult = 1.0f)
    {
        switch (type)
        {
            case TileType.Empty: return 1 * difficultyMult;
            case TileType.BitGenerator: return 16 * difficultyMult;
            case TileType.BitCapacitor: return 16 * difficultyMult;
            case TileType.SystemClock: return 48 * difficultyMult;
            case TileType.CPU: return 64 * difficultyMult;
            case TileType.Firewall: return 256 * difficultyMult;
            case TileType.System32: return 1024 * difficultyMult;
            default: return 0;
        }
    }

    public ref BoardTile this[int i]
    {
        get => ref _board[i];
    }

    public ref BoardTile this[Vector2Int i]
    {
        get => ref _board[i.y * _width + i.x];
    }
    public bool IsInBounds(Vector2Int point) { return point.x >= 0 && point.x < _width && point.y >= 0 && point.y < _height; }

    public Color TileToColor(ref BoardTile tile)
    {
        if (tile.IsHacked()) { return tile.flags.HasFlag(TileFlags.Overclocked) ? overClockedColorHacked : hackedColor; }
        return tile.flags.HasFlag(TileFlags.Overclocked) ? overClockedColor : tile.type != TileType.Empty ? unitColor : normalColor;
    }

    public void ScanTile(ref BoardTile tile)
    {
        tile.flags |= TileFlags.BeingScanned;
        int? ret = tile.unit?.ScanUnit();

        switch (ret.GetValueOrDefault())
        {

        }
    }

    public void SetTileState(Vector2Int tilePos, bool hacked)
    {
        ref var tile = ref this[tilePos];
        SetTileState(ref tile, hacked);
    }

    public void SetTileState(ref BoardTile tile, bool hacked, BoardTile? def = null, bool force = false)
    {
        int id = tile.gridPos.y * _width + tile.gridPos.x;
        if (!force)
        {
            if (tile.linker)
            {
                for (int i = 0; i < _board.Length; i++)
                {
                    if (_board[i].linkTile == id) { SetTileState(ref _board[i], hacked, _board[_board[i].linkTile], true); }
                }
            }
            else if (tile.linkTile > -1)
            {
                SetTileState(ref this[tile.linkTile], hacked);

                return;
            }
        }
        BoardTile tileC = def == null ? tile : def.Value;

        if (hacked)
        {
            tile.flags |= TileFlags.Hacked;
            tileC.flags |= TileFlags.Hacked;
            tile.currentColor = TileToColor(ref tileC);
        }
        else
        {
            tile.flags &= ~TileFlags.Hacked;
            tileC.flags &= ~TileFlags.Hacked;
            tile.currentColor = TileToColor(ref tileC);
        }

        if (!tile.beingUpdated)
        {
            _glowRends[id].color = tile.currentColor;
        }

    }

    public void OccupyTiles(Vector2Int start, int w, int h, TileType type, bool state)
    {
        TileFlags flags = state ? TileFlags.Hacked : TileFlags.None;
        ref var tile = ref this[start];

        int link = start.y * _width + start.x;
        tile.type = type;
        tile.flags |= flags;
        tile.linker = w > 0 || h > 0;

        bool wZer = w <= 0;
        bool hZer = h <= 0;
        if (wZer && hZer) { return; }

        if (hZer)
        {
            for (int i = 0; i < w; i++)
            {
                start.x++;
                ref var tileL = ref this[start];
                tileL.type = type;
                tileL.flags |= flags;
                tileL.linkTile = link;
            }
        }
        else if (wZer)
        {
            for (int i = 0; i < h; i++)
            {
                start.y++;
                ref var tileL = ref this[start];
                tileL.type = type;
                tileL.flags |= flags;
                tileL.linkTile = link;
            }
            return;
        }

        Vector2Int st = new Vector2Int(start.x, start.y);

        for (int i = 0; i < (h + 1); i++)
        {
            st.x = i == 0 ? start.x + 1 : start.x;
            for (int j = 0; j < (i == 0 ? w : w + 1); j++)
            {
                ref var tileL = ref this[st];
                tileL.type = type;
                tileL.flags |= flags;
                tileL.linkTile = link;
                st.x++;
            }
            st.y++;
        }
    }

    public void Generate(int width, int height)
    {
        if (_board != null)
        {
            foreach (var item in _board)
            {
                if (item.unit != null)
                {
                    MonoBehaviour.Destroy(item.unit.gameObject);
                }
            }
            _board = null;
        }

        if (_gridRoot != null)
        {
            GameObject.Destroy(_gridRoot.gameObject);
            _gridRoot = null;
        }

        _gridRoot = new GameObject("Grid Root").transform;
        _gridRoot.position = Vector3.zero;

        var grp = _gridRoot.gameObject.AddComponent<SortingGroup>();
        grp.sortingOrder = -100;

        _glowRends = null;

        _width = Mathf.Clamp(width, 1, 255);
        _height = Mathf.Clamp(height, 1, 255);

        _width += (_width & 0x1) == 0 ? 1 : 0;
        _height += (_height & 0x1) == 0 ? 1 : 0;

        int hPos = (_height >> 1);
        _virusMask[0] = -_width - 1;
        _virusMask[1] = -_width;
        _virusMask[2] = -_width + 1;

        _virusMask[3] = -1;
        _virusMask[4] = +1;

        _virusMask[5] = _width - 1;
        _virusMask[6] = _width;
        _virusMask[7] = _width + 1;

        int center = hPos * _width + (_width >> 1);
        _board = new BoardTile[_width * _height];

        for (int i = 0; i < _board.Length; i++)
        {
            _board[i].linkTile = -1;
        }

        _scans = new IEnumerator[_width];
        ref BoardTile mainTile = ref _board[center];
        ref BoardTile spawnTile = ref _board[center + (-2 * _width)];

        mainTile.type = TileType.MainBase;
        mainTile.flags = TileFlags.PermaHack;

        spawnTile.type = TileType.Empty;
        spawnTile.flags = TileFlags.PermaHack;

        for (int i = 0; i < _virusMask.Length; i++)
        {
            ref BoardTile bTile = ref _board[center + _virusMask[i]];
            bTile.type = TileType.MainBase;
            bTile.flags = TileFlags.PermaHack;
        }

        float wSize = (((_width - 1.0f) * (1.0f + (PADDING * 1f))) * 0.5f) + 0.5f;
        float hSize = (((_height - 1.0f) * (1.0f + (PADDING * 1f))) * 0.5f) + 0.5f;

        _glowRends = new SpriteRenderer[_board.Length];

        {
            Vector2Int leftSys32 = new Vector2Int(0, _height - 1);
            Vector2Int rightSys32 = new Vector2Int(_width - 1, _height - 1);

            ref var tileSysL = ref this[leftSys32];
            ref var tileSysR = ref this[rightSys32];

            tileSysL.type |= TileType.System32;
            tileSysR.type |= TileType.System32;


            Vector2Int leftFWA = leftSys32 + new Vector2Int(3, -1);
            Vector2Int leftFWB = leftSys32 + new Vector2Int(0, -4);

            OccupyTiles(leftFWA, 1, 1, TileType.Firewall, false);
            OccupyTiles(leftFWB, 1, 1, TileType.Firewall, false);

            Vector2Int rightFWA = rightSys32 + new Vector2Int(-2, -4);
            Vector2Int rightFWB = rightSys32 + new Vector2Int(-5, -2);

            OccupyTiles(rightFWA, 1, 1, TileType.Firewall, false);
            OccupyTiles(rightFWB, 1, 1, TileType.Firewall, false);

            const int NUM_OF_BIT_GEN = 24;
            const int NUM_OF_BIT_CAP = 24;
            const int NUM_OF_BIT_CPU = 24;
            const int NUM_OF_BIT_CLOCK = 24;

            (int, TileType)[] UNITS = new (int, TileType)[]
            {
                (NUM_OF_BIT_GEN,   TileType.BitGenerator),
                (NUM_OF_BIT_CAP,   TileType.BitCapacitor),
                (NUM_OF_BIT_CPU,   TileType.CPU),
                (NUM_OF_BIT_CLOCK, TileType.SystemClock),
            };


            List<Vector2Int> coords = new List<Vector2Int>();

            for (int i = 0; i < _board.Length; i++)
            {
                ref var tileL = ref _board[i];
                if (tileL.type == TileType.Empty)
                {
                    coords.Add(new Vector2Int(i % _width, i / _width));
                }
            }

            for (int i = 0; i < UNITS.Length; i++)
            {
                var unit = UNITS[i];
                for (int j = 0; j < unit.Item1; j++)
                {
                    int ind = Random.Range(0, coords.Count);
                    OccupyTiles(coords[ind], 0, 0, unit.Item2, false);

                    coords.RemoveAt(ind);
                    if (coords.Count < 1) { break; }
                }
                if (coords.Count < 1) { break; }
            }
        }

        float baseY = -hSize;
        for (int y = 0; y < _height; y++)
        {
            int yP = y * _width;
            float baseX = -wSize;
            for (int x = 0; x < _width; x++)
            {
                int ind = yP + x;
                ref BoardTile tile = ref _board[ind];

                tile.gridPos = new Vector2Int(x, y);

                _glowRends[ind] = new GameObject($"Glow #{ind}").AddComponent<SpriteRenderer>();
                var trGlow = _glowRends[ind].transform;
                _glowRends[ind].sharedMaterial = materialGrid;
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

        for (int i = 0; i < _board.Length; i++)
        {
            ref var tileL = ref _board[i];

            int ind = (int)tileL.type - 2;
            if (tileL.linkTile > -1) { continue; }
            switch (tileL.type)
            {
                case TileType.Firewall:
                    tileL.unit = MonoBehaviour.Instantiate(units[ind], tileL.worldPos + new Vector3(0.5f + PADDING * 0.5f, 0, 0.5f + PADDING * 0.5f), Quaternion.identity);

                    if (UnityEngine.Random.value <= 0.05f)
                    {
                        tileL.flags |= TileFlags.Overclocked;
                        SetTileState(tileL.gridPos, tileL.IsHacked());
                    }

                    tileL.unit.Setup(i);
                    break;

                case TileType.BitCapacitor:
                case TileType.BitGenerator:
                case TileType.CPU:
                case TileType.System32:
                case TileType.SystemClock:
                    tileL.unit = MonoBehaviour.Instantiate(units[ind], tileL.worldPos, Quaternion.identity);

                    if (tileL.type != TileType.System32 && UnityEngine.Random.value <= 0.15f)
                    {
                        tileL.flags |= TileFlags.Overclocked;
                        SetTileState(tileL.gridPos, tileL.IsHacked());
                    }
                    tileL.unit.Setup(i);
                    break;
            }
        }

        GameManager.Instance.GetStats.SetCorruption(GetCorruption());
    }

    public float GetCorruption()
    {
        int corrupt = 0;
        for (int i = 0; i < _board.Length; i++)
        {
            if (_board[i].IsHacked()) { corrupt++; }
        }

        return corrupt / (float)(_board.Length);
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
        return GetGridPointMoveCost(ref _board[ind]);
    }

    public int GetGridPointMoveCost(ref BoardTile tile)
    {
        if (tile.IsHacked()) { return 0; }
        return Mathf.FloorToInt(GameBoard.TileTypeToBitCost(tile.type, tile.flags.HasFlag(TileFlags.Overclocked) ? 2.0f : 1.0f));

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
            if (tile.IsHacked())
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
        int ind = coord.y * _width + coord.x;
        return ind < 0 || ind >= _board.Length ? default : _board[ind].worldPos;
    }

    private IEnumerator BeginScanColumn(int column)
    {
        int xPos = column;
        int scan = _width;

        int heightH = _height >> 1;
        float[] times = new float[heightH + 1];

        float offset = 0;
        float step = 0.075f;

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
                    _glowRends[xPos].color = scanColor;

                    ScanTile(ref _board[xPos]);
                    if (i != heightH)
                    {
                        _glowRends[xPosB].color = scanColor;
                        ScanTile(ref _board[xPosB]);
                    }
                }
                else
                {
                    _board[xPos].beingUpdated = true;
                    _glowRends[xPos].color = Color.Lerp(_board[xPos].currentColor, scanColor, n);
                    if (i != heightH)
                    {
                        _board[xPosB].beingUpdated = true;
                        _glowRends[xPosB].color = Color.Lerp(_board[xPosB].currentColor, scanColor, n);
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
        int xPos = column;
        int scan = _width;

        int heightH = _height >> 1;
        float[] times = new float[heightH + 1];

        float offset = 0;
        float step = 0.075f;

        for (int i = 0; i < heightH + 1; i++)
        {
            times[i] = offset;
            offset -= step;
        }

        float fastSweep = scanSweep * 0.75f;
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

                    _board[xPos].beingUpdated = false;
                    _glowRends[xPos].color = _board[xPos].currentColor;
                    if (i != heightH)
                    {
                        _board[xPosB].beingUpdated = false;
                        _glowRends[xPosB].color = _board[xPosB].currentColor;
                    }
                }
                else
                {
                    if (_board[xPos].IsBeingScanned())
                    {
                        _board[xPos].flags &= ~TileFlags.BeingScanned;
                    }

                    _glowRends[xPos].color = Color.Lerp(scanColor, _board[xPos].currentColor, n);
                    if (i != heightH)
                    {
                        _glowRends[xPosB].color = Color.Lerp(scanColor, _board[xPosB].currentColor, n);

                        if (_board[xPosB].IsBeingScanned())
                        {
                            _board[xPosB].flags &= ~TileFlags.BeingScanned;
                        }
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
