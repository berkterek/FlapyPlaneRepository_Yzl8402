using UnityEngine;

//button component'inin OnClicked event'ine birinci method atama yontemi bu yontemi button icin cok tercih etmem
public class ReturnMenuButton : MonoBehaviour
{
    /// <summary>
    /// Unity event'e atanmis method
    /// </summary>
    public void OnButtonClicked()
    {
        //TODO return menu
        GameManager.Instance.LoadMenu();
    }
}
