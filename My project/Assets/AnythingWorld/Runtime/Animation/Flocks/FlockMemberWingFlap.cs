using System.Collections;
using UnityEngine;

//[ExecuteAlways]
public class FlockMemberWingFlap : MonoBehaviour
{
    public float RotateSpeed = 5f;
    public Vector3 rotateTo = new Vector3(0, 0, 20f);
    public Vector3 rotateFrom= new Vector3(0, 0, -20f);
    private Vector3 startAngle;

    private bool wingsInitialized;
    private Vector3 currentAngle;

    private bool directionTo;
    private Transform wingTransform;
    private float delayStart;
    [SerializeField]
    public Transform leftWing;
    [SerializeField]
    public Transform rightWing;
    public void Start()
    {
        rotateTo = transform.localEulerAngles + rotateTo;
        wingsInitialized = false;
        delayStart = Random.Range(0f, 0.4f);
        
        StartCoroutine(StartFlap());

    }


    public IEnumerator StartFlap()
    {
        yield return new WaitForSeconds(delayStart);
        leftWing.localEulerAngles = rotateFrom;
        rightWing.localEulerAngles = new Vector3(-rotateFrom.x, rotateFrom.y, rotateFrom.z);
        if (leftWing != null && rightWing != null)
        {
            directionTo = true;
            startAngle = leftWing.localEulerAngles;
            currentAngle = leftWing.localEulerAngles;
            wingsInitialized = true;
            yield return null;
        }
        else
        {
            Debug.Log("Transforms not set for wing flap");
        }
        yield return null;
    }


    public void FixedUpdate()
    {
        if (wingsInitialized)
        {
            var speedStep = Time.deltaTime* RotateSpeed;

            float angle;
            bool sameRotation;

            if (directionTo)
            {
                currentAngle = new Vector3(
                Mathf.LerpAngle(currentAngle.x, rotateTo.x, speedStep),
                Mathf.LerpAngle(currentAngle.y, rotateTo.y, speedStep),
                Mathf.LerpAngle(currentAngle.z, rotateTo.z, speedStep));


                angle = Quaternion.Angle(Quaternion.Euler(currentAngle), Quaternion.Euler(rotateTo));
                sameRotation = Mathf.Abs(angle) < 1f;

                if (sameRotation)
                {
                    directionTo = false;
                }
            }
            else
            {
                currentAngle = new Vector3(
                Mathf.LerpAngle(currentAngle.x, rotateFrom.x, speedStep),
                Mathf.LerpAngle(currentAngle.y, rotateFrom.y, speedStep),
                Mathf.LerpAngle(currentAngle.z, rotateFrom.z, speedStep));

                angle = Quaternion.Angle(Quaternion.Euler(currentAngle), Quaternion.Euler(startAngle));
                sameRotation = Mathf.Abs(angle) < 1f;

                if (sameRotation)
                {
                    directionTo = true;
                }

            }

            leftWing.localEulerAngles = -currentAngle;
            rightWing.localEulerAngles = currentAngle;
        }
    }
}
