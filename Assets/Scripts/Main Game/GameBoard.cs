using System.Diagnostics;
using UnityEditor.Build.Content;
using UnityEngine;

using Debug = UnityEngine.Debug;

public class GameBoard : MonoBehaviour
{
    private const float PADDING = 0.125f;

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

    private struct BoardTile
    {
        public TileType type;
        public TileFlags flags;

        public Vector3 worldPos;
    }

    public Sprite gridGlow;
    private BoardTile[] _board;

    private Transform _gridRoot;
    private SpriteRenderer[] _glowRends;

    private void Start()
    {
        _gridRoot = transform.Find("GridGlow");
        Generate(31, 31);
    }
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

    public void Generate(int width, int height)
    {
        if(_glowRends != null)
        {
            foreach (var item in _glowRends)
            {
                Destroy(item.gameObject);
            }
            _glowRends = null;
        }

        width = Mathf.Clamp(width, 1, 255);
        height = Mathf.Clamp(height, 1, 255);

        width  += (width  & 0x1) == 0 ? 1 : 0; 
        height += (height & 0x1) == 0 ? 1 : 0;

        int center = (height >> 1) * width + width >> 1;
        _board = new BoardTile[width * height];

        ref BoardTile mainTile = ref _board[center];

        mainTile.type = TileType.MainBase;
        mainTile.flags = TileFlags.PermaHack;

        float wSize = (width  * 0.5f + PADDING * 0.5f);
        float hSize = (height * 0.5f + PADDING * 0.5f);

        _glowRends = new SpriteRenderer[_board.Length];


        Debug.Log($"{wSize}, {hSize}");
        float baseY = -hSize;
        for (int y = 0; y < height; y++)
        {
            int yP = y * width;
            float baseX = -wSize;
            for (int x = 0; x < width; x++)
            {
                int ind = yP + x;
                ref BoardTile tile = ref _board[ind];

                _glowRends[ind] = new GameObject($"Glow #{ind}").AddComponent<SpriteRenderer>();
                var trGlow = _glowRends[ind].transform;
                _glowRends[ind].sprite = gridGlow;

                tile.worldPos = new Vector3(baseX - 0.5f, 0, baseY - 0.5f);
                trGlow.eulerAngles = new Vector3(-90, 0, 0);
                trGlow.position = tile.worldPos;

                baseX += 1.0f + PADDING;
            }
            baseY += 1.0f + PADDING;
        }
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
            if(tile.flags.HasFlag(TileFlags.PermaHack) || tile.flags.HasFlag(TileFlags.Hacked))
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


}
