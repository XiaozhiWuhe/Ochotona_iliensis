using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [Header("=== 引导图片资源 ===")]
    public Sprite[] tutorialImages;       //放入你需要展示的教学图片步骤
    public Image displayImage;            //场景中用于显示图片的 UI Image 组件

    [Header("=== 控制按钮引用 ===")]
    public Button leftArrowBtn;           //左箭头按钮
    public Button rightArrowBtn;          //右箭头按钮
    public Button closeBtn;               //开始游戏/关闭按钮

    private int currentIndex = 0;         //当前处于第几页

    void Start()
    {
        if (tutorialImages == null || tutorialImages.Length == 0)
        {
            Debug.LogError("没有配置任何教学图片！");
            gameObject.SetActive(false);
            return;
        }

        //绑定按钮事件
        leftArrowBtn.onClick.AddListener(PrevPage);
        rightArrowBtn.onClick.AddListener(NextPage);
        closeBtn.onClick.AddListener(CloseTutorial);

        //初始化显示第一页
        currentIndex = 0;
        UpdateUI();

        //打开引导时暂停游戏，给玩家充足的时间阅读
        Time.timeScale = 0f;
    }

    //根据当前页码动态更新图片和按钮状态
    void UpdateUI()
    {
        //更换当前的教学图片
        displayImage.sprite = tutorialImages[currentIndex];

        //判断是否是第一张图：如果是，隐藏左箭头，否则显示
        leftArrowBtn.gameObject.SetActive(currentIndex > 0);

        //判断是否是最后一张图
        if (currentIndex == tutorialImages.Length - 1)
        {
            //最后一张图：隐藏右箭头，显示关闭（开始游戏）按钮
            rightArrowBtn.gameObject.SetActive(false);
            closeBtn.gameObject.SetActive(true);
        }
        else
        {
            //不是最后一张图：显示右箭头，隐藏关闭按钮（必须看到最后才能关闭）
            rightArrowBtn.gameObject.SetActive(true);
            closeBtn.gameObject.SetActive(false);
        }
    }

    //下一页
    void NextPage()
    {
        if (currentIndex < tutorialImages.Length - 1)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    //上一页
    void PrevPage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }

    //关闭弹窗
    void CloseTutorial()
    {
        //恢复游戏时间流速，玩家开始奔跑
        Time.timeScale = 1f;
        gameObject.SetActive(false); // 隐藏自己
        Debug.Log("新手引导结束，游戏正式开始！");
    }
}