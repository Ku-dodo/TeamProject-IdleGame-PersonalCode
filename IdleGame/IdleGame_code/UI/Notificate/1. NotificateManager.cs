using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotificateManager
{
    #region 장비 알림 필터

    //잠금이 해제된 장비가 있는지 확인합니다.
    public List<UserItemData> CheckUnlockEquipment(List<UserItemData> itemList)
    {
        return itemList.Where(item => item.level > 1 || item.hasCount > 0).ToList();
    }


    public bool CheckReinforceNotiState(List<UserItemData> userItemDatas)
    {
        var notiList = userItemDatas.Where(data => data.hasCount >= Mathf.Min(data.level + 1, 15)).ToList(); //강화 가능한 아이템이 있는지 필터
        if (notiList.Count == 0)
        {
            return false;
        }
        else if (notiList.Count == 1
            && notiList[0].itemID == Manager.Data.WeaponInvenList.Last().itemID 
            && notiList[0].level >= 100) // 무기 아이템이 1개인 상태에서 && 해당 아이템이 마지막 무기이며 && 해당 아이템 레벨이 100 이상인 경우
        {
            return false;
        }
        else if (notiList.Count == 1
            && notiList[0].itemID == Manager.Data.ArmorInvenList.Last().itemID
            && notiList[0].level >= 100) // 방어구 아이템이 1개인 상태에서 && 해당 아이템이 마지막 방어구이며 && 해당 아이템 레벨이 100 이상인 경우
        {
            return false;
        }
        else if (notiList.Count == 2
            && (notiList[0].itemID == Manager.Data.WeaponInvenList.Last().itemID
            && notiList[0].level >= 100 | notiList[0].itemID == Manager.Data.ArmorInvenList.Last().itemID
            && notiList[0].level >= 100)) // 무기, 방어구 아이템 합쳐서 2개인 상태에서 && 해당 아이템이 마지막 아이템이며 && 해당 아이템 레벨이 100 이상인 경우
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CheckEquipmentWeaponBtnNotiState()
    {
        return CheckReinforceNotiState(Manager.Data.WeaponItemList) || !CheckRecommendItem(Manager.Data.WeaponItemList).equipped ? true : false;
    }

    public bool CheckEquipmentArmorBtnNotiState()
    {
        return CheckReinforceNotiState(Manager.Data.ArmorItemList) || !CheckRecommendItem(Manager.Data.ArmorItemList).equipped ? true : false;
    }

    public UserItemData CheckRecommendItem(List<UserItemData> itemList)
    {
        var recommendItem = CheckUnlockEquipment(itemList)
            .OrderBy(item => Manager.Data.ItemDataBase[item.itemID].EquipStat + item.level * Manager.Data.ItemDataBase[item.itemID].ReinforceEquip)
            .ToList();

        return recommendItem.Count == 0 ? null : recommendItem.Last();
    }

    #endregion

    #region 메인 화면 장비 버튼

    public delegate void EquipmentNotificate();
    public event EquipmentNotificate ActiveEquipmentBtnNoti;
    public event EquipmentNotificate InactiveEquipmentBtnNoti;

    //추천 장비 착용 여부를 체크
    public bool CheckEquipRecommendItem()
    {
        var _weaponRecommendItem = CheckRecommendItem(Manager.Data.WeaponItemList);
        var _armorRecommendItem = CheckRecommendItem(Manager.Data.ArmorItemList);
        // 장비가 아예 없을 경우 false 반환
        if (_weaponRecommendItem == null | _armorRecommendItem == null)
        {
            return false;
        }
        //아니라면 추천 장비를 착용하고 있는 지
        return CheckReinforceNotiState(Manager.Data.Inventory.UserItemData) | !_weaponRecommendItem.equipped | !_armorRecommendItem.equipped;
    }

    public void SetPlayerStateNoti()
    {
        if (CheckEquipRecommendItem() | CheckSkillReinforceState() | CheckFollowerReinforceState())
        {
            ActiveEquipmentBtnNoti?.Invoke();
        }
        else
        {
            InactiveEquipmentBtnNoti?.Invoke();
        }
    }

    #endregion

    #region 장비 타입 버튼 알림

    public delegate void EquipmentTypeNotificate();
    public event EquipmentTypeNotificate ActiveWeaponEquipmentBtnNoti;
    public event EquipmentTypeNotificate InactiveWeaponEquipmentBtnNoti;

    public void SetWeaponEquipmentNoti()
    {
        if (CheckEquipmentWeaponBtnNotiState())
        {
            ActiveWeaponEquipmentBtnNoti?.Invoke();
        }
        else
        {
            InactiveWeaponEquipmentBtnNoti?.Invoke();
        }
    }

    public event EquipmentTypeNotificate ActiveArmorEquipmentBtnNoti;
    public event EquipmentTypeNotificate InactiveArmorEquipmentBtnNoti;

    public void SetArmorEquipmentNoti()
    {
        if (CheckEquipmentArmorBtnNotiState())
        {
            ActiveArmorEquipmentBtnNoti?.Invoke();
        }
        else
        {
            InactiveArmorEquipmentBtnNoti?.Invoke();
        }
    }

    #endregion

    #region 일괄 강화 알림 관련

    public delegate void EquipReinforceNotificate();
    public event EquipReinforceNotificate ActiveReinforceWeaponItemNoti;
    public event EquipReinforceNotificate InactiveReinforceWeaponItemNoti;

    public event EquipReinforceNotificate ActiveReinforceArmorItemNoti;
    public event EquipReinforceNotificate InactiveReinforceArmorItemNoti;

    public void SetReinforceWeaponNoti()
    {
        if (CheckReinforceNotiState(Manager.Data.WeaponItemList))
        {
            ActiveReinforceWeaponItemNoti?.Invoke();
        }
        else
        {
            InactiveReinforceWeaponItemNoti?.Invoke();
        }
    }

    public void SetReinforceArmorNoti()
    {
        if (CheckReinforceNotiState(Manager.Data.ArmorItemList))
        {
            ActiveReinforceArmorItemNoti?.Invoke();
        }
        else
        {
            InactiveReinforceArmorItemNoti?.Invoke();
        }
    }

    #endregion

    #region 추천 장비 알림 관련

    public delegate void RecommendEquipItemNotificate();
    public RecommendEquipItemNotificate SetRecommendWeaponItemNoti;

    public void SetRecommendWeaponNoti()
    {
        SetRecommendWeaponItemNoti?.Invoke();
    }



    public RecommendEquipItemNotificate SetRecommendArmorItemNoti;

    public void SetRecommendArmorNoti()
    {
        SetRecommendArmorItemNoti?.Invoke();
    }



    public void ResetRecommendDelegateSubscribed()
    {
        SetRecommendWeaponItemNoti = null;
        SetRecommendArmorItemNoti = null;
    }

    #endregion

    #region 스킬 강화 알림
    public delegate void SkillReinforceNotificate();
    public event SkillReinforceNotificate ActiveReinforceSkillNoti;
    public event SkillReinforceNotificate InactiveReinforceSkillNoti;

    private bool CheckSkillReinforceState()
    {
        var notiList = Manager.Data.UserSkillData.UserInvenSkill.Where(data => data.hasCount >= Mathf.Min(data.level + 1, 15)).ToList();
        if (notiList.Count == 0)
        {
            return false;
        }
        else if (notiList.Count == 1
            & notiList[0].itemID == Manager.Data.SkillInvenList.Last().itemID & notiList[0].level >= 100)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SetReinforceSkillNoti()
    {
        if (CheckSkillReinforceState())
        {
            ActiveReinforceSkillNoti?.Invoke();
        }
        else
        {
            InactiveReinforceSkillNoti?.Invoke();
        }
    }

    #endregion

    #region 동료 강화 알림
    public delegate void FollowerReinforceNotificate();
    public event FollowerReinforceNotificate ActiveReinforceFollowerNoti;
    public event FollowerReinforceNotificate InactiveReinforceFollowerNoti;

    private bool CheckFollowerReinforceState()
    {
        var notiList = Manager.Data.FollowerData.UserInvenFollower.Where(data => data.hasCount >= Mathf.Min(data.level + 1, 15)).ToList();
        if (notiList.Count == 0)
        {
            return false;
        }
        else if (notiList.Count == 1
            & notiList[0].itemID == Manager.Data.FollowerInvenList.Last().itemID & notiList[0].level >= 100)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SetReinforceFollowerNoti()
    {
        if (CheckFollowerReinforceState())
        {
            ActiveReinforceFollowerNoti?.Invoke();
        }
        else
        {
            InactiveReinforceFollowerNoti?.Invoke();
        }
    }

    #endregion
}