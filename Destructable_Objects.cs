using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct DestructableObjectsConfig
{
    public TileBase tile;
    public int maxHP;
}

public class Destructable_Objects : MonoBehaviour
{

    public Tilemap FurnitureMap;
    public TilemapCollider2D FurnitureCollider;
    public List<DestructableObjectsConfig> destructableObjectsConfigs;
    private Dictionary<Vector3Int, int> destructableObjects = new Dictionary<Vector3Int, int>();

    public AudioSource audioSource;
    public AudioClip hitSound;

    public int defaultHP = 1;



    void Reset()
    {
        if (!FurnitureMap) FurnitureMap = GetComponent<Tilemap>();
        if (!FurnitureCollider) FurnitureCollider = GetComponent<TilemapCollider2D>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
    }

    int GetMaxHPForTile(TileBase tile)
    {
        if (tile == null) return defaultHP;

        foreach (var c in destructableObjectsConfigs)
        {
            if (c.tile == tile) return Mathf.Max(1, c.maxHP);
        }
        return Mathf.Max(1, defaultHP);
    }

    public bool HasFurniture(Vector3Int cell)
    {
        return FurnitureMap && FurnitureMap.HasTile(cell);
    }

    public int GetHP(Vector3Int cell)
    {
        if (!HasFurniture(cell))
            return 0;
        if (!destructableObjects.TryGetValue(cell, out int hp))
        {
            var t = FurnitureMap.GetTile(cell);
            hp = GetMaxHPForTile(t);
            destructableObjects[cell] = hp;
        }
        return hp;
    }

    public bool Chop(Vector3Int cell)
    {
        if (!HasFurniture(cell))
        {
            Debug.Log($"No furniture at {cell} to chop");
            return false;
        }

        Debug.Log($"Chopping furniture at {cell}");

        int hp = GetHP(cell);
        hp -= 1;

        if (audioSource && hitSound)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (hp <= 0)
        {
            FurnitureMap.SetTile(cell, null);
            destructableObjects.Remove(cell);

            if (FurnitureCollider)
            {
                FurnitureCollider.enabled = false;
                FurnitureCollider.enabled = true;
            }
            return true;
        }
        else
        {
            destructableObjects[cell] = hp;
            return false;
        }
    }

}
