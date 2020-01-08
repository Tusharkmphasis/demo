using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEditor;

public class CrosswordsAlgorithm : MonoBehaviour {

    public List<WordPos> wordsposition = new List<WordPos>();
	
	public GameObject tile;
	
	public List<CrossedWord> CrToShow = new List<CrossedWord>();
	
	public List<GameObject> tileslist  = new List<GameObject>();	
	
	public int saveNumb = 0;

    public class WordPos
    {
       public List<GameObject> Word = new List<GameObject>();

    }

    public WordPos wordpos = new WordPos();

    private void Awake()
    {        
        print("------>>>" + CrToShow.Count);
        //tile = GameObject.Find("Tile");
    }

    // Use this for initialization
    void Start()
    {
        print("--CrToShow---->>>" + CrToShow.Count);

        print("--tileslist---->>>" + tileslist.Count);
        GenerateCrossWords();
        DrawGameTest();

       

    }

 

    // Update is called once per frame
    void Update () {
	
	}





    public void GenerateCrossWords()
    {
        List<CrossedWord> fixedwordsList = new List<CrossedWord>();

        //string[] words = System.IO.File.ReadAllLines("crossword.txt");

        //foreach (string line in words)
        //{
        //    fixedwordsList.Add(new CrossedWord(line.Trim(), ""));
        //}

        //int lineno = New.instace.RemainingList[New.NumberOfPragrency];
        print("---->>" + New.instace.RemainingList[New.NumberOfPragrency]);
        string pragnencyword = New.instace.PrgencyList[New.instace.RemainingList[New.NumberOfPragrency]].Trim();


        char[] myChars = pragnencyword.ToCharArray();


        string char1 = myChars[UnityEngine.Random.Range(0, pragnencyword.Length - 1)].ToString();
        string char2 = myChars[UnityEngine.Random.Range(0, pragnencyword.Length - 1)].ToString();
               
        fixedwordsList.Add(new CrossedWord(pragnencyword, ""));
        New.NumberOfPragrency++;



        for (int i = 0; i < New.instace.CommonList.Count; i++)
        {

            if (New.instace.CommonList[i].Contains(char1))
            {
                string commonword1 = New.instace.CommonList[i].Trim();
                fixedwordsList.Add(new CrossedWord(commonword1, ""));
                New.NumberOfCommon++;
                New.instace.CommonList.Remove(New.instace.CommonList[i]);
                break;
            }
        }

        for (int i = 0; i < New.instace.CommonList.Count; i++)
        {

            if (New.instace.CommonList[i].Contains(char2))
            {
                string commonword2 = New.instace.CommonList[i].Trim();
                fixedwordsList.Add(new CrossedWord(commonword2, ""));
                New.NumberOfCommon++;
                New.instace.CommonList.Remove(New.instace.CommonList[i]);
                break;
            }
        }





        //string commonword2 = New.instace.CommonList[New.NumberOfCommon].Trim();
        //fixedwordsList.Add(new CrossedWord(commonword2, ""));
        //New.NumberOfCommon++;



        // The very final crosswords

        List<CrossedWord> CrossWordsToKeep = new List<CrossedWord>();

        float crosswordLength = float.MaxValue;
        int WordsPlaced = 0;
        int crosswordMinX = 0, crosswordMinY = 0;

        // Looping to choose the best grid (looping arbitrary "the number of words" time) 

        for (int gen = 0; gen < fixedwordsList.Count; gen++)
        {
            // Not Touching the initial List
            List<CrossedWord> allWords = new List<CrossedWord>(fixedwordsList);

            if (gen % 2 == 1)
            {
                // Shuffling the words half the tries because sometimes not starting with the longer word can be a good option too
                for (int j = allWords.Count - 1; j > 0; j--)
                {
                    int r = UnityEngine.Random.Range(0, j + 1);
                    CrossedWord tmp = allWords[r];
                    allWords[r] = allWords[j];
                    allWords[j] = tmp;
                }
            }
            else
            {
                allWords.Sort();
                allWords.Reverse();
            }

            // The final crosswords for this loop only
            List<CrossedWord> finalWords = new List<CrossedWord>();

            // Adding the first word we found
            finalWords.Add(new CrossedWord(allWords[0]));
            // Removing this word from the list
            allWords.RemoveAt(0);

            // Initial size knowing the fact that the first word is Horizontal
            int minX = 0, maxX = finalWords[0].Size - 1, minY = 0, maxY = 0;

            // Loop on all the words we want in the crossword
            int maxLoop = Mathf.FloorToInt(allWords.Count * allWords.Count);

            int z = 0;
            int i = 0;
            for (; 0 != allWords.Count && z < maxLoop; z++)
            {

                // The current word we want to place 
                CrossedWord currentWordToPlace = new CrossedWord(allWords[i]);


                // Will tell us if we succeed placing it
                bool bIsPlaced = false;

                // Will always be the best position we find, initialise here with arbitrary values
                Tile BestStartingPosition = new Tile(0, 0);
                CrossedWord.Direction BestDirection = CrossedWord.Direction.Horizontal;

                // Will be a score to tell us which position is "conceptually" the best
                float score = float.MaxValue;

                // Loop on all the existing words in the crossword
                for (int j = 0; j < finalWords.Count; j++)
                {
                    // The current already placed word that we will used to try to place the new word
                    CrossedWord currentWordPlaced = finalWords[j];

                    // If we must placed the new one according to the existing one, the new one will be the other direction
                    currentWordToPlace.WordDirection = currentWordPlaced.WordDirection == CrossedWord.Direction.Horizontal ? CrossedWord.Direction.Vertical : CrossedWord.Direction.Horizontal;

                    // An array wich gave us for each letter (e.g. array[0] for the first letter) the tiles on which the current placed word has the same letter 
                    List<Tile>[] intersectionForEachLetter = currentWordPlaced.SimilarLetterTiles(currentWordToPlace);

                    // Loop on all the letters
                    for (int k = 0; k < intersectionForEachLetter.Length; k++)
                    {
                        // Looking for each given tile for one letter
                        for (int l = 0; l < intersectionForEachLetter[k].Count; l++)
                        {
                            // Getting the tile
                            Tile currentCommonTile = intersectionForEachLetter[k][l];

                            // Given the direction of the placed word and the intersection tile we calculate the new word potential starting position
                            if (currentWordPlaced.WordDirection == CrossedWord.Direction.Horizontal)
                            {
                                currentWordToPlace.StartingPosition = new Tile(currentCommonTile.X, currentCommonTile.Y - k);
                            }
                            else
                            {
                                currentWordToPlace.StartingPosition = new Tile(currentCommonTile.X - k, currentCommonTile.Y);
                            }

                            // Loop on all the words in the crossword to check if the place we want the new word isn't in conflict with the existings words
                            // the int to tell us how many correct intersection we have
                            int iCanBePlaced = 0;
                            // the boolean to tell us a conflict
                            bool bCanBePlaced = true;
                            for (int m = 0; m < finalWords.Count && bCanBePlaced; m++)
                            {
                                // ca = 0 means no conflict, -1 means a conflict, 1 means a good intersection
                                int ca = finalWords[m].CanAccept(currentWordToPlace);
                                if (ca > 0)
                                    iCanBePlaced += ca;
                                if (ca == -1)
                                    bCanBePlaced = false;
                            }

                            // The place is OK and have minimum one good intersection
                            if (bCanBePlaced && iCanBePlaced > 0)
                            {
                                // Calculate a score to find the best place

                                // how much intersection but the opposite value
                                int crossedNumber = (0 - iCanBePlaced);

                                // a conceptual score that should be the less the better
                                float tmpScore = UnityEngine.Random.Range(0, 10) + crossedNumber * 100;

                                // if this score si better than a previous one we keep this position and tell that we succeed placing at least one time this word
                                if (tmpScore < score)
                                {
                                    bIsPlaced = true;

                                    // Updating the new best score
                                    score = tmpScore;
                                    BestStartingPosition = currentWordToPlace.StartingPosition;
                                    BestDirection = currentWordToPlace.WordDirection;
                                }
                            }
                        }
                    }
                }

                // We have at least one position to place this new word
                if (bIsPlaced)
                {
                    // getting this saved position
                    currentWordToPlace.StartingPosition = BestStartingPosition;
                    currentWordToPlace.WordDirection = BestDirection;
                    // adding this word to the crossword
                    finalWords.Add(currentWordToPlace);

                    // Shuffling the crossword array to have more random factor on the grid creation (doesn't really matters but linear operation so it's ok)
                    for (int j = finalWords.Count - 1; j > 0; j--)
                    {
                        int r = UnityEngine.Random.Range(0, j + 1);
                        CrossedWord tmp = finalWords[r];
                        finalWords[r] = finalWords[j];
                        finalWords[j] = tmp;
                    }

                    // Updating the grid Rectangle if necessary
                    minX = Mathf.Min(minX, currentWordToPlace.StartingPosition.X);
                    minY = Mathf.Min(minY, currentWordToPlace.StartingPosition.Y);

                    maxX = Mathf.Max(maxX, currentWordToPlace.WordDirection == CrossedWord.Direction.Horizontal ? currentWordToPlace.StartingPosition.X + currentWordToPlace.Size - 1 : currentWordToPlace.StartingPosition.X);
                    maxY = Mathf.Max(maxY, currentWordToPlace.WordDirection == CrossedWord.Direction.Vertical ? currentWordToPlace.StartingPosition.Y + currentWordToPlace.Size - 1 : currentWordToPlace.StartingPosition.Y);

                    allWords.RemoveAt(i);
                    if (allWords.Count > 0)
                        i = i % allWords.Count;
                }
                else
                {
                    i = (i + 1) % allWords.Count;
                }
            }

            // Final new length of the grid
            float newLength = Mathf.Sqrt((maxX - minX) * (maxX - minX) + (maxY - minY) * (maxY - minY));
            // Final new number of word we succeed to put on the grid
            int currentWordsPlaced = finalWords.Count;

            // if it's a better grid (smaller and more words). Indeed, we allow a bigger crossword proportionally to how much more words it contains
            if (newLength - (currentWordsPlaced - WordsPlaced) * 4 < crosswordLength && WordsPlaced < currentWordsPlaced)
            {
                // Keeping this grid in memory
                CrossWordsToKeep = finalWords;
                // Updating best grid values
                crosswordLength = newLength;
                WordsPlaced = currentWordsPlaced;

                crosswordMinX = minX;
                crosswordMinY = minY;
            }

        }

        Debug.Log(CrossWordsToKeep.Count + "/" + fixedwordsList.Count + " size: " + crosswordLength);

        print("--CrossWordsToKeep.Count----->>>" + CrossWordsToKeep.Count);
        for (int r = 0; r < 3; r++)
        {
            CrossWordsToKeep[r].StartingPosition.X -= crosswordMinX;
            CrossWordsToKeep[r].StartingPosition.Y = -CrossWordsToKeep[r].StartingPosition.Y + crosswordMinY;
        }

        CrToShow = CrossWordsToKeep;

    }




    String Data = "";
    string Answers = "";
    float Row = 0;
    float Collum = 0;
    public void DrawGameTest()
    {
       
        foreach (GameObject go in tileslist)
        {

            Destroy(go);
        }
        tileslist.Clear();

        int position = 0;

        print("---DrawGameTest---->>>>>" + CrToShow.Count);
        foreach (CrossedWord crw in CrToShow)
        {
            WordPos wordpos = new WordPos();
            wordsposition.Add(wordpos);
          
            for (int i = 0; i < crw.Size; i++)
            {
                if (crw.WordDirection == CrossedWord.Direction.Horizontal)
                {
                   
                    Vector2 pos = new Vector2((crw.StartingPosition.X + i) * 100, crw.StartingPosition.Y * 100);
                    bool bAlreadyOnBoard = false;

                    //print("---CrossedWord---->>>>>" + i + "---->>" + tileslist.Count);
                    for (int j = 0; j < tileslist.Count && !bAlreadyOnBoard; j++)
                    {
                        if (tileslist[j].GetComponent<OTSprite>().position.Equals(pos))
                        {
                            bAlreadyOnBoard = true;
                        }
                    }
                    //if (!bAlreadyOnBoard)
                    {
                        GameObject aTile = Instantiate(tile) as GameObject;
                        aTile.GetComponent<OTSprite>().position = pos;
                        aTile.transform.parent = transform;
                        Answers += Char.ToUpper(crw.Word[i]);
                     
                        wordsposition[position].Word.Add(aTile);

                        aTile.GetComponentInChildren<OTTextSprite>().text = "" + Char.ToUpper(crw.Word[i]);
                        tileslist.Add(aTile);
                    }
                   
                }
                else
                {

                    Vector2 pos = new Vector2(crw.StartingPosition.X * 100, (crw.StartingPosition.Y - i) * 100);
                    bool bAlreadyOnBoard = false;
                    for (int j = 0; j < tileslist.Count && !bAlreadyOnBoard; j++)
                    {
                        if (tileslist[j].GetComponent<OTSprite>().position.Equals(pos))
                        {
                            bAlreadyOnBoard = true;
                        }
                    }
                    //if (!bAlreadyOnBoard)
                    {
                        GameObject aTile = Instantiate(tile) as GameObject;
                        aTile.GetComponent<OTSprite>().position = pos;
                        aTile.transform.parent = transform;
                        Answers += Char.ToUpper(crw.Word[i]);
                        //print("----Verticle-->>>>" + Char.ToUpper(crw.Word[i]));
                        wordsposition[position].Word.Add(aTile);
                        aTile.GetComponentInChildren<OTTextSprite>().text = "" + Char.ToUpper(crw.Word[i]);
                        tileslist.Add(aTile);
                    }

                }
            }
            position++;
            Answers += "|";
            print("----out of loop---->>>" + Answers);
        }

       


        float xMin = 0;
        float yMin = 0;
        float xMax = 0;
        float yMax = 0;



        for (int i = 0; i < tileslist.Count; i++)
        {
            if (tileslist[i].transform.position.x <= xMin)
            {
                xMin = tileslist[i].transform.position.x;
            }
            if (tileslist[i].transform.position.y <= yMin)
            {
                yMin = tileslist[i].transform.position.y;
            }

            if (tileslist[i].transform.position.x >= xMax)
            {
                xMax = tileslist[i].transform.position.x;
            }
            if (tileslist[i].transform.position.y >= yMax)
            {
                yMax = tileslist[i].transform.position.y;
            }

        }

        Row = (xMax) / 100;
        Collum = (Mathf.Abs(yMin)) / 100;


        int Writeposition = 0;

     
        foreach (CrossedWord crw in CrToShow)
        {



            for (float j = yMax; j >= yMin; j -= 100)
            {
                for (float k = xMin; k <= xMax; k += 100)
                {
                    for (int i = 0; i < wordsposition[Writeposition].Word.Count; i++)
                    {

                        if (wordsposition[Writeposition].Word[i].transform.position.x == k && wordsposition[Writeposition].Word[i].transform.position.y == j)
                        {
                            Data += wordsposition[Writeposition].Word[i].transform.GetChild(0).GetComponent<OTTextSprite>().text + " ";
                            //print("tileslist[i]------>>>>>" + wordsposition[Writeposition].Word[i].transform.GetChild(0).GetComponent<OTTextSprite>().text);
                            goto Found;
                        }

                    }
                    //print("+ ");
                    Data += "+ ";
                Found:
                    Data += "";
                }
                Data += "\n";
            }

           
            Data += "$";
            Data += "\n";

            Writeposition++;

        }
        print(Data);


        //String Data = "";
        //for (float j = yMax; j >= yMin; j -= 100)
        //{
        //    for (float k = xMin; k <= xMax; k += 100)
        //    {
        //        for (int i = 0; i < tileslist.Count; i++)
        //        {
        //            if (tileslist[i].transform.position.x == k && tileslist[i].transform.position.y == j)
        //            {
        //                Data += tileslist[i].transform.GetChild(0).GetComponent<OTTextSprite>().text + " ";
        //                //print("tileslist[i]------>>>>>" + tileslist[i].transform.GetChild(0).GetComponent<OTTextSprite>().text);
        //                goto Found;
        //            }

        //        }
        //        //print("+ ");
        //        Data += "+ ";
        //    Found:
        //        Data += "";
        //    }
        //    Data += "\n";
        //}

        //print(Data);


        //string[] linees = Data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        //for (int i = 0; i < linees.Length; i++)
        //{
        //    string[] Character = linees[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);


        //    for (int j = 0; j < Character.Length; j++)
        //    {
        //        print("------->>>>" + Character[j]);
        //        //    if (Character[j] != "+")
        //        //    {
        //        //        line.GenerateNumbers.Add(GameObject.Find("Cell" + i + "_" + j).GetComponent<RectTransform>());
        //        //    }
        //    }
        //}


        SaveMap();
    }
  

    public void SaveMap()
    {
        string saveString = "";
        //Create save string

       Answers = Answers.Substring(0, Answers.Length - 1);
        
        saveString += "Answers =" + Answers;
        saveString += "\r\n";
        saveString += "Row =" + (Row+1);
        saveString += "\r\n";
        saveString += "Collum =" + (Collum + 1);
        saveString += "\r\n";     
        Data = Data.Substring(0, Data.Length - 1);
        Data = Data.Substring(0, Data.Length - 1);
        saveString += Data;

        //set map data
        //for (int row = 0; row < maxRows; row++)
        //{
        //    for (int col = 0; col < maxCols; col++)
        //    {
        //        saveString += (int)levelSquares[row * maxCols + col];
        //        //if this column not yet end of row, add space between them
        //        if (col < (maxCols - 1))
        //            saveString += " ";
        //    }
        //    //if this row is not yet end of row, add new line symbol between rows
        //    if (row < (maxRows - 1))
        //        saveString += "\r\n";
        //}
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {


            //Write to file
            string activeDir = Application.dataPath + @"/Resources/Levels/";



            string newPath = System.IO.Path.Combine(activeDir, "Level_" + New.instace.RemainingList[New.NumberOfPragrency-1] + ".txt");
            StreamWriter sw = new StreamWriter(newPath);
            sw.Write(saveString);
            sw.Close();
            PlayerPrefs.SetInt("save", PlayerPrefs.GetInt("save", 0) + 1);
        }
        AssetDatabase.Refresh();
    }





    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 200), "Save"))
        {
            SaveMap();
            //saveNumb++;
            //print("------->>>>" + saveNumb);
            //FileStream fs = new FileStream("SavedCrosswords/crossword_" + saveNumb + ".dat", FileMode.Create);
            //BinaryFormatter formatter = new BinaryFormatter();
            //try
            //{
            //    formatter.Serialize(fs, CrToShow);
            //}
            //catch (SerializationException e)
            //{
            //    Debug.Log("Failed to serialize. Reason: " + e.Message);
            //    throw;
            //}
            //finally
            //{
            //    fs.Close();
            //}
        }


        //	if(GUI.Button(new Rect(200,0,200,200),"Load"))
        //	{

        //           // Open the file containing the data that you want to deserialize.
        //           print("--------->>>");
        //        FileStream fs = new FileStream("SavedCrosswords/crossword_"+saveNumb+".dat", FileMode.Open);
        //        try 
        //        {
        //            BinaryFormatter formatter = new BinaryFormatter();

        //            // Deserialize the hashtable from the file and  
        //            // assign the reference to the local variable.
        //            CrToShow = (List<CrossedWord>) formatter.Deserialize(fs);
        //			DrawGameTest();
        //        }
        //        catch (SerializationException e) 
        //        {
        //            Debug.Log("Failed to deserialize. Reason: " + e.Message);
        //            throw;
        //        }
        //        finally 
        //        {
        //            fs.Close();
        //        }
        //	}
        //	if(GUI.Button(new Rect(400,0,200,200),"Retry"))
        //	{
        //		GenerateCrossWords();
        //		DrawGameTest();
        //	}

    }
}
