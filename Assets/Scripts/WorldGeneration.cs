using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGeneration : MonoBehaviour
{
    public TileBase[] tiles;
    public Tilemap tilemap;
    public int chunkWidth;
    public int chunkHeight;
    public float renderDistance;
    public float seed;

    public bool save = false;

    GameObject holder;


    Dictionary<Vector2, Tilemap> chunks = new Dictionary<Vector2, Tilemap>();

    GameObject player;
    int startChunkX;
    int distance;

    void Start()
    {
        holder = GameObject.FindGameObjectWithTag("chunkHolder");
        player = GameObject.FindGameObjectWithTag("Player");

        if(seed == null) seed = Random.Range(5000, 100000);

        startChunkX = 0;//Random.Range(-10, 10);

        distance = Mathf.FloorToInt(renderDistance / 16);
    }

    public int getNumberOfLoadedChunksCount()
    {
        int i = 0;
        foreach(Transform c in holder.transform)
        {
            if (c.gameObject.activeSelf)
                i++;
        }
        return i;
    }

    public int getNumberOfChunksCount()
    {
        return holder.transform.childCount;
    }

    public Dictionary<Vector2, Tilemap> GetLoadedChunks()
    {
        return chunks;
    }

    private void Update()
    {
        Vector2 PlayerPos = player.transform.position;

        int xOff = Mathf.FloorToInt(PlayerPos.x / chunkWidth);
        int yOff = Mathf.FloorToInt(PlayerPos.y / chunkHeight);

        for(int x = xOff - distance; x < xOff + distance; x++)
        {
            for (int y = yOff - distance; y < yOff + distance - 1; y++)
            {
                Vector2 ChunkPos = new Vector2(x, y);

                if (chunks.ContainsKey(ChunkPos))
                {
                    foreach(Transform t in holder.transform)
                    {
                        float dist = Vector2.Distance(PlayerPos, t.position);

                        if (dist > renderDistance * 3)
                        {
                            Vector2 pos = (t.position + new Vector3(0,1)) / chunkWidth;
                            chunks.Remove(pos);

                            Destroy(t.gameObject);
                        }
                        else
                        {
                            t.gameObject.SetActive(dist < renderDistance);
                        }
                    }
                }
                else
                {
                    string name = $"{ChunkPos.x}_{ChunkPos.y}";
                    BuildingData chunk = SaveSystem.Load(name);

                    if(chunk != null)
                    {
                        Tilemap map = Instantiate(tilemap, holder.transform);
                        map.name = name;
                        map.transform.position = new Vector2(chunk.position[0], chunk.position[1]);

                        for (int xx = 0; xx < 16; xx++)
                        {
                            for (int yy = 0; yy < 17; yy++)
                            {
                                int i = xx + yy * 16;

                                int t = chunk.chunkTiles[i];

                                TileBase tile = null;

                                switch (t)
                                {
                                    case 1:
                                        tile = tiles[0];
                                        break;
                                    case 2:
                                        tile = tiles[1];
                                        break;
                                    case 3:
                                        tile = tiles[2];
                                        break;
                                    case 4:
                                        tile = tiles[3];
                                        break;
                                    case 5:
                                        tile = tiles[4];
                                        break;
                                }

                                map.SetTile(new Vector3Int(xx, yy, 0), tile);
                            }
                        }


                        chunks.Add(ChunkPos, map);
                    }
                    else
                    {
                        chunks.Add(ChunkPos, GenerateChunk(x, y));
                    }
                }
            }
        }
    }

    private Tilemap GenerateChunk(int chunkX, int chunkY)
    {
        Tilemap map = Instantiate(tilemap, holder.transform);
        map.name = $"{chunkX}_{chunkY}";

        int xx = chunkX * chunkWidth;
        int yy = chunkY * chunkHeight - 1;

        int ground = 0;
        for (int x = 0; x < chunkWidth; x++)
        {
            int yOff = Mathf.FloorToInt(PerlinNoise(xx + x, 0) * 100);
            for (int y = 0; y < chunkHeight; y++)
            {
                ground = yy + -y - yOff;
                Vector3Int pos = new Vector3Int(x, chunkHeight-y, 0);

                if (ground < 0)
                {
                    if (ground < toChunkHeight(-32))
                        map.SetTile(pos, tiles[3]);
                    else if (ground < toChunkHeight(-5))
                        map.SetTile(pos, tiles[2]);
                    else if (ground < toChunkHeight(-2))
                        map.SetTile(pos, tiles[1]);
                    else
                        map.SetTile(pos, tiles[0]);
                }
            }
        }

        map.transform.position = new Vector3(xx, yy);

        if(save) SaveSystem.Save(map);

        return map;
    }

    private int toChunkHeight(int num)
    {
        return num * chunkHeight;
    }

    private float PerlinNoise(int x, int y)
    {
        float[] octaveFrequencies = { 0.001f, 0.0015f, 0.005f, 0.025f };
        float[] octaveAmplitudes = { 1, 0.9f, 0.7f, 0f };
        float result = 0;
        for (int i = 0; i < octaveFrequencies.Length; i++)
            result += octaveAmplitudes[i] * Mathf.PerlinNoise(seed + octaveFrequencies[i] * x + .3f, seed + y * 0.02f) * 2f;

        result /= 3;

        return Mathf.Clamp(result, 0, 1);
    }
}
