﻿using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
public class SpriteSorter : MonoBehaviour
{
    public enum OriginLocation
    {
        Bottom,
        Center,
        Top
    }

    public Transform Origin;
    public OriginLocation OrigLoc = OriginLocation.Bottom;
    public int depthOffset = 0;
    public bool isStatic = true;

    private SpriteRenderer[] renderers;
    // private Sprite spr;


    private bool init = false;

    // Use this for initialization
	void Awake ()
    {
        Init();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!init)
            Init();

        if (!isStatic)
            Sort();
	}

    public void Sort()
    {
        foreach (SpriteRenderer rnd in renderers)
        {
            rnd.sortingOrder = (int)(-100 * (Origin.position.y + Offset(rnd,OrigLoc))) + depthOffset;
        }
    }

    private void Init()
    {
        if (Origin == null)
            Origin = this.transform;

        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        init = true;

        Sort();
    }

    private float Offset(SpriteRenderer renderer, OriginLocation loc)
    {
        switch(loc)
        {
            default:
            case OriginLocation.Bottom:
                return 0;
            case OriginLocation.Center:
                return renderer.bounds.size.y / 2;
            case OriginLocation.Top:
                return renderer.bounds.size.y;
        }
    }
}
