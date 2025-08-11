using Sirenix.OdinInspector;
using UnityEngine;

namespace PJR.Systems
{
    public class ForceField : FieldObject
    {
        /// <summary>
        /// 引力衰减模式，用这个来替代Curve
        /// </summary>
        public enum AttenuateMode
        {
            [LabelText("常数")]
            Constant = 0,
            [LabelText("曲线")]
            Curve,
            Linear,
            InSine,
            OutSine,
            InOutSine,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InElastic,
            OutElastic,
            InOutElastic,
            InBack,
            OutBack,
            InOutBack,
            InBounce,
            OutBounce,
            InOutBounce,
            Flash,
            InFlash,
            OutFlash,
            InOutFlash,
        }

        [LabelText("引力衰减模式")]
        public AttenuateMode attenuateMode;

        [LabelText("衰减曲线")]
        [ShowIf("@attenuateMode == AttenuateMode.Curve")]
        public AnimationCurve attenCurve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0), new Keyframe(1, 1) });

        /// <summary>
        /// 引力
        /// </summary>
        [LabelText("引力")]
        public float force = 8f;

        [LabelText("角度限制")]
        [PropertyRange(0f, 360f)]
        public float angleLimit = 360f;

        [LabelText("无视角度限制的半径半分比")]
        [PropertyRange(0f, 1f)]
        public float resistAngleLimitRadiusPct = 0.2f;

        [LabelText("无效区半径半分比")]
        [PropertyRange(0f, 1f)]
        public float invalidZone = 0.12f;

        /// <summary>
        /// 获取衰减系数
        /// </summary>
        /// <param name="normDisfromEdge">距离边缘的标准距离</param>
        /// <returns></returns>
        public float GetAttenFactor(float normDisfromEdge)
        {
            if (attenuateMode == AttenuateMode.Linear)
                return 1f;
            else if (attenuateMode == AttenuateMode.Curve)
                return attenCurve.Evaluate(normDisfromEdge);
            else if (attenuateMode >= AttenuateMode.Linear)
                return Easing.DoEasing((Easing.Ease)(attenuateMode - AttenuateMode.Linear + 1), normDisfromEdge);
            return 1;//Constant
        }

#if UNITY_EDITOR
        /// <summary>
        /// 轮询函数 
        /// </summary>
        /// <param name="deltaTime"></param>
        public override void Tick(float deltaTime)
        {
            //        //TODO：[Field]后面需要放到jobs里去
            //        base.Tick(deltaTime);

            //        if (shape == FieldShape.Sphere) {
            //            foreach (var entity in detectedEntities)
            //            {
            //                var kcc = entity.GetComponent<KinematicCreatureController>();
            //                if (kcc == null)
            //                    continue;
            //                Vector3 vector = transform.position - entity.transform.position;
            //                var distance = vector.magnitude;
            //                var normRadius = distance / _radius;
            //                var normDisfromEdge = Mathf.Clamp01(1 - normRadius);
            //                var angle = Vector3.Angle(-vector, transform.forward);
            //                if (angle > angleLimit/2 && normRadius > resistAngleLimitRadiusPct)
            //                    return;

            //#if FIELD_DEBUG
            //                Debug.Log($"[distance] {distance}  [normDisfromEdge] {normDisfromEdge} [normRadius] {normRadius}");
            //#endif
            //                float atten = GetAttenFactor(normDisfromEdge);
            //                if (normRadius <= invalidZone)
            //                    atten = 0;// Easing.DoEasing(Easing.Ease.InQuint, normRadius / invalidZone);
            //                var spotVelocity = vector.normalized * force * atten;
            //                kcc.SetAddictiveVelocity(spotVelocity, true);
            //            }
            //        }
        }

        public override void Editor_OnDebug()
        {
            ////扇形范围
            //if (angleLimit < 360f)
            //{ 
            //    float half = angleLimit * 0.5f;
            //    Vector3 center = transform.position;
            //    Vector3 start = transform.forward;
            //    Vector3 right = Quaternion.AngleAxis(half,transform.up) * start * _radius;
            //    Vector3 left =  Quaternion.AngleAxis(-half,transform.up) * start * _radius;
            //    start = start * _radius + center;
            //    right += center;
            //    left += center;
            //    //
            //    Drawing.Draw.gameAndEditor.SolidArc(transform.position, start, right, Color.green);
            //    Drawing.Draw.gameAndEditor.SolidArc(transform.position, start, left, Color.green);
            //}else
            //    Drawing.Draw.gameAndEditor.SolidCircle(transform.position, transform.up, _radius, Color.green);

            ////无视角度限制的半径半分比
            //Drawing.Draw.gameAndEditor.SolidCircle(transform.position,transform.up, resistAngleLimitRadiusPct * _radius,Color.green);
            ////无效区半径半分比
            //Drawing.Draw.gameAndEditor.SolidCircle(transform.position,transform.up, invalidZone * _radius,Color.red);
            ////sphere线框
            //Drawing.Draw.gameAndEditor.WireSphere(transform.position, _radius);
        }
#endif
    }

}
