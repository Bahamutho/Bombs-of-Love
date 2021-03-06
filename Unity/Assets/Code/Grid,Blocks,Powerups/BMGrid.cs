﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[ExecuteInEditMode]
public class BMGrid : MonoBehaviour
{
    #region Enums 

    public enum SnapSpotHor
    {
        Left, Mid, Right
    }

    public enum SnapSpotVer
    {
        Top, Mid, Bottom
    }

    #endregion

    #region Fields

    public GameObject Floor;
    public GameObject Wall;
    public GameObject InnerWall;
    public GameObject Block;

    public int rows = 6;
    public int columns = 6;

    [Range(0.0f,1.0f)]
    public float BlockChance = 0.3f;

    public static float TileWidth = 1.0f;
    public static float TileHeight = 1.0f;

    [SerializeField]
    public Array2D<BMGridElement> levelArray;

    [SerializeField]
    public Array2D<BMGridElement> blockArray;

    public int GridHeight { get { return 2 + rows * 2 + 1; } }
    public int GridWidth { get { return 2 + columns * 2 + 1; } }

    [SerializeField]
    private GameObject levelContainer;
    [SerializeField]
    private GameObject blockContainer;

    internal void RemoveMe(BMGridElement el)
    {
        if (levelArray[el.x, el.y] == el) levelArray[el.x, el.y] = null;
        if (blockArray[el.x, el.y] == el) blockArray[el.x, el.y] = null;
    }

    #endregion

    #region Awake

    public void Awake()
    {
        RestoreReferences();

        if(GameLogic.Instance.Rules.RandomBlockStart)
        {
            GameState.Instance.EventHookups.OnGameOver.AddListener(GenerateBlocks);
            GenerateBlocks();
        }
            
    }

    #endregion

    #region Generate

    public void GenerateGrid()
    {
        // Destroy old grid
        DeleteChildren(this.transform);

        // Make a new grid
        int height = 2 + rows * 2 + 1; // 2 rows of walls + 2 per row + 1 to finish
        int width = 2 + columns * 2 + 1;

        levelArray = new Array2D<BMGridElement>(width, height);
        levelContainer = newContainer("levelContainer");

        #region Walls
        // Top
        for (int i = 0; i < width; i++)
            Create(i, 0, Wall, levelArray, levelContainer, "levelContainer");
        // Bottom
        for (int i = 0; i < width; i++)
            Create(i, height-1, Wall, levelArray, levelContainer, "levelContainer");
        // Side 1
        for (int i = 1; i < height-1; i++)
            Create(0, i, Wall, levelArray, levelContainer, "levelContainer");
        // Side 1
        for (int i = 1; i < height - 1; i++)
            Create(width -1, i, Wall, levelArray, levelContainer, "levelContainer");
        #endregion

        #region Floors & inner pillars

        // Full floors
        for (int j = 1; j < height - 1; j++)
            for (int i = 1; i < width -1; i += 2)
                Create(j, i, Floor, levelArray, levelContainer, "levelContainer");

        for (int j = 1; j < height - 1; j++)
            for (int i = 2; i < width - 2; i +=2)
            {
                if(j%2==0)
                    Create(j, i, InnerWall, levelArray, levelContainer, "levelContainer");
                else
                    Create(j, i, Floor, levelArray, levelContainer, "levelContainer");
            }

        #endregion
    }

    public void GenerateBlocks()
    {
        RestoreReferences();

        blockArray = new Array2D<BMGridElement>(GridWidth, GridHeight);

        Block.GetComponent<Destructable>().RegisteredGrid = this;

        // Destroy old blocks
        if (blockContainer != null)
            DeleteChildren(blockContainer.transform);
        else
            blockContainer = newContainer("blockContainer");

        // Loop randomly over the grid with a randomChance to fill the floor tiles with a destructable block
        for (int y = 0; y < GridHeight; y++)
            for (int x = 0; x < GridWidth; x++)
            {
                BMGridElement el = levelArray[x, y];
                if (el.Type == BMGridElement.GridType.Floor 
                    && UnityEngine.Random.Range(0f, 1.0f) <= BlockChance)
                {
                    Create(x, y, Block, blockArray, blockContainer, "blockContainer");
                }
            }

        // Clear near the corners
        DestroyFromGridWithNeighbors(blockArray, 1, 1); // Bot Left
        DestroyFromGridWithNeighbors(blockArray, 1, GridHeight -2);
        DestroyFromGridWithNeighbors(blockArray, GridWidth - 2, GridHeight - 2); 
        DestroyFromGridWithNeighbors(blockArray, GridWidth - 2, 1); 
    }

    public void CreateBlock(Vector3 worldpos)
    {
        Vector2 g = GetGridCoordinates(worldpos);
        Create((int)g.x, (int)g.y, Block, blockArray, blockContainer, "blockContainer");
    }

    private void DestroyFromGridWithNeighbors(Array2D<BMGridElement> array, int x, int y)
    {
        DestroyFromGridIfExisting(array, x, y);
        DestroyFromGridIfExisting(array, x + 1, y);
        DestroyFromGridIfExisting(array, x - 1, y);
        DestroyFromGridIfExisting(array, x, y + 1);
        DestroyFromGridIfExisting(array, x, y - 1);
    }

    private void DestroyFromGridIfExisting(Array2D<BMGridElement> array, int x, int y)
    {
        if (x < 0 || y < 0 || x >= array.Width || y >= array.Height)
            return;

        if (array[x, y] == null)
            return;

        GameObject.DestroyImmediate(array[x, y].gameObject);
    }

    #endregion

    #region Restore Reference

    public void RestoreReferences()
    {
        if (transform.childCount >= 1)
            levelContainer = transform.GetChild(0).gameObject;
        if (transform.childCount > 1)
            blockContainer = transform.GetChild(1).gameObject;

        levelArray = RestoreArray(levelArray, levelContainer);
        if(blockContainer != null)
            blockArray = RestoreArray(blockArray, blockContainer);
    }

    private Array2D<BMGridElement> RestoreArray(Array2D<BMGridElement> array, GameObject container)
    {
        array = new Array2D<BMGridElement>(GridWidth, GridHeight);

        List<BMGridElement> elements = container.transform.GetComponentsInChildren<BMGridElement>().ToList();
        foreach (BMGridElement el in elements)
        {
            array[el.x, el.y] = el;
            //Debug.Log(el.name + " x " + el.x + " y " + el.y);
        }

        return array;
    }

    public void DebugArray()
    {
        // Make a new grid
        int height = 2 + rows * 2 + 1; // 2 rows of walls + 2 per row + 1 to finish
        int width = 2 + columns * 2 + 1;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Debug.Log("x: " + x + " y: " + y + " named: " + levelArray[x, y].name);
            }
    }

    #endregion

    #region Create & Destroy

    private void Create(int x, int y, GameObject obj, Array2D<BMGridElement> array, GameObject container, string containerName)
    {
        Vector3 pos = new Vector3(x * TileWidth, y * TileHeight) + transform.position;
        GameObject newObj = GameObject.Instantiate(obj, pos, Quaternion.identity) as GameObject;
        array[x, y] = newObj.GetComponent<BMGridElement>();
        array[x, y].x = x; //dunno if this is needed
        array[x, y].y = y;
        array[x, y].ParentGrid = this;

        if (container == null)
        {
            container = newContainer(containerName);
        }
        newObj.transform.SetParent(container.transform);
    }

    private GameObject newContainer(string name)
    {
        GameObject container = new GameObject(name);
        container.transform.SetParent(this.transform);
        container.transform.position = Vector3.zero;
        return container;
    }

    public static void DeleteChildren(Transform tr)
    {
        if (tr == null)
            return;

        int childCount = tr.childCount;
        
        for(int i = 0; i < childCount; i++)
            GameObject.DestroyImmediate(tr.GetChild(0).gameObject);
    }

    #endregion

    #region Get

    /// <summary>
    /// Returns x & y grid coordinate
    /// </summary>
    public Vector2 GetGridCoordinates(Vector3 worldLocation, SnapSpotVer ver = SnapSpotVer.Bottom, SnapSpotHor hor = SnapSpotHor.Left)
    {
        Vector3 targetLoc = worldLocation - this.transform.position;
        Vector3 offset = SnapOffset(ver) + SnapOffset(hor);
        targetLoc += offset;
        return new Vector2(Mathf.Floor((targetLoc.x)/TileWidth), Mathf.Floor((targetLoc.y) / TileHeight));
    }

    //public Vector3 WorldToLeftBottomSnappedGridPos(Vector3 worldLocation)
    //{
    //    return GetGridWorldPos(GetGridPos(worldLocation));
    //}

    //public Vector3 WorldToMidBottomSnappedGridPos(Vector3 worldLocation)
    //{
    //    return GetGridWorldPos(GetGridPos(worldLocation)) + new Vector3(GridWidth/2,0);
    //}

    public Vector3 WorldToSnappedGridPos(Vector3 worldLocation, SnapSpotHor offset = SnapSpotHor.Left)
    {
        return GetGridWorldPos(GetGridCoordinates(worldLocation), offset);
    }

    public Vector3 GetGridWorldPos(Vector2 gridCoord, SnapSpotHor offset = SnapSpotHor.Left)
    {
        return GetGridWorldPos((int)gridCoord.x, (int)gridCoord.y, offset);// gridArray[(int)gridPos.x, (int)gridPos.y].transform.position + SnapOffset(offset);
    }

    public Vector3 GetGridWorldPos(int x, int y, SnapSpotHor offset = SnapSpotHor.Left)
    {
        return levelArray[x, y].transform.position + SnapOffset(offset);
    }

    #region offset

    private Vector3 SnapOffset(SnapSpotHor offset)
    {
        switch (offset)
        {
            default:
            case SnapSpotHor.Left:
                return Vector3.zero;

            case SnapSpotHor.Mid:
                return new Vector3(TileWidth / 2, 0);

            case SnapSpotHor.Right:
                return new Vector3(TileWidth, 0);
        }
    }

    private Vector3 SnapOffset(SnapSpotVer offset)
    {
        switch (offset)
        {
            default:
            case SnapSpotVer.Bottom:
                return Vector3.zero;

            case SnapSpotVer.Mid:
                return new Vector3(0, TileHeight / 2);

            case SnapSpotVer.Top:
                return new Vector3(0, TileHeight);
        }
    }

    #endregion

    #region Element

    public BMGridElement GetGridElementFromWorld(Vector3 worldPos)
    {
        Vector2 p = GetGridCoordinates(worldPos);
        return levelArray[(int)p.x, (int)p.y];
    }

    public BMGridElement GetGridElement(Vector2 gridCoord)
    {
        return levelArray[(int)gridCoord.x, (int)gridCoord.y];
    }

    public BMGridElement GetGridElement(int x, int y)
    {
        return levelArray[x, y];
    }

    public BMGridElement GetBlockElement(int x, int y)
    {
        return blockArray[x, y];
    }

    public bool LegalBombLocation(Vector2 gridCoord)
    {
        return LegalBombLocation((int)gridCoord.x, (int)gridCoord.y);
    }

    public bool LegalBombLocation(int x, int y)
    {
        return levelArray[x, y].Type == BMGridElement.GridType.Floor && (blockArray[x, y] == null || blockArray[x, y].Type != BMGridElement.GridType.Block);
    }

    #endregion

    #endregion
}
