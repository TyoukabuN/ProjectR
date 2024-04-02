using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public partial class TPlayerEntity : StateMachineEntity, INumericalControl, IActionControl
    {
        public Action<ExtraAction> onActionControlBegin;
        public Action<ExtraAction> onActionControlUpdate;
        public Action<ExtraAction> onActionControlComplete;
        public Action<ExtraAction> onActionControlRemove;

        public void OnActionControlBegin(ExtraAction extraAction)
        {
            if (onActionControlBegin != null)
            {
                try
                {
                    onActionControlBegin.Invoke(extraAction);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            if (extraAction.actionType == (int)TPlayerActionType.CreateGhost)
                GhostSwitch(true);
            else if (extraAction.actionType == (int)TPlayerActionType.AddForce)
            {
                var evt = extraAction.tActionEvent;
                this.ExtraVelocityMapAdd(TPlayerActionType.AddForce.ToString(), evt.force, evt.duration, evt.damp);
            }
        }

        public void OnActionControlUpdate(ExtraAction extraAction)
        {
            if (onActionControlUpdate != null)
            {
                try
                {
                    onActionControlUpdate.Invoke(extraAction);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            var evt = extraAction.tActionEvent;
            if (extraAction.actionType == (int)TPlayerActionType.AttractBonus)
            {
                if (evt != null)
                {

                }
            }
            else if (extraAction.actionType == (int)TPlayerActionType.TweeningPosY)
            {
                if (evt != null)
                {
                    var pos = transform.position;
                    pos.y = Mathf.Lerp(pos.y, evt.floatValue, 1.0f - extraAction.counterNormalize);
                    transform.position = pos;
                }
            }

        }

        public void OnActionControlComplete(ExtraAction extraAction)
        {
            if (onActionControlComplete != null)
            {
                try
                {
                    onActionControlComplete.Invoke(extraAction);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            if (extraAction.actionType == (int)TPlayerActionType.CreateGhost)
                GhostSwitch(false);
        }
        public void OnActionControlRemove(ExtraAction extraAction)
        {
            if (onActionControlRemove != null)
            {
                try
                {
                    onActionControlRemove.Invoke(extraAction);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }


}
