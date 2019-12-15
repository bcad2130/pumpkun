using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GenericButton : MonoBehaviour
{
    // public UnitStats unit;
    public string action;
    public string LevelToLoad;
    public bool targetable;

    private TurnManager tm;

    // Start is called before the first frame update
    void Start()
    {
        // print("test");
        tm = GameObject.FindObjectOfType<TurnManager>();

        //tm.NextTurn();
        //print("start");

        if (this.transform.parent.GetComponent<Image>().color == Color.red) {
            this.transform.parent.GetComponent<Image>().color = Color.white;
        }
    }

    public void Push()
    {
        tm.NextTurn(action);
    }

    public void SetAction() 
    {
        ResetButtonColors();
        // var colors = this.GetComponent<Button> ().colors;
        // colors.normalColor = Color.red;
        // GetComponent<Button> ().colors = colors;
        // GetComponent<Image>().color = Color.red;
        // ColorBlock color = GetComponent<Button>().colors;
        var buttonImage = this.transform.parent.GetComponent<Image>();
        buttonImage.color = Color.red;
        // test.color = new Color(255, 0, 0);
        tm.SetAction(action, buttonImage, targetable);
        // this.opacity()
    }

    public void EnemyAction() 
    {
        // tm.SetTarget()
        tm.SetTargetToPlayer();
        tm.NextTurn(action);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void loadLevel() 
    {
		//Load the level from LevelToLoad
		SceneManager.LoadScene(LevelToLoad);
	}

    private void ResetButtonColors() 
    {
        // print("Resest");
        GameObject[] buttons;
        buttons = GameObject.FindGameObjectsWithTag("PlayerButton");

        foreach (GameObject button in buttons)
        {
            // print(button);
            var buttonImage = button.GetComponent<Image>();
            buttonImage.color = Color.white;
        }
    }
}
