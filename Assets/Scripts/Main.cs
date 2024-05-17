using UnityEngine;
using app;

public class Main : MonoBehaviour
{
    void Start()
    {
        initCore();
        LayerManager.ins.init(this.gameObject);
        ModuleManager.ins.init();
        doStart();
    }

    private void initCore()
    {
        //初始化计时器
        var timer = new GameObject("Timer");
        timer.AddComponent<FairyGUI.TimersEngine>();
        timer.hideFlags = HideFlags.HideInHierarchy;
        timer.SetActive(true);
        DontDestroyOnLoad(timer);
    }

    private async void doStart()
    {
        await ModuleManager.ins.initSkin("common");
        ModuleManager.ins.open(ModuleId.Login);
    }
}
