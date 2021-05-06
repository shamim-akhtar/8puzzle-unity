using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenuState : Patterns.State
{
    public enum StateTypes
    {
        MENU_MAIN,
        MENU_ADVENTURE,
        MENU_DAILY_QUEST,
        MENU_MINI_GAMES,
        MENU_ACHIEVEMENTS,
        MENU_SETTINGS,
    }
    protected GameMenu gameMenu;

    protected List<Image> m_buttons = new List<Image>();
    protected List<GameObject> m_gameobjects = new List<GameObject>();
    protected List<GameObject> m_menuText = new List<GameObject>();

    protected float m_deltaTime = 0.0f;

    public GameMenuState(Patterns.FSM fsm, int id, GameMenu menu) 
        : base(fsm, id)
    {
        gameMenu = menu;
        foreach (Transform child in gameMenu.Menu.transform)
        {
            m_buttons.Add(child.gameObject.GetComponent<Image>());
            m_gameobjects.Add(child.gameObject);
        }
        foreach (Transform child in gameMenu.MenuText.transform)
        {
            m_menuText.Add(child.gameObject);
        }
    }
    public override void Enter()
    {
        m_deltaTime = 0.0f;
    }
    public override void Exit() { }
    public override void Update() { }
    public override void FixedUpdate() { }

    protected void FadeIn(Image image, float duration)
    {
        m_deltaTime += Time.deltaTime;
        if (m_deltaTime <= duration)
        {
            Color c = image.color;
            c.a = m_deltaTime / duration;
            image.color = c;
        }
    }

    protected void FadeOut(Image image, float duration)
    {
        m_deltaTime += Time.deltaTime;
        if (m_deltaTime <= duration)
        {
            Color c = image.color;
            c.a = 1.0f - m_deltaTime / duration;
            image.color = c;
        }
    }
}

public class MENU_STATE : GameMenuState
{
    private float m_duration = 1.0f;

    public MENU_STATE(Patterns.FSM fsm, int id, GameMenu menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        base.Enter();
        gameMenu.BtnPrevious.SetActive(true);
        Image btnPrevImage = gameMenu.BtnPrevious.GetComponent<Image>();
        Color c = btnPrevImage.color;
        c.a = 0.0f;
        btnPrevImage.color = c;
        //AudioManager.Instance.PlayFadeIn(0, 2.0f, 0.0f, 1.0f, true);
    }
    public override void Exit()
    {
        gameMenu.BtnPrevious.SetActive(false);
    }
    public override void Update()
    {
        m_deltaTime += Time.deltaTime;
        if (m_deltaTime <= m_duration)
        {

            Image btnPrevImage = gameMenu.BtnPrevious.GetComponent<Image>();
            Color c = btnPrevImage.color;
            c.a = m_deltaTime / m_duration;
            btnPrevImage.color = c;
        }

        FixedButton prevBtn = gameMenu.BtnPrevious.GetComponent<FixedButton>();
        if (prevBtn.Pressed)
        {
            gameMenu.Source.PlayOneShot(gameMenu.audioClick);
            m_fsm.SetCurrentState((int)StateTypes.MENU_MAIN);
            prevBtn.Pressed = false;
        }
    }
}

public class MENU_MAIN : GameMenuState
{
    private float m_duration = 2.0f;

    private int m_index = 0;
    protected List<float> m_startTimes = new List<float>();

    public MENU_MAIN(Patterns.FSM fsm, int id, GameMenu menu)
        : base(fsm, id, menu)
    {
        int i = 0;
        foreach (Transform child in gameMenu.Menu.transform)
        {
            m_startTimes.Add(m_duration * 0.1f * i++);
        }
    }
    public override void Enter()
    {
        base.Enter();
        gameMenu.Menu.SetActive(true);
        gameMenu.MenuText.SetActive(true);
        gameMenu.BtnPrevious.SetActive(false);

        for(int i = 0; i < m_buttons.Count; ++i)
        {
            Color c = m_buttons[i].color;
            c.a = 0.0f;
            m_buttons[i].color = c;
            m_deltaTime = 0.0f;
        }
    }
    public override void Exit()
    {
        gameMenu.Menu.SetActive(false);
        gameMenu.MenuText.SetActive(false);
        gameMenu.BtnNext.SetActive(false);
        for (int j = 0; j < m_buttons.Count; ++j)
        {
            m_menuText[j].SetActive(false);
        }
    }
    public override void Update()
    {
        m_deltaTime += Time.deltaTime;
        for (int i = 0; i < m_buttons.Count; ++i)
        {
            if(m_deltaTime > m_startTimes[i] && m_deltaTime <= m_startTimes[i] + m_duration)
            {
                Color c = m_buttons[i].color;
                c.a = (m_deltaTime - m_startTimes[i]) / m_duration;
                m_buttons[i].color = c;
            }
        }
        for (int i = 0; i < m_buttons.Count; ++i)
        {
            FixedButton fixedButton = m_gameobjects[i].GetComponent<FixedButton>();
            if (fixedButton.Pressed)
            {
                gameMenu.Source.PlayOneShot(gameMenu.audioClick);
                m_menuText[i].SetActive(true);
                m_index = i;
                for (int j = 0; j < m_buttons.Count; ++j)
                {
                    if (j != i)
                    {
                        m_menuText[j].SetActive(false);
                    }
                }
                gameMenu.BtnNext.SetActive(true);

                fixedButton.Pressed = false;
            }
        }

        FixedButton nextBtn = gameMenu.BtnNext.GetComponent<FixedButton>();
        if(nextBtn.Pressed)
        {
            gameMenu.Source.PlayOneShot(gameMenu.audioClick);
            nextBtn.Pressed = false;
            m_fsm.SetCurrentState((int)StateTypes.MENU_MAIN + m_index + 1);
        }
    }
}

public class MENU_ADVENTURE : MENU_STATE
{
    public MENU_ADVENTURE(Patterns.FSM fsm, int id, GameMenu menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
    }
}

public class MENU_ACHIEVEMENTS : MENU_STATE
{
    public MENU_ACHIEVEMENTS(Patterns.FSM fsm, int id, GameMenu menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
    }
}

public class MENU_SETTINGS : MENU_STATE
{
    public MENU_SETTINGS(Patterns.FSM fsm, int id, GameMenu menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
    }
}

public class GameMenu : MonoBehaviour
{
    public AudioClip audioMenu;
    public AudioClip audioClick;

    [HideInInspector]
    public AudioSource Source;

    public GameObject Menu;
    public GameObject MenuText;
    public GameObject BtnNext;
    public GameObject BtnPrevious;

    private Patterns.FSM m_fsm;

    public GameObject prefabBtnMinigame;
    public GameObject btnMinigameParent;

    // MENU_MINI_GAMES
    [HideInInspector]
    public List<GameObject> btnMinigamesGameObjects = new List<GameObject>();
    [HideInInspector]
    public List<Image> btnMinigamesImages = new List<Image>();
    public GameObject[] miniGamesButtonPrefabs;

    // MENU_DAILY_QUEST

    void Awake()
    {
        // create the dynamic content for the menu.
        CreateMenuItems();

        Source = GetComponent<AudioSource>();

        // create the FSM.
        m_fsm = new Patterns.FSM();

        m_fsm.Add(new MENU_MAIN(m_fsm, (int)GameMenuState.StateTypes.MENU_MAIN, this));
        m_fsm.Add(new MENU_ADVENTURE(m_fsm, (int)GameMenuState.StateTypes.MENU_ADVENTURE, this));
        m_fsm.Add(new MENU_DAILY_QUEST(m_fsm, (int)GameMenuState.StateTypes.MENU_DAILY_QUEST, this));
        m_fsm.Add(new MENU_MINI_GAMES(m_fsm, (int)GameMenuState.StateTypes.MENU_MINI_GAMES, this));
        m_fsm.Add(new MENU_ACHIEVEMENTS(m_fsm, (int)GameMenuState.StateTypes.MENU_ACHIEVEMENTS, this));
        m_fsm.Add(new MENU_SETTINGS(m_fsm, (int)GameMenuState.StateTypes.MENU_SETTINGS, this));

        m_fsm.SetCurrentState((int)GameMenuState.StateTypes.MENU_MAIN);

        Source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        m_fsm.Update();
    }

    public GameObject CreateMinigameBtn()
    {
        return Instantiate<GameObject>(prefabBtnMinigame);
    }

    public void CreateMenuItems()
    {
        // create the minigame buttons.
        // create a grid of 3 by 4.
        float x, y;
        x = -80.0f;
        y = 200.0f;
        float d = 80;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int ind = i * 3 + j;
                GameObject button;
                if (ind < miniGamesButtonPrefabs.Length)
                {
                    button = Instantiate<GameObject>(miniGamesButtonPrefabs[ind]);
                    //Button btn = button.GetComponent<Button>();
                    //btn.onClick.AddListener(delegate { LoadMiniGame(ind); });
                }
                else
                {
                    button = CreateMinigameBtn();
                }
                button.name = "btnMinigame_" + i + "_" + j;
                button.transform.SetParent(btnMinigameParent.transform);
                button.transform.localScale = Vector3.one;
                button.transform.localRotation = Quaternion.Euler(Vector3.zero);
                button.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(x + d * i, y - d * j, 0);

                btnMinigamesGameObjects.Add(button);
                btnMinigamesImages.Add(button.GetComponent<Image>());
            }
        }
        btnMinigameParent.SetActive(false);
    }

    public void LoadMiniGame(int index)
    {
        // manually call exit because there are no other states.
        m_fsm.GetCurrentState().Exit();
        m_fsm = null;

        string sceneName = "mini_" + index.ToString();
        Source.Stop();
        // for now we only have the 8 puzzle game.
        SceneManager.LoadScene(sceneName);
    }

    public void LoadDailyQuestUI()
    {
        // manually call exit because there are no other states.
        m_fsm.GetCurrentState().Exit();
        m_fsm = null;

        // for now we only have the 8 puzzle game.
        SceneManager.LoadScene("003");
    }
}
