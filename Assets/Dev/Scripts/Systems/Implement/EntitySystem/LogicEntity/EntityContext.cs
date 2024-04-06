using UnityEngine;

namespace PJR
{
    public class EntityContext
    {
        public int logicEntityID = -1;
        public string assetFullName;

        public Vector2 inputAxi;
        public Vector2 mouseDelta;
        public float runValue;
        public int grounded = 1;
        struct State { }
    }
}