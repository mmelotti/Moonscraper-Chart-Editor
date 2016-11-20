﻿using UnityEngine;
using System.Collections;

public abstract class Snapable : MonoBehaviour {
    protected ChartEditor editor;
    protected Vector2 mousePos = Vector2.zero;
    protected uint objectSnappedChartPos = 0;
    protected Renderer objectRen;

    protected virtual void Awake()
    {
        editor = GameObject.FindGameObjectWithTag("Editor").GetComponent<ChartEditor>();
        objectRen = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	protected virtual void Update () {
        // Read in mouse world position
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float ypos = mousePos.y;

        objectSnappedChartPos = WorldPositionToSnappedChartPosition(ypos, Globals.step);

        transform.position = new Vector3(transform.position.x, editor.currentSong.ChartPositionToWorldYPosition(objectSnappedChartPos), transform.position.z);
    }

    protected virtual void Controls()
    {
        if (Toolpane.currentTool == Toolpane.Tools.Note && Globals.applicationMode == Globals.ApplicationMode.Editor && Input.GetMouseButtonDown(0))
        {
            AddObject();
        }
    }

    protected void LateUpdate()
    {
        if (objectRen)
        {
            objectRen.sortingOrder = 5;
        }

        Controls();
    }

    protected abstract void AddObject();

    public uint WorldPositionToSnappedChartPosition(float worldYPos, int step)
    {
        uint chartPos = editor.currentSong.WorldYPositionToChartPosition(worldYPos);

        return ChartPositionToSnappedChartPosition(chartPos, step);
    }

    public static uint ChartPositionToSnappedChartPosition(uint chartPosition, int step)
    {
        // Snap position based on step
        int factor = (int)Globals.FULL_STEP / step;
        float divisor = (float)chartPosition / (float)factor;
        uint lowerBound = (uint)((int)divisor * factor);
        float remainder = divisor - (int)divisor;

        if (remainder > 0.5f)
            chartPosition = lowerBound + (uint)factor;
        else
            chartPosition = (lowerBound);

        return chartPosition;
    }
}