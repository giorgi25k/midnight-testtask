using UnityEngine;

public class Fish_Flock : MonoBehaviour
{
    [SerializeField] private float _minSpeed = 0.1f;
    [SerializeField] private float _maxSpeed = 0.5f;
    [SerializeField] private float _rotationSpeed = 4.0f;
    [SerializeField] private Vector3 _averageHeading;
    [SerializeField] private Vector3 _averagePosition;
    [SerializeField] private float _neighbourDistance = 2.0f;

    private float _speed;
    private bool _turning;

    void Start()
    {
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

        transform.Translate(0, 0, Time.deltaTime * _speed);
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
                    vcenter = go.transform.position;
                    groupSize++;

                    if (dist < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Fish_Flock anotherFlock = go.GetComponent<Fish_Flock>();
                    gSpeed = gSpeed + anotherFlock._speed;
                }
            }

            if (groupSize > 0)
            {
                vcenter = vcenter / groupSize + (goalPos - transform.position);
                _speed = gSpeed / groupSize;

                Vector3 direction = (vcenter + vavoid) - transform.position;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), _rotationSpeed * Time.deltaTime);
                }
            }
        }
    }
}
