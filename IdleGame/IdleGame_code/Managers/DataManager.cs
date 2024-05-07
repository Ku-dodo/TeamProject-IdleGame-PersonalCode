using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Linq;

public class DataManager
{
    public GameUserProfile Profile { get; private set; }

    public InventoryData Inventory { get; private set; }
    public List<UserItemData> WeaponInvenList = new();
    public List<UserItemData> ArmorInvenList = new();
    public Dictionary<string, UserItemData> WeaponInvenDictionary = new();
    public Dictionary<string, UserItemData> ArmorInvenDictionary = new();

    public UserSkillData UserSkillData { get; private set; }
    public List<UserInvenSkillData> SkillInvenList = new();
    public Dictionary<string, UserInvenSkillData> SkillInvenDictionary = new();

    public UserFollowerData FollowerData { get; private set; }
    public List<UserInvenFollowerData> FollowerInvenList = new();
    public Dictionary<string, UserInvenFollowerData> FollowerInvenDictionary = new();

    #region Create

    public void CreateUserEquipment()
    {
        var jsonData = Manager.Asset.GetText("UserTableEquipment");
        Inventory = JsonUtility.FromJson<InventoryData>(jsonData);

        SaveToUserEquipment();
    }

    public void CreateUserSkill()
    {
        var jsonData = Manager.Asset.GetText("UserTableSkill");
        UserSkillData = JsonUtility.FromJson<UserSkillData>(jsonData);

        SaveToUserSkill();
    }

    public void CreateUserFollower()
    {
        var jsonData = Manager.Asset.GetText("UserTableFollower");
        FollowerData = JsonUtility.FromJson<UserFollowerData>(jsonData);

        SaveToUserFollower();
    }

    #endregion

    #region Load

    public void Load()
    {
        LoadFromUserProfile();
        LoadFromUserEquipment();
        LoadFromUserSkill();
        LoadFromUserFollower();

        topScene = GameObject.FindObjectOfType<UITopScene>();
        topScene.DataLoadFinished();
    }

    #endregion


    #region UserData

    public void LoadFromUserProfile(string fileName = "game_user.dat")
    {
        string filePath = $"{Application.persistentDataPath}/{fileName}";
        if (!File.Exists(filePath)) { CreateUserLocalFile(); return; }
        string jsonRaw = File.ReadAllText(filePath);
        Profile = JsonConvert.DeserializeObject<GameUserProfile>(jsonRaw);
    }

    public void LoadFromUserEquipment(string fileName = "game_equipment.dat")
    {
        string filePath = $"{Application.persistentDataPath}/{fileName}";
        if (!File.Exists(filePath)) { CreateUserEquipment(); }
        else
        {
            string jsonRaw = File.ReadAllText(filePath);
            Inventory = JsonConvert.DeserializeObject<InventoryData>(jsonRaw);
        }

        foreach (var item in Inventory.UserItemData)
        {
            if (item.itemID[0] == 'W')
            {
                WeaponInvenDictionary.Add(item.itemID, item);
                WeaponInvenList.Add(item);
            }

            else
            {
                ArmorInvenDictionary.Add(item.itemID, item);
                ArmorInvenList.Add(item);
            }
        }
  
        int _levelOverCount = 0;
        foreach (var item in WeaponInvenList)
        {
            item.hasCount += _levelOverCount;

            if (item.level > 100)
            {
                _levelOverCount = item.level - 100;
                item.level = 100;
            }
            else
            {
                _levelOverCount = 0;
            }
        }

        _levelOverCount = 0;
        foreach (var item in ArmorInvenList)
        {
            item.hasCount += _levelOverCount;

            if (item.level > 100)
            {
                _levelOverCount = item.level - 100;
                item.level = 100;
            }
            else
            {
                _levelOverCount = 0;
            }
        }
    }

    public void LoadFromUserSkill(string fileName = "game_skill.dat")
    {
        string filePath = $"{Application.persistentDataPath}/{fileName}";
        if (!File.Exists(filePath)) { CreateUserSkill(); }
        else
        {
            string jsonRaw = File.ReadAllText(filePath);
            UserSkillData = JsonConvert.DeserializeObject<UserSkillData>(jsonRaw);
        }

        int _levelOverCount = 0;

        foreach (var item in UserSkillData.UserInvenSkill)
        {
            SkillInvenDictionary.Add(item.itemID, item);
            SkillInvenList.Add(item);

            item.hasCount += _levelOverCount;

            if (item.level > 100)
            {
                _levelOverCount = item.level - 100;
                item.level = 100;
            }
            else
            {
                _levelOverCount = 0;
            }
        }
    }

    public void LoadFromUserFollower(string fileName = "game_follower.dat")
    {
        string filePath = $"{Application.persistentDataPath}/{fileName}";
        if (!File.Exists(filePath)) { CreateUserFollower(); }
        else
        {
            string jsonRaw = File.ReadAllText(filePath);
            FollowerData = JsonConvert.DeserializeObject<UserFollowerData>(jsonRaw);
        }

        int _levelOverCount = 0;

        foreach (var item in FollowerData.UserInvenFollower)
        {
            FollowerInvenDictionary.Add(item.itemID, item);
            FollowerInvenList.Add(item);

            item.hasCount += _levelOverCount;

            if (item.level > 100)
            {
                _levelOverCount = item.level - 100;
                item.level = 100;
            }
            else
            {
                _levelOverCount = 0;
            }
        }
    }

    #endregion

    #region DataBase

    private ItemContainerBlueprint _itemData;
    private Dictionary<string, ItemBlueprint> _itemDataBase = new();

    public Dictionary<string, ItemBlueprint> ItemDataBase => _itemDataBase;

    public void ParseItemData()
    {
        _itemData = Manager.Asset.GetBlueprint("ItemDataContainer") as ItemContainerBlueprint;
        foreach (var itemData in _itemData.itemDatas)
        {
            _itemDataBase.Add(itemData.ItemID, itemData);
        }
    }

    public List<UserItemData> WeaponItemList { get; private set; }
    public List<UserItemData> ArmorItemList { get; private set; }

    public void Initialize()
    {
        WeaponItemList = Inventory.UserItemData.Where(ItemData => ItemData.itemID[0] == 'W').ToList();
        ArmorItemList = Inventory.UserItemData.Where(ItemData => ItemData.itemID[0] == 'A').ToList();
    }

    public UserItemData SearchItem(string itemID)
    {
        List<UserItemData> pickItem = Inventory.UserItemData.Where(itemData => itemData.itemID == itemID).ToList();
        return pickItem[0];
    }

    private SkillContainerBlueprint _skillData;

    private Dictionary<string, SkillBlueprint> _skillDataDictionary = new();
    public Dictionary<string, SkillBlueprint> SkillDataDictionary => _skillDataDictionary;

    public void ParseSkillData()
    {
        _skillData = Manager.Asset.GetBlueprint("SkillDataContainer") as SkillContainerBlueprint;

        foreach (var skillData in _skillData.skillDatas)
        {
            _skillDataDictionary.Add(skillData.ItemID, skillData);
        }
    }

    public UserInvenSkillData SearchSkill(string id)
    {
        return Manager.Data.SkillInvenDictionary[id];
    }

    public void InitDataBase()
    {
        ParseItemData();
        ParseSkillData();
    }

    #endregion
}

#region UserInventoryData

[System.Serializable]
public class InventoryData
{
    public List<UserItemData> UserItemData;
}

[System.Serializable]
public class UserItemData
{
    public string itemID;
    public int level;
    public int hasCount;
    public bool equipped;

    public UserItemData(string ItemID, int Level, int HasCount, bool Equiped)
    {
        itemID = ItemID;
        level = Level;
        hasCount = HasCount;
        equipped = Equiped;
    }
}

#endregion

#region UserSkillData

[System.Serializable]
public class UserSkillData
{
    public List<UserEquipSkillData> UserEquipSkill;
    public List<UserInvenSkillData> UserInvenSkill;
}

[System.Serializable]
public class UserEquipSkillData
{
    public string itemID;
}

[System.Serializable]
public class UserInvenSkillData
{
    public string itemID;
    public int level;
    public int hasCount;
    public bool equipped;
}

#endregion