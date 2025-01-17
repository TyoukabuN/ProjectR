using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR.Timeline
{
    public interface IErrorRecorder
    {
        public string Error { get; }
        public bool AnyError { get; }
    }
}
