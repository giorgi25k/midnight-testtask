using UnityEngine;

public class Fish_Flock : MonoBehaviour
{
    [SerializeField] private float _minSpeed = 0.1f;
    [SerializeField] private float _maxSpeed = 0.5f;
    [SerializeField] private float _rotationSpeed = 4.0f;
    [SerializeField] private float _neighbourDistance = 2.0f;
    [SerializeField] private float _avoidanceDistance = 2.0f;

    [SerializeField] private Transform _targetFood;
    [SerializeField] private GameObject _agentPrefabToSpawn;

    private float _speed;
    private bool _turning;

    void Start()
    {
        GameManager.Instance.fish.Add(this);
        UIManager.Instance.UpdateFishCountText(GameManager.Instance.fish.Count);
        _speed = Random.Range(_minSpeed, _maxSpeed);
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) >= FishManager.tankSize)
        {
            _turning = true;
        }
        else
        {
            _turning = false;
        }

        if (_turning)
        {
            Vector3 direction = Vector3.zero - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), _rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 5) < 1)
            {
                ApplyRules();
            }
        }

        if (_targetFood == null)
        {
            foreach (GameObject food in FoodManager.allFood)
            {
                if (food != null) // Add a null check for food
                {
                    float distToFood = Vector3.Distance(food.transform.position, transform.position);
                    if (distToFood <= _neighbourDistance)
                    {
                        // Change the target food and spawn a new agent
                        _targetFood = food.transform;

                        // Spawn a new agent when agents interact near the food
                        SpawnNewAgent();

                        break;
                    }
                }
            }
        }


        if (_targetFood != null)
        {
            Vector3 foodDirection = _targetFood.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(foodDirection), _rotationSpeed * Time.deltaTime);

            // Move towards the food
            transform.Translate(foodDirection.normalized * Time.deltaTime * _speed);
        }
        else
        {
            // If there is no food target, continue your previous movement
            transform.Translate(0, 0, Time.deltaTime * _speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            _targetFood = null;
        }
    }

    private void SpawnNewAgent()
    {
        if (_agentPrefabToSpawn != null)
        {
            // Instantiate a new agent at the current position of this agent
            GameObject newAgent = Instantiate(_agentPrefabToSpawn, transform.position, Quaternion.identity);
        }
    }


    private void ApplyRules()
    {
        GameObject[] gos;
        gos = FishManager.allFish;

        Vector3 vcenter = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.1f;

        Vector3 goalPos = FishManager.goalPos;

        float dist;

        int groupSize = 0;
        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= _neighbourDistance)
                {
                    vcenter = vcenter + go.transform.position;
                    groupSize++;

                    if (dist < _avoidanceDistance)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Fish_Flock anotherFlock = go.GetComponent<Fish_Flock>();
                    gSpeed = gSpeed + anotherFlock._speed;
                }
            }
        }

        // Check for food target
        if (_targetFood == null)
        {
            foreach (GameObject food in FoodManager.allFood)
            {
                if (food != null) // Add a null check for food
                {
                    float distToFood = Vector3.Distance(food.transform.position, transform.position);
                    if (distToFood <= _neighbourDistance)
                    {
                        _targetFood = food.transform;
                        break;
                    }
                }
            }
        }

        if (_targetFood != null)
        {
            Vector3 foodDirection = _targetFood.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(foodDirection), _rotationSpeed * Time.deltaTime);

            // Move towards the food
            transform.Translate(foodDirection.normalized * Time.deltaTime * _speed);
        }
        else
        {
            // If there is no food target, continue your previous movement
            transform.Translate(0, 0, Time.deltaTime * _speed);
        }
    }
    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }

    // Method to set the fish's turning speed
    public void SetTurningSpeed(float newTurningSpeed)
    {
        _rotationSpeed = newTurningSpeed;
    }

}
