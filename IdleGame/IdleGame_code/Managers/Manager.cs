using UnityEngine;

public class Manager : MonoBehaviour
{
    #region Singleton

    //private static Manager instance;
    //private static bool initialized;
    //public static Manager Instance
    //{
    //    get
    //    {
    //        if (!initialized)
    //        {
    //            initialized = true;

    //            GameObject obj = GameObject.Find("@Manager");
    //            if (obj == null)
    //            {
    //                obj = new() { name = "@Manager" };
    //                obj.AddComponent<Manager>();
    //                DontDestroyOnLoad(obj);
    //                instance = obj.GetComponent<Manager>();
    //            }
    //        }
    //        return instance;
    //    }
    //}

    #endregion

    #region Manage

    private readonly DataManager data = new();
    private readonly InventoryPresenter inventory = new();
    private readonly NotificateManager notificate = new();
    private readonly SkillDataPresenter skillData = new();
    private readonly SystemAlertDataManager systemAlert = new();

    public static DataManager Data => Instance != null ? Instance.data : null;
    public static InventoryPresenter Inventory => Instance != null ? Instance.inventory : null;
    public static NotificateManager Notificate => Instance != null ? Instance.notificate : null;
    public static SkillDataPresenter SkillData => Instance != null ? Instance.skillData : null;
    public static SystemAlertDataManager SysAlert => Instance != null ? Instance.systemAlert : null;
    #endregion
}