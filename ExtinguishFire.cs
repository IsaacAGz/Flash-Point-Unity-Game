using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExtinguishFire : MonoBehaviour
{
    [Header("Maps")]
    public Tilemap fireMap;

    [Header("References")]
    public Camera cam;
    public Grid grid;
    public Character_movement mover;
    public PlayerAP ap;

    [Header("Gameplay")]
    public int extinguishCost = 1;

    public AudioSource audioSource;
    public AudioClip extinguishClip;

    // Start is called before the first frame update
    void Start()
    {
        if (!cam)   cam = Camera.main;
        if (!grid)  grid = FindObjectOfType<Grid>();
        if (!ap)    ap = GetComponent<PlayerAP>();
        if (!mover) mover = GetComponent<Character_movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnManager.I && TurnManager.I.phase != TurnPhase.Player)
        {
            return;
        }
        if (mover.IsMoving())
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

        if (!fireMap || !fireMap.HasTile(cell))
        {
            return;
        }

        Vector3Int playerCell = grid.WorldToCell(transform.position);
        if (!IsAdjacent(playerCell, cell))
        {
            return;
        }

        if (ap == null || !ap.CanSpend(extinguishCost))
        {
            return;
        }

        ap.Spend(extinguishCost);
        Debug.Log($"Player AP after extinguishing fire: {ap.currentAP}");
        fireMap.SetTile(cell, null);
        audioSource.PlayOneShot(extinguishClip);

        if (audioSource && extinguishClip)
        {
            audioSource.PlayOneShot(extinguishClip);
        }

        if (mover) mover.ConsumeNextClick();

        if (mover) mover.TryAutoEndTurn();
    }

    //checks if both tiles are adjacent by measuring difference in distance between two points
    bool IsAdjacent(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }
}
