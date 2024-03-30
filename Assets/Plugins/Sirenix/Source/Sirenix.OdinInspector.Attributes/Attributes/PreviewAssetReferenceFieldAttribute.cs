using System;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class PreviewAssetReferenceFieldAttribute : Attribute
    {
        private ObjectFieldAlignment alignment;
        private bool alignmentHasValue;

        /// <summary>
        /// The height of the object field
        /// </summary>
        public float Height;

        /// <summary>
        /// Left aligned.
        /// </summary>
        public ObjectFieldAlignment Alignment 
        { 
            get { return this.alignment; }

            set
            { 
                this.alignment = value; 
                this.alignmentHasValue = true; 
            } 
        }

        /// <summary>
        /// Whether an alignment value is specified.
        /// </summary>
        public bool AlignmentHasValue { get { return this.alignmentHasValue; } }

        public string OnPreviewGUIRect;
        
        /// <summary>
        /// Draws a square object field which renders a preview for UnityEngine.Object type objects.
        /// </summary>
        public PreviewAssetReferenceFieldAttribute()
        {
            this.Height = 0;
        }

        /// <summary>
        /// Draws a square object field which renders a preview for UnityEngine.Object type objects.
        /// </summary>
        /// <param name="height">The height of the preview field.</param>
        public PreviewAssetReferenceFieldAttribute(float height)
        {
            this.Height = height;
        }

        /// <summary>
        /// Draws a square object field which renders a preview for UnityEngine.Object type objects.
        /// </summary>
        /// <param name="height">The height of the preview field.</param>
        /// <param name="alignment">The alignment of the preview field.</param>
        public PreviewAssetReferenceFieldAttribute(float height, ObjectFieldAlignment alignment)
        {
            this.Height = height;
            this.Alignment = alignment;
        }

        /// <summary>
        /// Draws a square object field which renders a preview for UnityEngine.Object type objects.
        /// </summary>
        /// <param name="alignment">The alignment of the preview field.</param>
        public PreviewAssetReferenceFieldAttribute(ObjectFieldAlignment alignment)
        {
            this.Alignment = alignment;
        }
        public PreviewAssetReferenceFieldAttribute(float height, ObjectFieldAlignment alignment,string OnPreviewGUIRect = null)
        {
            this.Height = height;
            this.Alignment = alignment;
            this.OnPreviewGUIRect = OnPreviewGUIRect;
        }
    }
}
