using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    //��������һЩ��������
    private bool _pauseMenuOn = false; //�Ƿ������ͣ�˵�

    //������� inventorybar ��Ϊ��inventory bar �� �ѳ������ͬʱ ����ͣ�˵�,Ҫ�����ѳ����Ǹ�����,ͬʱԭ���� slot �� �Ǹ����岻��ѡ��
    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagemant pauseMenuInventoryManagemant = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null; //Tab0-6
    [SerializeField] private Button[] menuButtons = null; //sekection Button 0-6

    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();
        //�ս�����Ϸ��ʱ��,��ͣ���治��ʾ������ֻ�ǲ���ʾ���������������ű������Ҫ�ص�)
        pauseMenu.SetActive(false);
    }

    //ÿһ֡��Ҫ����
    private void Update()
    {
        PauseMenu();
    }


    private void PauseMenu()
    {
        // һ�������� trigger,ese�����Ʋ˵��Ŀ��͹�,����״̬�°� esc ����أ��ر�״̬�°� esc ���㿪
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
        //���ٵ�ǰ�ϳ���item 
        uiInventoryBar.DestroyCurrentlyDraggedItems();
        //��� UIinventorybar �� ѡ�е���Ʒ 
        uiInventoryBar.ClearCurrentlySelectedItems();

        PauseMenuOn = true;
        Player.Instance.PlayerInputIsDisabled = true; // ������ͣ���棬��Ҫ�ر��������
        Time.timeScale = 0; //��ͣ�˾Ͳ�Ӧ�ü�����ʱ
        pauseMenu.SetActive(true);  // ������ͣ�˵�����ʾ

        // ���� ����������  garbage collector
        System.GC.Collect(); //����ͣ������Գû���  ��������

        //������ǰѡ�е� button
        HighlightButtonForSelectedTab();
    }

    private void HighlightButtonForSelectedTab()
    {
        for (int i = 0; i<menuTabs.Length; i++)
        {//���� tabs �б� , ��ѡ�е� table �� ûѡ�е� table ����Ӧ�Ĵ���
            if (menuTabs[i].activeSelf) //���� activeSelf��ȡֵ���ܸ�����״̬��Ӱ��
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

        //��ԭ button ��  normalColor ���޸� ,�� normalColor ����Ϊ pressedColor 
        colors.normalColor = colors.pressedColor;

        button.colors = colors;
    }

    private void SetButtonColorToInActive(Button button)
    {
        ColorBlock colors = button.colors;

        //��ԭ button ��  normalColor ���޸� ���� normalColor ����Ϊ disabledColor 
        colors.normalColor = colors.disabledColor;

        button.colors = colors;

    }
    /// <summary>
    /// ���� tabNum �� �л���ͣ�˵��ı��,�����������ص� SelectionTabButton �� button ����� On click �� 
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


