using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
    [Conditional("UNITY_EDITOR")]
    public class HunterSearchableAttribute : SearchableAttribute
    {
        /// <summary>
        /// 自定义绘制
        /// </summary>
        public string DrawBeforeSearchBar;
        
        /// <summary>
        /// 自定义列表项是否被过滤 
        /// private bool ShouldDrawItem(int index){...} 
        /// </summary>
        public string CustomMatchFunc;
     
        /// <summary>
        /// 自定义过滤器是否激活 
        /// private bool CustomSearchIsEmpty()
        /// {
        ///     //类似这样
        ///     return currentFilterEnum == FilterEnum.None; 
        /// } 
        /// </summary>
        public string CustomSearchIsEmpty;
        
        /// <summary>
        /// 自定义过滤选项是否需要更新
        /// private bool FilterHasChanged(){...}
        /// </summary>
        public string CustomFilterDirtyFunc;
        
        
    }
}
