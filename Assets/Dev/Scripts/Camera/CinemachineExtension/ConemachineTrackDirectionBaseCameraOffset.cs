using Cinemachine;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachinePathBase;
using static Cinemachine.CinemachineTrackedDolly;

[AddComponentMenu("")] // Hide in menu
[ExecuteAlways]
[SaveDuringPlay]
public class CinemachineDollyDirectionBaseCameraOffset : CinemachineExtension
{
    public Vector3 FollowTargetPosition
    {
        get
        {
            var vcam = VirtualCamera.FollowTargetAsVcam;
            if (vcam != null)
                return vcam.State.FinalPosition;
            Transform target = FollowTarget;
            if (target != null)
                return TargetPositionCache.GetTargetPosition(target);
            return Vector3.zero;
        }
    }
    public Vector3 offset = Vector3.zero;

    public CinemachinePathBase m_Path;

    public Transform FollowTarget
    {
        get
        {
            CinemachineVirtualCameraBase vcam = VirtualCamera;
            return vcam == null ? null : vcam.ResolveFollow(vcam.Follow);
        }
    }

    public float m_PathPosition;

    /// <summary>Get the position of the Follow target.  Special handling: If the Follow target is
    /// a VirtualCamera, returns the vcam State's position, not the transform's position</summary>
    public CinemachinePathBase.PositionUnits m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;
    private float m_PreviousPathPosition = 0;

    public int m_SearchRadius = 2;
    public int m_SearchResolution = 5;

    [Range(0f, 20f)]
    public float m_XDamping = 0f;

    [Range(0f, 20f)]
    public float m_YDamping = 2f;

    [Range(0f, 20f)]
    public float m_ZDamping = 2f;

    public bool IsValid { get { return enabled && m_Path != null; } }

    public float CameraDistance = 5f;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (!IsValid)
            return;

        if (stage == CinemachineCore.Stage.Body)
        {
            if (FollowTarget != null)
            {
                float prevPos = m_Path.ToNativePathUnits(m_PreviousPathPosition, m_PositionUnits);
                // This works in path units
                m_PathPosition = m_Path.FindClosestPoint(
                FollowTargetPosition,
                    Mathf.FloorToInt(prevPos),
                    m_SearchRadius,
                    m_SearchResolution);
                m_PathPosition = m_Path.FromPathNativeUnits(m_PathPosition, m_PositionUnits);
            }
            float newPathPosition = m_PathPosition;

            if (deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
            {
                // Normalize previous position to find the shortest path
                float maxUnit = m_Path.MaxUnit(m_PositionUnits);
                if (maxUnit > 0)
                {
                    float prev = m_Path.StandardizeUnit(m_PreviousPathPosition, m_PositionUnits);
                    float next = m_Path.StandardizeUnit(newPathPosition, m_PositionUnits);
                    if (m_Path.Looped && Mathf.Abs(next - prev) > maxUnit / 2)
                    {
                        if (next > prev)
                            prev += maxUnit;
                        else
                            prev -= maxUnit;
                    }
                    m_PreviousPathPosition = prev;
                    newPathPosition = next;
                }

                // Apply damping along the path direction
                float offset = m_PreviousPathPosition - newPathPosition;
                offset = Damper.Damp(offset, m_ZDamping, deltaTime);
                newPathPosition = m_PreviousPathPosition - offset;
            }
            m_PreviousPathPosition = newPathPosition;
            Quaternion newPathOrientation = m_Path.EvaluateOrientationAtUnit(newPathPosition, m_PositionUnits);

            //Debug.Log(newPathOrientation * Vector3.forward);

            Vector3 cameraDir = newPathOrientation * Vector3.forward;

            Vector3 posOffset = state.RawPosition - FollowTargetPosition;

            Vector3 pos = FollowTargetPosition;

            pos += -cameraDir * CameraDistance;

            state.RawPosition = pos + posOffset;

            //Debug.Log(posOffset);
        }
    }
}
