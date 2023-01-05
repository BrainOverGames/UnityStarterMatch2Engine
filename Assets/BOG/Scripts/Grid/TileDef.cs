namespace BOG
{
    /// <summary>
    /// 2D definition of tile for grid
    /// </summary>
    public struct TileDef
    {
        public readonly int x;
        public readonly int y;

        public TileDef(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
