using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    public class FlockMember : MonoBehaviour
    {
        public enum MemberType
        {
            bird = 0,
            fish
        }

        public MemberType flockMemberType = MemberType.bird;
        public float minSpeed= 0;
        public float maxSpeed = 10;
        private float speed;
        Vector3 averageHeading;
        Vector3 averagePosition;

        float neighborDistance = 3.0f;

        bool turning = false;
        public FlockManager FlockManager
        {
            get
            {
                return flockManager;
            }
            set
            {
                flockManager = value;
            }
        }
        private FlockManager flockManager;
        private Transform _rotationTransform;
        private bool _delayTurning;

        public void OnEnable()
        {
            speed = Random.Range(minSpeed, maxSpeed);

        }
        public void OnValidate()
        {
            speed = Random.Range(minSpeed, maxSpeed);
        }
        public void Update()
        {
            if (flockManager == null)
            {
                if (transform.parent.TryGetComponent<FlockManager>(out flockManager)) { }
                else
                {
                    return;
                }

            }



            ApplyTankBoundary();

            if (_rotationTransform == null)
            {
                if (flockMemberType == MemberType.fish && gameObject.GetComponentInChildren<MeshRenderer>() != null)
                {
                    _rotationTransform = gameObject.GetComponentInChildren<MeshRenderer>().transform;
                }
                else
                {
                    _rotationTransform = transform.GetChild(0);
                }
            }

            if (turning)
            {
                var direction = flockManager.transform.position - transform.position;
                var targetRotation = Quaternion.Slerp(_rotationTransform.rotation,
                    Quaternion.LookRotation(direction),
                    TurnSpeed() * Time.deltaTime);

                _rotationTransform.rotation = targetRotation;

                var isAtRotation = isApproximate(targetRotation, transform.rotation, 0.1f);

                speed = Random.Range(minSpeed, maxSpeed);
            }
            else
            {
                if (Random.Range(0, 5) < 1)
                    ApplyRules();
            }

            if (_rotationTransform != null)
            {
                var translateAmnt = _rotationTransform.forward * Time.deltaTime * speed;
                transform.Translate(translateAmnt);
            }
        }

        public static bool isApproximate(Quaternion q1, Quaternion q2, float precision)
        {
            var prec = Mathf.Abs(Quaternion.Dot(q1, q2));
            var thresh = 1f - precision;
            return prec >= thresh;
        }

        private void ApplyTankBoundary()
        {
            if (!_delayTurning)
            {
                if (Vector3.Distance(transform.position, flockManager.transform.position) >= flockManager.flockZoneSize)
                {
                    turning = true;
                }
                else
                {
                    turning = false;
                }
            }
        }

        private IEnumerator DelayTurning()
        {
            _delayTurning = true;
            yield return new WaitForSeconds(5f);
            _delayTurning = false;
        }

        private void ApplyRules()
        {

            if (flockManager == null)
                flockManager = FindObjectOfType<FlockManager>();

            if (flockManager == null)
                return;


            var gos = flockManager.GetAllMembers();

            if (gos == null)
                return;

            var vCenter = flockManager.transform.position;
            var vAvoid = Vector3.zero;
            var gSpeed = 0.1f;

            var goalPos = flockManager.goalPos;

            float dist;
            var groupSize = 0;

            foreach (var go in gos)
            {
                if (go != null)
                {
                    if (go != this.gameObject)
                    {
                        dist = Vector3.Distance(go.transform.position, this.transform.position);
                        if (dist <= neighborDistance)
                        {
                            vCenter += go.transform.position;
                            groupSize++;

                            if (dist < 0.75f)
                            {
                                vAvoid = vAvoid + (this.transform.position - go.transform.position);
                            }

                            var anotherFish = go.GetComponent<FlockMember>();
                            gSpeed += anotherFish.speed;
                        }

                    }
                }
            }

            if (groupSize > 0)
            {
                vCenter = vCenter / groupSize + (goalPos - this.transform.position);
                speed = gSpeed / groupSize;

                var direction = (vCenter + vAvoid) - flockManager.transform.position;
                if (direction != Vector3.zero)
                {
                    _rotationTransform.rotation = Quaternion.Slerp(_rotationTransform.rotation,
                        Quaternion.LookRotation(direction),
                        TurnSpeed() * Time.deltaTime);
                }
            }

        }

        float TurnSpeed()
        {
            return Random.Range(minSpeed / 2f, maxSpeed / 2f);
        }
    }
}

