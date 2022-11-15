using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AnythingWorld.Animation;
using AnythingWorld.Behaviours;

namespace AnythingWorld.Utilities
{
    public class CameraFollowUtility : MonoBehaviour
    {
        public Transform target;
        public Vector3 minOffset = new Vector3(4, 5, 4);
        public Vector3 maxOffSet = new Vector3(10, 10, 10);
        public bool RandomTarget = true;

        public Vector3 m_Offset;
        private List<GameObject> randomTargets;
        private float _targetWait;
        private int _targetInd;
        private float _lerpSpeed = 0.2f;
        private AWObj _talkAWObj;
        private AWBehaviourController behaviourController;

        public enum CameraMode
        {
            Follow,
            TalkTo,
            Ride
        }
        private CameraMode _cameraMode;

        void Start()
        {
            SwitchModes(CameraMode.Follow);
            MakeOffset();
            if (RandomTarget)
            {
                SelectTarget();
                StartCoroutine(SwitchTargets());
            }

        }

        public void SwitchModes(CameraMode newMode)
        {
            _cameraMode = newMode;
            switch (_cameraMode)
            {
                case CameraMode.TalkTo:
                    _lerpSpeed = 20f;
                    break;
                case CameraMode.Ride:
                    _lerpSpeed = 20f;
                    break;
                default:
                    if (_talkAWObj != null)
                        _talkAWObj.ActivateRBs(true);
                    // TODO: extend this, object will not always have randommovement so should be list of active behaviours here actually - GM
                    if (behaviourController != null)
                    {
                        behaviourController.AddBehaviourScript("RandomMovement");
                        behaviourController.RefreshBehaviours();
                    }
                    _lerpSpeed = 0.5f;
                    SelectTarget();
                    MakeOffset();
                    break;
            }
        }

        void LateUpdate()
        {
            if (_cameraMode == CameraMode.Ride)
            {
                transform.position = target.transform.position + m_Offset;
                var adjustment = Quaternion.Euler(25f, 0f, 0f);
                transform.rotation = target.transform.rotation * adjustment;
            }
            else
            {
                if (target == null)
                    return;

                var newPosition = new Vector3(target.position.x + m_Offset.x, target.position.y + m_Offset.y,
                    target.position.z + m_Offset.z);
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, newPosition, _lerpSpeed * Time.deltaTime);

                var direction = target.position - transform.position;
                var toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 1f * Time.deltaTime);
            }

        }

        private IEnumerator SwitchTargets()
        {
            while (true)
            {
                _targetWait = Random.Range(4f, 10f);
                yield return new WaitForSeconds(_targetWait);

                if (_cameraMode == CameraMode.Follow)
                {
                    SelectTarget();
                    MakeOffset();
                }
            }
        }

        private void MakeOffset()
        {
            m_Offset = new Vector3(Random.Range(minOffset.x, maxOffSet.x), Random.Range(minOffset.y, maxOffSet.y), Random.Range(minOffset.z, maxOffSet.z));
        }

        public void SelectTarget()
        {

            randomTargets = new List<GameObject>();

            // find moving AW things
            var awThings = GameObject.FindObjectsOfType<AnythingAnimationController>();
            foreach (var awT in awThings)
            {
                randomTargets.Add(awT.gameObject);
            }

            // find flock AW things
            var flockThings = GameObject.FindObjectsOfType<FlockMember>();
            foreach (var flockT in flockThings)
            {
                randomTargets.Add(flockT.gameObject);
            }

            // try to get a rigidbody target
            if (randomTargets.Count == 0)
            {
                var rBodies = GameObject.FindObjectsOfType<Rigidbody>();
                foreach (var rBody in rBodies)
                {
                    if (!rBody.isKinematic)
                    {
                        randomTargets.Add(rBody.gameObject);
                    }
                }
            }

            // if this fails try to get obj piece to follow by name
            if (randomTargets.Count == 0)
            {
                randomTargets = GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name == "WavefrontObject").ToList();
            }

            _targetInd = Random.Range(0, randomTargets.Count - 1);
            if (randomTargets.Count == 0)
                return;
            target = randomTargets[_targetInd].transform;
        }
        public void TalkTo(string objectName)
        {
            SwitchModes(CameraMode.TalkTo);

            //GameObject talkObject = GameObject.Find(objectName);
            AWObj talkObject = null;
            var listAWOBJ = FindObjectsOfType<AWObj>().ToList();
            foreach (var creature in listAWOBJ)
            {
                if (creature.ModelName == objectName)
                {
                    talkObject = creature;
                    break;
                }
            }
            if (talkObject == null)
            {
                talkObject = AnythingCreator.Instance.MakeObject(objectName);
            }
            if (talkObject == null) { return; }
            else
            {
                StartCoroutine(TalkWhenFinished(talkObject));
            }
        }


        public void RideTarget(Transform newTarget)
        {
            SwitchModes(CameraMode.Ride);
            target = newTarget;
            m_Offset = new Vector3(0f, 5f, 0f);
        }

        public void RideTarget(AWObj _aWObj)
        {
            StartCoroutine(RideWhenFinished(_aWObj));
        }

        private IEnumerator RideWhenFinished(AWObj _aWObj)
        {
            while (!_aWObj.ObjMade)
            {
                yield return new WaitForSeconds(1);
            }

            SwitchModes(CameraMode.Ride);
            target = _aWObj.PrefabTransform.transform;
            m_Offset = new Vector3(0f, 5f, 0f);
        }

        private IEnumerator TalkWhenFinished(AWObj targetAWObj)
        {
            while (!targetAWObj.ObjMade)
            {
                yield return new WaitForSeconds(1);
            }

            var talkObject = targetAWObj.gameObject;
            var targetTrans = targetAWObj.PrefabTransform.transform;
            //Transform targetTrans = talkObject.transform.GetChild(0);
            //AWObj targetAWObj = talkObject.GetComponent<AWObj>();
            var camYPos = 2f;
            var camZPos = 7f;
            if (targetAWObj != null)
            {
                camYPos += targetAWObj.BoundsYOffset;
                camZPos += targetAWObj.BoundsYOffset;
            }
            target = targetTrans; // 1st child has the position!!
            m_Offset = (targetTrans.transform.forward * camZPos) + (targetTrans.transform.up * camYPos);
            _talkAWObj = talkObject.GetComponent<AWObj>();
            if (_talkAWObj != null)
            {
                _talkAWObj.ActivateRBs(false);
            }

            // TODO: this is temportary code for a demo! we need to disable / then enable aactive behaviours actually - GM
            behaviourController = talkObject.GetComponent<AWBehaviourController>();
            if (behaviourController != null)
            {
                behaviourController.RemoveBehaviour("RandomMovement");
            }
        }
    }
}

