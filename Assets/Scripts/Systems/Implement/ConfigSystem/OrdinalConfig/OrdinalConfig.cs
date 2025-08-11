using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PJR.Config
{
    /// <summary>
    /// 顺序表配置,以List存储配置
    /// </summary>
    /// <typeparam name="TAsset">存配置的Asset</typeparam>
    /// <typeparam name="TItemAsset">配置Asset里的用于存Item的Asset</typeparam>
    /// <typeparam name="TItem">Item类型</typeparam>
    public abstract class OrdinalConfig<TAsset, TItemAsset, TItem> : OrdinalConfig<TAsset, TItemAsset>
        where TAsset : OrdinalConfigAsset<TItemAsset, TItem> 
        where TItemAsset : OrdinalConfigItemAsset<TItem> 
        where TItem : OrdinalConfigItem
    {
        protected Dictionary<int, TItem> _id2Item;
        protected List<TItem> _items;
        public List<TItem> Items => _items;
        protected override void OnPreInitialize()
        {
            _items ??= new List<TItem>();
            _id2Item ??= new Dictionary<int, TItem>();

            _items.Clear();
            _id2Item.Clear();
        } 

        protected override void OnCacheItemAsset(TItemAsset itemAsset)
        {
            var item = itemAsset.item;

            if (!_id2Item.TryGetValue(item.ID, out var temp))
                _id2Item[item.ID] = item;
            else
                Debug.LogError($"[{this.GetType().Name}._  id2Item] 有重复ID = {item.ID}  [{itemAsset.Name}]  [{temp.Name}]");

            _items.Add(item);
             
            ExtraItemStorageProcess(item);
        }

        protected virtual void ExtraItemStorageProcess(TItem item) { }
        public override bool CheckValid()
        {
            if (!string.IsNullOrEmpty(_error))
                return false;
            if (_items == null || _id2Item == null || !_initialized)
                Initialize();
            return _initialized;
        }
        public override void Reset()
        {
            base.Reset();
            _items = null;
            _id2Item = null;
        }
        public virtual TItem GetConfigItem(int id)
        {
            if (!CheckValid())
                return null;
            return _id2Item.GetValueOrDefault(id);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 往Asset里放个新配置
        /// </summary>
        /// <param name="item"></param>
        public virtual string Editor_NewConfig(TItem item) => Editor_NewConfig(ItemAssetRoot, item, false);
        public virtual string Editor_NewConfig(TItem item, bool throwError) => Editor_NewConfig(ItemAssetRoot, item, throwError);
        public virtual string Editor_NewConfig(string directory, TItem item) => Editor_NewConfig(directory, item, false);
        public virtual string Editor_NewConfig(string directory, TItem item, bool throwError)
        {
            if (!Editor_IsItemCreatable(item, out string errorCode))
            {
                if (throwError)
                    throw new Exception(errorCode);
                return errorCode;
            }

            string assetPath = Path.Combine(directory, $"{ItemFileNamePrefix}_{item.ID}.asset");

            TItemAsset asset = CreateInstance<TItemAsset>();
            asset.item = item;
            AssetDatabase.CreateAsset(asset, assetPath);

            Editor_Asset.itemAssets.Add(asset);
            Editor_MaskAssetDirty(true);
            EditorApplication.delayCall += Editor_RefreshConfig;
            return null;
        }

        

        /// <summary>
        /// 往Asset里放个新配置
        /// </summary>
        /// <param name="item"></param>
        /// <returns>返回是否创建成功</returns>
        public bool Editor_NewConfig(TItem item, out string errorCode)
        {
            errorCode = Editor_NewConfig(item);
            return string.IsNullOrEmpty(Editor_NewConfig(item));
        }

        public override bool Editor_IsItemAssetCorrect(TItemAsset itemAsset)
        {
            if (itemAsset == null)
                return false;
            if (itemAsset.item == null)
            {
                Debug.Log($"{itemAsset.GetType().Name}.item == null", itemAsset);
                return false;
            }

            return true;
        }

#endif
    }
}
