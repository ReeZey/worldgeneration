using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class BuildingData
{
    public float[] position = new float[2];
    public int[] chunkTiles;
    public BuildingData(Tilemap g)
    {
        position[0] = g.transform.position.x;
        position[1] = g.transform.position.y;

        chunkTiles = new int[16 * 17];

        for(int x = 0; x < 16; x++)
        {
            for(int y = 0; y < 17; y++){
                TileBase t = g.GetTile(new Vector3Int(x, y, 0 ));

                int i = x + y * 16;

                if (t == null)
                {
                    chunkTiles[i] = 0;
                    continue;
                }

                switch (t.name)
                {
                    case "mark":
                        chunkTiles[i] = 1;
                        break;
                    case "jord":
                        chunkTiles[i] = 2;
                        break;
                    case "stone":
                        chunkTiles[i] = 3;
                        break;
                    case "bedrock":
                        chunkTiles[i] = 4;
                        break;
                    case "block":
                        chunkTiles[i] = 5;
                        break;
                }
            }
        }
    }
}