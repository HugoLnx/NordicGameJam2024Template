using System.Collections;
using System.Collections.Generic;
using Jam;
using SensenToolkit.Gizmosx;
using SensenToolkit.Rand;
using UnityEngine;
using PrefabDict = System.Collections.Generic.Dictionary<(CustomerType, CustomerDifficulty), Customer>;

public class EnemySpawner : ASingleton<EnemySpawner>
{
    private Coroutine _spawnRoutine;
    [SerializeField] private List<Customer> _customerPrefabs;
    private PrefabDict _customerPrefabDict;
    private PrefabDict CustomerPrefabDict => _customerPrefabDict ??= BuildCustomerPrefabDict();

    public List<Vector2> Spawnpoints => _spawnpoints ??= FindAndPrepareAllSpawnpoints();

    private List<Vector2> _spawnpoints;
    private List<Vector2> usedSpawns = new();

    public bool shouldSpawn = true;
    private RandomEnumerableWeighted<CustomerDifficulty> _difficultyRandomizer;

    private DifficultyProperties CurrentProperties
        => SpawnerPropertiesCurve.Instance.GetCurrentProperties();
    private float SpawnCooldownFromDifficulty
        => CurrentProperties.Cooldown;
    private const float InitialCooldown = 2f;
    private readonly WaitForEndOfFrame _waitForEndOfFrame = new();

    private new void Awake()
    {
        base.Awake();
        System.Array customerTypes = System.Enum.GetValues(typeof(CustomerDifficulty));
        var customerTypesList = new List<CustomerDifficulty>();
        foreach (CustomerDifficulty type in customerTypes)
        {
            customerTypesList.Add(type);
        }
        _difficultyRandomizer = new RandomEnumerableWeighted<CustomerDifficulty>(
            values: customerTypesList,
            weights: new List<float> { 0.2f, 0.35f, 0.45f }
        );
    }

    private void Start()
    {
        // Enforce that the spawnpoints doesn't have srenderers
        List<Vector2> points = Spawnpoints;
    }

    public void StartSpawning()
    {
        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        StopCoroutine(_spawnRoutine);

        Customer[] allCustomersRemaining = FindObjectsOfType<Customer>();
        foreach (Customer cust in allCustomersRemaining)
        {
            Destroy(cust.gameObject);
        }
    }

    private IEnumerator SpawnRoutine()
    {
        bool hasSpawnedBefore = false;
        while (shouldSpawn)
        {
            if (hasSpawnedBefore)
            {
                yield return new WaitUntil(() => Spawnpoints.Count > 0);
                yield return WaitCooldown();
            }
            else
            {
                yield return new WaitForSeconds(InitialCooldown);
                hasSpawnedBefore = true;
            }
            Customer customer = Instantiate(GetRandomCustomer());
            Vector2 chosenSpawn = GetRandomSpawnpoint();
            customer.OnFinish += () => FreeSpawnpoint(chosenSpawn);
            customer.transform.position = chosenSpawn;
        }
    }

    private IEnumerator WaitCooldown()
    {
        float waited = 0f;
        while (waited < SpawnCooldownFromDifficulty)
        {
            waited += Time.deltaTime;
            yield return _waitForEndOfFrame;
        }
        Debug.Log($"Waited for {waited} seconds");
    }

    private Vector2 GetRandomSpawnpoint()
    {
        Vector2 randomPoint = Spawnpoints[Random.Range(0, Spawnpoints.Count)];
        Spawnpoints.Remove(randomPoint);
        usedSpawns.Add(randomPoint);
        return randomPoint;
    }

    private void FreeSpawnpoint(Vector2 spawnpointToFree)
    {
        usedSpawns.Remove(spawnpointToFree);
        Spawnpoints.Add(spawnpointToFree);
    }

    private Customer GetRandomCustomer()
    {
        CustomerDifficulty difficulty = _difficultyRandomizer.Select();
        DifficultyProperties prop = CurrentProperties;
        var typeRandomizer = new RandomEnumerableWeighted<CustomerType>(
            values: new List<CustomerType> { CustomerType.A, CustomerType.B, CustomerType.C },
            weights: new List<float> { prop.ChanceCustomerA, prop.ChanceCustomerB, prop.ChanceCustomerC }
        );
        CustomerType ctype = typeRandomizer.Select();
        return CustomerPrefabDict[(ctype, difficulty)];
    }

    private PrefabDict BuildCustomerPrefabDict()
    {
        var dict = new PrefabDict();
        foreach (Customer customer in _customerPrefabs)
        {
            dict.Add((customer.Type, customer.Difficulty), customer);
        }
        return dict;
    }

    private const string SPAWNPOINT_TAG = "TableSpot";
    private List<Vector2> FindAndPrepareAllSpawnpoints()
    {
        GameObject[] spots = GameObject.FindGameObjectsWithTag(SPAWNPOINT_TAG);
        List<Vector2> spawnpoints = new();
        foreach (GameObject spot in spots)
        {
            spot.GetComponentInChildren<SpriteRenderer>().enabled = false;
            spawnpoints.Add(spot.transform.position);
        }
        return spawnpoints;
    }

}
