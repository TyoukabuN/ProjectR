using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 残影特效
/// </summary>
public class AfterImageEffects : MonoBehaviour
{

    //开启残影
    public bool _OpenAfterImage;
    public RenderingMode renderMode = RenderingMode.Fade;
    //残影颜色
    public Color _AfterImageColor = Color.black;
    //残影的生存时间
    public float _SurvivalTime = 1;
    //生成残影的间隔时间
    public float _IntervalTime = 0.05f;
    private float _Time = 0;
    //残影初始透明度
    [Range(0.1f, 1.0f)]
    public float _InitialAlpha = 1.0f;

    private List<AfterImage> _AfterImageList;
    private List<SkinnedMeshRenderer> _SkinnedMeshRenderers;

    public string shaderName = "X_Shader/D_Particles/Particles";

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (_AfterImageList != null)
        {
            for (int i = 0; i < _AfterImageList.Count; i++)
            {
                Destroy(_AfterImageList[i]);
            }
        }
        _AfterImageList = new List<AfterImage>();
        _SkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
    }
    void Update()
    {
        if (_AfterImageList != null)
        {
            if (_SkinnedMeshRenderers == null)
            {
                _OpenAfterImage = false;
                return;
            }

            _Time += Time.deltaTime;
            if (_OpenAfterImage)
            { 
                //生成残影
                CreateAfterImage();
            }
            //刷新残影
            UpdateAfterImage();
        }
    }
    /// <summary>
    /// 生成残影
    /// </summary>
    void CreateAfterImage()
    {
        //生成残影
        if (_Time >= _IntervalTime)
        {
            _Time = 0;

            foreach (var renderer in _SkinnedMeshRenderers)
            {
                Mesh mesh = new Mesh();
                renderer.BakeMesh(mesh);

                Shader shader = null;//Shader.GetShader(shaderName);
                if (shader == null)
                    continue;

                Material material = new Material(shader);
                SetMaterialRenderingMode(material, renderMode);

                Color color = _AfterImageColor;
                //if (material.HasProperty("_Color"))
                //{
                //    color = material.GetColor("_Color");
                //}
                //material.DisableKeyword("_QUALITY_HIGH");
                //material.EnableKeyword("_QUALITY_MIDDLE");
                //material.DisableKeyword("_QUALITY_LOW");

                _AfterImageList.Add(new AfterImage(
                    mesh,
                    material,
                    renderer.transform.localToWorldMatrix,
                    _InitialAlpha,
                    Time.realtimeSinceStartup,
                    _SurvivalTime)
                { 
                    wpos = renderer.transform.position,
                    rot = renderer.transform.rotation,
                    color = color,
                });
            }
        }
    }
    /// <summary>
    /// 刷新残影
    /// </summary>
    void UpdateAfterImage()
    {
        //刷新残影，根据生存时间销毁已过时的残影
        for (int i = 0; i < _AfterImageList.Count; i++)
        {
            float _PassingTime = Time.realtimeSinceStartup - _AfterImageList[i]._StartTime;

            if (_PassingTime > _AfterImageList[i]._Duration)
            {
                //Destroy(_AfterImageList[i]);
                _AfterImageList.RemoveAt(i);
                i--;
                continue;
            }

            if (_AfterImageList[i]._Material.HasProperty("_TintColor"))
            {
                _AfterImageList[i]._Alpha *= (1 - _PassingTime / _AfterImageList[i]._Duration);
                _AfterImageList[i].color.a = _AfterImageList[i]._Alpha;
                Debug.Log(_AfterImageList[i].color);
                _AfterImageList[i]._Material.SetColor("_TintColor", _AfterImageList[i].color);
                if (_AfterImageList[i]._Material.HasProperty("_FresnelColor"))
                    _AfterImageList[i]._Material.SetColor("_FresnelColor", _AfterImageList[i].color);
            }

            Graphics.DrawMesh(_AfterImageList[i]._Mesh, _AfterImageList[i].wpos, _AfterImageList[i].rot, _AfterImageList[i]._Material, gameObject.layer);
        }
    }
    /// <summary>
    /// 设置纹理渲染模式
    /// </summary>
    void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case RenderingMode.Cutout:
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case RenderingMode.Fade:
                material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case RenderingMode.Transparent:
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }
}
public enum RenderingMode
{
    Opaque,
    Cutout,
    Fade,
    Transparent,
}
class AfterImage : Object
{
    //残影网格
    public Mesh _Mesh;
    //残影纹理
    public Material _Material;
    //残影位置
    public Matrix4x4 _Matrix;
    //残影透明度
    public float _Alpha;
    //残影启动时间
    public float _StartTime;
    //残影保留时间
    public float _Duration;

    public Vector3 wpos;
    public Quaternion rot;
    public Color color;

    public AfterImage(Mesh mesh, Material material, Matrix4x4 matrix4x4, float alpha, float startTime, float duration)
    {
        _Mesh = mesh;
        _Material = material;
        _Matrix = matrix4x4;
        _Alpha = alpha;
        _StartTime = startTime;
        _Duration = duration;
    }
}