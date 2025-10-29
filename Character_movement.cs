using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Character_movement : MonoBehaviour
{

    public Grid grid;
    public Tilemap Floortilemap;
    public Tilemap FurnitureMap;
    public Tilemap WallMap;
    public Tilemap POIMap;
    public Tilemap LootMap;
    public Tilemap VictimMap;
    public Tilemap FireMap;

    public int lootPoints = 25;
    public int recuePoints = 100;

    public Destructable_Objects destructableObjects;
    public Camera cam;

    public float cellsPerSecond = 5f;
    public int damagePerHit = 1;

    Vector3Int currentCell;
    public bool isMoving = false;
    public PlayerAP ap;
    public int moveCost = 1;
    public int chopCost = 1;

    bool endTurnWhenMoveEnds = false;
    bool supressNextClick = false;




    // Start is called before the first frame update
    void Start()
    {
        if (!cam) cam = Camera.main;
        if (!ap) ap = GetComponent<PlayerAP>();
        currentCell = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(currentCell);
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnManager.I && TurnManager.I.phase != TurnPhase.Player)
        {
            return;
        }
        if (isMoving)
        {
            return;
        }
        if (!Input.GetMouseButtonDown(0))
            return;
            
        if(supressNextClick)
        {
            supressNextClick = false;
            return;
        }

        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
        var clicked = grid.WorldToCell(world);

        if (!IsAdjacent(clicked))
        {
            return;
        }

        if (IsWalkable(clicked))
        {
            Debug.Log("[Mover] Move action");
            if (ap && ap.CanSpend(moveCost))
            {
                ap.Spend(moveCost);
                Debug.Log($"Player AP after move: {ap.currentAP}");
                StartCoroutine(MoveToCell(clicked));
                TryAutoEndTurn();
            }
            else Debug.Log("[Mover] Not enough AP to move");
            return;
        }


        if (destructableObjects && destructableObjects.HasFurniture(clicked))
        {
            Debug.Log("[Mover] Chop action");
            if (ap && ap.CanSpend(chopCost))
            {
                ap.Spend(chopCost);
                Debug.Log($"Player AP after chop: {ap.currentAP}");
                destructableObjects.Chop(clicked);
                TryAutoEndTurn();
            }
            else Debug.Log("[Mover] Not enough AP to chop");
            return;
        }
        Debug.Log("[Mover] Return: not walkable and no destructible here");
    }

    public void TryAutoEndTurn()
    {
        if (ap != null && ap.currentAP <= 0)
        {
            if (isMoving)
            {
                endTurnWhenMoveEnds = true;
            }
            else
            {
                if(TurnManager.I)
                {
                    TurnManager.I.EndPlayerTurn();
                }
            }
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    bool IsAdjacent(Vector3Int cell)
    {
        int dx = Math.Abs(cell.x - currentCell.x);
        int dy = Math.Abs(cell.y - currentCell.y);

        if (dx <= 1 && dy <= 1 && (dx + dy > 0))
            return true;

        return false;
    }

    bool IsWalkable(Vector3Int cell)
    {
        if(FurnitureMap && FurnitureMap.HasTile(cell))
            return false;
        if (WallMap && WallMap.HasTile(cell))
            return false;
        if (POIMap && POIMap.HasTile(cell))
            return false;
        if(FireMap && FireMap.HasTile(cell))
            return false;
        return Floortilemap.HasTile(cell);
    }

    IEnumerator MoveToCell(Vector3Int targetCell)
    {
        isMoving = true;

        Vector3 start = transform.position;
        Vector3 target = grid.GetCellCenterWorld(targetCell);

        float distance = Vector3.Distance(start, target);
        float duration = distance / Mathf.Max(0.01f, cellsPerSecond);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        currentCell = targetCell;
        isMoving = false;

        if (LootMap && LootMap.HasTile(currentCell))
        {
            LootMap.SetTile(currentCell, null);
            if (ScoreManager.I != null)
            {
                ScoreManager.I.AddPoints(lootPoints);
            }
        }
        if(VictimMap && VictimMap.HasTile(currentCell))
        {
            VictimMap.SetTile(currentCell, null);
            if (ScoreManager.I != null)
            {
                ScoreManager.I.AddPoints(recuePoints);
            }
        }

        if (endTurnWhenMoveEnds)
        {
            endTurnWhenMoveEnds = false;
            Debug.Log("[Mover] Ending turn after move");
            if (TurnManager.I)
                TurnManager.I.EndPlayerTurn();
        }
    }

    public void ConsumeNextClick()
    {
        supressNextClick = true;
    }
    
}
