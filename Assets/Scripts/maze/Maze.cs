using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Games.Pathfinding;

namespace sha
{
    public enum Directions
    {
        DOWN = 0,
        RIGHT = 1,
        UP = 2,
        LEFT = 3,
        NONE,
    };

    public class Cell
    {
        public int x = 0;
        public int y = 0;
        private bool visited = false;

        public Cell()
        {
        }
        public Cell(int xx, int yy)
        {
            x = xx;
            y = yy;
        }
        public void SetVisited()
        {
            visited = true;
        }
        public bool GetVisited()
        {
            return visited;
        }
    }

    public class GridMaze
    {
        public int xNum;
        public int yNum;

        public Cell[,] m_cells;
        private Stack<Cell> m_stack = new Stack<Cell>();

        public Cell GetCell(int x, int y)
        {
            return m_cells[x, y];
        }

        // In this grid only the odd m_cells can be visited.
        // The even m_cells forming the border.
        public GridMaze(int X, int Y)
        {
            xNum = X;
            yNum = Y;

            m_cells = new Cell[xNum, yNum];
            for (int i = 0; i < xNum; ++i)
            {
                for (int j = 0; j < yNum; ++j)
                {
                    m_cells[i, j] = new Cell(i, j);
                    m_cells[i, j].x = i;
                    m_cells[i, j].y = j;
                }
            }
            // we only use odd cell indices.
            m_cells[1, 1].SetVisited();
            m_stack.Push(m_cells[1, 1]);
        }

        void RemoveCellWall(int x, int y, Directions dir)
        {
            switch (dir)
            {
                case Directions.DOWN:
                    --y;
                    break;
                case Directions.RIGHT:
                    x++;
                    break;
                case Directions.UP:
                    y++;
                    break;
                case Directions.LEFT:
                    --x;
                    break;
            }
            m_cells[x, y].SetVisited();
        }

        public Dictionary<Directions, Cell> GetNeighbours(Cell cell)
        {
            Dictionary<Directions, Cell> neighbours = new Dictionary<Directions, Cell>();
            for (int i = 0; i < 4; ++i)
            {
                Directions dir = (Directions)i;
                int x = cell.x;
                int y = cell.y;
                switch (dir)
                {
                    case Directions.DOWN:
                        if (y > 1)
                        {
                            y -= 2;
                            if (!m_cells[x, y].GetVisited())
                            {
                                neighbours.Add(Directions.DOWN, m_cells[x, y]);
                            }
                        }
                        break;
                    case Directions.RIGHT:
                        if (x < xNum - 2)
                        {
                            x += 2;
                            if (!m_cells[x, y].GetVisited())
                            {
                                neighbours.Add(Directions.RIGHT, m_cells[x, y]);
                            }
                        }
                        break;
                    case Directions.UP:
                        if (y < yNum - 2)
                        {
                            y += 2;
                            if (!m_cells[x, y].GetVisited())
                            {
                                neighbours.Add(Directions.UP, m_cells[x, y]);
                            }
                        }
                        break;
                    case Directions.LEFT:
                        if (x > 1)
                        {
                            x -= 2;
                            if (!m_cells[x, y].GetVisited())
                            {
                                neighbours.Add(Directions.LEFT, m_cells[x, y]);
                            }
                        }
                        break;
                }
            }
            return neighbours;
        }

        public bool GenerateStep()
        {
            if (m_stack.Count == 0)
                return false;

            Cell cell = m_stack.Peek();

            Dictionary<Directions, Cell> neighbours = GetNeighbours(cell);
            System.Random random = new System.Random(System.DateTime.Now.Millisecond);
            if (neighbours.Count > 0)
            {
                int index = random.Next(0, neighbours.Count);
                Cell neighbour = neighbours.ElementAt(index).Value;
                Directions dir = neighbours.ElementAt(index).Key;
                neighbour.SetVisited();

                RemoveCellWall(cell.x, cell.y, dir);

                m_stack.Push(neighbour);
            }
            else
            {
                m_stack.Pop();
            }
            return true;
        }
    }

    // The main map for the path finding to happen.
    public class Map2d
    {
        /// <summary>
        /// Initialize the Map2d from a maze.
        /// </summary>
        /// <param name="maze"></param>
        private float[,] m_cost;
        private int xNum, yNum;
        public Map2d(GridMaze maze)
        {
            m_cost = new float[maze.xNum, maze.yNum];
            xNum = maze.xNum;
            yNum = maze.yNum;

            // if maze tiles are not visited then we set the 
            // cost for those cells to be -1 or non-traversable
            for(int i = 0; i < xNum; ++i)
            {
                for(int j = 0; j < yNum; ++j)
                {
                    // by default put all cells to be non traversable.
                    m_cost[i, j] = -1.0f;
                    if (maze.m_cells[i,j].GetVisited())
                    {
                        // set the cost to a default value of 1.0f
                        m_cost[i, j] = 1.0f;
                    }
                }
            }
        }

        /// <summary>
        /// Gets movement cost from the 2-dimensional map
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        /// <returns>Returns movement cost at the specified point in the map</returns>
        public float GetMapCost(int x, int y)
        {
            if ((x < 0) || (x > xNum))
                return (-1);
            if ((y < 0) || (y > yNum))
                return (-1.0f);
            return m_cost[x, y];
        }
    }
    /// <summary>
    /// A node class for doing pathfinding on a 2-dimensional map
    /// </summary>
    public class AStarNode2D : AStarNode
    {
        #region Properties

        /// <summary>
        /// The X-coordinate of the node
        /// </summary>
        public int X
        {
            get
            {
                return FX;
            }
        }
        private int FX;

        /// <summary>
        /// The Y-coordinate of the node
        /// </summary>
        public int Y
        {
            get
            {
                return FY;
            }
        }
        private int FY;
        private Map2d m_map;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for a node in a 2-dimensional map
        /// </summary>
        /// <param name="AParent">Parent of the node</param>
        /// <param name="AGoalNode">Goal node</param>
        /// <param name="ACost">Accumulative cost</param>
        /// <param name="AX">X-coordinate</param>
        /// <param name="AY">Y-coordinate</param>
        public AStarNode2D(Map2d map, AStarNode AParent, AStarNode AGoalNode, double ACost, int AX, int AY) : base(AParent, AGoalNode, ACost)
        {
            m_map = map;
            FX = AX;
            FY = AY;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds a successor to a list if it is not impassible or the parent node
        /// </summary>
        /// <param name="ASuccessors">List of successors</param>
        /// <param name="AX">X-coordinate</param>
        /// <param name="AY">Y-coordinate</param>
        private void AddSuccessor(ArrayList ASuccessors, int AX, int AY)
        {
            int CurrentCost = (int) m_map.GetMapCost(AX, AY);
            if (CurrentCost == -1)
            {
                return;
            }
            AStarNode2D NewNode = new AStarNode2D(m_map, this, GoalNode, Cost + CurrentCost, AX, AY);
            if (NewNode.IsSameState(Parent))
            {
                return;
            }
            ASuccessors.Add(NewNode);
        }

        #endregion

        #region Overidden Methods

        /// <summary>
        /// Determines wheather the current node is the same state as the on passed.
        /// </summary>
        /// <param name="ANode">AStarNode to compare the current node to</param>
        /// <returns>Returns true if they are the same state</returns>
        public override bool IsSameState(AStarNode ANode)
        {
            if (ANode == null)
            {
                return false;
            }
            return ((((AStarNode2D)ANode).X == FX) &&
                (((AStarNode2D)ANode).Y == FY));
        }

        /// <summary>
        /// Calculates the estimated cost for the remaining trip to the goal.
        /// </summary>
        public override void Calculate()
        {
            if (GoalNode != null)
            {
                double xd = FX - ((AStarNode2D)GoalNode).X;
                double yd = FY - ((AStarNode2D)GoalNode).Y;
                // "Euclidean distance" - Used when search can move at any angle.
                //GoalEstimate = Math.Sqrt((xd*xd) + (yd*yd));
                // "Manhattan Distance" - Used when search can only move vertically and 
                // horizontally.
                //GoalEstimate = Math.Abs(xd) + Math.Abs(yd); 
                // "Diagonal Distance" - Used when the search can move in 8 directions.
                GoalEstimate = Math.Max(Math.Abs(xd), Math.Abs(yd));
            }
            else
            {
                GoalEstimate = 0;
            }
        }

        /// <summary>
        /// Gets all successors nodes from the current node and adds them to the successor list
        /// </summary>
        /// <param name="ASuccessors">List in which the successors will be added</param>
        public override void GetSuccessors(ArrayList ASuccessors)
        {
            ASuccessors.Clear();
            AddSuccessor(ASuccessors, FX - 1, FY);
            //AddSuccessor(ASuccessors, FX - 1, FY - 1);
            AddSuccessor(ASuccessors, FX, FY - 1);
            //AddSuccessor(ASuccessors, FX + 1, FY - 1);
            AddSuccessor(ASuccessors, FX + 1, FY);
            //AddSuccessor(ASuccessors, FX + 1, FY + 1);
            AddSuccessor(ASuccessors, FX, FY + 1);
            //AddSuccessor(ASuccessors, FX - 1, FY + 1);
        }

        /// <summary>
        /// Prints information about the current node
        /// </summary>
        public override void PrintNodeInfo()
        {
            Console.WriteLine("X:\t{0}\tY:\t{1}\tCost:\t{2}\tEst:\t{3}\tTotal:\t{4}", FX, FY, Cost, GoalEstimate, TotalCost);
        }

        #endregion
    }
}

public class Maze : MonoBehaviour
{
    [HideInInspector]
    public sha.GridMaze maze;
    public int xNum = 5;
    public int yNum = 5;

    public GameObject quadPrefab;
    public GameObject quad_0_Prefab;
    public GameObject quad_startPrefab;
    public GameObject quad_goalPrefab;

    public float TileWidth = 1.0f;
    public float TileHeight = 1.0f;

    private GameObject[,] quads;
    private sha.Map2d m_map;

    int startX = 1, startY = 1;
    int goalX = 1, goalY = 1;
    [HideInInspector]
    public GameObject gameObjStart;
    [HideInInspector]
    public GameObject gameObjGoal;
    [HideInInspector]
    public GameObject gameObjCurrent;

    public float speed = 100.0f;

    private Games.Pathfinding.AStar astar = null;

    // Start is called before the first frame update
    void Start()
    {
        maze = new sha.GridMaze(xNum, yNum);
        quads = new GameObject[xNum, yNum];
        bool done = false;
        while (!done)
        {
            done = !maze.GenerateStep();
            Debug.Log("Generating maze");
        }

        // We generate the map from the maze.
        m_map = new sha.Map2d(maze);

        // now create the drid.
        for (int i = 0; i < xNum; ++i)
        {
            for(int j = 0; j < yNum; ++j)
            {
                if (maze.GetCell(i, j).GetVisited())
                {
                    quads[i, j] = Instantiate(quad_0_Prefab);
                    quads[i, j].transform.localPosition = new Vector3(i * TileWidth, j * TileHeight, 1.0f);
                    quads[i, j].transform.SetParent(transform);
                }
                else
                {
                    quads[i, j] = Instantiate(quadPrefab);
                    quads[i, j].transform.localPosition = new Vector3(i * TileWidth, j * TileHeight, 1.0f);
                    quads[i, j].transform.SetParent(transform);
                }
            }
        }
        gameObjStart = Instantiate(quad_startPrefab);
        gameObjStart.transform.localPosition = new Vector3(startX * TileWidth, startY * TileHeight, 0.0f);
        gameObjStart.transform.SetParent(transform);

        gameObjCurrent = Instantiate(quad_startPrefab);
        gameObjCurrent.transform.localPosition = new Vector3(startX * TileWidth, startY * TileHeight, 0.0f);
        gameObjCurrent.transform.SetParent(transform);

        gameObjGoal = Instantiate(quad_goalPrefab);
        gameObjGoal.transform.localPosition = new Vector3(goalX * TileWidth, goalY * TileHeight, 0.0f);
        gameObjGoal.transform.SetParent(transform);
    }

    public void HandleTileClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject selected = Puzzle.Utils.Pick3D();
            if (selected != null)
            {
                int x = (int) (selected.transform.position.x + 0.5f);
                int y = (int) (selected.transform.position.y + 0.5f);
                if (maze.GetCell(x, y).GetVisited())
                {
                    goalX = x;
                    goalY = y;
                    gameObjGoal.transform.localPosition = new Vector3(goalX * TileWidth, goalY * TileHeight, 0.0f);
                    StartAStarSearchInBackground();
                }
            }
        }
    }

    /// <summary>
    /// Prints the solution
    /// </summary>
    /// <param name="ASolution">The list that holds the solution</param>
    private IEnumerator PrintSolution(ArrayList ASolution)
    {
        for(int i = 0; i < ASolution.Count; ++i)
        {
            sha.AStarNode2D node = (sha.AStarNode2D)ASolution[i];
            gameObjCurrent.transform.localPosition = new Vector3(node.X * TileWidth, node.Y * TileHeight, 0.0f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator SearchUsingAStar()
    {
        Debug.Log("Started AStar Search");
        AStar.SearchResultEnum result = AStar.SearchResultEnum.RUNNING;
        while(result == AStar.SearchResultEnum.RUNNING)
        {
            result = astar.FindPath_Next();
            sha.AStarNode2D currNode = (sha.AStarNode2D)astar.NodeCurrent;
            if (currNode != null)
            {
                Debug.Log("CurrentNode: " + currNode.X + ", " + currNode.Y);
            }
            yield return new WaitForEndOfFrame();
        }
        if (result == AStar.SearchResultEnum.SUCCESS)
        {
            startX = goalX;
            startY = goalY;
            Debug.Log("Completed AStar Search with SUCCESS");
            StartCoroutine(PrintSolution(astar.Solution));
        }

        if (result == AStar.SearchResultEnum.FAILURE)
        {
            Debug.Log("Completed AStar Search with FAILURE");
        }
    }

    public void StartAStarSearchInBackground()
    {
        sha.AStarNode2D GoalNode = new sha.AStarNode2D(m_map, null, null, 0, goalX, goalY);
        sha.AStarNode2D StartNode = new sha.AStarNode2D(m_map, null, GoalNode, 0, startX, startY);
        StartNode.GoalNode = GoalNode;
        astar = new Games.Pathfinding.AStar(StartNode, GoalNode);

        IEnumerator func = SearchUsingAStar();
        StartCoroutine(func);
    }

    // Update is called once per frame
    void Update()
    {
        HandleTileClick();
    }
}
