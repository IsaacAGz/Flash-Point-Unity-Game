using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class POIRevealer : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public Grid grid;
    public Character_movement mover;
    public PlayerAP ap;

    [Header("Maps")]
    public Tilemap POIMap;
    public Tilemap LootMap;
    public Tilemap VictimMap;


    [Header("Tiles")]
    public TileBase LootTile;
    public TileBase VictimTile;

    [Header("Gameplay:")]
    public int revealCost = 1;
    [Range(0f, 1f)] public float npcChance = 0.5f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (!cam) cam = Camera.main;
        if (!grid) grid = FindObjectOfType<Grid>();
        if (!ap) ap = GetComponent<PlayerAP>();
        if (!mover) mover = GetComponent<Character_movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnManager.I && TurnManager.I.phase != TurnPhase.Player)
        {
            return;
        }

        if (mover && mover.IsMoving())
        {
            return;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        var mouse = Input.mousePosition;

        Vector3 world = cam.ScreenToWorldPoint(mouse);
        Vector3Int cell = grid.WorldToCell(world);

        if (!POIMap.HasTile(cell))
        {
            return;
        }

        Vector3Int playerCell = grid.WorldToCell(transform.position);
        if (!IsAdjacent(playerCell, cell))
        {
            return;
        }

        if (!ap.CanSpend(revealCost))
        {
            Debug.Log("Not enough AP to reveal POI");
            return;
        }

        ap.Spend(revealCost);
        Debug.Log($"Player AP after revealing POI: {ap.currentAP}");
        POIMap.SetTile(cell, null);

        if(mover) mover.TryAutoEndTurn();

        bool isNPC = Random.value < npcChance;

        if (isNPC)
        {
            VictimMap.SetTile(cell, VictimTile);
            Debug.Log("Revealed NPC at " + cell);
        }
        else
        {
            LootMap.SetTile(cell, LootTile);
            Debug.Log("Revealed Loot at " + cell);
        }
        mover.ConsumeNextClick();
        mover.TryAutoEndTurn();
    }

    bool IsAdjacent(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }
    
    
}
