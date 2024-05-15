using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;
using System;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Unity.EditorCoroutines.Editor;
using UnityEngine.SceneManagement;
//

public class AssetAnalysisTool : AssetAnalysisBase
{
    [MenuItem(@"Tools/Asset分析工具")]
    static void Open()
    {
        var handle = GetWindow<AssetAnalysisTool>();
        handle.OnRegisterEvent();
    }

    public void OnRegisterEvent()
    {
        Selection.selectionChanged -= this.Repaint;
        Selection.selectionChanged += this.Repaint;

        Selection.selectionChanged -= this.AssetRenameTool_OnSelectionChange;
        Selection.selectionChanged += this.AssetRenameTool_OnSelectionChange;

        EditorSceneManager.sceneOpened -= OnSceneOpened;
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorSceneManager.sceneClosed -= OnSceneClosed;
        EditorSceneManager.sceneClosed += OnSceneClosed;
    }
    public void OnUndoRedoPerformed()
    {
        Debug.Log($"OnUndoRedoPerformed: {Undo.GetCurrentGroupName()}");
    }
    public void WillFlushUndoRecord()
    {
        Debug.Log($"WillFlushUndoRecord: {Undo.GetCurrentGroupName()}");
    }
    public void OnScene(SceneView scene)
    {

    }

    public void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
    }
    public void OnSceneClosed(Scene scene)
    {
    }

    public enum GlobalShaderPropertyType
    {
        Int,
        Float,
    }

    public static AnimBoolHandle animBool_globalProperty;
    public static AnimBoolHandle animBool_setPropertyByQuality;
    //
    public static AnimBoolHandle animBool_weather;
    //
    public static AnimBoolHandle animBool_hitEffect;
    //
    public static AnimBoolHandle animBool_material_modify;
    public static AnimBoolHandle animBool_material_shader_replace;
    public static AnimBoolHandle animBool_set_shader_property;
    public static AnimBoolHandle animBool_set_shader_property_in_path;
    //
    public static AnimBoolHandle animBool_material_check;
    public static AnimBoolHandle animBool_material_check_unemployed_tex;
    public static AnimBoolHandle animBool_material_check_select_standard_shader;
    public static AnimBoolHandle animBool_material_check_select_materials_in_path;
    public static AnimBoolHandle animBool_material_check_select_materials;
    //
    public static AnimBoolHandle animBool_resource;
    public static AnimBoolHandle animBool_resource_import;
    public static AnimBoolHandle animBool_resource_memery_report;
    public static AnimBoolHandle animBool_resource_memery_report_specific_prefab;
    public static AnimBoolHandle animBool_resource_memery_report_except_paths;
    public static AnimBoolHandle animBool_resource_memery_report_filter;
    public static AnimBoolHandle animBool_resource_lookingfor_same;
    public static AnimBoolHandle animBool_resource_print_selected_memery;
    public static AnimBoolHandle animBool_resource_find_assets_name_includeChineseSgin;
    public static AnimBoolHandle animBool_resource_effect_export_quality_csv;
    //
    public static AnimBoolHandle animBool_resource_sameassetname;
    public static AnimBoolHandle animBool_resource_sameassetname_except_paths;
    public static AnimBoolHandle animBool_resource_sameassetname_filter;
    //
    public static AnimBoolHandle animBool_resource_rename;
    //
    public static AnimBoolHandle animBool_resource_empty_ref_report;
    public static AnimBoolHandle animBool_resource_mess_ref_report;
    //
    public static AnimBoolHandle animBool_LookingForTheSame_except_paths;
    public static AnimBoolHandle animBool_LookingForTheSame_filter;
    //
    public static AnimBoolHandle animBool_animation;
    public static AnimBoolHandle animBool_animation_oc_dereference;
    //
    public static AnimBoolHandle animBool_lua;
    public static AnimBoolHandle animBool_lua_optimize;
    //
    public static AnimBoolHandle animBool_prefab;
    public static AnimBoolHandle animBool_prefab_findEffectNgenLowVerOne;
    public static AnimBoolHandle animBool_prefab_findEffectWithoutPartcleLowLayer;
    public static AnimBoolHandle animBool_prefab_prefabReferenceDeduplication;
    public static AnimBoolHandle animBool_prefab_prefabNameReplace;
    public static AnimBoolHandle animBool_prefab_deduplication_filter;
    public static AnimBoolHandle animBool_prefab_dereference_filter;
    //
    public static AnimBoolHandle animBool_scene;
    public static AnimBoolHandle animBool_scene_setup_current_as_newTypeStream;
    public static AnimBoolHandle animBool_scene_streaming_asset_except_paths;
    public static AnimBoolHandle animBool_scene_streaming_asset_except_hierarchyPaths;
    public static AnimBoolHandle animBool_scene_streaming_asset_priority_hierarchyPaths;
    public static AnimBoolHandle animBool_scene_setup_current_as_newTypeStream_setup;
    public static AnimBoolHandle animBool_scene_setup_current_as_newTypeStream_animeParam;
    public static AnimBoolHandle animBool_scene_setup_current_as_newTypeStream_test;
    public static AnimBoolHandle animBool_scene_streaming_error_check;
    public static AnimBoolHandle animBool_scene_streaming_error_check_material_asset_except_paths;
    public static AnimBoolHandle animBool_scene_valid_check;
    public static AnimBoolHandle animBool_scene_streaming_invalid_shader_path;
    public static AnimBoolHandle animBool_scene_streaming_rough_make;
    //
    public static AnimBoolHandle animBool_scene_genAllStreaming;
    public static AnimBoolHandle animBool_scene_genAllStreaming_asset_except_paths;


    public static List<SearchType> SearchTypes;
    public static List<SearchType> SearchTypes_MemeryReport;
    public static List<SearchType> SearchTypes_LookingForTheSame;
    public static List<SearchType> SearchTypes_SameNameAsset;

    public static BoolHandle modify_xtime;
    public static BoolHandle use_ditherMap;
    public static ObjectHandle<Texture> ditherMap;
    public static FloatHandle ditherSize;
    public static FloatHandle ditherFadeGlobalControl;
    //public static BoolHandle ditherFadeGlobalControlReverse;
    public static BoolHandle ditherFadeGlobalControlEnabled;
    //
    public static Vector4Handle ditherGlobalCircleFadeAnimaCenterPreview;
    public static FloatHandle ditherGlobalCircleFadeAnimaEdgeSpread;
    public static FloatHandle ditherGlobalCircleFadeAnimaEdgeRoughness;

    protected virtual void OnEnable()
    {
        OnRegisterEvent();
        //
        animBool_globalProperty = new AnimBoolHandle(this.GetType().Name + "animBool_globalProperty", false);
        animBool_globalProperty.valueChanged.AddListener(Repaint);
        animBool_setPropertyByQuality = new AnimBoolHandle(this.GetType().Name + "animBool_setPropertyByQuality", false);
        animBool_setPropertyByQuality.valueChanged.AddListener(Repaint);
        //
        animBool_weather = new AnimBoolHandle(this.GetType().Name + "animBool_weather", false);
        animBool_weather.valueChanged.AddListener(Repaint);
        //
        animBool_hitEffect = new AnimBoolHandle(this.GetType().Name + "animBool_hitEffect", false);
        animBool_hitEffect.valueChanged.AddListener(Repaint);
        //
        animBool_material_modify = new AnimBoolHandle(this.GetType().Name + "animBool_quality", false);
        animBool_material_modify.valueChanged.AddListener(Repaint);
        animBool_material_shader_replace = new AnimBoolHandle(this.GetType().Name + "animBool_material_change_to_scene_mege_shader", false);
        animBool_material_shader_replace.valueChanged.AddListener(Repaint);
        animBool_set_shader_property = new AnimBoolHandle(this.GetType().Name + "animBool_set_shader_property", false);
        animBool_set_shader_property.valueChanged.AddListener(Repaint);
        animBool_set_shader_property_in_path = new AnimBoolHandle(this.GetType().Name + "animBool_set_shader_property_in_path", false);
        animBool_set_shader_property_in_path.valueChanged.AddListener(Repaint);
        //
        animBool_material_check = new AnimBoolHandle(this.GetType().Name + "animBool_material_check", false);
        animBool_material_check.valueChanged.AddListener(Repaint);
        animBool_material_check_unemployed_tex = new AnimBoolHandle(this.GetType().Name + "animBool_material_check_unrefered_property", true);
        animBool_material_check_unemployed_tex.valueChanged.AddListener(Repaint);
        animBool_material_check_select_standard_shader = new AnimBoolHandle(this.GetType().Name + "animBool_material_check_select_standard_shader", false);
        animBool_material_check_select_standard_shader.valueChanged.AddListener(Repaint);
        animBool_material_check_select_materials = new AnimBoolHandle(this.GetType().Name + "animBool_material_check_select_materials", false);
        animBool_material_check_select_materials.valueChanged.AddListener(Repaint);
        animBool_material_check_select_materials_in_path = new AnimBoolHandle(this.GetType().Name + "animBool_material_check_select_materials_in_path", false);
        animBool_material_check_select_materials_in_path.valueChanged.AddListener(Repaint);
        //
        animBool_resource = new AnimBoolHandle(this.GetType().Name + "animBool_resource", false);
        animBool_resource.valueChanged.AddListener(Repaint);
        animBool_resource_import = new AnimBoolHandle(this.GetType().Name + "animBool_resource_import", false);
        animBool_resource_import.valueChanged.AddListener(Repaint);
        animBool_resource_memery_report = new AnimBoolHandle(this.GetType().Name + "animBool_resource_memery_report", false);
        animBool_resource_memery_report.valueChanged.AddListener(Repaint);
        animBool_resource_memery_report_filter = new AnimBoolHandle(this.GetType().Name + "animBool_resource_memery_report_filter", false);
        animBool_resource_memery_report_filter.valueChanged.AddListener(Repaint);
        animBool_resource_lookingfor_same = new AnimBoolHandle(this.GetType().Name + "animBool_resource_lookingfor_same", false);
        animBool_resource_lookingfor_same.valueChanged.AddListener(Repaint);
        animBool_resource_print_selected_memery = new AnimBoolHandle(this.GetType().Name + "animBool_resource_print_selected_memery", false);
        animBool_resource_print_selected_memery.valueChanged.AddListener(Repaint);
        animBool_resource_find_assets_name_includeChineseSgin = new AnimBoolHandle(this.GetType().Name + "animBool_resource_find_assets_name_includeChineseSgin", false);
        animBool_resource_find_assets_name_includeChineseSgin.valueChanged.AddListener(Repaint);
        animBool_resource_effect_export_quality_csv = new AnimBoolHandle(this.GetType().Name + "animBool_resource_effect_export_quality_csv", false);
        animBool_resource_effect_export_quality_csv.valueChanged.AddListener(Repaint);
        animBool_resource_sameassetname = new AnimBoolHandle(this.GetType().Name + "animBool_resource_sameassetname", false);
        animBool_resource_sameassetname.valueChanged.AddListener(Repaint);
        animBool_resource_sameassetname_except_paths = new AnimBoolHandle(this.GetType().Name + "animBool_resource_sameassetname_except_paths", false);
        animBool_resource_sameassetname_except_paths.valueChanged.AddListener(Repaint);
        animBool_resource_sameassetname_filter = new AnimBoolHandle(this.GetType().Name + "animBool_resource_sameassetname_filter", false);
        animBool_resource_sameassetname_filter.valueChanged.AddListener(Repaint);
        animBool_resource_rename = new AnimBoolHandle(this.GetType().Name + "animBool_resource_rename", false);
        animBool_resource_rename.valueChanged.AddListener(Repaint);

        animBool_LookingForTheSame_except_paths = new AnimBoolHandle(this.GetType().Name + "animBool_LookingForTheSame_except_paths", false);
        animBool_LookingForTheSame_except_paths.valueChanged.AddListener(Repaint);
        animBool_LookingForTheSame_filter = new AnimBoolHandle(this.GetType().Name + "animBool_LookingForTheSame_filter", false);
        animBool_LookingForTheSame_filter.valueChanged.AddListener(Repaint);
        //
        animBool_resource_memery_report_specific_prefab = new AnimBoolHandle(this.GetType().Name + "animBool_resource_memery_report_specific_prefab", false);
        animBool_resource_memery_report_specific_prefab.valueChanged.AddListener(Repaint);
        animBool_resource_memery_report_except_paths = new AnimBoolHandle(this.GetType().Name + "animBool_resource_memery_report_except_paths", false);
        animBool_resource_memery_report_except_paths.valueChanged.AddListener(Repaint);
        animBool_resource_empty_ref_report = new AnimBoolHandle(this.GetType().Name + "animBool_resource_empty_ref_report", false);
        animBool_resource_empty_ref_report.valueChanged.AddListener(Repaint);
        animBool_resource_mess_ref_report = new AnimBoolHandle(this.GetType().Name + "animBool_resource_mess_ref_report", false);
        animBool_resource_mess_ref_report.valueChanged.AddListener(Repaint);
        //
        animBool_prefab = new AnimBoolHandle(this.GetType().Name + "animBool_prefab", false);
        animBool_prefab.valueChanged.AddListener(Repaint);
        animBool_prefab_findEffectNgenLowVerOne = new AnimBoolHandle(this.GetType().Name + "animBool_prefab_findEffectNgenLowVerOne", false);
        animBool_prefab_findEffectNgenLowVerOne.valueChanged.AddListener(Repaint);
        animBool_prefab_findEffectWithoutPartcleLowLayer = new AnimBoolHandle(this.GetType().Name + "animBool_prefab_findEffectWithoutPartcleLowLayer", false);
        animBool_prefab_findEffectWithoutPartcleLowLayer.valueChanged.AddListener(Repaint);
        animBool_prefab_prefabReferenceDeduplication = new AnimBoolHandle(this.GetType().Name + "animBool_prefab_prefabReferenceDeduplication", false);
        animBool_prefab_prefabReferenceDeduplication.valueChanged.AddListener(Repaint);
        animBool_prefab_prefabNameReplace = new AnimBoolHandle(this.GetType().Name + "animBool_prefab_prefabNameReplace", false);
        animBool_prefab_prefabNameReplace.valueChanged.AddListener(Repaint);
        animBool_prefab_deduplication_filter = new AnimBoolHandle(this.GetType().Name + "animBool_prefab_prefabReferenceDeduplication_filter", false);
        animBool_prefab_deduplication_filter.valueChanged.AddListener(Repaint);
        animBool_prefab_dereference_filter = new AnimBoolHandle(this.GetType().Name + "animBool_prefab_dereference_filter", false);
        animBool_prefab_dereference_filter.valueChanged.AddListener(Repaint);
        //
        animBool_animation = new AnimBoolHandle(this.GetType().Name + "animBool_animation", false);
        animBool_animation.valueChanged.AddListener(Repaint);
        animBool_animation_oc_dereference = new AnimBoolHandle(this.GetType().Name + "animBool_animation_oc_dereference", false);
        animBool_animation_oc_dereference.valueChanged.AddListener(Repaint);
        //
        animBool_lua = new AnimBoolHandle(this.GetType().Name + "animBool_lua", false);
        animBool_lua.valueChanged.AddListener(Repaint);
        animBool_lua_optimize = new AnimBoolHandle(this.GetType().Name + "animBool_lua_optimize", false);
        animBool_lua_optimize.valueChanged.AddListener(Repaint);
        //
        animBool_scene = new AnimBoolHandle(this.GetType().Name + "animBool_scene", false);
        animBool_scene.valueChanged.AddListener(Repaint);
        animBool_scene_setup_current_as_newTypeStream = new AnimBoolHandle(this.GetType().Name + "animBool_scene_setup_current_as_newTypeStream", false);
        animBool_scene_setup_current_as_newTypeStream.valueChanged.AddListener(Repaint);
        animBool_scene_streaming_asset_except_paths = new AnimBoolHandle(this.GetType().Name + "animBool_scene_streaming_asset_except_paths", false);
        animBool_scene_streaming_asset_except_paths.valueChanged.AddListener(Repaint);
        animBool_scene_streaming_asset_except_hierarchyPaths = new AnimBoolHandle(this.GetType().Name + "animBool_scene_streaming_asset_except_hierarchyPaths", false);
        animBool_scene_streaming_asset_except_hierarchyPaths.valueChanged.AddListener(Repaint);
        animBool_scene_streaming_asset_priority_hierarchyPaths = new AnimBoolHandle(this.GetType().Name + "animBool_scene_streaming_asset_priority_hierarchyPaths", false);
        animBool_scene_streaming_asset_priority_hierarchyPaths.valueChanged.AddListener(Repaint);
        animBool_scene_setup_current_as_newTypeStream_setup = new AnimBoolHandle(this.GetType().Name + "animBool_scene_setup_current_as_newTypeStream_setup", false);
        animBool_scene_setup_current_as_newTypeStream_setup.valueChanged.AddListener(Repaint);
        animBool_scene_setup_current_as_newTypeStream_animeParam = new AnimBoolHandle(this.GetType().Name + "animBool_scene_setup_current_as_newTypeStream_animeParam", false);
        animBool_scene_setup_current_as_newTypeStream_animeParam.valueChanged.AddListener(Repaint);
        animBool_scene_setup_current_as_newTypeStream_test = new AnimBoolHandle(this.GetType().Name + "animBool_scene_setup_current_as_newTypeStream_test", false);
        animBool_scene_setup_current_as_newTypeStream_test.valueChanged.AddListener(Repaint);
        animBool_scene_streaming_error_check = new AnimBoolHandle(this.GetType().Name + "animBool_scene_streaming_error_check", false);
        animBool_scene_streaming_error_check.valueChanged.AddListener(Repaint);
        animBool_scene_streaming_error_check_material_asset_except_paths = new AnimBoolHandle(this.GetType().Name + "animBool_scene_streaming_error_check_material_asset_except_paths", false);
        animBool_scene_streaming_error_check_material_asset_except_paths.valueChanged.AddListener(Repaint);
        animBool_scene_valid_check = new AnimBoolHandle(this.GetType().Name + "animBool_scene_valid_check", false);
        animBool_scene_valid_check.valueChanged.AddListener(Repaint);
        animBool_scene_streaming_invalid_shader_path = new AnimBoolHandle(this.GetType().Name + "animBool_scene_streaming_invalid_shader_path", true);
        animBool_scene_streaming_invalid_shader_path.valueChanged.AddListener(Repaint);
        animBool_scene_streaming_rough_make = new AnimBoolHandle(this.GetType().Name + "animBool_scene_streaming_rough_make", true);
        animBool_scene_streaming_rough_make.valueChanged.AddListener(Repaint);
        //
        animBool_scene_genAllStreaming = new AnimBoolHandle(this.GetType().Name + "animBool_scene_genAllStreaming", true);
        animBool_scene_genAllStreaming.valueChanged.AddListener(Repaint);
        animBool_scene_genAllStreaming_asset_except_paths = new AnimBoolHandle(this.GetType().Name + "animBool_scene_genAllStreaming_asset_except_paths", true);
        animBool_scene_genAllStreaming_asset_except_paths.valueChanged.AddListener(Repaint);
        //
        modify_xtime = new BoolHandle(this.GetType().Name + "modify_xtime", true);
        use_ditherMap = new BoolHandle(this.GetType().Name + "use_ditherMap", true);
        ditherMap = new ObjectHandle<Texture>(this.GetType().Name + "ditherMap", null);
        ditherSize = new FloatHandle(this.GetType().Name + "ditherSize", 4);
        ditherFadeGlobalControl = new FloatHandle(this.GetType().Name + "ditherFadeGlobalControl", 0);
        //ditherFadeGlobalControlReverse = new BoolHandle(this.GetType().Name + "ditherFadeGlobalControlRervse", true);
        ditherFadeGlobalControlEnabled = new BoolHandle(this.GetType().Name + "ditherFadeGlobalControlEnabled", false);
        ditherGlobalCircleFadeAnimaCenterPreview = new Vector4Handle(this.GetType().Name + "ditherGlobalCircleFadeAnimaCenter", Vector4.zero);
        ditherGlobalCircleFadeAnimaEdgeSpread = new FloatHandle(this.GetType().Name + "ditherGlobalCircleFadeAnimaEdgeSpread", 1);
        ditherGlobalCircleFadeAnimaEdgeRoughness = new FloatHandle(this.GetType().Name + "ditherGlobalCircleFadeAnimaEdgeRoughness", 0);
        //
        SearchTypes = GetSearchTypes("SearchTypes_ResourceReimport_");
        SearchTypes_MemeryReport = GetSearchTypes("SearchTypes_MemeryReport");
        SearchTypes_LookingForTheSame = GetSearchTypes("SearchTypes_LookingForTheSame");
        SearchTypes_SameNameAsset = GetSearchTypes("SearchTypes_SameNameAsset");
    }

    protected List<SearchType> GetSearchTypes(string editorPrefKeyPre)
    {
        return new List<SearchType>() {
            new SearchType("t:AnimationClip",   new GUIContent("AnimationClip"),new string[]{".anim"}, editorPrefKeyPre),
            new SearchType("t:AudioClip",       new GUIContent("AudioClip"),    new string[]{".ogg"}, editorPrefKeyPre),
            new SearchType("t:AudioMixer",      new GUIContent("AudioMixer"),   new string[]{".mixer"}, editorPrefKeyPre),
            new SearchType("t:ComputeShader",   new GUIContent("ComputeShader"),new string[]{".compute"}, editorPrefKeyPre),
            new SearchType("t:Font",            new GUIContent("Font"),         new string[]{".tff"}, editorPrefKeyPre),
            new SearchType("t:GUISkin",         new GUIContent("GUISkin"),      new string[]{}, editorPrefKeyPre),
            new SearchType("t:Material",        new GUIContent("Material"),     new string[]{".mat"}, editorPrefKeyPre),
            new SearchType("t:Mesh",            new GUIContent("Mesh"),         new string[]{".fbx",".fbc"}, editorPrefKeyPre),
            new SearchType("t:Model",           new GUIContent("Model"),        new string[]{".fbx",".fbc"}, editorPrefKeyPre),
            new SearchType("t:PhysicMaterial",  new GUIContent("PhysicMaterial"),new string[]{}, editorPrefKeyPre),
            new SearchType("t:Prefab",          new GUIContent("Prefab"),       new string[]{".prefab"}, editorPrefKeyPre),
            new SearchType("t:Scene",           new GUIContent("Scene"),        new string[]{".unity"}, editorPrefKeyPre),
            new SearchType("t:Script",          new GUIContent("Script"),       new string[]{".cs"}, editorPrefKeyPre),
            new SearchType("t:Shader",          new GUIContent("Shader"),       new string[]{".shader"}, editorPrefKeyPre),
            new SearchType("t:Sprite",          new GUIContent("Sprite"),       new string[]{}, editorPrefKeyPre),
            new SearchType("t:Texture",         new GUIContent("Texture"),      new string[]{".png"}, editorPrefKeyPre),
            new SearchType("t:Folder",         new GUIContent("Folder"),      new string[]{""}, editorPrefKeyPre),
        };
    }


    static Vector2 scroll_value = Vector2.zero;

    protected override void OnGUI()
    {
        scroll_value = EditorGUILayout.BeginScrollView(scroll_value);
        DrawAboutResource();
        DrawAboutPrefab();
        EditorGUILayout.EndScrollView();
    }


    public static StringHandle partialPath_forMaterialCheckUnEmployedTex;

    public void CheckMaterialUnEmployedTex()
    {
        animBool_material_check_unemployed_tex.target = Foldout(animBool_material_check_unemployed_tex.target, "批量去除[材质]没有使用到的[图片引用]");
        if (EditorGUILayout.BeginFadeGroup(animBool_material_check_unemployed_tex.faded))
        {
            if (partialPath_forMaterialCheckUnEmployedTex == null) partialPath_forMaterialCheckUnEmployedTex = new StringHandle(this.GetType().Name + "partialPath_forMaterialCheckUnReferedProperty", "Assets");
            partialPath_forMaterialCheckUnEmployedTex.value = EditorGUILayout.TextField("局部路径", partialPath_forMaterialCheckUnEmployedTex.value);
            if (GUILayout.Button("Run"))
            {
                EditorUtility.DisplayCancelableProgressBar("Replacing...", "", 1.0f);

                string[] assetPaths;
                if (string.IsNullOrEmpty(partialPath_forMaterialCheckUnEmployedTex.value))
                    assetPaths = FindAssets("t:Material").ToArray();
                else
                    assetPaths = FindAssets("t:Material", new string[] { partialPath_forMaterialCheckUnEmployedTex.value }).ToArray();

                Dictionary<Shader, List<string>> shader2texProperties = new Dictionary<Shader, List<string>>();
                for (int i = 0; i < assetPaths.Length; i++)
                {
                    string path = assetPaths[i];

                    var mat = AssetDatabase.LoadAssetAtPath<Material>(path) as Material;
                    if (mat == null)
                        continue;

                    var shader = mat.shader;
                    if (shader == null)
                        continue;

                    List<string> texProperties;
                    if (!shader2texProperties.TryGetValue(shader, out texProperties))
                    {
                        texProperties = new List<string>();

                        for (int j = 0; j < ShaderUtil.GetPropertyCount(shader); j++)
                        {
                            if (ShaderUtil.GetPropertyType(shader, j) == ShaderUtil.ShaderPropertyType.TexEnv)
                            {
                                texProperties.Add(ShaderUtil.GetPropertyName(shader, j));
                            }
                        }

                        shader2texProperties[shader] = texProperties;
                    }

                    int modifyCount = 0;
                    foreach (var propertyName in mat.GetTexturePropertyNames())
                    {
                        if (!texProperties.Contains(propertyName) && mat.GetTexture(propertyName) != null)
                        {
                            mat.SetTexture(propertyName, null);
                            modifyCount++;
                        }
                    }
                    if (modifyCount > 0)
                        EditorUtility.SetDirty(mat);
                }
                EditorUtility.ClearProgressBar();
                AssetDatabase.SaveAssets();
            }
        }
        EditorGUILayout.EndFadeGroup();

    }

    private static ReferenceFinderData data = new ReferenceFinderData();
    private static bool initializedData = false;
    //初始化数据
    static void InitReferenceDataIfNeeded()
    {
        if (!initializedData)
        {
            //初始化数据
            if (!data.ReadFromCache())
            {
                data.CollectDependenciesInfo();
            }
            initializedData = true;
        }
    }

    /// <summary>
    /// 绘制资源相关信息
    /// </summary>
    protected void DrawAboutResource()
    {
        animBool_resource.target = Foldout(animBool_resource.target, "资源相关");
        if (EditorGUILayout.BeginFadeGroup(animBool_resource.faded))
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                DrawResourceReimport();
                GUILayout.Space(3);
                //DrawExportResoureceMemeryCSVReport();
                //GUILayout.Space(3);
                //DrawExportResoureceEmptyReferenceCSVReport();
                //GUILayout.Space(3);
                //DrawExportResoureceMessReferenceCSVReport();
                //GUILayout.Space(3);
                DrawLookingForTheSame();
                GUILayout.Space(3);
                FindAssetNameWithChineseSign();
                GUILayout.Space(3);
                DrawPrintSelectedAssetMemery();
                GUILayout.Space(3);
                DrawExportSameNameAssets();
                GUILayout.Space(3);
                SourceDeduplication();
                GUILayout.Space(3);
                SourceNameReplace();
                GUILayout.Space(3);
                AssetRenameTool();
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndFadeGroup();
    }

    public static StringHandle partialPath_ResourceReimport;
    public static StringHandle partialPath_ResourceMemeryReport;
    public static StringHandle path_ResourceMemeryReportOutput;
    public static StringHandle path_ResourceEmptyReferenceReportOutput;
    public static StringHandle path_ResourceMessReferenceReportOutput;
    public static StringHandle filter_ResourceReimport;

    public class SearchType
    {
        public string value;
        public string filter
        {
            get { return value; }
        }
        public GUIContent content;
        public string editorPrefKeyPre;
        public string[] lowCaseFileExtension;
        public BoolHandle isOn;
        public SearchType(string value, GUIContent content, string[] lowCaseFileExtension, string editorPrefKeyPre)
        {
            this.value = value;
            this.content = content;
            this.lowCaseFileExtension = lowCaseFileExtension;
            this.editorPrefKeyPre = editorPrefKeyPre;
            this.isOn = new BoolHandle(editorPrefKeyPre + value + "isOn", false);
        }
    }

    /// <summary>
    /// 重新导入指定路径的资源
    /// </summary>
    protected void DrawResourceReimport()
    {
        animBool_resource_import.target = Foldout(animBool_resource_import.target, "资源重新导入[Reimport]");
        if (EditorGUILayout.BeginFadeGroup(animBool_resource_import.faded))
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("过滤选项:");

                foreach (var searchType in SearchTypes)
                {
                    searchType.isOn.value = EditorGUILayout.Toggle(searchType.content, searchType.isOn.value);
                }
                EditorGUILayout.EndVertical();
            }
            string filter = string.Empty;
            foreach (var searchType in SearchTypes)
                if (searchType.isOn.value)
                {
                    filter = string.IsNullOrEmpty(filter) ? searchType.value : string.Format("{0} {1}", filter, searchType.value);
                }

            EditorGUILayout.TextField("过滤标记", filter);
            ////filter
            //if (filter_ResourceReimport == null) filter_ResourceReimport = new StringHandle(this.GetType().Name + "filter_ResourceReimport", string.Empty);
            //filter_ResourceReimport.value = EditorGUILayout.TextField("过滤标记", filter_ResourceReimport.value);
            //partial path
            if (partialPath_ResourceReimport == null) partialPath_ResourceReimport = new StringHandle(this.GetType().Name + "partialPath_ResourceReimport", string.Empty);
            partialPath_ResourceReimport.value = EditorGUILayout.TextField("局部路径", partialPath_ResourceReimport.value);

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Reimport"))
                {
                    if (!EditorUtility.DisplayDialog("Tips", "可能会消耗2至3小时,是否继续?", "continue", "cancal"))
                        return;

                    string[] assetPaths;
                    if (string.IsNullOrEmpty(partialPath_ResourceReimport.value))
                        assetPaths = FindAssets(filter).ToArray();
                    else
                        assetPaths = FindAssets(filter, new string[] { partialPath_ResourceReimport.value }).ToArray();

                    try
                    {
                        bool isCancel = false;
                        for (int i = 0; i < assetPaths.Length; i++)
                        {
                            string assetPath = assetPaths[i];
                            isCancel = isCancel || EditorUtility.DisplayCancelableProgressBar(string.Format("[{0}/{1}]Reimporting...", i.ToString(), assetPaths.Length), assetPath, (float)i / (float)assetPaths.Length);
                            if (isCancel)
                            {
                                EditorUtility.ClearProgressBar();
                                break;
                            }
                            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                        }
                        EditorUtility.ClearProgressBar();
                    }
                    catch (Exception e)
                    {
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndFadeGroup();
    }


    public static ObjectHandleList ObjectHandleList;
    public static StringHandleList pathExceptList;
    public static List<string> ResoureceMemeryCSV_Exts;
    public static List<string> assetPaths;
    public static Dictionary<string, List<string>> Path2Depends;
    /// 导出资源内存CSV报告
    /// </summary>
    public void DrawExportResoureceMemeryCSVReport()
    {
        if (ObjectHandleList == null)
        {
            ObjectHandleList = new ObjectHandleList("[检查特定预置体]", this.GetType().Name + "MemeryCSVObjectHandleList");
            ObjectHandleList.Init();
        }
        if (pathExceptList == null)
        {
            pathExceptList = new StringHandleList("[忽略路径]", this.GetType().Name + "MemeryCSVObjectPathExceptList");
            pathExceptList.Init();
        }

        animBool_resource_memery_report.target = Foldout(animBool_resource_memery_report.target, "资源内存占用[CSV]报告");
        if (EditorGUILayout.BeginFadeGroup(animBool_resource_memery_report.faded))
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                //设置指定预制
                animBool_resource_memery_report_specific_prefab.target = Foldout(animBool_resource_memery_report_specific_prefab.target, "设置指定预制");
                if (EditorGUILayout.BeginFadeGroup(animBool_resource_memery_report_specific_prefab.faded))
                {
                    ObjectHandleList.OnGUI();
                }
                EditorGUILayout.EndFadeGroup();
                //过滤路径
                animBool_resource_memery_report_except_paths.target = Foldout(animBool_resource_memery_report_except_paths.target, "过滤路径");
                if (EditorGUILayout.BeginFadeGroup(animBool_resource_memery_report_except_paths.faded))
                {
                    pathExceptList.OnGUI();
                }
                EditorGUILayout.EndFadeGroup();
                //过滤选项
                animBool_resource_memery_report_filter.target = Foldout(animBool_resource_memery_report_filter.target, "过滤选项");
                if (EditorGUILayout.BeginFadeGroup(animBool_resource_memery_report_filter.faded))
                {
                    foreach (var searchType in SearchTypes_MemeryReport)
                    {
                        searchType.isOn.value = EditorGUILayout.Toggle(searchType.content, searchType.isOn.value);
                    }
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUILayout.EndVertical();
            }
            //
            string filter = string.Empty;
            foreach (var searchType in SearchTypes_MemeryReport)
                if (searchType.isOn.value)
                {
                    filter = string.IsNullOrEmpty(filter) ? searchType.value : string.Format("{0} {1}", filter, searchType.value);
                }

            EditorGUILayout.TextField("过滤标记", filter);

            if (partialPath_ResourceMemeryReport == null) partialPath_ResourceMemeryReport = new StringHandle(this.GetType().Name + "partialPath_ResourceMemeryReport", "Assets/");
            partialPath_ResourceMemeryReport.value = EditorGUILayout.TextField("分析路径", partialPath_ResourceMemeryReport.value);

            EditorGUILayout.BeginHorizontal();
            {
                if (path_ResourceMemeryReportOutput == null) path_ResourceMemeryReportOutput = new StringHandle(this.GetType().Name + "path_ResourceMemeryReportOutput", string.Empty);

                path_ResourceMemeryReportOutput.value = EditorGUILayout.TextField("报告导出路径", path_ResourceMemeryReportOutput.value);
                if (GUILayout.Button("选择路径"))
                {
                    var res = EditorUtility.OpenFolderPanel("选择报告导出路径", path_ResourceMemeryReportOutput.value, path_ResourceMemeryReportOutput.value);
                    if (!string.IsNullOrEmpty(res))
                        path_ResourceMemeryReportOutput.value = res;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Export Report"))
                {
                    if (string.IsNullOrEmpty(path_ResourceMemeryReportOutput.value))
                        return;
                    if (!Directory.Exists(path_ResourceMemeryReportOutput.value))
                        Directory.CreateDirectory(path_ResourceMemeryReportOutput.value);
                    assetPaths = new List<string>();
                    Path2Depends = new Dictionary<string, List<string>>();

                    if (ObjectHandleList.Enabled.value || ObjectHandleList.Infos.Any(info => info.Enabled.value))
                    {
                        ResoureceMemeryCSV_Exts = new List<string>();
                        foreach (var searchType in SearchTypes_MemeryReport)
                        {
                            if (searchType.isOn.value)
                                ResoureceMemeryCSV_Exts.AddRange(searchType.lowCaseFileExtension);
                        }
                        if (ResoureceMemeryCSV_Exts.Count <= 0)
                        {
                            EditorUtility.DisplayDialog("Tips", "请选择要分析的内容!", "ok");
                            return;
                        }

                        List<string> assetPaths2 = new List<string>();
                        List<string> csvPaths = new List<string>();

                        for (int i = 0; i < ObjectHandleList.Infos.Count; i++)
                        {
                            var objectHandle = ObjectHandleList.Infos[i];
                            if (objectHandle == null || !objectHandle.Enabled.value || string.IsNullOrEmpty(objectHandle.assetPath))
                                continue;
                            var depends = AssetDatabase.GetDependencies(objectHandle.assetPath);
                            foreach (var path in depends)
                            {
                                if (string.IsNullOrEmpty(path) || assetPaths.Contains(path))
                                    continue;
                                //过滤后缀
                                var ext = Path.GetExtension(path).ToLower();
                                if (!ResoureceMemeryCSV_Exts.Contains(ext))
                                    continue;
                                //过滤路径
                                if (pathExceptList.Enabled.value && pathExceptList.Infos.Any(item => !string.IsNullOrEmpty(item.value) && path.IndexOf(item.value.Replace('\\', '/')) >= 0))
                                    continue;

                                List<string> subdepends = null;
                                if (!Path2Depends.TryGetValue(objectHandle.assetPath, out subdepends))
                                {
                                    subdepends = new List<string>();
                                    Path2Depends[objectHandle.assetPath] = subdepends;
                                }
                                subdepends.Add(path);
                                //
                                assetPaths.Add(path);
                                assetPaths2.Add(path);
                            }
                            try
                            {
                                string fileName = Path.GetFileNameWithoutExtension(objectHandle.assetPath);
                                csvPaths.Add(AssetMemoryAnalysis.process(assetPaths.ToArray(), path_ResourceMemeryReportOutput.value, fileName));
                            }
                            catch (Exception e)
                            {
                            }
                            assetPaths.Clear();
                        }

                        string csvStr = string.Empty;
                        for (int i = 0; i < csvPaths.Count; i++)
                        {
                            string cpath = csvPaths[i];
                            using (TextReader textReader = File.OpenText(cpath))
                            {
                                int count = 2000;
                                var line = textReader.ReadLine();
                                if (i > 0)
                                    line = textReader.ReadLine();

                                while (!string.IsNullOrEmpty(line) && count > 0)
                                {
                                    csvStr += line;
                                    csvStr += "\n";
                                    line = textReader.ReadLine();
                                    count--;
                                }
                            }
                        }
                        string s = string.Format("{0:yyyyMMdd_HHmmss}", System.DateTime.Now);
                        File.WriteAllText(Path.Combine(path_ResourceMemeryReportOutput.value, Path.Combine(reporttPath, "combine_" + s + ".csv")), csvStr, System.Text.Encoding.UTF8);

                        //var explorer = path_ResourceMemeryReportOutput.value.Replace('/', '\\');
                        //System.Diagnostics.Process.Start("explorer.exe", explorer);

                        EditorCoroutineUtility.StartCoroutine(ResoureceMemeryCSVReportCoroutinue(), this);
                    }
                    else
                    {
                        if (!EditorUtility.DisplayDialog("Tips", "可能会消耗5至25分钟,是否继续?", "continue", "cancal"))
                            return;

                        if (string.IsNullOrEmpty(path_ResourceMemeryReportOutput.value))
                        {
                            EditorUtility.DisplayDialog("Tips", "请设置报错输出路径!", "ok");
                            EditorGUIUtility.ExitGUI();
                            return;
                        }
                        if (!Directory.Exists(path_ResourceMemeryReportOutput.value))
                        {
                            if (!EditorUtility.DisplayDialog("Tips", "输出路径不存在,是否创建?", "yes", "cancal"))
                                return;
                            Directory.CreateDirectory(path_ResourceMemeryReportOutput.value);
                        }

                        if (string.IsNullOrEmpty(partialPath_ResourceMemeryReport.value))
                            assetPaths = FindAssets(filter).ToList();
                        else
                            assetPaths = FindAssets(filter, new string[] { partialPath_ResourceMemeryReport.value }).ToList();

                        try
                        {
                            AssetMemoryAnalysis.process(assetPaths.ToArray(), path_ResourceMemeryReportOutput.value);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndFadeGroup();
    }

    public static IEnumerator ResoureceMemeryCSVReportCoroutinue()
    {
        float timeScale = 0.5f;
        //综合csv
        yield return new EditorWaitForSeconds(1 * timeScale);

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

        CSVReport.Init(path_ResourceMemeryReportOutput.value, "prfabInfo");
        CSVReport.csvStr = @"预制名字,DrawCall,面数,顶点数,图片数,网格数,总内存(MB)";

        for (int i = 0; i < ObjectHandleList.Infos.Count; i++)
        {
            var objectHandle = ObjectHandleList.Infos[i];
            if (objectHandle == null || !objectHandle.Enabled.value || string.IsNullOrEmpty(objectHandle.assetPath))
                continue;

            var gobj = GameObject.Instantiate(objectHandle.value);
            yield return new EditorWaitForSeconds(1.2f * timeScale);

            //var camera = GameObject.FindObjectOfType<Camera>(true);
            var camera = GameObject.FindObjectOfType<Camera>();
            if (camera)
                camera.clearFlags = CameraClearFlags.SolidColor;
            var ParticleSystemRenderers = GameObject.FindObjectsOfType<ParticleSystemRenderer>();
            var meshFilters = GameObject.FindObjectsOfType<MeshFilter>();
            var skinnedMeshRenderers = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();

            SceneView.RepaintAll();

            yield return new EditorWaitForSeconds(2 * timeScale);

            var line = new CSVLine(objectHandle.assetPath);
            line.SetValue(0, Path.GetFileNameWithoutExtension(objectHandle.assetPath));
            line.SetValue(1, UnityEditor.UnityStats.batches.ToString());
            line.SetValue(2, UnityEditor.UnityStats.triangles.ToString());
            line.SetValue(3, UnityEditor.UnityStats.vertices.ToString());

            int texCount = 0;
            int modelCount = 0;
            float memoryCount = 0;
            List<string> depends = null;
            if (Path2Depends.TryGetValue(objectHandle.assetPath, out depends))
            {
                Debug.Log(depends.Count);
                foreach (var dependPath in depends)
                {
                    var res = AssetMemoryAnalysis.ReadFromCache(dependPath);
                    if (res != null)
                    {
                        AssetMemoryAnalysis.AssetMemoryResult res1 = (AssetMemoryAnalysis.AssetMemoryResult)res;
                        if (res1.type == "Texture")
                        {
                            texCount++;
                            memoryCount += res1.memory;
                        }
                    }
                }
            }
            //mesh
            List<Mesh> meshFilterList = new List<Mesh>();
            foreach (var meshFilter in meshFilters)
            {
                if (!meshFilter.sharedMesh || meshFilterList.Contains(meshFilter.sharedMesh))
                    continue;
                memoryCount += Profiler.GetRuntimeMemorySizeLong(meshFilter.sharedMesh) / 1024f / 1024f;
                meshFilterList.Add(meshFilter.sharedMesh);
                modelCount++;
                Debug.Log(meshFilter.gameObject.name);
            }
            //mesh
            foreach (var ParticleSystemRenderer in ParticleSystemRenderers)
            {
                if (!ParticleSystemRenderer.mesh || meshFilterList.Contains(ParticleSystemRenderer.mesh))
                    continue;
                memoryCount += Profiler.GetRuntimeMemorySizeLong(ParticleSystemRenderer.mesh) / 1024f / 1024f;
                meshFilterList.Add(ParticleSystemRenderer.mesh);
                modelCount++;
                Debug.Log(ParticleSystemRenderer.gameObject.name);
            }
            //skinnedMeshRenderer
            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                if (!skinnedMeshRenderer.sharedMesh || meshFilterList.Contains(skinnedMeshRenderer.sharedMesh))
                    continue;
                memoryCount += Profiler.GetRuntimeMemorySizeLong(skinnedMeshRenderer.sharedMesh) / 1024f / 1024f;
                meshFilterList.Add(skinnedMeshRenderer.sharedMesh);
                modelCount++;
                Debug.Log(skinnedMeshRenderer.gameObject.name);
            }

            line.SetValue(4, texCount.ToString());
            line.SetValue(5, modelCount.ToString());
            line.SetValue(6, memoryCount.ToString());
            Debug.Log(line.ApplyLine());

            GameObject.DestroyImmediate(gobj);
        }
        CSVReport.Output(true);

        yield return 0;
    }

    /// <summary>
    /// 
    /// 导出资源空引用CSV报告
    /// </summary>
    protected void DrawExportResoureceEmptyReferenceCSVReport()
    {
        animBool_resource_empty_ref_report.target = Foldout(animBool_resource_empty_ref_report.target, "资源[空]引用[CSV]报告");
        if (EditorGUILayout.BeginFadeGroup(animBool_resource_empty_ref_report.faded))
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (path_ResourceEmptyReferenceReportOutput == null) path_ResourceEmptyReferenceReportOutput = new StringHandle(this.GetType().Name + "path_ResourceEmptyReferenceReportOutput", string.Empty);

                path_ResourceEmptyReferenceReportOutput.value = EditorGUILayout.TextField("报告导出路径", path_ResourceEmptyReferenceReportOutput.value);
                if (GUILayout.Button("选择路径"))
                {
                    var res = EditorUtility.OpenFolderPanel("选择报告导出路径", path_ResourceEmptyReferenceReportOutput.value, path_ResourceEmptyReferenceReportOutput.value);
                    if (!string.IsNullOrEmpty(res))
                    {
                        path_ResourceEmptyReferenceReportOutput.value = res;
                        Repaint();
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            {

                if (GUILayout.Button("Export Report"))
                {
                    if (!EditorUtility.DisplayDialog("Tips", "可能会消耗5至25分钟,是否继续?", "continue", "cancal"))
                        return;

                    InitReferenceDataIfNeeded();

                    if (string.IsNullOrEmpty(path_ResourceEmptyReferenceReportOutput.value))
                    {
                        EditorUtility.DisplayDialog("Tips", "请设置报错输出路径!", "ok");
                        EditorGUIUtility.ExitGUI();
                        return;
                    }
                    if (!Directory.Exists(path_ResourceEmptyReferenceReportOutput.value))
                    {
                        if (!EditorUtility.DisplayDialog("Tips", "输出路径不存在,是否创建?", "yes", "cancal"))
                            return;
                        Directory.CreateDirectory(path_ResourceEmptyReferenceReportOutput.value);
                    }
                    try
                    {
                        //data.GetNoRefs(path_ResourceEmptyReferenceReportOutput.value);
                    }
                    catch (Exception e)
                    {
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndFadeGroup();
    }
    /// <summary>
    /// 导出资源乱引用CSV报告
    /// </summary>
    protected void DrawExportResoureceMessReferenceCSVReport()
    {
        animBool_resource_mess_ref_report.target = Foldout(animBool_resource_mess_ref_report.target, "资源[乱]引用[CSV]报告");
        if (EditorGUILayout.BeginFadeGroup(animBool_resource_mess_ref_report.faded))
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (path_ResourceMessReferenceReportOutput == null) path_ResourceMessReferenceReportOutput = new StringHandle(this.GetType().Name + "path_ResourceMessReferenceReportOutput", string.Empty);

                path_ResourceMessReferenceReportOutput.value = EditorGUILayout.TextField("报告导出路径", path_ResourceMessReferenceReportOutput.value);
                if (GUILayout.Button("选择路径"))
                {
                    var res = EditorUtility.OpenFolderPanel("选择报告导出路径", path_ResourceMessReferenceReportOutput.value, path_ResourceMessReferenceReportOutput.value);
                    if (!string.IsNullOrEmpty(res))
                    {
                        path_ResourceMessReferenceReportOutput.value = res;
                        Repaint();
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Export Report"))
                {
                    if (!EditorUtility.DisplayDialog("Tips", "可能会消耗5至25分钟,是否继续?", "continue", "cancal"))
                        return;

                    InitReferenceDataIfNeeded();

                    if (string.IsNullOrEmpty(path_ResourceMessReferenceReportOutput.value))
                    {
                        EditorUtility.DisplayDialog("Tips", "请设置报错输出路径!", "ok");
                        EditorGUIUtility.ExitGUI();
                        return;
                    }
                    if (!Directory.Exists(path_ResourceMessReferenceReportOutput.value))
                    {
                        if (!EditorUtility.DisplayDialog("Tips", "输出路径不存在,是否创建?", "yes", "cancal"))
                            return;
                        Directory.CreateDirectory(path_ResourceMessReferenceReportOutput.value);
                    }
                    try
                    {
                        //data.GetMassRefs(path_ResourceMessReferenceReportOutput.value);
                    }
                    catch (Exception e)
                    {
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndFadeGroup();
    }


    public static StringHandle partialPath_LookingForTheSame;
    public static StringHandle path_LookingForTheSameReportOutput;
    public static StringHandleList pathExceptList_LookingForTheSame;
    public static StringHandle reportPreName;
    public class AssetDuplicateInfo
    {
        public string type;
        public string sha1;
        public List<string> duplicates = new List<string>();
    }


    protected void DrawLookingForTheSame()
    {
        if (pathExceptList_LookingForTheSame == null)
        {
            pathExceptList_LookingForTheSame = new StringHandleList("[忽略路径]", this.GetType().Name + "pathExceptList_LookingForTheSame");
            pathExceptList_LookingForTheSame.Init();
        }
        if (reportPreName == null) reportPreName = new StringHandle(this.GetType().Name + "reportPreName", "");

        animBool_resource_lookingfor_same.target = Foldout(animBool_resource_lookingfor_same.target, "寻找相同的文件[CSV]报告");
        if (EditorGUILayout.BeginFadeGroup(animBool_resource_lookingfor_same.faded))
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                //过滤路径
                animBool_LookingForTheSame_except_paths.target = Foldout(animBool_LookingForTheSame_except_paths.target, "过滤路径");
                if (EditorGUILayout.BeginFadeGroup(animBool_LookingForTheSame_except_paths.faded))
                {
                    pathExceptList_LookingForTheSame.OnGUI();
                }
                EditorGUILayout.EndFadeGroup();
                //过滤选项
                animBool_LookingForTheSame_filter.target = Foldout(animBool_LookingForTheSame_filter.target, "过滤选项");
                if (EditorGUILayout.BeginFadeGroup(animBool_LookingForTheSame_filter.faded))
                {
                    foreach (var searchType in SearchTypes_LookingForTheSame)
                    {
                        searchType.isOn.value = EditorGUILayout.Toggle(searchType.content, searchType.isOn.value);
                    }
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUILayout.EndVertical();
            }
            //
            string filter = string.Empty;
            foreach (var searchType in SearchTypes_LookingForTheSame)
                if (searchType.isOn.value)
                {
                    filter = string.IsNullOrEmpty(filter) ? searchType.value : string.Format("{0} {1}", filter, searchType.value);
                }
            EditorGUILayout.TextField("过滤标记", filter);

            if (partialPath_LookingForTheSame == null) partialPath_LookingForTheSame = new StringHandle(this.GetType().Name + "partialPath_LookingForTheSame", "Assets");
            partialPath_LookingForTheSame.value = EditorGUILayout.TextField("分析路径", partialPath_LookingForTheSame.value);

            reportPreName.value = EditorGUILayout.TextField("报告文件名前缀", reportPreName.value);

            EditorGUILayout.BeginHorizontal();
            {
                if (path_LookingForTheSameReportOutput == null) path_LookingForTheSameReportOutput = new StringHandle(this.GetType().Name + "path_LookingForTheSameReportOutput", string.Empty);

                path_LookingForTheSameReportOutput.value = EditorGUILayout.TextField("报告导出路径", path_LookingForTheSameReportOutput.value);
                if (GUILayout.Button("选择路径"))
                {
                    var res = EditorUtility.OpenFolderPanel("选择报告导出路径", path_LookingForTheSameReportOutput.value, path_LookingForTheSameReportOutput.value);
                    if (!string.IsNullOrEmpty(res))
                        path_LookingForTheSameReportOutput.value = res;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Run"))
            {
                if (!Directory.Exists(path_LookingForTheSameReportOutput.value))
                    Directory.CreateDirectory(path_LookingForTheSameReportOutput.value);

                Dictionary<string, AssetDuplicateInfo> md5ToGUIDs = new Dictionary<string, AssetDuplicateInfo>();
                List<string> assetPaths;

                if (string.IsNullOrEmpty(partialPath_LookingForTheSame.value))
                    assetPaths = FindAssets(filter).ToList();
                else
                    assetPaths = FindAssets(filter, new string[] { partialPath_LookingForTheSame.value }).ToList();

                if (assetPaths == null)
                    return;

                bool cancel = false;
                float count = 0;
                float vaild = 0;
                float total = assetPaths.Count;
                foreach (var assetPath in assetPaths)
                {
                    count++;
                    cancel = cancel || EditorUtility.DisplayCancelableProgressBar(string.Format("[{0}/{1}]遍历特定assets", count, total), "", count / total);
                    if (cancel)
                        break;
                    if (string.IsNullOrEmpty(assetPath))
                        continue;

                    var md5 = FileMd5(assetPath);
                    if (string.IsNullOrEmpty(md5))
                        continue;

                    AssetDuplicateInfo duplicateInfo;
                    if (!md5ToGUIDs.TryGetValue(md5, out duplicateInfo))
                    {
                        vaild++;
                        duplicateInfo = new AssetDuplicateInfo();
                        duplicateInfo.type = FilePath2FileType(assetPath);
                        //
                        md5ToGUIDs[md5] = duplicateInfo;
                    }
                    if (!duplicateInfo.duplicates.Contains(assetPath))
                    {
                        duplicateInfo.duplicates.Add(assetPath);
                    }
                }
                EditorUtility.ClearProgressBar();
                //输出报告
                count = 0;
                total = vaild;
                cancel = false;
                //storger
                var sha1ToMemey = new Dictionary<string, float>();
                var sha1ToSubMeshCount = new Dictionary<string, float>();
                //require
                MethodInfo GetStorageMemorySizeLong;
                Texture target = Selection.activeObject as Texture;
                //var type = Types.GetType("UnityEditor.TextureUtil", "UnityEditor.dll");
                var type = System.Reflection.Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
                GetStorageMemorySizeLong = type.GetMethod("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

                CSVReport.Init(path_LookingForTheSameReportOutput.value, reportPreName.value + "重复文件");
                CSVReport.csvStr = @"路径,类型,大小(MB),总重复文件大小(MB),是否大于(1MB),SHA-1";

                foreach (var pairs in md5ToGUIDs)
                {
                    count++;
                    cancel = cancel || EditorUtility.DisplayCancelableProgressBar(string.Format("[{0}/{1}]生成报告", count, total), "", count / total);
                    if (cancel)
                        break;
                    if (pairs.Value.duplicates.Count <= 1)
                        continue;
                    if (string.IsNullOrEmpty(pairs.Value.type))
                        continue;

                    var lines = new List<CSVLine>();
                    float totalMemery = 0;
                    foreach (var assetPath in pairs.Value.duplicates)
                    {
                        var line = new CSVLine(assetPath);
                        float memery = 0;
                        float SubMeshCount = 0;
                        if (!sha1ToMemey.TryGetValue(pairs.Key, out memery))
                        {
                            memery = 0;
                            SubMeshCount = 0;
                            if (pairs.Value.type == TYPE_TEXTURE)
                            {
                                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Texture)) as UnityEngine.Texture;
                                memery = float.Parse(GetStorageMemorySizeLong.Invoke(null, new object[] { asset }).ToString()) / 1024f / 1024f;
                                if (!sha1ToMemey.ContainsKey(pairs.Key)) sha1ToMemey[pairs.Key] = memery;
                            }
                            else if (pairs.Value.type == TYPE_MODEL)
                            {
                                //var model = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object)) as UnityEngine.Object;
                                var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                                List<Mesh> meshFilterList = new List<Mesh>();
                                foreach (var asset in assets)
                                {
                                    if (asset is Mesh)
                                    {
                                        var mesh = asset as Mesh;
                                        if (!mesh || meshFilterList.Contains(mesh))
                                            continue;
                                        memery += Profiler.GetRuntimeMemorySizeLong(asset) / 1024f / 1024f;
                                        meshFilterList.Add(mesh);
                                        SubMeshCount++;
                                    }
                                }

                                if (!sha1ToMemey.ContainsKey(pairs.Key)) sha1ToMemey[pairs.Key] = memery;
                                if (!sha1ToSubMeshCount.ContainsKey(pairs.Key)) sha1ToSubMeshCount[pairs.Key] = SubMeshCount;
                            }
                        }
                        totalMemery += memery;
                        line.SetValue(0, assetPath);
                        line.SetValue(2, memery.ToString());
                        lines.Add(line);
                    }
                    foreach (var line in lines)
                    {
                        line.SetValue(1, pairs.Value.type);
                        line.SetValue(3, totalMemery.ToString());
                        line.SetValue(4, totalMemery > 1.0f ? "TRUE" : "FALSE");
                        line.SetValue(5, pairs.Key);
                        line.ApplyLine();
                    }
                }
                EditorUtility.ClearProgressBar();
                CSVReport.Output(true);
            }
        }
        EditorGUILayout.EndFadeGroup();
    }
    /// <summary>
    /// 获取指定目录下的重复文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public List<AssetDuplicateInfo> GetDuplicationFileAtPath(string path, string filter = "")
    {
        List<string> assetPaths;

        if (string.IsNullOrEmpty(path))
            assetPaths = FindAssets(filter).ToList();
        else
            assetPaths = FindAssets(filter, new string[] { path }).ToList();

        return GetDuplicationFileAtPath(assetPaths);
    }
    public List<AssetDuplicateInfo> GetDuplicationFileAtPath(List<string> assetPaths, string subTitle = "")
    {
        List<AssetDuplicateInfo> res = new List<AssetDuplicateInfo>();
        Dictionary<string, AssetDuplicateInfo> map = new Dictionary<string, AssetDuplicateInfo>();

        bool cancel = false;
        float count = 0;
        float total = assetPaths.Count;
        foreach (var assetPath in assetPaths)
        {
            count++;
            if (string.IsNullOrEmpty(subTitle))
                cancel = cancel || EditorUtility.DisplayCancelableProgressBar(string.Format("[{0}/{1}]Finding Duplications...", count, total), "", count / total);
            else
                cancel = cancel || EditorUtility.DisplayCancelableProgressBar(string.Format("[{0}/{1}][{2}]Finding Duplications...", count, total, subTitle), "", count / total);

            if (cancel)
                break;
            if (string.IsNullOrEmpty(assetPath))
                continue;

            var sha1 = FileSHA1(assetPath);
            if (string.IsNullOrEmpty(sha1))
                continue;

            AssetDuplicateInfo duplicateInfo;
            if (!map.TryGetValue(sha1, out duplicateInfo))
            {
                duplicateInfo = new AssetDuplicateInfo();
                duplicateInfo.type = FilePath2FileType(assetPath);
                duplicateInfo.sha1 = sha1;
                //
                map[sha1] = duplicateInfo;
                res.Add(duplicateInfo);
            }
            if (!duplicateInfo.duplicates.Contains(assetPath))
            {
                duplicateInfo.duplicates.Add(assetPath);
            }
        }
        map.Clear();
        map = null;
        EditorUtility.ClearProgressBar();
        return res;
    }

    public static string FileSHA1(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            var md5 = SHA1.Create();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            return System.BitConverter.ToString(retVal).Replace("-", "");
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }
    public static string FileMd5(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            var md5 = SHA1.Create();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            return System.BitConverter.ToString(retVal).Replace("-", "");
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    public void DrawPrintSelectedAssetMemery()
    {
        animBool_resource_print_selected_memery.target = Foldout(animBool_resource_print_selected_memery.target, "输出选中文件内存");
        if (EditorGUILayout.BeginFadeGroup(animBool_resource_print_selected_memery.faded))
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                if (Selection.objects != null)
                {
                    //selected
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        EditorGUILayout.LabelField("选中文件:");
                        foreach (var obj in Selection.objects)
                        {
                            if (obj == null)
                                continue;
                            EditorGUILayout.ObjectField(obj, typeof(UnityEngine.Object), true);
                        }
                        EditorGUILayout.EndVertical();
                    }//selected
                }
                EditorGUILayout.EndVertical();
            }


            if (GUILayout.Button("RUN"))
            {
                foreach (var obj in Selection.objects)
                {
                    float memery = 0;
                    float SubMeshCount = 0;
                    string path = AssetDatabase.GetAssetPath(obj);
                    if (string.IsNullOrEmpty(path)) continue;
                    string type = Extension2FileType(Path.GetExtension(path).ToLower());
                    if (string.IsNullOrEmpty(type)) continue;

                    Dictionary<string, float> assetPath2SizeKB = new Dictionary<string, float>();

                    float sizeKB = GetObjectMemorySize(obj, assetPath2SizeKB);
                    foreach (var pairs in assetPath2SizeKB)
                    {
                        Debug.Log(string.Format("名字: {0}  大小: {1}KB", Path.GetFileName(pairs.Key), pairs.Value), AssetDatabase.LoadAssetAtPath(pairs.Key, typeof(UnityEngine.Object)));
                    }
                    Debug.Log(string.Format("Path:{0}  总内存:{1}MB", path, sizeKB / 1048f));

                    //if (type == TYPE_TEXTURE)
                    //{
                    //    memery = float.Parse(GetStorageMemorySizeLong.Invoke(null, new object[] { obj }).ToString()) / 1024f;
                    //    Debug.Log(string.Format("Path:{0}  内存:{1}", AssetDatabase.GetAssetPath(obj), memery));
                    //}
                    //else if (type == TYPE_MODEL)
                    //{
                    //    //var model = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object)) as UnityEngine.Object;
                    //    var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                    //    List<Mesh> meshFilterList = new List<Mesh>();
                    //    foreach (var asset in assets)
                    //    {
                    //        if (asset is Mesh)
                    //        {
                    //            var mesh = asset as Mesh;
                    //            if (!mesh || meshFilterList.Contains(mesh))
                    //                continue;
                    //            float temp = Profiler.GetRuntimeMemorySizeLong(asset) / 1024f / 1024f;
                    //            meshFilterList.Add(mesh);
                    //            SubMeshCount++;
                    //            Debug.Log(string.Format("Path:{0}  内存:{1}  子网格索引:{2}", path + "/" + mesh.name, temp, SubMeshCount));
                    //            memery += temp;
                    //        }
                    //    }
                    //}
                    //Debug.Log(string.Format("Path:{0}  总内存:{1}", path, memery));
                    //Debug.Log("--------------------------------------------");
                }
            }
        }
        EditorGUILayout.EndFadeGroup();


    }


    public static StringHandle path_FindAssetNameWithChineseSign;
    public static BoolHandle DoubleByteSymbol;
    public static BoolHandle ChineseSymbol;
    public void FindAssetNameWithChineseSign()
    {
        if (DoubleByteSymbol == null) DoubleByteSymbol = new BoolHandle(this.GetType().Name + "DoubleByteSymbol", false);
        if (ChineseSymbol == null) ChineseSymbol = new BoolHandle(this.GetType().Name + "ChineseSymbol", true);
        animBool_resource_find_assets_name_includeChineseSgin.target = Foldout(animBool_resource_find_assets_name_includeChineseSgin.target, "寻找名字带有中文(中文字符)的Asssets[CSV]");
        if (EditorGUILayout.BeginFadeGroup(animBool_resource_find_assets_name_includeChineseSgin.faded))
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                DoubleByteSymbol.value = EditorGUILayout.Toggle("双字节符号", DoubleByteSymbol.value);
                ChineseSymbol.value = EditorGUILayout.Toggle("中文标点字符", ChineseSymbol.value);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.BeginHorizontal();
            {
                if (path_FindAssetNameWithChineseSign == null) path_FindAssetNameWithChineseSign = new StringHandle(this.GetType().Name + "path_FindAssetNameWithChineseSign", string.Empty);

                path_FindAssetNameWithChineseSign.value = EditorGUILayout.TextField("报告导出路径", path_FindAssetNameWithChineseSign.value);
                if (GUILayout.Button("选择路径"))
                {
                    var res = EditorUtility.OpenFolderPanel("选择报告导出路径", path_FindAssetNameWithChineseSign.value, path_FindAssetNameWithChineseSign.value);
                    if (!string.IsNullOrEmpty(res))
                        path_FindAssetNameWithChineseSign.value = res;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("RUN"))
            {
                if (string.IsNullOrEmpty(path_FindAssetNameWithChineseSign.value))
                {
                    EditorUtility.DisplayDialog("Tips", "未选中报告导出路径", "ok");
                    return;
                }

                var dataPath = Application.dataPath;
                var root = dataPath.Substring(0, dataPath.Length - 6);
                CSVReport.Init(path_FindAssetNameWithChineseSign.value);
                CSVReport.csvStr = @"path,类型,错误类型";
                var chinese = new Regex("[\u4e00-\u9fa5]+");
                var doubleByteSymbol = new Regex(@"[^\x00-\xff]+");
                //包括空格
                var chineseSymbol = new Regex(@"[\u3002\uff1b\uff0c\uff1a\u201c\u201d\uff08\uff09\u3001\uff1f\u300a\u300b\u4e00-\u9fa5]+");
                var space = new Regex(@"[\s]+");
                Action<string, string, string> AddLine = (string dir, string type, string error) =>
                {
                    CSVLine line = new CSVLine();
                    line.SetValue(0, dir.Replace(root, string.Empty));
                    line.SetValue(1, type);
                    line.SetValue(2, error);
                    line.ApplyLine();
                };

                var directorys = Directory.GetDirectories(dataPath, "*", SearchOption.AllDirectories);
                int amount = directorys.Length;
                Debug.Log(amount);
                bool isCancel = false;
                float count = 0;
                foreach (var dir in directorys)
                {
                    count++;
                    isCancel = isCancel || EditorUtility.DisplayCancelableProgressBar("search directory...", "", (float)count / (float)amount);
                    if (isCancel)
                    {
                        EditorUtility.ClearProgressBar();
                        break;
                    }
                    if (string.IsNullOrEmpty(dir))
                        continue;
                    if (chinese.IsMatch(dir))
                    {
                        AddLine(dir, "文件夹", "中文"); continue;
                    }
                    else if (DoubleByteSymbol.value && doubleByteSymbol.IsMatch(dir))
                    {
                        AddLine(dir, "文件夹", "双字节字符"); continue;
                    }
                    else if (ChineseSymbol.value && chineseSymbol.IsMatch(dir))
                    {
                        AddLine(dir, "文件夹", "中文字符"); continue;
                    }
                    else if (space.IsMatch(dir))
                    {
                        AddLine(dir, "文件夹", "空格"); continue;
                    }
                }

                var assetPaths = AssetDatabase.GetAllAssetPaths();
                count = 0;
                amount = assetPaths.Length;
                foreach (var assetPath in assetPaths)
                {
                    count++;
                    isCancel = isCancel || EditorUtility.DisplayCancelableProgressBar("search files...", "", (float)count / (float)amount);
                    if (isCancel)
                    {
                        EditorUtility.ClearProgressBar();
                        break;
                    }

                    var assetName = Path.GetFileNameWithoutExtension(assetPath);
                    if (string.IsNullOrEmpty(assetName))
                        continue;
                    if (chinese.IsMatch(assetName))
                    {
                        AddLine(assetPath, "文件", "中文"); continue;
                    }
                    else if (DoubleByteSymbol.value && doubleByteSymbol.IsMatch(assetName))
                    {
                        AddLine(assetPath, "文件", "双字节字符"); continue;
                    }
                    else if (ChineseSymbol.value && chineseSymbol.IsMatch(assetName))
                    {
                        AddLine(assetPath, "文件", "中文字符"); continue;
                    }
                    else if (space.IsMatch(assetName))
                    {
                        AddLine(assetPath, "文件", "空格"); continue;
                    }
                }
                EditorUtility.ClearProgressBar();

                CSVReport.Output(true);
            }

            if (GUILayout.Button("替换不规则命名"))
            {
                if (string.IsNullOrEmpty(path_FindAssetNameWithChineseSign.value))
                {
                    EditorUtility.DisplayDialog("Tips", "未选中修改路径", "ok");
                    return;
                }

                string dataPath = path_FindAssetNameWithChineseSign.value;
                var chinese = new Regex("[\u4e00-\u9fa5]+");
                var doubleByteSymbol = new Regex(@"[^\x00-\xff]+");
                //包括空格
                var chineseSymbol = new Regex(@"[\u3002\uff1b\uff0c\uff1a\u201c\u201d\uff08\uff09\u3001\uff1f\u300a\u300b\u4e00-\u9fa5]+");
                var space = new Regex(@"[\s]+");
                var directorys = Directory.GetDirectories(dataPath, "*", SearchOption.AllDirectories);
                int amount = directorys.Length;
                bool isCancel = false;
                float count = 0;
                bool isChange = false;
                foreach (var dir in directorys)
                {
                    isCancel = isCancel || EditorUtility.DisplayCancelableProgressBar("search directory...", "", (float)++count / (float)amount);
                    if (isCancel)
                    {
                        EditorUtility.ClearProgressBar();
                        break;
                    }
                    if (string.IsNullOrEmpty(dir))
                        continue;

                    string rePath = string.Empty;
                    if (space.IsMatch(dir))
                    {
                        rePath = Regex.Replace(dir, @"[\s]+", m => m.Value.Replace(' ', '_'));
                    }
                    else if (DoubleByteSymbol.value && doubleByteSymbol.IsMatch(dir))
                    {
                        Debug.Log("双字节字符：" + dir);
                    }
                    else if (ChineseSymbol.value && chineseSymbol.IsMatch(dir))
                    {
                        Debug.Log("中文字符：" + dir);
                    }
                    else if (chinese.IsMatch(dir))
                    {
                        Debug.Log("中文：" + dir);
                    }

                    if (!string.IsNullOrEmpty(rePath))
                    {
                        isChange = true;
                        string[] array = dir.Replace('\\', '/').Split('/');
                        string[] array2 = rePath.Replace('\\', '/').Split('/');
                        string tpath = string.Empty;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            if (string.IsNullOrEmpty(tpath))
                                tpath = array2[i] + "\\";
                            else
                                tpath = Path.Combine(tpath, array2[i]);
                            if (!Directory.Exists(tpath))
                            {
                                string path = Path.Combine(Path.GetDirectoryName(tpath), array[i]);
                                DirectoryInfo directoryinfo = new DirectoryInfo(path);
                                directoryinfo.MoveTo(tpath);
                            }
                        }
                    }
                }

                count = 0;
                string[] files = Directory.GetFiles(dataPath);
                amount = files.Length;
                foreach (var assetPath in files)
                {
                    string path = GetPorjectPath(assetPath);
                    isCancel = isCancel || EditorUtility.DisplayCancelableProgressBar("search files...", "", (float)++count / (float)amount);
                    if (isCancel)
                    {
                        EditorUtility.ClearProgressBar();
                        break;
                    }

                    var assetName = Path.GetFileNameWithoutExtension(assetPath);
                    if (string.IsNullOrEmpty(assetName))
                        continue;

                    string rePath = string.Empty;
                    if (space.IsMatch(assetName))
                    {
                        rePath = path.Replace(' ', '_');
                    }
                    else if (DoubleByteSymbol.value && doubleByteSymbol.IsMatch(assetName))
                    {
                        //AddLine(assetPath, "文件", "双字节字符"); continue;
                    }
                    else if (ChineseSymbol.value && chineseSymbol.IsMatch(assetName))
                    {
                        //AddLine(assetPath, "文件", "中文字符"); continue;
                    }
                    else if (chinese.IsMatch(assetName))
                    {
                        //AddLine(assetPath, "文件", "中文"); continue;
                    }

                    if (!string.IsNullOrEmpty(rePath))
                    {
                        isChange = true;
                        AssetDatabase.RenameAsset(path, rePath);
                    }
                }

                if (isChange)
                {
                    //AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                EditorUtility.ClearProgressBar();
            }

        }
        EditorGUILayout.EndFadeGroup();
    }



    /// <summary>
    /// 绘制预制相关信息
    /// </summary>
    protected void DrawAboutPrefab()
    {
        animBool_prefab.target = Foldout(animBool_prefab.target, "预制体相关");
        if (EditorGUILayout.BeginFadeGroup(animBool_prefab.faded))
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                PrefabDeduplication();
                GUILayout.Space(3);
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndFadeGroup();
    }

    public static UnityEngine.Object src;
    public static UnityEngine.Object dst;
    public static StringHandle partialPath_prefabReferenceDeduplication;
    public static List<SearchType> SearchTypes_Deduplication;
    public static List<SearchType> SearchTypes_SoureceDeduplication;
    public static List<SearchType> SearchTypes_TargetAssets;

    public void PrefabDeduplication()
    {
        if (SearchTypes_Deduplication == null)
            SearchTypes_Deduplication = GetSearchTypes("SearchTypes_Deduplication");
        //if (SearchTypes_TargetAssets == null)
        //    SearchTypes_TargetAssets = GetSearchTypes("SearchTypes_TargetAssets");
        animBool_prefab_prefabReferenceDeduplication.target = Foldout(animBool_prefab_prefabReferenceDeduplication.target, "目录重复文件整理工具");
        if (EditorGUILayout.BeginFadeGroup(animBool_prefab_prefabReferenceDeduplication.faded))
        {
            //要去重Asset类型(图片,网管,...)
            animBool_prefab_deduplication_filter.target = Foldout(animBool_prefab_deduplication_filter.target, "要去重文件类型(图片,网格,...)");
            if (EditorGUILayout.BeginFadeGroup(animBool_prefab_deduplication_filter.faded))
            {
                foreach (var searchType in SearchTypes_Deduplication)
                {
                    searchType.isOn.value = EditorGUILayout.Toggle(searchType.content, searchType.isOn.value);
                }
            }
            //去重之后要重新设置引用的类型
            //animBool_prefab_dereference_filter.target = Foldout(animBool_prefab_dereference_filter.target, "要重新设置引用的Asset类型(Prefab,Material,...)");
            //if (EditorGUILayout.BeginFadeGroup(animBool_prefab_dereference_filter.faded))
            //{
            //    foreach (var searchType in SearchTypes_TargetAssets)
            //    {
            //        searchType.isOn.value = EditorGUILayout.Toggle(searchType.content, searchType.isOn.value);
            //    }
            //}
            EditorGUILayout.EndFadeGroup();
            if (partialPath_prefabReferenceDeduplication == null) partialPath_prefabReferenceDeduplication = new StringHandle(this.GetType().Name + "partialPath_prefabReferenceDeduplication", "Assets/Art/Env/Scenes");
            partialPath_prefabReferenceDeduplication.value = EditorGUILayout.TextField("处理路径", partialPath_prefabReferenceDeduplication.value);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("开始整理"))
            {
                if (string.IsNullOrEmpty(partialPath_prefabReferenceDeduplication.value))
                {
                    EditorUtility.DisplayDialog("Tips", "请输入处理路径!", "ok", "cancal");
                    return;
                }
                if (!EditorUtility.DisplayDialog("Tips", "请谨慎填写,否则会导致漫长的处理时间!", "继续", "重新填写"))
                    return;

                //
                //string dereferenceAssetFilter = string.Empty;
                //foreach (var searchType in SearchTypes_MemeryReport)
                //    if (searchType.isOn.value)
                //    {
                //        dereferenceAssetFilter = string.IsNullOrEmpty(dereferenceAssetFilter) ? searchType.value : string.Format("{0} {1}", dereferenceAssetFilter, searchType.value);
                //    }

                foreach (var searchType in SearchTypes_Deduplication)
                {
                    if (!searchType.isOn.value)
                        continue;
                    var sourceType = searchType.content.text;
                    var commonFolderName = "CommonSource";

                    Debug.Log(string.Format("<color=white>[开始整理] [{0}]</color> ", sourceType));

                    //collect assets
                    List<string> assetPaths;
                    var executePath = partialPath_prefabReferenceDeduplication.value;
                    assetPaths = FindAssets(searchType.filter, new string[] { executePath }).ToList();
                    //except commonFolderName
                    for (int i = 0; i < assetPaths.Count; i++)
                        if (assetPaths.Contains(commonFolderName))
                            assetPaths.RemoveAt(i--);
                    //collect Duplicate assets
                    var res = GetDuplicationFileAtPath(assetPaths, sourceType);
                    if (res.Count <= 0)
                        continue;
                    //collect dependences
                    Dictionary<string, string[]> assetPath2dependences = new Dictionary<string, string[]>();
                    List<string> otherAssetPaths;
                    otherAssetPaths = FindAssets("", new string[] { executePath }).ToList();
                    foreach (var assetPath in otherAssetPaths)
                    {
                        if (!assetPaths.Contains(assetPath))
                            if (!assetPath2dependences.ContainsKey(assetPath))
                                assetPath2dependences.Add(assetPath, AssetDatabase.GetDependencies(assetPath));
                    }
                    //except commonFolderName
                    for (int i = 0; i < assetPaths.Count; i++)
                        if (assetPaths.Contains(commonFolderName))
                            assetPaths.RemoveAt(i--);
                    //check duplicates
                    for (int i = 0; i < res.Count; i++)
                        if (res[i].duplicates.Count <= 1)
                            res.RemoveAt(i--);
                    //process
                    bool isCancel = false;
                    int count = 0;
                    foreach (var dinfo in res)
                    {
                        count++;
                        string targetMeta = dinfo.duplicates[0];

                        isCancel = isCancel || EditorUtility.DisplayCancelableProgressBar(string.Format("[{0}/{1}][{2}]Deduplication & Dereference...", count, res.Count, sourceType), targetMeta, (float)count / (float)res.Count);
                        if (isCancel)
                        {
                            EditorUtility.ClearProgressBar();
                            break;
                        }

                        var targetGuid = AssetDatabase.AssetPathToGUID(targetMeta);
                        if (string.IsNullOrEmpty(targetGuid)) continue;

                        //collect refers
                        List<string> refers = new List<string>();
                        foreach (var duplicatePath in dinfo.duplicates)
                        {
                            foreach (var pair in assetPath2dependences)
                            {
                                var assetPath = pair.Key;
                                var dependences = pair.Value;
                                if (dependences.Contains(duplicatePath) && !refers.Contains(assetPath))
                                    refers.Add(assetPath);
                            }
                            // Debug.Log(path);
                            //Debug.Log(GetAssetFullPath(path));
                        }
                        if (refers.Count <= 0)
                            continue;
                        //log duplicates
                        for (int i = 0; i < dinfo.duplicates.Count; i++)
                        {
                            if (i == 0)
                                Debug.Log("<color=yellow>[重复文件][保留]</color> " + dinfo.duplicates[i]);
                            else
                                Debug.Log("<color=yellow>[重复文件]</color> " + dinfo.duplicates[i]);
                        }
                        foreach (var refer in refers)
                        {
                            var filePath = GetAssetFullPath(refer);
                            if (!File.Exists(filePath))
                                continue;
                            var contents = File.ReadAllText(filePath);

                            List<string> dereferencePath = new List<string>();
                            //replace guid
                            for (int i = 1; i < dinfo.duplicates.Count; i++)
                            {
                                var duplicatePath = dinfo.duplicates[i];
                                var guid = AssetDatabase.AssetPathToGUID(duplicatePath);
                                if (string.IsNullOrEmpty(guid)) continue;
                                if (!contents.Contains(guid)) continue;
                                contents = contents.Replace(guid, targetGuid);
                                dereferencePath.Add(duplicatePath);
                            }
                            if (dereferencePath.Count > 0)
                            {
                                File.WriteAllText(filePath, contents);
                                Debug.Log("<color=green>[需要修改引用的对象]</color> " + refer);
                                foreach (var derefPath in dereferencePath)
                                    Debug.Log("[替换GUID] " + Path.GetFileName(derefPath) + "  [替换为]  " + Path.GetFileName(targetMeta));
                            }
                        }
                        //clear duplicate
                        for (int i = 1; i < dinfo.duplicates.Count; i++)
                        {
                            var duplicatePath = dinfo.duplicates[i];
                            AssetDatabase.DeleteAsset(duplicatePath);
                            Debug.Log("<color=red>[删除重复文件]</color> " + duplicatePath);
                        }
                        //leave one move to common folder
                        var commonRelativePath = commonFolderName + "/" + sourceType;
                        var commonFolder = executePath + "/" + commonFolderName;
                        var sourceFolder = executePath + "/" + commonRelativePath;

                        var newTargetMeta = sourceFolder + "/" + Path.GetFileName(targetMeta);
                        var gui = string.Empty;
                        if (!AssetDatabase.IsValidFolder(commonFolder))
                            gui = AssetDatabase.CreateFolder(executePath, commonFolderName);
                        if (!AssetDatabase.IsValidFolder(sourceFolder))
                            gui = AssetDatabase.CreateFolder(commonFolder, sourceType);

                        AssetDatabase.MoveAsset(targetMeta, newTargetMeta);

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
                EditorUtility.ClearProgressBar();
            }
            if (GUILayout.Button("参考文档", GUILayout.Width(120)))
            {
                OpenWebsite("https://docs.qq.com/doc/DY3ZweENwc2ZUdVhF");
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndFadeGroup();
    }

    public static StringHandleList pathList_sourceDeduplication;

    //特效重命名加提取
    public void SourceDeduplication()
    {
        if (pathList_sourceDeduplication == null)
        {
            pathList_sourceDeduplication = new StringHandleList("[路径]", this.GetType().Name + "pathList_sourceDeduplication");
            pathList_sourceDeduplication.Init();
        }
        if (SearchTypes_SoureceDeduplication == null)
            SearchTypes_SoureceDeduplication = GetSearchTypes("SearchTypes_SoureceDeduplication");
        //if (SearchTypes_TargetAssets == null)
        //    SearchTypes_TargetAssets = GetSearchTypes("SearchTypes_TargetAssets");
        animBool_prefab_prefabReferenceDeduplication.target = Foldout(animBool_prefab_prefabReferenceDeduplication.target, "冗余资源去重");
        if (EditorGUILayout.BeginFadeGroup(animBool_prefab_prefabReferenceDeduplication.faded))
        {
            //要去重Asset类型(图片,网管,...)
            //animBool_prefab_deduplication_filter.target = Foldout(animBool_prefab_deduplication_filter.target, "要去重文件类型(图片,网格,...)");
            //if (EditorGUILayout.BeginFadeGroup(animBool_prefab_deduplication_filter.faded))
            {
                foreach (var searchType in SearchTypes_SoureceDeduplication)
                {
                    searchType.isOn.value = EditorGUILayout.Toggle(searchType.content, searchType.isOn.value);
                }
            }
            //EditorGUILayout.EndFadeGroup();
            //if (partialPath_prefabReferenceDeduplication == null) partialPath_prefabReferenceDeduplication = new StringHandle(this.GetType().Name + "partialPath_prefabReferenceDeduplication", "Assets/Art/Env/Scenes");
            //partialPath_prefabReferenceDeduplication.value = EditorGUILayout.TextField("处理路径", partialPath_prefabReferenceDeduplication.value);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                pathList_sourceDeduplication.OnGUI();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("开始整理"))
            {
                if (pathList_sourceDeduplication.InfosCount <= 0)
                {
                    EditorUtility.DisplayDialog("Tips", "请输入处理路径!", "ok", "cancal");
                    return;
                }
                if (!EditorUtility.DisplayDialog("Tips", "请谨慎填写,否则会导致漫长的处理时间!", "继续", "重新填写"))
                    return;

                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                int deleteCount = 0;
                int modifyGUIDCount = 0;

                for (int pathIndex = 0; pathIndex < pathList_sourceDeduplication.Infos.Count; pathIndex++)
                {
                    string executePath = pathList_sourceDeduplication.Infos[pathIndex];
                    if (string.IsNullOrEmpty(executePath))
                        continue;

                    Debug.Log(string.Format("<color=white>[开始整理路径] [{0}]</color> ", executePath));
                    foreach (var searchType in SearchTypes_SoureceDeduplication)
                    {
                        if (!searchType.isOn.value)
                            continue;
                        var sourceType = searchType.content.text;
                        bool isCancel = false;
                        int count = 0;
                        int amount = 0;

                        Debug.Log(string.Format("<color=white>[开始整理类型] [{0}]</color> ", sourceType));

                        //collect assets
                        List<string> assetPaths;
                        assetPaths = FindAssets(searchType.filter, new string[] { executePath }).ToList();
                        string commonSourceFolderPath = string.Format("{0}/{1}s", executePath, searchType.content);
                        //except commonFolderName
                        //for (int i = 0; i < assetPaths.Count; i++)
                        //    if (assetPaths.Contains(commonFolderName))
                        //        assetPaths.RemoveAt(i--);
                        //collect Duplicate assets
                        var res = GetDuplicationFileAtPath(assetPaths, sourceType);
                        if (res.Count <= 0)
                            continue;
                        //collect dependences
                        Dictionary<string, string[]> assetPath2dependences = new Dictionary<string, string[]>();
                        List<string> otherAssetPaths;
                        otherAssetPaths = FindAssets("t:Material t:Prefab", new string[] { executePath }).ToList();
                        count = 0;
                        amount = otherAssetPaths.Count;
                        foreach (var assetPath in otherAssetPaths)
                        {
                            count++;
                            isCancel = isCancel || EditorUtility.DisplayCancelableProgressBar(string.Format("[{0}/{1}][{2}][{3}]Collect Dependences...", count, otherAssetPaths.Count, pathIndex + 1, sourceType), assetPath, (float)count / (float)otherAssetPaths.Count);
                            if (isCancel)
                            {
                                EditorUtility.ClearProgressBar();
                                break;
                            }
                            if (!assetPaths.Contains(assetPath))
                                if (!assetPath2dependences.ContainsKey(assetPath))
                                    assetPath2dependences.Add(assetPath, AssetDatabase.GetDependencies(assetPath));
                        }
                        //except commonFolderName
                        //for (int i = 0; i < assetPaths.Count; i++)
                        //    if (assetPaths.Contains(commonFolderName))
                        //        assetPaths.RemoveAt(i--);
                        //check duplicates
                        for (int i = 0; i < res.Count; i++)
                            if (res[i].duplicates.Count <= 1)
                                res.RemoveAt(i--);
                        //process
                        count = 0;
                        foreach (var dinfo in res)
                        {
                            bool isChanged = false;
                            count++;
                            int targetMetaIndex = 0;
                            string targetMeta = dinfo.duplicates[targetMetaIndex];
                            for (int i = 0; i < dinfo.duplicates.Count; i++)
                            {
                                if (dinfo.duplicates[i].IndexOf(commonSourceFolderPath) >= 0)
                                {
                                    targetMeta = dinfo.duplicates[i];
                                    targetMetaIndex = i;
                                }
                            }


                            isCancel = isCancel || EditorUtility.DisplayCancelableProgressBar(string.Format("[{0}/{1}][{2}][{3}]Deduplication & Dereference...", count, res.Count, pathIndex + 1, sourceType), targetMeta, (float)count / (float)res.Count);
                            if (isCancel)
                            {
                                EditorUtility.ClearProgressBar();
                                break;
                            }

                            var targetGuid = AssetDatabase.AssetPathToGUID(targetMeta);
                            if (string.IsNullOrEmpty(targetGuid)) continue;

                            //collect refers
                            List<string> refers = new List<string>();
                            foreach (var duplicatePath in dinfo.duplicates)
                            {
                                foreach (var pair in assetPath2dependences)
                                {
                                    var assetPath = pair.Key;
                                    var dependences = pair.Value;
                                    if (dependences.Contains(duplicatePath) && !refers.Contains(assetPath))
                                        refers.Add(assetPath);
                                }
                                // Debug.Log(path);
                                //Debug.Log(GetAssetFullPath(path));
                            }
                            if (refers.Count <= 0)
                                continue;
                            //log duplicates
                            for (int i = 0; i < dinfo.duplicates.Count; i++)
                            {
                                if (i == targetMetaIndex)
                                    Debug.Log("<color=yellow>[重复文件][保留]</color> " + dinfo.duplicates[i]);
                                else
                                    Debug.Log("<color=yellow>[重复文件]</color> " + dinfo.duplicates[i]);
                            }
                            foreach (var refer in refers)
                            {
                                var filePath = GetAssetFullPath(refer);
                                if (!File.Exists(filePath))
                                    continue;
                                var contents = File.ReadAllText(filePath);

                                List<string> dereferencePath = new List<string>();
                                //replace guid
                                for (int i = 1; i < dinfo.duplicates.Count; i++)
                                {
                                    var duplicatePath = dinfo.duplicates[i];
                                    var guid = AssetDatabase.AssetPathToGUID(duplicatePath);
                                    if (string.IsNullOrEmpty(guid)) continue;
                                    if (!contents.Contains(guid)) continue;
                                    contents = contents.Replace(guid, targetGuid);
                                    dereferencePath.Add(duplicatePath);
                                    modifyGUIDCount++;
                                }
                                if (dereferencePath.Count > 0)
                                {
                                    isChanged = true;
                                    File.WriteAllText(filePath, contents);
                                    Debug.Log("<color=green>[需要修改引用的对象]</color> " + refer);
                                    foreach (var derefPath in dereferencePath)
                                        Debug.Log("[替换GUID] " + Path.GetFileName(derefPath) + "  [替换为]  " + Path.GetFileName(targetMeta));
                                }
                            }
                            //clear duplicate
                            for (int i = 0; i < dinfo.duplicates.Count; i++)
                            {
                                if (i == targetMetaIndex)
                                    continue;
                                var duplicatePath = dinfo.duplicates[i];
                                if (AssetDatabase.DeleteAsset(duplicatePath))
                                {
                                    Debug.Log("<color=red>[删除重复文件]</color> " + duplicatePath);
                                    deleteCount++;
                                    isChanged = true;
                                }
                            }


                            //leave one move to common folder
                            //var commonRelativePath = commonFolderName + "/" + sourceType;
                            //var commonFolder = executePath + "/" + commonFolderName;
                            //var sourceFolder = executePath + "/" + commonRelativePath;

                            //var newTargetMeta = sourceFolder + "/" + Path.GetFileName(targetMeta);
                            //var gui = string.Empty;
                            //if (!AssetDatabase.IsValidFolder(commonFolder))
                            //    gui = AssetDatabase.CreateFolder(executePath, commonFolderName);
                            //if (!AssetDatabase.IsValidFolder(sourceFolder))
                            //    gui = AssetDatabase.CreateFolder(commonFolder, sourceType);

                            //AssetDatabase.MoveAsset(targetMeta, newTargetMeta);

                            if (isChanged)
                            {
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                            }
                        }
                    }
                }
                Debug.Log("<color=white>[改动GUID预制体数] </color> " + modifyGUIDCount);
                Debug.Log("<color=white>[删除文件数] </color> " + deleteCount);
                sw.Stop();
                Debug.Log("[开始结束]耗时 " + sw.ElapsedMilliseconds * 0.001f);
                EditorUtility.ClearProgressBar();
            }
            if (GUILayout.Button("参考文档", GUILayout.Width(120)))
            {
                OpenWebsite("https://docs.qq.com/doc/DY3ZweENwc2ZUdVhF");
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndFadeGroup();
    }

    public static StringHandleList pathList_sourceNameReplace;
    public static StringHandleList pathList_replaceStrings;
    public static List<SearchType> SearchTypes_SourceNameReplace;

    public void SourceNameReplace()
    {
        if (pathList_sourceNameReplace == null)
        {
            pathList_sourceNameReplace = new StringHandleList("[路径]", this.GetType().Name + "pathList_sourceNameReplace");
            pathList_sourceNameReplace.Init();
        }
        if (pathList_replaceStrings == null)
        {
            pathList_replaceStrings = new StringHandleList("[要替换成_的字符串]", this.GetType().Name + "pathList_replaceStrings");
            pathList_replaceStrings.Init();
        }
        if (SearchTypes_SourceNameReplace == null)
            SearchTypes_SourceNameReplace = GetSearchTypes("SearchTypes_SourceNameReplace");
        //if (SearchTypes_TargetAssets == null)
        //    SearchTypes_TargetAssets = GetSearchTypes("SearchTypes_TargetAssets");
        animBool_prefab_prefabNameReplace.target = Foldout(animBool_prefab_prefabNameReplace.target, "资源名字替换(非法符号替换成_)");
        if (EditorGUILayout.BeginFadeGroup(animBool_prefab_prefabNameReplace.faded))
        {
            //要去重Asset类型(图片,网管,...)
            //animBool_prefab_deduplication_filter.target = Foldout(animBool_prefab_deduplication_filter.target, "资源类型");
            //if (EditorGUILayout.BeginFadeGroup(animBool_prefab_deduplication_filter.faded))
            {
                foreach (var searchType in SearchTypes_SourceNameReplace)
                {
                    searchType.isOn.value = EditorGUILayout.Toggle(searchType.content, searchType.isOn.value);
                }
            }
            //EditorGUILayout.EndFadeGroup();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                pathList_replaceStrings.OnGUI();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                pathList_sourceNameReplace.OnGUI();
                EditorGUILayout.EndVertical();
            }


            if (GUILayout.Button("开始整理"))
            {
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                for (int pathIndex = 0; pathIndex < pathList_sourceNameReplace.Infos.Count; pathIndex++)
                {
                    string executePath = pathList_sourceNameReplace.Infos[pathIndex];
                    if (string.IsNullOrEmpty(executePath))
                        continue;

                    Debug.Log(string.Format("<color=white>[开始整理路径] [{0}]</color> ", executePath));
                    foreach (var searchType in SearchTypes_SourceNameReplace)
                    {
                        if (!searchType.isOn.value)
                            continue;
                        var sourceType = searchType.content.text;

                        Debug.Log(string.Format("<color=white>[开始整理类型] [{0}]</color> ", sourceType));


                        for (int replaceStringIndex = 0; replaceStringIndex < pathList_replaceStrings.Infos.Count; replaceStringIndex++)
                        {
                            string replaceStr = pathList_replaceStrings.Infos[replaceStringIndex];
                            if (string.IsNullOrEmpty(replaceStr))
                                continue;
                            List<string> assetPaths;
                            assetPaths = FindAssets(searchType.filter, new string[] { executePath }).Where(item => item.IndexOf(replaceStr) >= 0).ToList();
                            bool isCancel = false;
                            int count = 0;
                            int amount = assetPaths.Count;
                            for (int i = 0; i < assetPaths.Count; i++)
                            {
                                count++;
                                string assetPath = assetPaths[i];
                                isCancel = isCancel || EditorUtility.DisplayCancelableProgressBar(string.Format("[{0}/{1}][{2}][{3}]Replace Name...", count, assetPaths.Count, sourceType, replaceStr), assetPath, (float)count / (float)assetPaths.Count);
                                if (isCancel)
                                {
                                    EditorUtility.ClearProgressBar();
                                    break;
                                }
                                string fileName = Path.GetFileName(assetPath);
                                string newName = fileName.Replace(replaceStr, "_");
                                string res = AssetDatabase.RenameAsset(assetPath, newName);
                                if (!string.IsNullOrEmpty(res))
                                {
                                    newName = fileName.Replace(replaceStr, "__");
                                    res = AssetDatabase.RenameAsset(assetPath, newName);
                                    if (!string.IsNullOrEmpty(res))
                                        Debug.Log("<color=red>[重命名失败]</color> " + res);
                                }

                            }
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                    }
                }
                EditorUtility.ClearProgressBar();

                sw.Stop();
                Debug.Log("[开始结束]耗时 " + sw.ElapsedMilliseconds * 0.001f);
            }


        }
        EditorGUILayout.EndFadeGroup();
    }


    public enum ERenameType
    {
        FineAndReplace = 0,
        SetName = 1,
        StripCharacters = 2,
        ChangeCase = 3,
    }
    public enum ERenameTypeCN
    {
        查找和替换 = 0,
        设置名字 = 1,
        字符剥离 = 2,
        改变大小写 = 3,
    }
    public enum ESetNameMethod
    { 
        New = 0,
        Prefix = 1,
        Suffix = 2,
    }
    public enum ESetNameMethodCN
    {
        新命名 = 0,
        设置前缀 = 1,
        设置后缀 = 2,
    }

    public static IntHandle RenameType;
    public static IntHandle RenameMethodType;
    public static StringHandle RenameName;

    public static List<string> _selectedGUIDs = new List<string>();
    public static int activeMainSelected = -1;

    public static AssetRenameOperationGroup renameOperationGroup;
    /// <summary>
    /// 资源重命名工具
    /// </summary>
    public void AssetRenameTool()
    {
        if (RenameType == null) RenameType = new IntHandle("AssetRenameTool_RenameType",(int)ERenameType.SetName);
        if (RenameMethodType == null) RenameMethodType = new IntHandle("AssetRenameTool_RenameMethodType", (int)ESetNameMethod.New);
        if (RenameName == null) RenameName = new StringHandle("AssetRenameTool_RenameName", string.Empty);
        //
        animBool_resource_rename.target = Foldout(animBool_resource_rename.target, "资源重命名工具");
        if (EditorGUILayout.BeginFadeGroup(animBool_resource_rename.faded))
        {
            RenameType.value = (int)Enum.ToObject(typeof(ERenameType), EditorGUILayout.EnumPopup("Type", (ERenameTypeCN)RenameType.value));

            if (((ERenameType)RenameType.value) == ERenameType.SetName)
            {
                AssetRenameTool_SetName();
            }
            else
            {
                EditorGUILayout.HelpBox("这个类型还没有弄", MessageType.Warning);
            }
        }
        EditorGUILayout.EndFadeGroup();
    }


    /// <summary>
    /// Type=设置名字
    /// </summary>
    public void AssetRenameTool_SetName()
    {
        RenameMethodType.value = (int)Enum.ToObject(typeof(ESetNameMethod), EditorGUILayout.EnumPopup("Method", (ESetNameMethodCN)RenameMethodType.value));
        RenameName.value = EditorGUILayout.TextField("Name", RenameName.value);
        if (_selectedGUIDs != null)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                for (int i = 0; i < _selectedGUIDs.Count; i++)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(_selectedGUIDs[i]);
                    if (string.IsNullOrEmpty(assetPath))
                        continue;
                    string fileName = Path.GetFileName(assetPath);
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(fileName);
                        if (i == activeMainSelected)
                        {
                            GUI.color = Color.yellow;
                            EditorGUILayout.LabelField("MainAsset", GUILayout.Width(60f));
                            GUI.color = Color.white;
                        }
                        else if (GUILayout.Button("As Main", GUILayout.Width(60f)))
                        {
                            activeMainSelected = i;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
        if (GUILayout.Button("OK"))
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                if (RenameMethodType.ToEnum<ESetNameMethod>() == ESetNameMethod.New)
                    AssetRenameTool_RenameAsset_Method_New();
                else if (RenameMethodType.ToEnum<ESetNameMethod>() == ESetNameMethod.Prefix)
                    AssetRenameTool_RenameAsset_Method_Prefix();
            }
            catch (Exception e)
            { 
                Debug.LogError(e.ToString());
            }
            AssetDatabase.StopAssetEditing();
        }
        if (renameOperationGroup != null && GUILayout.Button("撤销"))
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                renameOperationGroup.Undo();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            renameOperationGroup = null;
            AssetDatabase.StopAssetEditing();

        }
    }

    /// <summary>
    /// 新命名的ok按钮
    /// </summary>
    public void AssetRenameTool_RenameAsset_Method_New()
    {
        renameOperationGroup = new AssetRenameOperationGroup();

        if (_selectedGUIDs == null || _selectedGUIDs.Count <= 0)
            return;
        if (string.IsNullOrEmpty(RenameName.value))
            return;
        if (activeMainSelected < 0)
            return;
        int count = 0;
        for (int i = 0; i < _selectedGUIDs.Count; i++)
        {
            string guid = _selectedGUIDs[i];
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var ext = Path.GetExtension(assetPath);
            if (i == activeMainSelected)
            {
                renameOperationGroup.AddRenameOp(guid, $"{RenameName.value}{ext}");
            }
            else
            {
                renameOperationGroup.AddRenameOp(guid, $"{RenameName.value}_{++count}{ext}");
            }
        }
        renameOperationGroup.Perform();
    }

    /// <summary>
    /// 前缀的ok按钮
    /// </summary>
    public void AssetRenameTool_RenameAsset_Method_Prefix()
    {
        renameOperationGroup = new AssetRenameOperationGroup();

        if (_selectedGUIDs == null || _selectedGUIDs.Count <= 0)
            return;
        if (string.IsNullOrEmpty(RenameName.value))
            return;
        for (int i = 0; i < _selectedGUIDs.Count; i++)
        {
            string guid = _selectedGUIDs[i];
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var ext = Path.GetExtension(assetPath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
            renameOperationGroup.AddRenameOp(guid, $"{RenameName.value}{fileNameWithoutExtension}{ext}");
        }
        renameOperationGroup.Perform();
    }

    /// <summary>
    /// 重命名操作
    /// </summary>
    public class AssetRenameOperationGroup
    {
        //后面可能会改成记录多个Group
        public static int s_guid = -1;

        private int _guid = -1;
        private List<AssetRenameOperation> _operations;
        public int guid => s_guid;
        public List<AssetRenameOperation> Operations => _operations;

        public AssetRenameOperationGroup()
        {
            _guid = ++s_guid;
            _operations = new List<AssetRenameOperation>();
        }
        public void AddRenameOp(string guid, string newAssetName)
        {
            var op = new AssetRenameOperation(guid, newAssetName);
            _operations.Add(op);
        }
        public void Perform()
        {
            for (int i = 0; i < _operations.Count; i++)
            { 
                var op = _operations[i];
                op.Perform();
            }
        }
        public void Undo()
        {
            for (int i = 0; i < _operations.Count; i++)
            {
                var op = _operations[i];
                op.Undo();
            }
        }
        public class AssetRenameOperation
        {
            public string guid = string.Empty;
            public string newAssetName = string.Empty;
            //
            public string preAssetPath = string.Empty;
            public string preAssetName = string.Empty;
            //
            private bool _isValid = false;
            public bool IsValid => _isValid;

            private bool _isPerformed = false;
            public bool IsPerformed => _isPerformed;
            //
            public string CurrentAssetPath
            {
                get { return AssetDatabase.GUIDToAssetPath(guid); }
            }
            public AssetRenameOperation(string guid, string newAssetName)
            {
                Init(guid, newAssetName);
            }
            private void Init(string guid, string newAssetName)
            {
                this.guid = guid;
                this.newAssetName = newAssetName;
                //
                _isValid = false;
                if (string.IsNullOrEmpty(guid))
                    return;
                if (string.IsNullOrEmpty(newAssetName))
                    return;
                if (string.IsNullOrEmpty(CurrentAssetPath))
                    return;
                //
                preAssetPath = CurrentAssetPath;
                if (string.IsNullOrEmpty(preAssetPath))
                    return;
                _isPerformed = false;
                _isValid = true;
                preAssetName = Path.GetFileName(preAssetPath);
            }
            public void Perform()
            {
                if (!_isValid)
                    return;
                _isPerformed = true;
                AssetDatabase.RenameAsset(preAssetPath, newAssetName);
            }
            public void Undo()
            {
                if (!_isValid)
                    return;
                if (!_isPerformed)
                    return;
                AssetDatabase.RenameAsset(CurrentAssetPath, preAssetName);
            }
        }
    }


    //private MethodInfo RegisterAssetsMoveUndo = null;
    public void AssetRenameTool_RecordUndo(ref List<string> assetGUIDs, string recordName = "AssetRenameTool_RecordUndo")
    {
        //unity好像没有这个功能
        //if (RegisterAssetsMoveUndo == null)
        //    RegisterAssetsMoveUndo = typeof(Undo).GetMethod("RegisterAssetsMoveUndo", BindingFlags.NonPublic | BindingFlags.Static);
        //if (RegisterAssetsMoveUndo == null)
        //    return;
        if (assetGUIDs == null || assetGUIDs.Count <= 0)
            return;

        var assets = new List<UnityEngine.Object>();
        var assetPaths = new List<string>();

        for (int i = 0; i < assetGUIDs.Count; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if(asset == null)
                continue;
            assets.Add(asset);
            assetPaths.Add(assetPath);
        }
        //RegisterAssetsMoveUndo.Invoke(null, new object[] { assetPaths.ToArray() });
    }
    public void AssetRenameTool_OnSelectionChange()
    {
        activeMainSelected = 0;
        _selectedGUIDs = _selectedGUIDs ?? new List<string>();
        _selectedGUIDs.Clear();
        foreach (var guids in Selection.assetGUIDs)
        { 
            string assetPath = AssetDatabase.GUIDToAssetPath(guids);
            if (string.IsNullOrEmpty(assetPath))
                continue;
            _selectedGUIDs.Add(guids);
        }
    }

    /// <summary>
    /// 有对应layer的GameObject?
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    private bool AnyGameObjectInLayout(Transform trans, int layer)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            var child = trans.GetChild(i);
            if (AnyGameObjectInLayout(child, layer))
                return true;
        }
        if (trans.gameObject.layer == layer)
            return true;

        return false;
    }
    /// <summary>
    /// 获取对应layer的GameObject:List
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="layer"></param>
    /// <param name="gobjs"></param>
    private void GetGameObjectInLayout(Transform trans, int layer, List<GameObject> gobjs)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            var child = trans.GetChild(i);
            GetGameObjectInLayout(child, layer, gobjs);
        }
        if (trans.gameObject.layer == layer)
            gobjs.Add(trans.gameObject);
    }


    public static StringHandle partialPath_SameAssetNameReport;
    public static StringHandle path_SameAssetNameReportOutput;
    public static StringHandleList pathExceptList_SameNameAssets;
    public static BoolHandle caseFileExt_SameNameAssets;

    /// <summary>
    /// 画重名资源[CSV]报告
    /// </summary>
    /// 
    void DrawExportSameNameAssets()
    {
        if (pathExceptList_SameNameAssets == null)
        {
            pathExceptList_SameNameAssets = new StringHandleList("[忽略路径]", this.GetType().Name + "pathExceptList_SameNameAssets");
            pathExceptList_SameNameAssets.Init();
        }

        animBool_resource_sameassetname.target = Foldout(animBool_resource_sameassetname.target, "重名资源[CSV]报告");
        if (EditorGUILayout.BeginFadeGroup(animBool_resource_sameassetname.faded))
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                //过滤路径
                animBool_resource_sameassetname_except_paths.target = Foldout(animBool_resource_sameassetname_except_paths.target, "过滤路径");
                if (EditorGUILayout.BeginFadeGroup(animBool_resource_sameassetname_except_paths.faded))
                {
                    pathExceptList_SameNameAssets.OnGUI();
                }
                EditorGUILayout.EndFadeGroup();
                //过滤选项
                animBool_resource_sameassetname_filter.target = Foldout(animBool_resource_sameassetname_filter.target, "过滤选项");
                if (EditorGUILayout.BeginFadeGroup(animBool_resource_sameassetname_filter.faded))
                {
                    foreach (var searchType in SearchTypes_SameNameAsset)
                    {
                        searchType.isOn.value = EditorGUILayout.Toggle(searchType.content, searchType.isOn.value);
                    }
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUILayout.EndVertical();
            }
            //
            string filter = string.Empty;
            foreach (var searchType in SearchTypes_SameNameAsset)
                if (searchType.isOn.value)
                {
                    filter = string.IsNullOrEmpty(filter) ? searchType.value : string.Format("{0} {1}", filter, searchType.value);
                }

            EditorGUILayout.TextField("过滤标记", filter);

            if (caseFileExt_SameNameAssets == null) caseFileExt_SameNameAssets = new BoolHandle(this.GetType().Name + "caseFileExt_SameNameAssets", false);
            caseFileExt_SameNameAssets.value = EditorGUILayout.Toggle("关注文件后缀", caseFileExt_SameNameAssets.value);

            if (partialPath_SameAssetNameReport == null) partialPath_SameAssetNameReport = new StringHandle(this.GetType().Name + "partialPath_SameAssetNameReport", "Assets/");
            partialPath_SameAssetNameReport.value = EditorGUILayout.TextField("分析路径", partialPath_SameAssetNameReport.value);


            EditorGUILayout.BeginHorizontal();
            {
                if (path_SameAssetNameReportOutput == null) path_SameAssetNameReportOutput = new StringHandle(this.GetType().Name + "path_SameAssetNameReportOutput", string.Empty);

                path_SameAssetNameReportOutput.value = EditorGUILayout.TextField("报告导出路径", path_SameAssetNameReportOutput.value);
                if (GUILayout.Button("选择路径"))
                {
                    var res = EditorUtility.OpenFolderPanel("选择报告导出路径", path_SameAssetNameReportOutput.value, path_SameAssetNameReportOutput.value);
                    if (!string.IsNullOrEmpty(res))
                        path_SameAssetNameReportOutput.value = res;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Export Report"))
                {
                    if (string.IsNullOrEmpty(partialPath_SameAssetNameReport.value))
                        return;
                    if (!Directory.Exists(partialPath_SameAssetNameReport.value))
                        Directory.CreateDirectory(partialPath_SameAssetNameReport.value);

                    InitReferenceDataIfNeeded();


                    if (!EditorUtility.DisplayDialog("Tips", "是否继续?", "continue", "cancal"))
                        return;

                    if (string.IsNullOrEmpty(path_SameAssetNameReportOutput.value))
                    {
                        EditorUtility.DisplayDialog("Tips", "请设置报错输出路径!", "ok");
                        EditorGUIUtility.ExitGUI();
                        return;
                    }
                    if (!Directory.Exists(path_SameAssetNameReportOutput.value))
                    {
                        if (!EditorUtility.DisplayDialog("Tips", "输出路径不存在,是否创建?", "yes", "cancal"))
                            return;
                        Directory.CreateDirectory(path_SameAssetNameReportOutput.value);
                    }

                    //collect all assets
                    assetPaths = new List<string>();
                    if (string.IsNullOrEmpty(partialPath_SameAssetNameReport.value))
                        assetPaths = FindAssets(filter).ToList();
                    else
                        assetPaths = FindAssets(filter, new string[] { partialPath_SameAssetNameReport.value }).ToList();

                    var name2assets = new Dictionary<string, List<string>>();
                    foreach (var assetPath in assetPaths)
                    {
                        if (AssetDatabase.IsValidFolder(assetPath))
                            continue;
                        if (pathExceptList_SameNameAssets.Enabled.value && pathExceptList_SameNameAssets.Infos.Any(item => !string.IsNullOrEmpty(item.value) && assetPath.IndexOf(item.value.Replace('\\', '/')) >= 0))
                            continue;
                        var assetName = string.Empty;
                        if (caseFileExt_SameNameAssets.value)
                            assetName = Path.GetFileName(assetPath);
                        else
                            assetName = Path.GetFileNameWithoutExtension(assetPath);
                        //collect
                        if (!name2assets.TryGetValue(assetName, out var names))
                        {
                            names = new List<string>();
                            name2assets[assetName] = names;
                        }
                        names.Add(assetPath);
                    }
                    //gen csv
                    CSVReport.Init(path_SameAssetNameReportOutput.value, "SameNameAssets");
                    CSVReport.csvStr = @"Asset名字,Asset名字(带后缀),AssetPath";

                    foreach (var pair in name2assets)
                    {
                        var assetName = pair.Key;
                        var assetPaths = pair.Value;
                        if (assetPaths.Count <= 1)
                            continue;
                        foreach (var assetPath in assetPaths)
                        {
                            var line = new CSVLine(assetPath);
                            line.SetValue(0, Path.GetFileNameWithoutExtension(assetPath));
                            line.SetValue(1, Path.GetFileName(assetPath));
                            line.SetValue(2, assetPath);
                            line.ApplyLine();
                        }
                    }

                    CSVReport.Output(true);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndFadeGroup();

    }


    public static StringHandle luaDBRoot;
    public static StringHandle luaTestTargetName;
    public static string default_luaEditCodePath = string.Empty;
    public static string default_luaDBRoot = string.Empty;
    public static string default_optLuaOutput = string.Empty;
    public static StringHandle path_luaEditCodePath;
    public static StringHandle path_optimizedLuaOutput;
    public static BoolHandle genMeta;
    public static BoolHandle genCol;
    public static BoolHandle genStrings;
    public static BoolHandle genFieldPadding;
    public static StringHandle fieldPadding;
    public static StringHandleList ignoreLuas;
    public static StringHandle optimizedLuaFileExt;
    public static BoolHandle regeneral;
    public static StringHandle luaOptimize_helpDecURL;


    public bool Streaming_IsRoughAsset(Texture texture)
    {
        return texture != null && texture.name.IndexOf("_rough") > 0;
    }

    /// <summary>
    /// 生成粗图片(缩略图)
    /// </summary>
    /// <param name="savePathImage">保存路径</param>
    /// <param name="sourceTexture">原图片</param>
    /// <param name="newWidth">粗图片宽度</param>
    /// <param name="newHeight">粗图片高度</param>
    /// <returns></returns>
    public Texture Streaming_GeneralRoughTexture(string savePathImage, Texture sourceTexture, int newWidth = 32, int newHeight = 32)
    {
        savePathImage = savePathImage.Replace('/', '\\');
        Texture roughTex = AssetDatabase.LoadAssetAtPath(savePathImage, typeof(Texture)) as Texture;

        if (sourceTexture != null && roughTex == null)
        {
            sourceTexture.filterMode = FilterMode.Bilinear;
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight, 0);
            rt.filterMode = FilterMode.Bilinear;
            RenderTexture.active = rt;
            Graphics.Blit(sourceTexture, rt);
            Texture2D nTex = new Texture2D(newWidth, newHeight);
            nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            nTex.Apply();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            File.WriteAllBytes(savePathImage, nTex.EncodeToPNG());
            //Debug.Log("save   " + savePathImage);
            UnityEngine.Object.DestroyImmediate(nTex);

            AssetDatabase.ImportAsset(savePathImage, ImportAssetOptions.ForceUpdate);
            TextureImporter import = null;
            if (!import)
                import = AssetImporter.GetAtPath(savePathImage) as TextureImporter;
            import.mipmapEnabled = false;
            import.isReadable = false;
            //重新设置压缩格式
            TextureImporterPlatformSettings defaultSet = import.GetDefaultPlatformTextureSettings();

            TextureImporterPlatformSettings tips = new TextureImporterPlatformSettings();
            tips.textureCompression = TextureImporterCompression.Compressed;
            bool isHaveAlphaChannel = import.DoesSourceTextureHaveAlpha();
            if (import.alphaSource == TextureImporterAlphaSource.FromGrayScale)
                isHaveAlphaChannel = true;

            int maxTextureSize = 32;
            //地形通道图不改格式
            if (import.wrapMode == TextureWrapMode.Clamp && import.filterMode == FilterMode.Point) { }
            else
                tips.overridden = true;
            tips.maxTextureSize = maxTextureSize;

            TextureImporterPlatformSettings android = import.GetPlatformTextureSettings("Android");

            tips.maxTextureSize = maxTextureSize;
            tips.format = android != null ? android.format : defaultSet.format;
#if UNITY_2019_1_OR_NEWER
            tips.format = isHaveAlphaChannel ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ASTC_8x8;
#else
            tips.format = isHaveAlphaChannel ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ASTC_RGBA_8x8;
#endif
            tips.name = "Android";
            import.SetPlatformTextureSettings(tips);

#if UNITY_2019_1_OR_NEWER
            tips.format = isHaveAlphaChannel ? TextureImporterFormat.ASTC_6x6 : TextureImporterFormat.ASTC_6x6;
#else
            tips.format = isHaveAlphaChannel ? TextureImporterFormat.ASTC_RGBA_6x6 : TextureImporterFormat.ASTC_RGB_6x6;
#endif
            tips.name = "iPhone";
            import.SetPlatformTextureSettings(tips);


            var oldtps = import.GetPlatformTextureSettings("WebGL");
            tips.maxTextureSize = maxTextureSize;
#if UNITY_2020_1_OR_NEWER
            tips.format = TextureImporterFormat.ASTC_6x6;
#else
            tips.format = isHaveAlphaChannel ? TextureImporterFormat.DXT5 : TextureImporterFormat.DXT1;
#endif
            tips.name = "WebGL";
            import.SetPlatformTextureSettings(tips);
            import.SaveAndReimport();

            roughTex = AssetDatabase.LoadAssetAtPath(savePathImage, typeof(Texture)) as Texture;
        }
        return roughTex;
    }

    /// <summary>
    /// 清理Missing的Prefab
    /// </summary>
    public bool Streaming_RemoveMissingAttachedScript()
    {
        bool anyClear = false;
        //var gobjs = FindObjectsOfType<GameObject>(true);
        var gobjs = FindObjectsOfType<GameObject>();
        for (int i = 0; i < gobjs.Length; i++)
        {
            var gobj = gobjs[i];
            if (gobj == null)
                continue;
            var components = gobj.GetComponents<Component>();
            for (int j = 0; j < components.Length; j++)
            {
                var component = components[j];
                if (component == null)
                {
                    Debug.Log("[Clear Missing Script] " + gobj.name, gobj);
                    //GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gobj);
                    anyClear = true;
                }
            }
        }
        return anyClear;
    }
    public void Streaming_DelectMissingPrefab()
    {
        var curScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        var rootGameObjects = curScene.GetRootGameObjects();

        bool isCancel = false;
        foreach (var rootGameObject in rootGameObjects)
        {
            var gobjs = rootGameObject.GetComponentsInChildren<Transform>(true);

            int amount = gobjs.Length;
            int count = 0;
            for (int i = 0; i < gobjs.Length; i++)
            {
                count++;
                if (gobjs[i] == null)
                    continue;
                var gobj = gobjs[i].gameObject;

                isCancel = isCancel || EditorUtility.DisplayCancelableProgressBar(string.Format("[{0}/{1}]Delect Missing Prafab", count, amount), gobj.name, (float)count / (float)amount);
                if (isCancel)
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }
                if (!PrefabUtility.IsPrefabAssetMissing(gobj))
                    continue;
                GameObject.DestroyImmediate(gobj);
            }
        }
        EditorUtility.ClearProgressBar();
    }
    /// <summary>
    /// 清理Navigation里的MeshRenderer
    /// </summary>
    public void Streaming_ClearMeshRenderersOfNavigationObject()
    {
        var curScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        //使用GetRootGameObjects
        //是为了可以使用下面的rootGameObject.GetComponentsInChildren<Transform>(true);
        //T[] GetComponentsInChildren<T>(bool includeInactive); 可以获取隐藏的GameObject
        var rootGameObjects = curScene.GetRootGameObjects();
        foreach (var rootGameObject in rootGameObjects)
        {
            if (rootGameObject == null)
                continue;
            if (!rootGameObject.name.Equals("XScene"))
                continue;
            for (int i = 0; i < rootGameObject.transform.childCount; i++)
            {
                var childTransform = rootGameObject.transform.GetChild(i);
                if (childTransform.gameObject.name.IndexOf("Navigation") < 0)
                    continue;
                var meshRenderers = childTransform.GetComponentsInChildren<MeshRenderer>(true);
                for (int j = 0; j < meshRenderers.Length; j++)
                {
                    var meshRenderer = meshRenderers[j];
                    if (!meshRenderer)
                        continue;
                    GameObject.DestroyImmediate(meshRenderer);
                }
            }
        }
        MarkSceneDirty();
    }

    //------------------------------------------------------------------------------------------------------------

}

