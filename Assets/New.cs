using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class New : MonoBehaviour
{

    public List<string> PrgencyList = new List<string>();
    public List<string> CommonList = new List<string>();

    public List<int> RemainingList = new List<int>();

    public TextAsset PragnecyText;
    public TextAsset CommonText;
    public static New instace;

    public static int NumberOfPragrency = 0;
    public static int NumberOfCommon = 0;

    public GameObject Game;

    // Use this for initialization
    void Awake()
    {
        instace = this;
        TextAsset text = PragnecyText;

        string[] Words = text.ToString().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);


        foreach (string word in Words)
        {
            PrgencyList.Add(word);

        }



        TextAsset text1 = CommonText;

        string[] Words1 = text1.ToString().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);


        foreach (string word in Words1)
        {
            CommonList.Add(word);

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Start()
    {
		StartCoroutine(GenerateCrossWordsinumrator());
	}

	IEnumerator GenerateCrossWordsinumrator()
	{
		GameObject games;
		yield return new WaitForSeconds(1);
		for (int i = 0; i < RemainingList.Count; i++)
		{

            games = Instantiate(Game);
			yield return new WaitForSeconds(1);

			Destroy(games);
			
			}

		}
	}

