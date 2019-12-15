using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TurnManager : MonoBehaviour
{

    public GameObject atkButton;
    public GameObject dfdButton;
    public GameObject psnButton;
    public GameObject healButton;
    public GameObject chgButton;
    public GameObject lsrButton;
    public GameObject enmButton;
    public GameObject nextLvlButton;
    public GameObject gameOverText;
    public GameObject gameWinText;
    public GameObject levelUpText;


    private string nextAction;
    private TurnManager tm;
    private UnitStats unit;
    private UnitStats targetUnit;
    private UnitStats player;
    private UnitStats enemy;

    private List<UnitStats> completeList;
    private List<UnitStats> nextUpList;
    private List<UnitStats> enemyList;

    private bool gameOver = false;


    //I SHOULD GRAB THE UNIT STATS SOONER?

    // Start is called before the first frame update
    void Start()
    {
        if (tm == null) {
            tm = this.gameObject.GetComponent<TurnManager>();
        }

        completeList = new List<UnitStats>();
        nextUpList = new List<UnitStats>();
        enemyList = new List<UnitStats>();

        player = GameObject.FindWithTag("Player").GetComponent<UnitStats>();
        completeList.Add(player);

        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyObj in enemyObjs) {
            UnitStats current = enemyObj.GetComponent<UnitStats>();
            enemyList.Add(current);
            completeList.Add(current);
        }

        InitialButtonDisplay();

    }
    
    private void InitialButtonDisplay()
    {
        unit = WhoseTurn();

        ShowUnitButtons(unit);
    }

    public UnitStats WhoseTurn()
    {
        if (nextUpList.Count == 0)
            RefreshTurnList();
        UnitStats current = nextUpList[0];

        return current;
    }

    void RefreshTurnList()
    {
        foreach (UnitStats unit in completeList)
        {
            nextUpList.Add(unit);
        }
        //objList.Sort((emp1, emp2) => emp1.FirstName.CompareTo(emp2.FirstName));

        nextUpList = nextUpList.OrderBy(w => w.speed).ToList();
        nextUpList.Reverse();
        //MAKE SORTING SLIGHTLY RANDOM, BUT HIGHER CHANCE IF HIGH STAT
    }

    void DisplayCurrentUnit()
    {
        print(nextUpList[0]);
    }

    void DisplayobjList()
    {
        foreach (UnitStats item in nextUpList)
        {
            print(item);
        }
    }

    public void SetAction(string action, Image buttonImage, bool targetable) {
        // print("start");
        nextAction = action;
        StopAllCoroutines();
        if (targetable) {
            IEnumerator coroutine = SelectTarget(buttonImage);
            StartCoroutine(coroutine);
        } else {
            buttonImage.color = Color.white;
            targetUnit = player;
            NextTurn(nextAction);
        }

    }

    public string GetAction() {
        return nextAction;
    }

    public void SetTargetToPlayer()
    {
        targetUnit = player;
    }

    IEnumerator SelectTarget(Image buttonImage)
    {
        bool targetSet = false;

        while (true) {
            // print("test");
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null) {
                // print("test");
                targetUnit = hit.collider.gameObject.GetComponent<UnitStats>();
                targetSet = true;

                SpriteRenderer renderer = targetUnit.GetComponent<SpriteRenderer>();
                renderer.color = Color.red;
                if (Input.GetMouseButtonDown(0)) {
                    // Debug.Log(hit.collider.gameObject.name);
                    ResetEnemyColors();
                    NextTurn(nextAction);
                    buttonImage.color = Color.white;
                    renderer.color = Color.white;
                    yield break;
                }
            } else if (targetSet == true){
                SpriteRenderer renderer = targetUnit.GetComponent<SpriteRenderer>();
                renderer.color = Color.white;
                targetUnit = null;
                targetSet = false;
            }
            yield return null;
        }
    }

    public void NextTurn(string action)
    {
        
        unit = WhoseTurn();

        //SendMessage bad?
        if (unit.TryMove(action,targetUnit)) {
            unit.TurnCheck();
            CheckDeaths();
            CheckGameOver();

            if (!gameOver) {
                // unit.TurnCheck();

                nextUpList.RemoveAt(0);

                ShowUnitButtons(WhoseTurn());
            }
        }
    }

    private void ShowUnitButtons(UnitStats current)
    {
        switch (current.tag)
        {
            case "Player":
                ShowPlayerButtons();
                HideEnemyButtons();

                break;
            case "Enemy":
                ShowEnemyButtons();
                HidePlayerButtons();

                break;
            default:
                print("Error");
                break;
        }
    }

    private void CheckGameOver()
    {
        if (player.health <= 0)
        {
            gameOver = true;
            GameOver();
        }
        if (enemyList.Count == 0) {
            gameOver = true;
            GameWin();
        }
    }

    private void GameWin()
    {
        HideAllButtons();

        gameWinText.SetActive(true);
        levelUpText.SetActive(true);

        Invoke("GameWinNext", 2.0f);
    }

    private void GameWinNext()
    {
        gameWinText.SetActive(false);
        levelUpText.SetActive(false);


        nextLvlButton.SetActive(true);

    }

    private void GameOver()
    {
        HideAllButtons();

        gameOverText.SetActive(true);
        player.HideGameObject();
    }

    private void CheckDeaths()
    {
        completeList.RemoveAll(item => item.isDead == true);
        enemyList.RemoveAll(item => item.isDead == true);
        nextUpList.RemoveAll(item => item.isDead == true);
    }

    private void ShowEnemyButtons()
    {
        enmButton.SetActive(true);
    }

    private void ShowPlayerButtons()
    {
        if (atkButton) {
            atkButton.SetActive(true);
        }

        if (dfdButton) {
            dfdButton.SetActive(true);
        }

        if (psnButton) {
            psnButton.SetActive(true);
        }

        if (healButton) {
            healButton.SetActive(true);
        }

        if (chgButton) {
            chgButton.SetActive(true);
        }
        
        if (lsrButton) {
            lsrButton.SetActive(true);
        }
    }

    private void HideAllButtons()
    {
        HidePlayerButtons();

        HideEnemyButtons();     
    }

    private void HidePlayerButtons()
    {
        if (atkButton) {
            atkButton.SetActive(false);
        }

        if (dfdButton) {
            dfdButton.SetActive(false);
        }
        
        if (psnButton) {
            psnButton.SetActive(false);
        } 

        if (healButton) {
            healButton.SetActive(false);
        } 

        if (chgButton) {
            chgButton.SetActive(false);
        } 

        if (lsrButton) {
            lsrButton.SetActive(false);
        }
    }

    private void HideEnemyButtons()
    {
        enmButton.SetActive(false);
    }

    private void ResetEnemyColors()
    {
        foreach (UnitStats enemy in enemyList) {
            // print(renderer.color);
            SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();
            renderer.color = Color.white;
        }
    }

}

//TODO: make speed, randomly assign turn order
//do damage
//stop game when dead or win
// 