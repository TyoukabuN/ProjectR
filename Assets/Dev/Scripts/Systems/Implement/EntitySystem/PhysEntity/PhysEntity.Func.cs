using UnityEngine;

namespace PJR
{
    public partial class PhysEntity : MonoBehaviour
    {
        public void SetPosition(Vector3 pos)
        {
            motor?.SetPosition(pos);
        }
    }
}
