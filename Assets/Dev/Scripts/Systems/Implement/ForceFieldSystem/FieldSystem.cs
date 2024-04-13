using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using System;
using PJR;

namespace PJR
{

    public partial class FieldSystem : MonoSingletonSystem<FieldSystem>
    {
        public int s_id = -1;

        private List<FieldObject> fields = new List<FieldObject>();
        private Dictionary<int, FieldObject> id2handles = new Dictionary<int,FieldObject>();

        public FieldObject GetField(int id)
        {
            if (!TryGetField(id, out var obj))
                return null;
            return obj;
        }

        public bool ExistField(int id)
        {
            if (!id2handles.TryGetValue(id, out var obj))
                return false;
            return obj != null;
        }

        public bool TryGetField(int id,out FieldObject obj)
        {
            return id2handles.TryGetValue(id, out obj);
        }

        private T CreateFieldObject<T>(string name = "[ForceField]_NoName") where T : ForceField
        {
            if (instance == null)
                return null;
            //
            GameObject gobj = new GameObject(name);
            //
            var fobj = gobj.AddComponent<T>();
            fobj.OnCreate();
            //
            gobj.transform.SetParent(GetFieldRoot(), false);
            //
            return fobj;
        }
        public void RemoveField(int id)
        {
            if (id < 0)
                return;
            if (!TryGetField(id, out var field))
                return;
            RemoveField(field);
        }
        public void RemoveField(FieldObject field)
        {
            if (field == null)
                return;
            field.Dispose();
            //
            fields.Remove(field);
            id2handles.Remove(field.id);
            //
            DestroyImmediate(field.gameObject);
        }

        [NonSerialized]
        private GameObject fieldObjectRoot = null;
        public const string fieldObjectLocalPath = "FieldObjects";

        private Transform GetFieldRoot()
        {
            if (instance == null)
                return null;
            if (fieldObjectRoot == null)
            { 
                fieldObjectRoot = new GameObject(fieldObjectLocalPath);
                fieldObjectRoot.transform.SetParent(instance.transform, false);
            }
            return fieldObjectRoot.transform;
        }
        public static void Tick(float deltaTime)
        {
            //EntitySystem.instance.AllEntities();
            //for (int i = 0; i < instance.fields.Count; i++)
            //{
            //    var field = instance.fields[i];
            //    field.Tick(deltaTime);
            //}
        }
    }

}
