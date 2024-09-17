using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiFPS
{
    public class MinimapCamera : MonoBehaviour
    {
        public static MinimapCamera Instance;
        private Transform target;
        private Camera _minimapCamera;


        private void Awake()
        {
            Instance = this;

            if (!target)
            {
                target = transform;
            }

            _minimapCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (target)
                transform.SetPositionAndRotation(target.position, target.rotation);
        }

        public void SetTarget(Transform _target)
        {
            if (!_target)
                return;

            target = _target;
        }
    }
}