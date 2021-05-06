using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Block
{
    public int index { get; set; } = 0;
    public GameObject gameObj { get; set; } = null;
    public Block(int id, GameObject obj)
    {
        index = id;
        gameObj = obj;
    }
}

public class DailyQuestMenuState : Patterns.State
{
    public enum StateTypes
    {
        DQM_FADE_IN = 0,
        DQM_FADE_IN_QUESTS,
        DQM_IDLE,
        DQM_FADE_OUT,
    }

    protected float m_deltaTime = 0.0f;
    protected DailyQuestMenu m_menu;

    public DailyQuestMenuState(Patterns.FSM fsm, int id, DailyQuestMenu menu)
        : base(fsm, id)
    {
        m_menu = menu;
    }
    public override void Enter()
    {
        m_deltaTime = 0.0f;
    }
    public override void Exit() { }
    public override void Update()
    {
        m_deltaTime += Time.deltaTime;
    }
    public override void FixedUpdate() { }

    protected void FadeIn(Image image, float duration, float startTime = 0.0f)
    {
        //m_deltaTime += Time.deltaTime;
        if (m_deltaTime > startTime && m_deltaTime <= startTime + duration)
        {
            Color c = image.color;
            c.a = (m_deltaTime - startTime) / duration;
            image.color = c;
        }
    }

    protected void FadeOut(Image image, float duration)
    {
        //m_deltaTime += Time.deltaTime;
        if (m_deltaTime <= duration)
        {
            Color c = image.color;
            c.a = 1.0f - m_deltaTime / duration;
            image.color = c;
        }
    }
}

public class DQM_FADE_IN : DailyQuestMenuState
{
    private float m_duration = 2.0f;
    public DQM_FADE_IN(Patterns.FSM fsm, int id, DailyQuestMenu menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        base.Enter();
        m_menu.btnPrevious.SetActive(true);
        Puzzle.Utils.SetImageTransparency(m_menu.btnPrevious.GetComponent<Image>(), 0.0f);
    }
    public override void Exit()
    {
        Puzzle.Utils.SetImageTransparency(m_menu.btnPrevious.GetComponent<Image>(), 1.0f);
    }
    public override void Update()
    {
        base.Update();
        Image img = m_menu.btnPrevious.GetComponent<Image>();
        //FadeIn(img, m_duration);
        FadeOut(m_menu.imageFiller, m_duration);
        if (m_deltaTime > m_duration)
        {
            m_fsm.SetCurrentState((int)StateTypes.DQM_FADE_IN_QUESTS);
        }
    }
}

public class DQM_FADE_IN_QUESTS : DailyQuestMenuState
{
    float m_duration = 1.0f;
    public DQM_FADE_IN_QUESTS(Patterns.FSM fsm, int id, DailyQuestMenu menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        m_menu.btnBlocksParent.SetActive(true);
        for (int i = 0; i < m_menu.blocks.Count; ++i)
        {
            Image img = m_menu.blocks[i].gameObj.GetComponent<Image>();
            Puzzle.Utils.SetImageTransparency(img, 0.0f);
        }
        base.Enter();
    }
    public override void Exit()
    {
    }
    public override void Update()
    {
        base.Update();
        for (int i = 0; i < m_menu.blocks.Count; ++i)
        {
            Image img = m_menu.blocks[i].gameObj.GetComponent<Image>();
            FadeIn(img, m_duration, i*0.01f);
        }
        //Debug.Log(m_deltaTime);
        if (m_deltaTime > m_duration * m_menu.blocks.Count * 0.01f)
        {
            for (int i = 0; i < m_menu.blocks.Count; ++i)
            {
                Image img = m_menu.blocks[i].gameObj.GetComponent<Image>();
                Puzzle.Utils.SetImageTransparency(img, 1.0f);
            }
            m_fsm.SetCurrentState((int)StateTypes.DQM_IDLE);
        }
    }
}

public class DQM_IDLE : DailyQuestMenuState
{
    private int selectedQuest = 0;
    public DQM_IDLE(Patterns.FSM fsm, int id, DailyQuestMenu menu)
        : base(fsm, id, menu)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
    }
    public override void Update()
    {
        base.Update();
        for (int i = 0; i < m_menu.blocks.Count; ++i)
        {
            if (m_menu.blocks[i].gameObj.GetComponent<FixedButton>().Pressed)
            {
                m_menu.blocks[i].gameObj.GetComponent<FixedButton>().Pressed = false;
                selectedQuest = i;
                m_menu.btnNext.SetActive(true);
            }
        }
        if(m_menu.btnPrevious.GetComponent<FixedButton>().Pressed)
        {
            m_menu.btnPrevious.GetComponent<FixedButton>().Pressed = false;
            m_menu.LoadPreviousMenu();
        }
        if (m_menu.btnNext.GetComponent<FixedButton>().Pressed)
        {
            m_menu.btnPrevious.GetComponent<FixedButton>().Pressed = false;
            m_menu.LoadQuest(selectedQuest);
        }
    }
}

//public class DQM_FADE_OUT : DailyQuestMenuState
//{
//    private float m_duration = 1.0f;
//    public DQM_FADE_OUT(Patterns.FSM fsm, int id, DailyQuestMenu menu)
//        : base(fsm, id, menu)
//    {
//    }
//    public override void Enter()
//    {
//        base.Enter();
//    }
//    public override void Exit()
//    {
//        m_menu.btnBlocksParent.SetActive(false);
//        m_menu.btnPrevious.SetActive(false);
//        base.Exit();
//    }
//    public override void Update()
//    {
//        FadeIn(m_menu.imageFiller, m_duration);
//        FadeOut(m_menu.btnPrevious.GetComponent<Image>(), m_duration);
//        for (int i = 0; i < m_menu.blocks.Count; ++i)
//        {
//            Image img = m_menu.blocks[i].gameObj.GetComponent<Image>();
//            FadeOut(img, 1.0f);
//        }
//        if (m_deltaTime > m_duration)
//        {
//            // Exit function.

//        }
//    }
//}

public class DailyQuestMenu : MonoBehaviour
{
    private Patterns.FSM m_fsm;

    //----------------------------------------------------------//
    public GameObject btnBlockPrefab;
    public GameObject btnBlocksParent;

    public GameObject btnNext;
    public GameObject btnPrevious;

    public Image imageFiller;

    [HideInInspector]
    public List<Block> blocks = new List<Block>();

    public int Rows = 10;
    public int Cols = 7;

    // Start is called before the first frame update
    void Start()
    {
        m_fsm = new Patterns.FSM();
        Image img = btnNext.GetComponent<Image>();

        CreateQuestBlocks();

        m_fsm.Add(new DQM_FADE_IN(m_fsm, (int)DailyQuestMenuState.StateTypes.DQM_FADE_IN, this));
        //m_fsm.Add(new DQM_FADE_OUT(m_fsm, (int)DailyQuestMenuState.StateTypes.DQM_FADE_OUT, this));
        m_fsm.Add(new DQM_IDLE(m_fsm, (int)DailyQuestMenuState.StateTypes.DQM_IDLE, this));
        m_fsm.Add(new DQM_FADE_IN_QUESTS(m_fsm, (int)DailyQuestMenuState.StateTypes.DQM_FADE_IN_QUESTS, this));

        m_fsm.SetCurrentState((int)DailyQuestMenuState.StateTypes.DQM_FADE_IN);
    }

    // Update is called once per frame
    void Update()
    {
        m_fsm.Update();
    }

    int RowColToIndex(int r, int c)
    {
        return r* Rows +c;
    }

    void CreateQuestBlocks()
    {

        const int rows = 10;
        const int cols = 8;
        float startx = -350.0f;
        float starty = 500.0f;
        float w = 100.0f;

        for (int i = 0; i < rows; ++i)
        {
            for(int j = 0; j < cols; ++j)
            {
                GameObject obj = Instantiate(btnBlockPrefab);
                obj.name = "btn_block_" + i + "_" + j;
                Vector3 pos = new Vector3(startx + j * w, starty - i * w, 0.0f);

                obj.transform.SetParent(btnBlocksParent.transform);
                obj.transform.localScale = Vector3.one;
                obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
                obj.GetComponent<RectTransform>().anchoredPosition3D = pos;

                blocks.Add(new Block(RowColToIndex(i, j), obj));
            }
        }
    }
    public void LoadPreviousMenu()
    {
        SceneManager.LoadScene("002");
    }
    public void LoadQuest(int i)
    {
        string questName = ManagerQuestLoader.Instance.QuestIndexToSceneName(i);
        SceneManager.LoadScene(questName);
    }
}
