using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace AnythingWorld.Utilities
{
    [Serializable]
    public class GridUtility
    {
        //private const bool SHOW_DEBUG_STATUS = true;
        private const int EMPTY_VALUE = 0;
        private const int FULL_VALUE = 1;
        public bool ShowGridLines = false;
        #region Grid Size Props
        [SerializeField]
        private int width;
        public int Width => width;

        [SerializeField]
        private int depth;
        public int Depth => depth;

        [SerializeField]
        private float cellSize;
        public float Cellsize => cellSize;
        #endregion

        private int[,] gridArray = null;
        public int[,] GridArray
        {
            get
            {
                if (gridArray == null)
                {
                    if (DeSerializeGridArray())
                    {
                        return gridArray;
                    }
                    else
                    {
                        InitNewGrid(10, 10);
                        return gridArray;
                    }

                }
                else
                {
                    if (gridArray.Length == 0)
                    {
                        InitNewGrid(10, 10);
                    }
                    return gridArray;
                }
            }
            set
            {
                gridArray = value;
            }
        }

        #region Creation Props
        [SerializeField]
        private List<GridInstruction> instructionList;
        [SerializeField]
        private Vector3 offset;

        public Vector3 Offset { get => offset; set => offset = value; }


        [SerializeField, HideInInspector]
        byte[] stream;
        [SerializeField]
        private int creationWidth;
        [SerializeField]
        private int creationDepth;
        #endregion

        [SerializeField]
        private bool gridFull = false;

        public void InitNewGrid(int _width, int _depth, float _cellSize = 10f)
        {
            gridFull = false;
            gridArray = null;
            GridArray = null;
            stream = null;
            width = creationWidth = _width;
            depth = creationDepth = _depth;
            cellSize = _cellSize;
            /*
             * Initializes maximum grid size of 1000mx1000m
             * Grid starts with width of 10x10 cells, and can expand into the array,
             * width and depth refer to active cells that have/can have objects placed in them.
             */
            GridArray = new int[width * 100, depth * 100];
            instructionList = new List<GridInstruction>();
            for (int i = 0; i < depth; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    GridArray[i, j] = EMPTY_VALUE;

                    if (ShowGridLines)
                    {
                        if (j < width - 1)
                        {
                            Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.magenta, 1f);
                        }
                        if (i < depth - 1)
                        {
                            Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.magenta, 1f);
                        }
                    }

                }
            }
        }

        public void InitNewGrid(int _width, int _depth, Vector3 _offset, float _cellSize = 10f)
        {
            gridFull = false;
            offset = _offset;
            gridArray = null;
            GridArray = null;
            stream = null;
            width = creationWidth = _width;
            depth = creationDepth = _depth;
            cellSize = _cellSize;

            /*
             * Initializes maximum grid size of 1000mx1000m
             * Grid starts with width of 10x10 cells, and can expand into the array,
             * width and depth refer to active cells that have/can have objects placed in them.
             */
            GridArray = new int[width * 100, depth * 100];
            instructionList = new List<GridInstruction>();
            if (offset == null) offset = Vector3.zero;
            for (int i = 0; i < depth; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    GridArray[i, j] = EMPTY_VALUE;
                    if (ShowGridLines)
                    {
                        if (j < width - 1)
                        {
                            Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.magenta, 1f);
                        }
                        if (i < depth - 1)
                        {
                            Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.magenta, 1f);
                        }
                    }
                }
            }
        }


        private void DrawDebugStatus()
        {
            float lineWidth = 1f;
            for (int y = 0; y < depth; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color gridColor = Color.red;
                    if (GridArray[y, x] == 0)
                        gridColor = Color.white;

                    Debug.DrawLine(GetWorldPosition(y, x), GetWorldPosition(y, x + 1), gridColor, lineWidth, false);
                    Debug.DrawLine(GetWorldPosition(y, x), GetWorldPosition(y + 1, x), gridColor, lineWidth, false);


                    if (x == width - 1)
                    {
                        Debug.DrawLine(GetWorldPosition(y, x + 1), GetWorldPosition(y + 1, x + 1), gridColor, lineWidth, false);
                    }
                    if (y == depth - 1)
                    {
                        Debug.DrawLine(GetWorldPosition(y + 1, x), GetWorldPosition(y + 1, x + 1), gridColor, lineWidth, false);
                    }
                }


            }
        }

        public void ExpandGrid(int extraWidth, int extraDepth)
        {

            int newWidth = width + extraWidth;
            int newDepth = depth + extraDepth;

            if (ExceedsCapacity(width + extraWidth, depth + extraDepth))
            {
                gridFull = true;
            }
            else
            {
                width = newWidth;
                depth = newDepth;
            }



        }

        private bool ExceedsCapacity(int x, int y)
        {
            return GridArray.GetLength(0) <= x || GridArray.GetLength(1) <= y;
        }
        private Vector3 GetWorldPosition(float z, float x)
        {
            Vector3 worldPos = new Vector3(x, 0, z) * cellSize;
            var temp = worldPos + offset;
            return temp;
        }

        public void SetValueRange(int xStart, int zStart, int xRange, int zRange, int value)
        {
            if (GridArray == null) return;

            for (int i = zStart; i < zStart + zRange; i++)
            {
                for (int j = xStart; j < xStart + xRange; j++)
                {
                    GridArray[i, j] = value;
                }
            }

            if (value != EMPTY_VALUE)
            {
                GridInstruction newInstruction = new GridInstruction(xStart, zStart, xRange, zRange);
                instructionList.Add(newInstruction);
            }

            if (ShowGridLines)
                DrawDebugStatus();
        }

        public void RemoveLastInstruction()
        {
            int removeIndex = instructionList.Count - 1;
            if (removeIndex < 0)
            {
                Debug.LogError("RemoveLastInstruction index error!");
                return;
            }

            GridInstruction lastInstruction = instructionList[removeIndex];
            SetValueRange(lastInstruction.startX, lastInstruction.startZ, lastInstruction.rangeX, lastInstruction.rangeZ, EMPTY_VALUE);
            instructionList.RemoveAt(removeIndex);

            if (ShowGridLines)
                DrawDebugStatus();

            if (instructionList.Count < creationWidth * creationDepth)
            {
                //Debug.Log("return to normal grid!");
                width = creationWidth;
                depth = creationDepth;
            }
        }

        public Vector3 TryGetNextAvailablePosition(float objScale)
        {
            if (gridFull) return Vector3.zero;

            if (ExceedsCapacity(width, depth))
            {
                gridFull = true;
                return Vector3.zero;
            }

            if (ExceedsCapacity(Mathf.CeilToInt(objScale), Mathf.CeilToInt(objScale)))
            {
                return Vector3.zero;
            }

            if (TryGetPositionInGrid(objScale, out Vector3 outPosition))
            {
                return outPosition;
            }
            else
            {
                for (int attempts = 0; attempts < 10; attempts++)
                {

                    ExpandGrid(10 * (attempts + 1), 10 * (attempts + 1));
                    if (gridFull)
                    {
                        //Debug.Log("Grid full, will spawn at zero.");
                        return Vector3.zero;
                    }
                    else
                    {
                        if (TryGetPositionInGrid(objScale, out Vector3 outPosition2))
                        {
                            return outPosition2;
                        }
                    }

                }
            }
            Debug.LogWarning("Could not place on grid");
            return Vector3.zero;
        }



        private bool TryGetPositionInGrid(float objScale, out Vector3 availablePosition)
        {
            if (GridArray.Length < width || GridArray.Length < depth)
            {
                InitNewGrid(10, 10);
            }

            int objSquares = Mathf.CeilToInt(objScale);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < depth; j++)
                {
                    if (GridArray[i, j] == EMPTY_VALUE)
                    {

                        int sqCount = 0;
                        bool haveSpace = true;
                        while (sqCount < objSquares)
                        {
                            int targSquare = j + sqCount;
                            if (targSquare < width)
                            {
                                if (GridArray[i, targSquare] > EMPTY_VALUE)
                                {
                                    haveSpace = false;
                                }
                            }
                            else
                            {
                                haveSpace = false;
                            }
                            sqCount++;
                        }

                        if (haveSpace)
                        {
                            if (ExceedsCapacity(i + sqCount, j + sqCount))
                            {
                                gridFull = true;
                                availablePosition = Vector3.zero;
                                return false;
                            }
                            else
                            {
                                SetValueRange(j, i, sqCount, sqCount, FULL_VALUE);
                                float sqCenter = sqCount / 2f;
                                availablePosition = GetWorldPosition(i + sqCenter, j + sqCenter);
                                return true;
                            }

                        }
                    }
                }
            }
            availablePosition = Vector3.zero;
            return false;
        }



#if UNITY_EDITOR
        public void SerializeChanges(PlayModeStateChange state)
        {

            //Debug.Log("SerializeChanges : " + state);
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                //Debug.Log("exit edit mode");
                SerializeGridArray();
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                //Debug.Log("entered edit mode");
                DeSerializeGridArray();
            }

        }
#endif


        private bool SerializeGridArray()
        {
            if (GridArray != null)
            {
                stream = AnythingWorld.Utilities.ObjectSerializationExtension.SerializeToByteArray(gridArray);
                return true;
            }
            else
            {
                Debug.LogWarning("Grid array is null, cannot serialize to byte array.");
                return false;
            }
        }

        private bool DeSerializeGridArray()
        {
            if (stream != null)
            {
                if (stream.Length > 0)
                {
                    GridArray = AnythingWorld.Utilities.ObjectSerializationExtension.Deserialize<int[,]>(stream);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



    }
}

