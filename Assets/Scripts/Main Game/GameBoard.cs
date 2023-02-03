using UnityEngine;

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

    private BoardTile[] _board;

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
        width = Mathf.Clamp(width, 1, 255);
        height = Mathf.Clamp(height, 1, 255);

        width  += (width  & 0x1) == 0 ? 1 : 0; 
        height += (height & 0x1) == 0 ? 1 : 0;

        int center = (height >> 1) * width + width >> 1;
        _board = new BoardTile[width * height];

        ref BoardTile mainTile = ref _board[center];

        mainTile.type = TileType.MainBase;
        mainTile.flags = TileFlags.PermaHack;

        float wSize = (width  * PADDING * 0.5f);
        float hSize = (height * PADDING * 0.5f);

        float baseY = -hSize;
        for (int y = 0; y < height; y++)
        {
            int yP = y * width;
            float baseX = -wSize;
            for (int x = 0; x < width; x++)
            {
                int ind = yP + x;
                ref BoardTile tile = ref _board[ind];

                tile.worldPos = new Vector3(baseX, 0, baseY);

                baseX += 1.0f + PADDING;
            }
            baseY += 1.0f + PADDING;
        }
    }

    public void RefreshStats()
    {
        //float add
        //for (int i = 0; i < length; i++)
        //{

        //}
    }
}
