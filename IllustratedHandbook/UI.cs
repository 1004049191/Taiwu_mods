using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace IllustratedHandbook
{
    public class IllustratedHandbookUI : MonoBehaviour
    {
        DateFile dataInstance;

        // �Ƿ���ʾ����
        public static bool mainCavasActive = true;

        GameObject mainCanvas; // ����
        GameObject mainPanel; // �����
        GameObject handbookPanel; // �������
        GameObject loadedPanel; // �������
        GameObject loadingPanel; // ��ʼ�����
        GameObject filterPanel; // �������
        GameObject itemTypePanel; // ������� -> ��Ʒ�������
        GameObject itemLevelPanel; // ������� -> ��ƷƷ�����
        GameObject filterTypeCurrent; // ������� -> ��ǰ��������
        GameObject filterLevelCurrent;// ������� -> ��ǰƷ������
        GameObject loadingTextPanel; // ��ʼ���������
        GameObject itemPanel; // ������� -> ��Ʒ���
        GameObject titlePanel; // ����
        EventTrigger titlePanelOndrag; // �϶��¼�
        GameObject titleText; // ���� -> ��������
        GameObject tipText; // ��ʼ����� -> ��ʾ
        GameObject aboutText; // ��ʼ��������� -> ����
        Text itemPageText;  // ������� -> ��ǰҳ
        GameObject NGAButton;
        GameObject GithubButton;
        GameObject itemGrid; // ��ƷGrid
        GameObject prePageButton; // ������� -> ��һҳ����
        GameObject nextPageButton; // ������� -> ��һҳ����
        List<GameObject> itemSlots = new List<GameObject>(); // 54����Ʒ��

        Sprite[] itemBack; // ��Ʒ�۱���ͼƬ
        Sprite[] itemIcon; // ��Ʒͼ��

        Vector3 offsetPostion; // ���Լ�¼��ק�¼�ƫ��
        Vector3 offsetPostionLoding;

        int selectedLevel = 0; // ѡ�����ƷƷ��
        int selectedType = 0;  // ѡ�����Ʒ����

        int selectedLevelUnsaved = 0;
        int selectedTypeUnsaved = 0;

        string itemType = "��";
        string[] itemLevel = { "ȫ��|#FFF", "��Ʒ|#8E8E8EFF", "��Ʒ|#FBFBFBFF", "��Ʒ|#6DB75FFF", "��Ʒ|#8FBAE7FF", "��Ʒ|#63CED0FF", "��Ʒ|#AE5AC8FF", "��Ʒ|#E3C66DFF", "��Ʒ|#F28234FF", "һƷ|#E4504DFF" };
        Dictionary<int, Dictionary<int, string>> itemList = new Dictionary<int, Dictionary<int, string>>();
        int itemSlotPage = 0; // ��ǰ����ҳ

        // �Ƿ񵽴�����һҳ ��������itemSlots�Ŀ���
        bool finalPageReached = false;
        // �ж���Ϸ�Ƿ����
        private bool gameLoaded = false;
        private bool gameExited = false;

        void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            DontDestroyOnLoad(this);
            InitUI();
        }

        void Update()
        {

            if (!gameLoaded)
            {
                if (DateFile.instance != null && ActorMenu.instance != null)
                {
                    gameLoaded = true;
                    OnGameLoaded();
                }
            }

            // ��¼���λ��ƫ��ֵ
            if (Input.GetMouseButtonDown(0))
            {
                // �����Ϸδ���� ��ôUI��ȾģʽΪScreenSpaceOverlay, ��ʱֱ��ʹ����Ļλ�ü���
                // �����Ϸ�Ѽ��� ��ôUI��ͨ��MainCamera��Ⱦ, ��ʱ��Ҫ����Ļλ�û��������λ��
                if (gameLoaded && !gameExited)
                    offsetPostion = mainPanel.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainPanel.transform.position.z));
                else
                    offsetPostionLoding = mainPanel.transform.position - Input.mousePosition;
            }

            if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyUp(KeyCode.F11))
                {
                    if (mainCavasActive)
                    {
                        mainCavasActive = false;
                        mainCanvas.SetActive(false);
                    }
                    else if (!mainCavasActive)
                    {
                        mainCavasActive = true;
                        mainCanvas.SetActive(true);
                    }
                }
            }
        }

        // �����ж��Ƿ񷵻������˵�
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "1_StartMenu")
            {
                if (gameLoaded)
                {
                    gameExited = true;
                    mainCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                    loadedPanel.SetActive(false);
                    filterPanel.SetActive(false);
                    loadingPanel.SetActive(true);
                }
            }
            else if (scene.name == "3_WorldMap")
            {
                if (gameExited)
                {
                    gameExited = false;
                    mainCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                    mainCanvas.GetComponent<Canvas>().worldCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
                    loadedPanel.SetActive(true);
                    filterPanel.SetActive(false);
                    loadingPanel.SetActive(false);
                }
            }
        }
        void OnGameLoaded()
        {
            dataInstance = DateFile.instance;
            itemType = DateFile.instance.massageDate[301][0];
            itemIcon = GetSprites.instance.itemSprites;
            itemBack = GetSprites.instance.itemBackSprites;

            itemList = dataInstance.presetitemDate;

            // ��Ϸ���غ���Ⱦģʽ��ΪScreenSpaceCamera������MainCamera
            mainCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            mainCanvas.GetComponent<Canvas>().worldCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            mainCanvas.GetComponent<Canvas>().sortingLayerName = "GUI";
            mainCanvas.GetComponent<Canvas>().sortingOrder = 601;  // ShowTips �� sortingorder Ϊ1000
            mainCanvas.transform.position = new Vector3(0, 0, 10);
            loadingPanel.SetActive(false);

            // �ڹ�����崴����Ʒ���ť
            string[] itemTypes = itemType.Split('|');
            for (int i = 0; i < itemTypes.Length; i++)
            {
                var itemTypeButton = CreateButton("ItemTypeButton|" + itemTypes[i], itemTypePanel, itemTypes[i], Color.white, "Graphics/BaseUI/GUI_Base", Color.white, v2(0, 1), v2(0, 1), v2(100, 30), v3(100, -30, 0));

                int selectedIndex = i;

                itemTypeButton.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        selectedTypeUnsaved = selectedIndex;
                        filterTypeCurrent.GetComponent<Text>().text = itemTypes[selectedTypeUnsaved];
                    }
                );
            }

            // ����Ʒ�����˰�ť
            for (int i = 0; i < itemLevel.Length; i++)
            {
                string[] levelSplit = itemLevel[i].Split('|');
                Color textColor = Color.white;

                // ת��HEX��ɫ��RGB��ɫ
                ColorUtility.TryParseHtmlString(levelSplit[1], out textColor);

                var itemLevelButton = CreateButton("ItemLevelButton|" + i, itemLevelPanel, levelSplit[0], textColor, "Graphics/BaseUI/GUI_Base", Color.white, v2(0, 1), v2(0, 1), v2(100, 30), v3(100, -30, 0));

                int selectedIndex = i;

                itemLevelButton.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        selectedLevelUnsaved = selectedIndex;
                        filterLevelCurrent.GetComponent<Text>().text = itemLevel[selectedIndex].Split('|')[0];
                    }
                );
            }
            // ��ʼ����Ʒ
            DrawItems();

            loadedPanel.SetActive(true);

            Debug.Log("OnLoadedBreakPoint");
        }


        void InitUI()
        {
            // ��Canvs ����
            #region MainCanvas
            mainCanvas = new GameObject("IllustratedHandbookCanvas", typeof(Canvas), typeof(GraphicRaycaster));
#if UNITY_EDITOR
            mainCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            mainCanvas.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
#else
            mainCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
#endif
            // mainCanvas.GetComponent<Canvas>().sortingLayerID = 1000;
            mainCanvas.GetComponent<Canvas>().sortingLayerName = "GUI";
            mainCanvas.GetComponent<Canvas>().sortingOrder = 998;  // ShowTips �� sortingorder Ϊ1000
            mainCanvas.transform.position = new Vector3(0, 0, 0);
            DontDestroyOnLoad(mainCanvas);
            mainPanel = new GameObject("MainPanel", typeof(Image));

            Main.Logger.Log("mainPanelsprite");
            mainPanel.GetComponent<Image>().sprite = Resources.Load("Graphics/BaseUI/GUI_Window_Big_Black_NoColor", typeof(Sprite)) as Sprite;
            Main.Logger.Log("mainPanelcolor");
            mainPanel.GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            mainPanel.transform.SetParent(mainCanvas.transform);
            var mainPanelTransform = mainPanel.GetComponent<RectTransform>();
            mainPanelTransform.localScale = v3(1, 1, 1);
            mainPanelTransform.anchorMin = v2(0.5f);
            mainPanelTransform.anchorMax = v2(0.5f);
            mainPanelTransform.sizeDelta = v2(700f, 600f);
            mainPanelTransform.anchoredPosition3D = v3(0);
            #endregion

            // �����
            #region MainPanel
            handbookPanel = new GameObject("IllustratedHandbookPanel", typeof(Image));
            handbookPanel.GetComponent<Image>().sprite = null;
            handbookPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            handbookPanel.transform.SetParent(mainPanelTransform);
            var handbookPanelPanelTransform = handbookPanel.GetComponent<RectTransform>();
            handbookPanelPanelTransform.localScale = v3(1, 1, 1);
            handbookPanelPanelTransform.anchorMin = v2(0);
            handbookPanelPanelTransform.anchorMax = v2(1);
            handbookPanelPanelTransform.offsetMin = v2(10);
            handbookPanelPanelTransform.offsetMax = v2(-10, -25);
            handbookPanelPanelTransform.anchoredPosition3D = v3(0);

            titlePanel = new GameObject("TitlePanel", typeof(Image));
            titlePanel.GetComponent<Image>().sprite = null;
            titlePanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            titlePanel.transform.SetParent(handbookPanelPanelTransform);
            var titlePanelTransform = titlePanel.GetComponent<RectTransform>();
            titlePanelTransform.localScale = v3(1, 1, 1);
            titlePanelTransform.anchorMin = v2(0.5f, 1);
            titlePanelTransform.anchorMax = v2(0.5f, 1);
            titlePanelTransform.offsetMin = v2(0);
            titlePanelTransform.offsetMax = v2(0);
            titlePanelTransform.sizeDelta = v2(680, 50);
            titlePanelTransform.anchoredPosition3D = v3(0, -25, 0);

            titlePanel.AddComponent(typeof(EventTrigger));
            titlePanelOndrag = titlePanel.GetComponent<EventTrigger>();
            EventTrigger.Entry eventEntry = new EventTrigger.Entry();
            eventEntry.eventID = EventTriggerType.Drag;
            eventEntry.callback.AddListener((data) =>
            {
                OnDragDelegate((PointerEventData)data);
            });
            titlePanelOndrag.triggers.Add(eventEntry);


            titleText = CreateText("TitleText", titlePanel, "<b>ͼ��/�����Ʒ 1.0.0</b>", Color.white, 27, v2(0.5f, 1), v2(0.5f, 1), v2(680, 30), v2(0, -25));
            #endregion


            // ���������
            #region LoadingPanel
            loadingPanel = new GameObject("LoadingPanel", typeof(Image));
            loadingPanel.GetComponent<Image>().sprite = null;
            loadingPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            loadingPanel.transform.SetParent(handbookPanelPanelTransform);
            var loadingPanelTransform = loadingPanel.GetComponent<RectTransform>();
            loadingPanelTransform.localScale = v3(1, 1, 1);
            loadingPanelTransform.anchorMin = v2(0);
            loadingPanelTransform.anchorMax = v2(1);
            loadingPanelTransform.offsetMin = v2(0);
            loadingPanelTransform.offsetMax = v2(0, -50);

            // ��������������
            loadingTextPanel = new GameObject("LoadingTextPanel", typeof(Image));
            loadingTextPanel.GetComponent<Image>().sprite = Resources.Load("Graphics/BaseUI/GUI_BarBack", typeof(Sprite)) as Sprite;
            loadingTextPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 44);
            loadingTextPanel.transform.SetParent(loadingPanelTransform);
            var loadingTextPanelTransform = loadingTextPanel.GetComponent<RectTransform>();
            loadingTextPanelTransform.localScale = v3(1, 1, 1);
            loadingTextPanelTransform.anchorMin = v2(0);
            loadingTextPanelTransform.anchorMax = v2(1);
            loadingTextPanelTransform.offsetMin = v2(20, 130);
            loadingTextPanelTransform.offsetMax = v2(-20, 0);
            // loadingTextPanelTransform.anchoredPosition3D = v3(0);

            tipText = CreateText("TipText", loadingTextPanel,
             "��Ϸδ��ʼ ��δ��������\n\n���ȿ�ʼ��Ϸ~\n\n\n" +
             "<size=16><color=white><b>Tips:</b>\n Ctrl + F11���ش���\n<b>�϶�����</b>�����ƶ�����Ŷ\n" +
             "��ȡ�鼮�������ҳ���޷��鿴��֯\n" +
             "��ȡ��Ʒ�������´򿪱����ſ��ĵ�\n�Ƽ���ϡ������鼮��ʾ��MODʹ��\n<b>�����޴�</b> ��<b>�Ծ�</b>ƽ����Ϸ����</color></size>",
              new Color32(0, 255, 213, 255), 28, v2(0.5f), v2(0.5f), v2(600, 400), v2(0)
            );

            aboutText = CreateText("AboutText", loadingPanel, "By: yyuueexxiinngg", Color.white, 14, v2(0), v2(0), v2(160, 30), v2(128.8f, 13.9f));
            aboutText.GetComponent<Text>().alignment = TextAnchor.UpperLeft;

            NGAButton = CreateButton("NGAButton", loadingPanel, "��˴򿪱�MOD NGA������ַ", Color.white, "Graphics/BaseUI/GUI_Base", Color.black, v2(0.5f, 0), v2(0.5f, 0), v2(500, 31), v2(0, 100));
            NGAButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                Application.OpenURL("https://nga.178.com/read.php?tid=15239374");
            });

            GithubButton = CreateButton("GithubBotton", loadingPanel, "��˴�MOD��Դ��ĿGithub��ַ", Color.white, "Graphics/BaseUI/GUI_Base", Color.black, v2(0.5f, 0), v2(0.5f, 0), v2(500, 31), v2(0, 55));
            GithubButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                Application.OpenURL("https://github.com/yyuueexxiinngg/Taiwu_mods");
            });
            #endregion

            // �������
            #region FilterPanel
            filterPanel = new GameObject("FilterPanel", typeof(Image));
            filterPanel.GetComponent<Image>().sprite = null;
            filterPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            filterPanel.transform.SetParent(handbookPanelPanelTransform);
            var filterPanelTransform = filterPanel.GetComponent<RectTransform>();
            filterPanelTransform.localScale = v3(1, 1, 1);
            filterPanelTransform.anchorMin = v2(0);
            filterPanelTransform.anchorMax = v2(1);
            filterPanelTransform.offsetMin = v2(50, 50);
            filterPanelTransform.offsetMax = v2(-50, -50);

            itemTypePanel = new GameObject("ItemTypeGrid", typeof(Image), typeof(GridLayoutGroup));
            itemTypePanel.GetComponent<Image>().sprite = null;
            itemTypePanel.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
            itemTypePanel.transform.SetParent(filterPanelTransform);
            var itemTypePanelTransform = itemTypePanel.GetComponent<RectTransform>();
            itemTypePanelTransform.localScale = v3(1, 1, 1);
            itemTypePanelTransform.anchorMin = v2(0);
            itemTypePanelTransform.anchorMax = v2(1);
            itemTypePanelTransform.offsetMin = v2(0, 100);
            itemTypePanelTransform.offsetMax = v2(0, -50);

            itemTypePanel.GetComponent<GridLayoutGroup>().cellSize = v2(110, 30);
            itemTypePanel.GetComponent<GridLayoutGroup>().spacing = v2(6, 3);

            CreateText("FilterTypeLabel", filterPanel, "��ǰѡ������:", Color.white, 14, v2(0, 1), v2(0, 1), v2(160, 30), v3(50, -30, 0));
            filterTypeCurrent = CreateText("FilterTypeCurrent", filterPanel, "δ��ʼ��", Color.white, 20, v2(0, 1), v2(0, 1), v2(160, 30), v3(120, -30, 0));


            itemLevelPanel = new GameObject("ItemLevelGrid", typeof(Image), typeof(GridLayoutGroup));
            itemLevelPanel.GetComponent<Image>().sprite = null;
            itemLevelPanel.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
            itemLevelPanel.transform.SetParent(filterPanelTransform);
            var itemLevelPanelTransform = itemLevelPanel.GetComponent<RectTransform>();
            itemLevelPanelTransform.localScale = v3(1, 1, 1);
            itemLevelPanelTransform.anchorMin = v2(0);
            itemLevelPanelTransform.anchorMax = v2(1);
            itemLevelPanelTransform.offsetMin = v2(0);
            itemLevelPanelTransform.offsetMax = v2(0, -370);

            itemLevelPanel.GetComponent<GridLayoutGroup>().cellSize = v2(140, 30);
            itemLevelPanel.GetComponent<GridLayoutGroup>().spacing = v2(6, 3);

            CreateText("FilterLevelLabel", filterPanel, "��ǰѡ��Ʒ��:", Color.white, 14, v2(0, 1), v2(0, 1), v2(160, 30), v3(300, -30, 0));
            filterLevelCurrent = CreateText("FilterLevelCurrent", filterPanel, itemLevel[selectedLevel].Split('|')[0], Color.white, 20, v2(0, 1), v2(0, 1), v2(160, 30), v3(380, -30, 0));

            var filterCancelBtn = CreateButton("FilterCancelBtn", filterPanel, "ȡ��", Color.white, "Graphics/BaseUI/GUI_ValuBack", Color.white, v2(0), v2(0), v2(200, 30), v2(160, -25));
            var filterRefreshBtn = CreateButton("FilterRefreshBtn", filterPanel, "ˢ����Ʒ�б�", Color.white, "Graphics/BaseUI/GUI_ValuBack", Color.white, v2(1, 0), v2(1, 0), v2(200, 30), v2(-160, -25));

            filterCancelBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                filterPanel.SetActive(false);
                loadedPanel.SetActive(true);
            });

            filterRefreshBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedType = selectedTypeUnsaved;
                selectedLevel = selectedLevelUnsaved;
                // �����Ʒ�б�
                itemList = new Dictionary<int, Dictionary<int, string>>();

                if (selectedType != 0) // ���ѡ����ȫ��
                {
                    foreach (var item in dataInstance.presetitemDate)
                    {
                        // item.Value[5]��Ϊ��Ʒ���� item.Value[8]��Ϊ��ƷƷ��
                        if (int.Parse(item.Value[5]) == selectedType && (selectedLevel == 0 || int.Parse(item.Value[8]) == selectedLevel))
                        {
                            itemList[item.Key] = item.Value;
                        }
                    }
                }
                else // �����ȫ��
                {
                    if (selectedLevel == 0) //�����Ʒ����ȫ��
                    {
                        itemList = dataInstance.presetitemDate;
                    }
                    else
                    {
                        Main.Logger.Log("selectedLevel: " + selectedLevel);
                        foreach (var item in dataInstance.presetitemDate)
                        {
                            if (int.Parse(item.Value[8]) == selectedLevel)
                            {
                                itemList[item.Key] = item.Value;
                            }
                        }
                    }
                }
                Main.Logger.Log("�������");
                DrawItems();
                filterPanel.SetActive(false);
                loadedPanel.SetActive(true);
            });


            #endregion


            // ������ɺ����
            #region LoadedPanel
            loadedPanel = new GameObject("LoadedPanel", typeof(Image));
            loadedPanel.GetComponent<Image>().sprite = null;
            loadedPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            loadedPanel.transform.SetParent(handbookPanel.transform);
            var loadedPanelTransform = loadedPanel.GetComponent<RectTransform>();
            loadedPanelTransform.localScale = v3(1);
            loadedPanelTransform.anchorMin = v2(0);
            loadedPanelTransform.anchorMax = v2(1);
            loadedPanelTransform.offsetMin = v2(0, 50);
            loadedPanelTransform.offsetMax = v2(0, -50);


            itemPanel = new GameObject("ItemPanel", typeof(Image));
            itemPanel.GetComponent<Image>().sprite = Resources.Load("Graphics/BaseUI/GUI_BarBack", typeof(Sprite)) as Sprite;
            itemPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 44);
            itemPanel.transform.SetParent(loadedPanelTransform);
            var itemPanelTransform = itemPanel.GetComponent<RectTransform>();
            itemPanelTransform.localScale = v3(1);
            itemPanelTransform.anchorMin = v2(0);
            itemPanelTransform.anchorMax = v2(1);
            itemPanelTransform.offsetMin = v2(20, 0);
            itemPanelTransform.offsetMax = v2(-20, 0);
            itemPanelTransform.anchoredPosition3D = v2(0);

            var selectionPanel = new GameObject("SelectionPanel", typeof(Image));
            selectionPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            selectionPanel.transform.SetParent(itemPanelTransform);
            var selectionPanelTransform = selectionPanel.GetComponent<RectTransform>();
            selectionPanelTransform.localScale = v3(1);
            selectionPanelTransform.anchorMin = v2(0);
            selectionPanelTransform.anchorMax = v2(1);
            selectionPanelTransform.offsetMin = v2(30, 415);
            selectionPanelTransform.offsetMax = v2(-30, -15);

            var itemFilterButton = CreateButton("ItemFilterButton", selectionPanel, "����", Color.white, "Graphics/BaseUI/GUI_ValuBack", Color.white, v2(0.5f), v2(0.5f), v2(160, 30), v3(0));
            itemFilterButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                string[] itemTypes = itemType.Split('|');
                filterTypeCurrent.GetComponent<Text>().text = itemTypes[selectedType];
                filterLevelCurrent.GetComponent<Text>().text = itemLevel[selectedLevel].Split('|')[0];
                loadedPanel.SetActive(false);
                filterPanel.SetActive(true);
            });

            // ��Ʒ��
            itemGrid = new GameObject("ItemGrid", typeof(GridLayoutGroup));
            itemGrid.transform.SetParent(itemPanelTransform);
            var itemGridComponent = itemGrid.GetComponent<GridLayoutGroup>();
            itemGridComponent.cellSize = v2(65, 65);
            itemGridComponent.startCorner = GridLayoutGroup.Corner.UpperLeft;
            itemGridComponent.startAxis = GridLayoutGroup.Axis.Horizontal;
            var itemGridTransform = itemGrid.GetComponent<RectTransform>();
            itemGridTransform.localScale = v3(1);
            itemGridTransform.anchorMin = v2(0);
            itemGridTransform.anchorMax = v2(1);
            itemGridTransform.offsetMin = v2(30, 0);
            itemGridTransform.offsetMax = v2(0, -50);

            itemBack = Resources.LoadAll<Sprite>("Graphics/ItemIcon/ItemIconBack");
            itemIcon = Resources.LoadAll<Sprite>("Graphics/ItemIcon/ItemIcon");


            EventTrigger.Entry itemSlotOnMouseEnterEntry = new EventTrigger.Entry();
            itemSlotOnMouseEnterEntry.eventID = EventTriggerType.PointerEnter;
            itemSlotOnMouseEnterEntry.callback.AddListener((data) =>
            {
                OnMouseEnterDelegate((PointerEventData)data);
            });

            EventTrigger.Entry itemSlotOnMouseExitEntry = new EventTrigger.Entry();
            itemSlotOnMouseExitEntry.eventID = EventTriggerType.PointerExit;
            itemSlotOnMouseExitEntry.callback.AddListener((data) =>
            {
                OnMouseExitDelegate((PointerEventData)data);
            });

            // ��Ʒ��
            for (int i = 0; i < 54; i++)
            {
                var itemSlot = new GameObject("ItemSlot|" + i, typeof(RectTransform));
                itemSlot.transform.SetParent(itemGridTransform);
                itemSlot.transform.localScale = v3(1);

                itemSlot.AddComponent(typeof(Button));
                itemSlot.GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        if (itemSlot.name.Split('|')[1] != "10000")
                        {
                            if (DateFile.instance.GetItem(ActorMenu.instance.acotrId, int.Parse(itemSlot.name.Split('|')[1]), 1, true) > 0)
                            {
                                TipsWindow.instance.SetTips(5007, new string[] { dataInstance.GetActorName(ActorMenu.instance.acotrId), itemList[int.Parse(itemSlot.name.Split('|')[1])][0], "" }, 100);
                            }
                            else
                            {
                                TipsWindow.instance.SetTips(0, new string[] { "�޷���Ӵ���Ʒ~" }, 100);
                            }
                        }
                        else
                        {
                            TipsWindow.instance.SetTips(0, new string[] { "�޷���Ӵ�֯~" }, 100);
                        }

                    }
                );

                itemSlot.AddComponent(typeof(EventTrigger));
                var itemSlotOnHover = itemSlot.GetComponent<EventTrigger>();
                itemSlotOnHover.triggers.Add(itemSlotOnMouseEnterEntry);
                itemSlotOnHover.triggers.Add(itemSlotOnMouseExitEntry);

                var itemBackObj = new GameObject("ItemBack", typeof(Image));
                itemBackObj.transform.SetParent(itemSlot.transform);
                itemBackObj.GetComponent<Image>().sprite = itemBack[4];
                itemBackObj.GetComponent<Image>().color = new Color32(254, 47, 17, 255);
                itemBackObj.transform.localScale = v3(1);
                itemBackObj.GetComponent<RectTransform>().sizeDelta = v2(60, 60);
                itemBackObj.GetComponent<Image>().preserveAspect = true;

                var itemIconObj = new GameObject("ItemIcon", typeof(Image));
                itemIconObj.transform.SetParent(itemBackObj.transform);
                itemIconObj.GetComponent<Image>().sprite = itemIcon[i];
                itemIconObj.transform.localScale = v3(1);
                itemIconObj.GetComponent<RectTransform>().sizeDelta = v2(40, 40);

                // ��Ʒ�±����� i.e. 0/0 ��MODδʹ�õ�����û�и�ֵ
                CreateText("ItemText", itemBackObj, "", Color.white, 14, v2(0), v2(1, 0.3f), v2(1), v3(0));
                itemIconObj.GetComponent<Image>().preserveAspect = true;

                // itemSlot.SetActive(false);

                itemSlots.Add(itemSlot);
            }

            prePageButton = CreateButton("PrePageBotton", loadedPanel, "��һҳ", Color.white, "Graphics/BaseUI/GUI_ValuBack", Color.white, v2(0), v2(0), v2(200, 30), v2(160, -25));
            nextPageButton = CreateButton("PrePageBotton", loadedPanel, "��һҳ", Color.white, "Graphics/BaseUI/GUI_ValuBack", Color.white, v2(1, 0), v2(1, 0), v2(200, 30), v2(-160, -25));

            var itemPageTextObj = CreateText("ItemPageText", loadedPanel, "0/0", Color.white, 20, v2(0.5f, 0), v2(0.5f, 0), v2(80, 30), v3(0, -25, 0));
            itemPageText = itemPageTextObj.GetComponent<Text>();

            // ��ʼ��ҳ��
            itemPageText.text = "1/1";

            prePageButton.GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    if (itemSlotPage > 0)
                    {
                        // �����������һҳ ����һ��itemSlotsȫ������
                        if (finalPageReached)
                        {
                            foreach (var itemSlot in itemSlots)
                            {
                                itemSlot.SetActive(true);
                            }
                        }

                        --itemSlotPage;

                        // ���Ի�ȡ��ǰѭ����Ӧ��itemSlot
                        int index = 0;

                        foreach (var item in itemList.Skip(itemSlotPage * 54).Take(54))
                        {
                            itemSlots[index].name = "ItemSlot|" + item.Key;
                            itemSlots[index].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = itemIcon[int.Parse(item.Value[98])];
                            itemSlots[index].transform.GetChild(0).GetComponent<Image>().sprite = itemBack[int.Parse(item.Value[4])];
                            Color backgroundColor = Color.white;
                            ColorUtility.TryParseHtmlString(itemLevel[int.Parse(item.Value[8])].Split('|')[1], out backgroundColor);
                            itemSlots[index].transform.GetChild(0).GetComponent<Image>().color = backgroundColor;
                            index++;
                        }

                        itemPageText.text = string.Format("{0}/{1}", itemSlotPage + 1, itemList.Count % 54 == 0 ? itemList.Count / 54 : (itemList.Count / 54) + 1);
                    }

                }
            );

            nextPageButton.GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    if (itemSlotPage < itemList.Count / 54)
                    {
                        ++itemSlotPage;

                        int index = 0;

                        foreach (var item in itemList.Skip(itemSlotPage * 54).Take(54))
                        {
                            if (itemSlotPage * 54 <= itemList.Count - 54)
                                finalPageReached = false;
                            else

                                finalPageReached = true;
                            itemSlots[index].name = "ItemSlot|" + item.Key;
                            itemSlots[index].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = itemIcon[int.Parse(item.Value[98])];
                            itemSlots[index].transform.GetChild(0).GetComponent<Image>().sprite = itemBack[int.Parse(item.Value[4])];
                            Color backgroundColor = Color.white;
                            ColorUtility.TryParseHtmlString(itemLevel[int.Parse(item.Value[8])].Split('|')[1], out backgroundColor);
                            itemSlots[index].transform.GetChild(0).GetComponent<Image>().color = backgroundColor;
                            index++;
                        }

                        itemPageText.text = string.Format("{0}/{1}", itemSlotPage + 1, itemList.Count % 54 == 0 ? itemList.Count / 54 : (itemList.Count / 54) + 1);

                        if (finalPageReached)
                        {
                            for (int i = 53; i >= index; i--)
                            {
                                itemSlots[i].SetActive(false);
                            }
                        }
                    }
                }
            );
            #endregion

            loadedPanel.SetActive(false);
            loadingPanel.SetActive(true);
            filterPanel.SetActive(false);

        }

        private void DrawItems()
        {
            // ��ʼ��ҳ��
            itemPageText.text = string.Format("1/{0}", itemList.Count % 54 == 0 ? itemList.Count / 54 : (itemList.Count / 54) + 1);

            // �Ƿ񵽴�����һҳ ��������itemSlots�Ŀ���
            if (itemList.Count <= 54)
            {
                finalPageReached = true;
            }
            else
            {
                finalPageReached = false;
            }

            itemSlotPage = 0;

            // ��ʼ��itemSlots
            int index = 0;
            foreach (var item in itemList.Skip(itemSlotPage * 54).Take(54))
            {
                itemSlots[index].SetActive(true);
                itemSlots[index].name = "ItemSlot|" + item.Key;
                if (itemSlotPage * 54 + index <= itemList.Count - 54)
                    finalPageReached = false;
                else
                    finalPageReached = true;

                itemSlots[index].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = itemIcon[int.Parse(item.Value[98])];
                itemSlots[index].transform.GetChild(0).GetComponent<Image>().sprite = itemBack[int.Parse(item.Value[4])];
                Color backgroundColor = Color.white;
                ColorUtility.TryParseHtmlString(itemLevel[int.Parse(item.Value[8])].Split('|')[1], out backgroundColor);
                itemSlots[index].transform.GetChild(0).GetComponent<Image>().color = backgroundColor;

                index++;
            }

            // ����������һҳ �������itemSlots�ر�
            if (finalPageReached)
            {
                for (int i = 53; i >= index; i--)
                {
                    itemSlots[i].SetActive(false);
                }
            }
        }

        public void OnMouseEnterDelegate(PointerEventData data)
        {
            GameObject OnHover = data.pointerEnter;

            while (!OnHover.name.Contains("ItemSlot"))
            {
                OnHover = OnHover.transform.parent.gameObject;
            }

                GameObject tips = new GameObject();
                // ��ʱ�½�һ��Item��ȡID����ʾ��Ϣ
                tips.name = "ActorItem," + DateFile.instance.MakeNewItem(int.Parse(OnHover.name.Split('|')[1]), -5713);
                tips.tag = "ActorItem";
                WindowManage.instance.WindowSwitch(true, tips);

        }
        public void OnMouseExitDelegate(PointerEventData data)
        {
            GameObject OnHover = data.pointerEnter;

            while (!OnHover.name.Contains("ItemSlot"))
            {
                OnHover = OnHover.transform.parent.gameObject;
            }
            WindowManage.instance.WindowSwitch(false);
        }

        public void OnDragDelegate(PointerEventData data)
        {
            if (gameLoaded && !gameExited)
            {
                // ��ȡ��ǰ���λ��
                Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainPanel.transform.position.z);
                // ת��Ϊ��������
                Vector3 objPosition = Camera.main.ScreenToWorldPoint(newPosition) + offsetPostion;
                mainPanel.transform.position = objPosition;
            }
            else
            {
                mainPanel.transform.position = Input.mousePosition + offsetPostionLoding;
            }
        }

        private GameObject CreateText(string name, GameObject parent, string content, Color color, int fontSize, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector3 anchoredPosition)
        {
            var text = new GameObject(name, typeof(Text));
            text.transform.SetParent(parent.transform);
            text.GetComponent<Text>().text = content;
            text.GetComponent<Text>().color = color;
            text.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            text.GetComponent<Text>().fontSize = fontSize;
            var textTransform = text.GetComponent<RectTransform>();
            textTransform.localScale = v3(1, 1, 1);
            textTransform.anchorMax = anchorMax;
            textTransform.anchorMin = anchorMin;
            textTransform.sizeDelta = size;
            textTransform.anchoredPosition3D = anchoredPosition;
            return text;
        }

        private GameObject CreateButton(string buttonName, GameObject parent, string text, Color textColor, string texture, Color textureColor, Vector2 anchorMax, Vector2 anchorMin, Vector2 size, Vector3 position)
        {
            GameObject button = new GameObject(buttonName, typeof(Button), typeof(Image));
            var buttonText = new GameObject(buttonName + "Text", typeof(Text));
            buttonText.transform.SetParent(button.transform);
            buttonText.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            buttonText.GetComponent<Text>().text = text;
            buttonText.GetComponent<Text>().color = textColor;
            buttonText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            buttonText.GetComponent<Text>().fontSize = 14;
            buttonText.GetComponent<RectTransform>().localScale = v3(1, 1, 1);
            buttonText.GetComponent<RectTransform>().anchorMin = v2(0);
            buttonText.GetComponent<RectTransform>().anchorMax = v2(1);
            buttonText.GetComponent<RectTransform>().offsetMin = v2(0);
            buttonText.GetComponent<RectTransform>().offsetMax = v2(0);
            button.transform.SetParent(parent.transform);
            button.GetComponent<Image>().sprite = Resources.Load(texture, typeof(Sprite)) as Sprite;
            button.GetComponent<Image>().color = textureColor;
            var buttonTransform = button.GetComponent<RectTransform>();
            buttonTransform.localScale = v3(1, 1, 1);
            buttonTransform.anchorMax = anchorMax;
            buttonTransform.anchorMin = anchorMin;
            buttonTransform.sizeDelta = size;
            buttonTransform.anchoredPosition3D = position;
            return button;
        }

        #region Utils
        private static Vector2 v2(float var)
        {
            return new Vector2(var, var);
        }

        private static Vector2 v2(float x, float y)
        {
            return new Vector2(x, y);
        }

        private static Vector3 v3(float var)
        {
            return new Vector3(var, var, var);
        }

        private static Vector3 v3(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }
        #endregion
    }
}
