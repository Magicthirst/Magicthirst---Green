using System;
using UnityEngine;

namespace Levels.Util
{
    public class PublishContacts : MonoBehaviour
    {
        public event Action<Collider> ContactEntered;
        public event Action<Collider> ContactExited;

        private void OnTriggerEnter(Collider other)
        {
            ContactEntered?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            ContactExited?.Invoke(other);
        }
    }
}