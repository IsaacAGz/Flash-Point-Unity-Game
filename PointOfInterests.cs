using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PointOfInterests : MonoBehaviour
{
    [Header("Maps")]
    public Tilemap FloorTilemap;
    public Tilemap FurnitureTilemap;
    public Tilemap WallTilemap;
    public Tilemap POIMap;
    public Tilemap FireMap;


    [Header("POI")]
    public TileBase POITile;
    public int MaxPOIsOnBoard = 3;
    public Vector2Int POISpawnRange = new Vector2Int(1, 3);

    

    // Start is called before the first frame update
    void Start()
    {
        SpawnPOIs();
    }

    void Update()
    {
        
    }
    void OnEnable()
    {
        TrySusbcribe();
    }

    void OnDisable()
    {
        TryUnsusbcribe();
    }

    void TrySusbcribe()
    {
        if (TurnManager.I)
        {
            TurnManager.I.OnRoundEnded -= SpawnPOIs;
            TurnManager.I.OnRoundEnded += SpawnPOIs;
        }
    }

    void TryUnsusbcribe()
    {
        if (TurnManager.I)
        {
            TurnManager.I.OnRoundEnded -= SpawnPOIs;
        }
    }

    void SpawnPOIs()
    {
        Debug.Log("Spawning POIs");
        if (!POIMap || !FloorTilemap || !POITile) return;

        int existing = CountExistingPOIs();
        int capacity = Mathf.Max(0, MaxPOIsOnBoard - existing);

        if (capacity <= 0) return;

        int toSpawn = capacity;

        var openTiles = GetOpenTiles();

        if (openTiles.Count == 0) return;


        for (int i = openTiles.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (openTiles[i], openTiles[j]) = (openTiles[j], openTiles[i]);
        }

        int count = Mathf.Min(toSpawn, openTiles.Count);
        for (int i = 0; i < count; i++)
        {
            POIMap.SetTile(openTiles[i], POITile);
        }
    }
    
    int CountExistingPOIs()
    {
        int count = 0;
        var bounds = POIMap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (POIMap.HasTile(cell))
                {
                    count++;
                }
            }
        }
        return count;
    }
    
    List<Vector3Int> GetOpenTiles()
    {
        List<Vector3Int> openTiles = new List<Vector3Int>();

        var bounds = FloorTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (FloorTilemap.HasTile(cell) &&
                    !FurnitureTilemap.HasTile(cell) &&
                    !WallTilemap.HasTile(cell) &&
                    !FireMap.HasTile(cell))
                {
                    openTiles.Add(cell);
                }
            }
        }

        return openTiles;
    }
}
