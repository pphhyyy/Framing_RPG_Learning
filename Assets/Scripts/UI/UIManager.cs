using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    //管理器的一些基础属性
    private bool _pauseMenuOn = false; //是否打开了暂停菜单

    //这里加入 inventorybar 是为从inventory bar 中 脱出物体的同时 打开暂停菜单,要销毁脱出的那个物体,同时原来的 slot 中 那个物体不再选中
    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagemant pauseMenuInventoryManagemant = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null; //Tab0-6
    [SerializeField] private Button[] menuButtons = null; //sekection Button 0-6

    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();
        //刚进入游戏的时候,暂停界面不显示（这里只是不显示，但不代表整个脚本组件都要关掉)
        pauseMenu.SetActive(false);
    }

    //每一帧都要更新
    private void Update()
    {
        PauseMenu();
    }


    private void PauseMenu()
    {
        // 一个开关器 trigger,ese键控制菜单的开和关,开启状态下按 esc 就算关，关闭状态下按 esc 就算开
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }

    }

    public void DisablePauseMenu()
    {
        pauseMenuInventoryManagemant.DestroyyCurrentlyDraggedItems();

        PauseMenuOn = false;
        Player.Instance.PlayerInputIsDisabled = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    private void EnablePauseMenu()
    {
        //销毁当前拖出的item 
        uiInventoryBar.DestroyCurrentlyDraggedItems();
        //清除 UIinventorybar 中 选中的物品 
        uiInventoryBar.ClearCurrentlySelectedItems();

        PauseMenuOn = true;
        Player.Instance.PlayerInputIsDisabled = true; // 进入暂停界面，就要关闭玩家输入
        Time.timeScale = 0; //暂停了就不应该继续计时
        pauseMenu.SetActive(true);  // 激活暂停菜单的显示

        // 触发 垃圾回收器  garbage collector
        System.GC.Collect(); //在暂停界面可以趁机做  垃圾清理

        //高亮当前选中的 button
        HighlightButtonForSelectedTab();
    }

    private void HighlightButtonForSelectedTab()
    {
        for (int i = 0; i<menuTabs.Length; i++)
        {//遍历 tabs 列表 , 对选中的 table 和 没选中的 table 做相应的处理
            if (menuTabs[i].activeSelf) //这里 activeSelf的取值不受父对象状态的影响
            { 
                SetButtonColorToActive(menuButtons[i]);
            }    
            else
            {
                SetButtonColorToInActive(menuButtons[i]);
            }
        }
    }


    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;

        //对原 button 的  normalColor 做修改 ,将 normalColor 设置为 pressedColor 
        colors.normalColor = colors.pressedColor;

        button.colors = colors;
    }

    private void SetButtonColorToInActive(Button button)
    {
        ColorBlock colors = button.colors;

        //对原 button 的  normalColor 做修改 ，将 normalColor 设置为 disabledColor 
        colors.normalColor = colors.disabledColor;

        button.colors = colors;

    }
    /// <summary>
    /// 根据 tabNum 来 切换暂停菜单的表格,这个函数会挂载到 SelectionTabButton 的 button 组件的 On click 上 
    /// </summary>
    /// <param name="tabNum"></param>

    public void SwitchPauseMenuTab(int tabNum) 
    {
        
        for (int i = 0; i <menuTabs.Length;i++)
        {
            if (i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);
            }
        }

        HighlightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}


