using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;


public class FireSpawner : MonoBehaviour
{

    public Tilemap FloorTilemap;
    public Tilemap FurnitureTilemap;
    public Tilemap WallTilemap;
    public Tilemap FireMap;
    public Tilemap POIMap;
    public TileBase FireTile;
    
    public Grid grid;
    public Transform player;
    public PlayerHealth playerHealth;

    public int damagePerHit = 1;
    int MaxFiresCap = 10;
    public int MaxFiresPerRound = 3;


    public AudioSource audioSource;
    public AudioClip FireClip;

    static readonly Vector3Int[] adjacentOffsets = new[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
    };

    // Start is called before the first frame update
    void Start()
    {
        int saved = MaxFiresPerRound;
        MaxFiresPerRound = Mathf.Max(saved, 5);
        SpawnFires();
        MaxFiresPerRound = saved;
    }


    //
    void OnEnable()
    {
        StartCoroutine(SubscribeWhenReady());
    }

    void OnDisable()
    {
        if (TurnManager.I != null) TurnManager.I.OnRoundEnded -= OnRoundEndedHandler;
    }

    //Increases number of fires per round and lights new fires
    void OnRoundEndedHandler()
    {
        if (MaxFiresPerRound < MaxFiresCap)
        {
            MaxFiresPerRound++;
        }

        SpawnFires();
    }

    IEnumerator SubscribeWhenReady()
    {
        while (TurnManager.I == null) yield return null;

        TurnManager.I.OnRoundEnded -= OnRoundEndedHandler;
        TurnManager.I.OnRoundEnded += OnRoundEndedHandler;
    }

    //Spawns new fires from the available tiles 
    void SpawnFires()
    {
        //plays audio clip for lighting fires
        if (audioSource && FireClip)
        {
            audioSource.PlayOneShot(FireClip);
        }

        Debug.Log("Spawning Fires");
        
        //checks that tilemaps arent null
        if (!FireMap || !FloorTilemap || !FireTile || !POIMap) return;

        //stores a list of Available tiles to light fires in
        var openTiles = GetOpenTilesIncludingFires();

        //count of availble tiles to light on fire is not 0
        if (openTiles.Count == 0) return;

        //Selects random tile form list of available tiles
        for (int i = openTiles.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (openTiles[i], openTiles[j]) = (openTiles[j], openTiles[i]);
        }

        //number of tiles to place fire on this round
        int toPlace = Mathf.Min(MaxFiresPerRound, openTiles.Count);

        //Places fires or spread existing ones
        for (int i = 0; i < toPlace; i++)
        {
            var cell = openTiles[i];

            if (!FireMap.HasTile(cell))
            {
                IgniteFromSpawn(cell);
            }
            else
            {
                Debug.Log("Spreading fire from existing fire at " + cell);
                SpreadFrom(cell);
            }
        }
    }

    //Gets list of available tiles to light fire
    List<Vector3Int> GetOpenTilesIncludingFires()
    {
        List<Vector3Int> openTiles = new List<Vector3Int>();

        var bounds = FloorTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (!IsOpenCell(cell))
                {
                    continue;
                }
                openTiles.Add(cell);
            }
        }

        return openTiles;
    }

    //When fire is ignited on a tile without fire, just places new fires in tile.
    void IgniteFromSpawn(Vector3Int cell)
    {
        if (!IsOpenCell(cell)) return;
        if (FireMap.HasTile(cell)) return;
        if (isPlayerAtCell(cell)) return;
        FireMap.SetTile(cell, FireTile);
    }

    //When fire is ignited on tiles with fire on it, sreads fire in all four directions.
    void IgniteFromSpread(Vector3Int cell)
    {
        if (!IsOpenCell(cell)) return;
        if (FireMap.HasTile(cell)) return;

        FireMap.SetTile(cell, FireTile);
        DamagePlayer(cell);
    }

    //Checks if cell is without structure tiles
    bool IsOpenCell(Vector3Int cell)
    {
        if (!FloorTilemap.HasTile(cell)) return false;
        if (FurnitureTilemap && FurnitureTilemap.HasTile(cell)) return false;
        if (WallTilemap && WallTilemap.HasTile(cell)) return false;
        if (POIMap && POIMap.HasTile(cell)) return false;
        return true;
    }

    //Checks that player is not at cell
    bool isPlayerAtCell(Vector3Int cell)
    {
        Vector3Int playerCell = grid.WorldToCell(player.position);
        return cell == playerCell;
    }

    //Calls Take Damage function from playerHealth.cs
    void DamagePlayer(Vector3Int fireCell)
    {
        var playerCell = grid.WorldToCell(player.position);
        if (fireCell == playerCell)
        {
            Debug.Log("Damage Taken");
            playerHealth?.TakeDamage(damagePerHit);
        }
    }
    
    //Lights fires in all four directions of fire spread
    void SpreadFrom(Vector3Int cel)
    {
        foreach (var offset in adjacentOffsets)
        {
            Vector3Int adjacentCell = cel + offset;
            IgniteFromSpread(adjacentCell);
        }
    }
}
