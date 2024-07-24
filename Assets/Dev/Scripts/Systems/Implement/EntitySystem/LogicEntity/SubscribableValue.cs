using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.Serialization;

namespace PJR
{
    public abstract class SubscribableValue<T>
    {
        public class OnValueChangeEvent : UnityEvent<T, T> { }

        public OnValueChangeEvent OnValueChange = new OnValueChangeEvent();

        public T Value;

        public void AddListener(UnityAction<T, T> callback) => OnValueChange.AddListener(callback);
        public void RemoveListener(UnityAction<T, T> callback) => OnValueChange.RemoveListener(callback);
    }
    public class IntegerValue : SubscribableValue<int>
    {
        public static implicit operator int(IntegerValue rhs)
        {
            return rhs.Value;
        }
    }
    public class FloatValue : SubscribableValue<float>
    {
        public static implicit operator float(FloatValue rhs)
        {
            return rhs.Value;
        }
    }
    public class DoubleValue : SubscribableValue<double>
    {
        public static implicit operator double(DoubleValue rhs)
        {
            return rhs.Value;
        }
    }
    public class StringValue : SubscribableValue<string>
    {
        public static implicit operator string(StringValue rhs)
        {
            return rhs.Value;
        }
    }
    public class Vector3Value : SubscribableValue<Vector3>
    {
        public static implicit operator Vector3(Vector3Value rhs)
        {
            return rhs.Value;
        }
    }
    public class Vector2Value : SubscribableValue<Vector2>
    {
        public static implicit operator Vector2(Vector2Value rhs)
        {
            return rhs.Value;
        }
    }
}