using UnityEngine;

public class LegMovement : MonoBehaviour
{
    // Direction of the leg movement rotation (helps in leg synchronisation)
    public float stepDirection = -1.0f;
    private float _movementForce = 50;

    // Helper variable used for changing the direction of the leg movement
    private int _timeCount;
    private int _N = 50;
    private Rigidbody _rBody;

    // Start is called before the first frame update
    void Start()
    {
        _timeCount = _N;
        _rBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Define the step direction
        _timeCount--;

        // Change the direction of the step
        if (stepDirection == 1.0f && _timeCount == 0)
            stepDirection = -1.0f;
        else if (stepDirection == -1.0f && _timeCount == 0)
            stepDirection = 1.0f;

        // Restart the movement
        if (_timeCount == 0)
            _timeCount = _N;

        var forceAmnt = stepDirection * transform.forward * _movementForce;
        // Debug.Log($"forceAmnt = {forceAmnt}");

        // Move the leg in a specified direction
        _rBody.AddForce(forceAmnt, ForceMode.Acceleration);

    }
}
