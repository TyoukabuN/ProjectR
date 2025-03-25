using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YooAsset.Editor
{
    public class TaskCopyBuildToPaths_SBP : TaskCopyBuildToPaths, IBuildTask
    {
        void IBuildTask.Run(BuildContext context)
        {
            var buildParametersContext = context.GetContextObject<BuildParametersContext>();
            var manifestContext = context.GetContextObject<ManifestContext>();
   
            CopyBuildToPaths(buildParametersContext, manifestContext.Manifest);
        }
    }
}