using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Area : MonoBehaviour
{
    public string ID;

    public float minBorderX;
    public float maxBorderX;
    public float minBorderY;
    public float maxBorderY;

    [SerializeField] private Tilemap spawnableArea;
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private List<Monster> spawnableMonsters;
    [SerializeField] private List<WildMonster> wildMonsters;
    [SerializeField] private int maxMonstersSpawned;
    private List<Vector3> validSpawnLocations = new List<Vector3>();

    private void Start()
    {
        SetSpawnableTiles();
    }

    private void Update()
    {
        if (wildMonsters.Count < maxMonstersSpawned)
        {
            SpawnMonster();
        }
    }

    private void OnDestroy()
    {
        foreach (WildMonster wildMonster in wildMonsters)
        {
            Destroy(wildMonster.gameObject);
        }
        wildMonsters.Clear();
    }

    public Monster GetRandomWildMonster()
    {
        Monster monster = spawnableMonsters[Random.Range(0, spawnableMonsters.Count)];
        monster.Init();
        return monster;
    }

    private void SpawnMonster()
    {
        if (validSpawnLocations.Count > 0)
        {
            Vector3 spawnLocation = validSpawnLocations[Random.Range(0, validSpawnLocations.Count)];
            WildMonster wildMonster = Instantiate(monsterPrefab, spawnLocation, Quaternion.identity, spawnableArea.transform).GetComponent<WildMonster>();
            wildMonster.Monster = GetRandomWildMonster();
            wildMonster.GetComponent<SpriteRenderer>().sprite = wildMonster.Monster.MonsterBase.WorldSprite;
            wildMonster.OnDefeated += this.DefeatedWildMonster;
            wildMonster.Monster.Init();
            wildMonsters.Add(wildMonster);
        }
    }

    private void SetSpawnableTiles()
    {
        validSpawnLocations.Clear();

        BoundsInt bounds = spawnableArea.cellBounds;
        TileBase[] allTiles = spawnableArea.GetTilesBlock(bounds);

        for (int n = spawnableArea.cellBounds.xMin; n < spawnableArea.cellBounds.xMax; n++)
        {
            for (int p = spawnableArea.cellBounds.yMin; p < spawnableArea.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)spawnableArea.transform.position.y));
                Vector3 place = spawnableArea.CellToWorld(localPlace);
                if (spawnableArea.HasTile(localPlace))
                {
                    validSpawnLocations.Add(place);
                }
            }
        }
    }

    private void DefeatedWildMonster(WildMonster wildMonster)
    {
        wildMonsters.Remove(wildMonster);
        Destroy(wildMonster.gameObject);
    }
}
