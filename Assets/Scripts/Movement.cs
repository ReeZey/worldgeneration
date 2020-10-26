﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    Rigidbody2D rb;

    Vector2 move;
    Text txt;
    WorldGeneration WorldGen;

    public TileBase tile;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        txt = GameObject.FindGameObjectWithTag("pos").GetComponent<Text>();

        WorldGen = GameObject.FindGameObjectWithTag("worldHandler").GetComponent<WorldGeneration>();
    }

    void Update()
    {
        Vector2 pos = transform.position;

        int current = (int)(1f / Time.unscaledDeltaTime);
        txt.text = $"{current} fps \n" +
            $"{pos.x} x \n" +
            $"{pos.y} y \n" +
            $"{rb.velocity} vel \n" +
            $"{WorldGen.getNumberOfChunksCount()} chunkNum \n" +
            $"{WorldGen.getNumberOfLoadedChunksCount()} loadedChunkNum \n" +
            $"{WorldGen.GetLoadedChunks().Count} loaded";

        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetMouseButton(0))
        {
            BlockChange(null);
        }

        if (Input.GetMouseButton(1))
        {
            BlockChange(tile);
        }
    }

    private void BlockChange(TileBase block)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 chunkPos = toChunkVector(mousePos);
        Dictionary<Vector2,Tilemap> test = WorldGen.GetLoadedChunks();

        Tilemap map = test[chunkPos];

        Vector3Int tilepos = map.WorldToCell(mousePos);
        if(block == null || map.GetTile(tilepos) == null)
            map.SetTile(tilepos, block);

        if(WorldGen.save) SaveSystem.Save(map.GetComponent<Tilemap>());
        /*
         * Explosion effect
         * 
        Collider2D[] xd = Physics2D.OverlapCircleAll(mousePos, 10);
        Tilemap map = getMap(xd);

        if(map == null)
        {
            Debug.Log("no");
        }
        else
        {
            for (int i = -5; i < 5; i++)
            {
                for (int j = -5; j < 5; j++)
                {
                    if (Mathf.Abs(j) + Mathf.Abs(i) > 4)
                        continue;

                    Vector3Int tilepos = map.WorldToCell(mousePos + new Vector2(i, j));
                    map.SetTile(tilepos, lmao);
                }
            }
        }
        */
    }

    private Vector2 toChunkVector(Vector2 vec)
    {
        vec.x = Mathf.FloorToInt(vec.x / WorldGen.chunkWidth);
        vec.y = Mathf.FloorToInt(vec.y / WorldGen.chunkHeight);
        return vec;
    }

    private void FixedUpdate()
    {
        rb.velocity += move;
    }
}
