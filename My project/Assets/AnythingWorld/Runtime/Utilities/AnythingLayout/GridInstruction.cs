using System;

namespace AnythingWorld.Utilities
{
    [Serializable]
    public class GridInstruction
    {
        public GridInstruction(int sX, int sZ, int rX, int rZ)
        {
            startX = sX;
            startZ = sZ;
            rangeX = rX;
            rangeZ = rZ;
        }

        public int startX;
        public int startZ;
        public int rangeX;
        public int rangeZ;
    }
}
