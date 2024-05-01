using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PJR
{
    public static class ItemFunc
    {
        //ÿ�����͵���ʵ��
        public static void ExcuteEntrance(int type,ItemEntity itemEntity, LogicEntity targetEntity)
        {
            if (type ==0)
            {
                Debug.Log("�޵���");
                if (!itemEntity.itembase.isMugen)
                {
                    if (itemEntity.config.CanRegenerateTimes -1 <=0)
                    {
                        itemEntity.Destroy();
                    }
                    else
                    {
                        CountDownToDo(itemEntity);
                    }
                }
                else
                {
                    CountDownToDo(itemEntity);
                }
            }
        }
        private static void CountDownToDo(ItemEntity itemEntity)
        {
            itemEntity.config.CanRegenerateTimes--;
            itemEntity.physEntity.avatar.SetActive(false);
            itemEntity.itembase.StartCoroutine(itemEntity.itembase.CountDown(() => { itemEntity.physEntity.avatar.SetActive(true); }));
        }
        #region ��չ
        
        
        #endregion
    }
}

