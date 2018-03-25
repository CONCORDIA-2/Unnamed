using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class QuestionnaireScript : MonoBehaviour {
    
    //storage for all questions by category
    public GameObject[] dualQuestions, ravenQuestions, rabbitQuestions;
    public GameObject waitScreen;

    //decider score is the score used to determine which player receives which character. Positive scores lean toward the rabbit, negatives lean towards the raven
    //currentQuestion stores the index of the current question in its respective array
    //questionCount is the total number of questions which have been presented to the player
    private int deciderScore, currentQuestion, questionCount;

    public GameObject networkManager;
    public bool isHost;

    // Use this for initialization
    void Start () {
        //load the first question
        loadQuestion1();
	}

    //the method called whenever the player answers a question. Value of answer is 1 for left hand answer, -1 for right hand answer
    public void processAnswer(int answer)
    {
        if (answer == 1 || answer == -1) {
            switch (questionCount)
            {
                case 1:
                    if (answer == 1)
                        deciderScore--;
                    else
                        deciderScore++;
                    loadQuestion2();
                    break;
                case 2:
                    if (answer == 1)
                        deciderScore--;
                    else
                        deciderScore++;
                    loadQuestion3();
                    break;
                case 3:
                    if (answer == 1)
                        deciderScore--;
                    loadQuestion4();
                    break;
                case 4:
                    if (answer == 1)
                        deciderScore++;
                    loadWaitScreen();
                    break;
            }
        }
    }

    //select a random dual question and make it visible
    private void loadQuestion1()
    {
        currentQuestion = (int)Random.Range(0, dualQuestions.Length);
        dualQuestions[currentQuestion].SetActive(true);
        questionCount++;
    }

    //select a different dual question than the one use in loadQA1(), and make it the new visible question.
    private void loadQuestion2()
    {
        int selectQuestion;
        do
        {
            selectQuestion = (int)Random.Range(0, dualQuestions.Length);
        } while (selectQuestion == currentQuestion);

        dualQuestions[currentQuestion].SetActive(false);
        dualQuestions[selectQuestion].SetActive(true);

        currentQuestion = selectQuestion;

        questionCount++;
    }

    //select a random raven question and make it the new visible question.
    private void loadQuestion3()
    {
        dualQuestions[currentQuestion].SetActive(false);
        currentQuestion = (int)Random.Range(0, ravenQuestions.Length);
        ravenQuestions[currentQuestion].SetActive(true);
        questionCount++;
    }

    //select a random rabbit question and make it the new visible question.
    private void loadQuestion4()
    {
        ravenQuestions[currentQuestion].SetActive(false);
        currentQuestion = (int)Random.Range(0, rabbitQuestions.Length);
        rabbitQuestions[currentQuestion].SetActive(true);
        questionCount++;
    }

    //
    private void loadWaitScreen()
    {
        rabbitQuestions[currentQuestion].SetActive(false);
        waitScreen.SetActive(true);
        if (isHost)
            networkManager.GetComponent<NetworkInitializer>().StartHost();
        else
            networkManager.GetComponent<NetworkInitializer>().StartClient();
    }

    public class ReadyMessage : MessageBase
    {

    }
}
