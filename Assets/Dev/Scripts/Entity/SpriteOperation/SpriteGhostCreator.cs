using UnityEngine;
using System.Collections.Generic;

namespace TinyGame
{
    public class SpriteGhostCreator : MonoBehaviour
    {
        public class Ghost {
            public GameObject gameObject;
            public SpriteRenderer renderer;
            public float duration = 0;
            float counter = 0;
            public float counterNormalize
            {
                get { return counter / duration; }
            }
            public Ghost(GameObject gobj, float duration)
            {
                this.gameObject = gobj;
                this.renderer = gobj.GetComponent<SpriteRenderer>();
                this.counter = duration;
                this.duration = duration;
            }
            public void Update(float deltaTime)
            {
                counter -= deltaTime;
                if (renderer)
                {
                    var color = renderer.color;
                    color.a = counterNormalize;
                    renderer.color = color;
                }
            }
            public bool IsValid()
            {
                return counter > 0;
            }
        }
        List<Ghost> ghosts = new List<Ghost>();

        public SpriteRenderer targetSpriteRenderer;
        public bool creating = false;
        [Range(0.01f,100f)]
        public float createInterval = 0.2f;
        [Range(0.01f, 100f)]
        public float ghostLifetime = 0.33f;
        public Color ghostColor = Color.white;
        private float counter = 0;

        public void Awake()
        {
            targetSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if(targetSpriteRenderer == null)
                targetSpriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        }
        // Update is called once per frame
        void FixedUpdate()
        {
            if (targetSpriteRenderer == null)
                return;

            for (var i = 0; i < ghosts.Count; i++)
            {
                var ghost = ghosts[i];
                if (ghost == null)
                    continue;
                if (!ghost.IsValid())
                {
                    GameObject.Destroy(ghost.gameObject);
                    ghosts.RemoveAt(i--);
                    continue;
                }
                ghost.Update(Time.fixedDeltaTime);
            }

            counter += Time.fixedDeltaTime;
            if (counter >= createInterval && creating)
            {
                counter = 0;
                var gobj = new GameObject();
                //gobj.transform.SetParent(TinyGameManager.instance.gameRoot.transform);
                gobj.transform.position = targetSpriteRenderer.transform.position;
                gobj.transform.rotation = targetSpriteRenderer.transform.rotation;
                gobj.transform.localScale = targetSpriteRenderer.transform.localScale;
                var renderer = gobj.AddComponent<SpriteRenderer>();
                renderer.sprite = targetSpriteRenderer.sprite;
                ghosts.Add(new Ghost(gobj, ghostLifetime));
                renderer.color = ghostColor;
                renderer.sortingOrder = targetSpriteRenderer.sortingOrder - 1;
            }
        }
        public void SetSpriteRenderer(SpriteRenderer renderer)
        {
            targetSpriteRenderer = renderer;
        }
    }
}