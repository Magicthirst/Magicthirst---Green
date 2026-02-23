using UnityEngine;

namespace Levels.Visual
{
    public class RotatingByConstant : MonoBehaviour
    {
        [SerializeField] public bool running;
        [SerializeField] private Vector3 vector;

        private void Update()
        {
            if (running)
            {
                transform.eulerAngles += vector * Time.deltaTime;
            }
        }
    }
}