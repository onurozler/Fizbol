using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public Dictionary<QuestionType, Question[]> questions;

    private Question currentQuestion;

    private static string binFileName = "questions.bytes";

    public GameObject gamePanel;
    public GameObject questionPanel;
    public GameObject helpPanel;
    public GameObject popupPanel;
    public GameObject trueOrFalse;

    public Text questionText;
    public InputField answerInput;
    public Text answerText;
    

    public LevelManager levelManager;

    private QuestionType[] questionTypes = (QuestionType[])Enum.GetValues(typeof(QuestionType));

    private int incorrectAnswerCount = 0;

    private int renewQuestionCount = 3;

    private float lastshotTimestamp = 0f;

    private System.Random random = new System.Random(System.DateTime.Now.Millisecond);

    QuestionType currentQuestionType;

    // Start is called before the first frame update
    void Start()
    {
        ReadQuestionsFromBin();
    }

    private void GenerateQuestions()
    {
        // Initialize a new Dictionary Object that will hold all types of questions
        questions = new Dictionary<QuestionType, Question[]>();

        // Generate questions of types one by one and add all of them to the dictionary

        List<Question> maxHQuestions = QuestionGenerator.GenerateSpeed_AngleToMaxH();
        questions.Add(QuestionType.Speed_AngleToMaxH, maxHQuestions.ToArray());

        questions.Add(QuestionType.Speed_AngleToMaxX, QuestionGenerator.GenerateSpeed_AngleToMaxX().ToArray());

        List<Question> flightTimeQuestions = QuestionGenerator.GenerateSpeed_AngleToFlightTime();
        questions.Add(QuestionType.Speed_AngleToFlightTime, flightTimeQuestions.ToArray());

        questions.Add(QuestionType.MaxH_AngleToMaxX, QuestionGenerator.GenerateMaxH_AngleToMaxX(maxHQuestions).ToArray());

        questions.Add(QuestionType.MaxH_AngleToSpeed, QuestionGenerator.GenerateMaxH_AngleToSpeed(maxHQuestions).ToArray());

        questions.Add(QuestionType.MaxHToFlightTime, QuestionGenerator.GenerateMaxHToFlightTime().ToArray());

        questions.Add(QuestionType.SpeedX_FlightTimeToSpeed, QuestionGenerator.GenerateSpeedX_FlightTimeToSpeed().ToArray());

        questions.Add(QuestionType.Angle_Time_DistanceToSpeed, QuestionGenerator.GenerateAngle_Time_DistanceToSpeed().ToArray());

        questions.Add(QuestionType.Speed_AngleToTc, QuestionGenerator.GenerateSpeed_AngleToTc().ToArray());

        questions.Add(QuestionType.Speed_Time_AngleToHeight, QuestionGenerator.GenerateSpeed_Time_AngleToHeight(flightTimeQuestions).ToArray());

        questions.Add(QuestionType.Speed_Time_AngleToDistance, QuestionGenerator.GenerateSpeed_Time_AngleToDistance(flightTimeQuestions).ToArray());

        questions.Add(QuestionType.SpeedYToMaxH, QuestionGenerator.GenerateSpeedYToMaxH().ToArray());

        questions.Add(QuestionType.SpeedX_FlightTimeToDistance, QuestionGenerator.GenerateSpeedX_FlightTimeToDistance().ToArray());

        questions.Add(QuestionType.SpeedYToFlightTime, QuestionGenerator.GenerateSpeedYToFlightTime().ToArray());

    }
    private void ReadQuestionsFromBin()
    {
        // To read a binary from Assets, we need to get its unity relative path and load it with TextAsset
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(binFileName);
        TextAsset textAsset = Resources.Load(fileNameWithoutExtension) as TextAsset;
        
        try
        {
            // Open the bin file in byte format and read it directly to our dictionary
            using (Stream stream = new MemoryStream(textAsset.bytes))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                questions = formatter.Deserialize(stream) as Dictionary<QuestionType, Question[]>;
            }

            Debug.Log("Read bin successful!");
        }
        catch (System.Exception e)
        {
            throw e;
        }


    }
    private void WriteQuestionsToBin()
    {

        // Create a new file in project folder
        var fi = new System.IO.FileInfo(@binFileName);

        try
        {
            // Open file stream, serialize the dictionary and write it to the bin file.
            using (var binaryFile = fi.Create())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(binaryFile, questions);
                binaryFile.Flush();
            }

            Debug.Log("Write to bin successful!");
        }
        catch (System.Exception e)
        {
            throw e;
        }

    }
    public void AskQuestion(bool repeatLastQuestion)
    {
        
        if (!repeatLastQuestion)
        {

            // Switch to question UI
            gamePanel.SetActive(false);
            questionPanel.SetActive(true);

            incorrectAnswerCount = 0;

            // Get a random question type
            currentQuestionType = questionTypes[random.Next(questionTypes.Length)];

            // Get a random question of that type
            currentQuestion = questions[currentQuestionType][random.Next(questions[currentQuestionType].Length)];

            levelManager.DisplayInfoSprites(currentQuestion.angle, currentQuestion.distance);

            questionText.text = GetQuestionText();

            answerText.text = currentQuestion.result.ToString();
            Debug.Log("Answer: " + currentQuestion.result);
        }
        else
        {

            // when the incorrectAnswerCount hits 4, show the correct answer, other wise show wrong answer message
            if (incorrectAnswerCount > 3)
            {
                popupPanel.GetComponentInChildren<Text>().text = "Hakkınız Doldu!\nDoğru cevap :" + currentQuestion.result + ".";
                popupPanel.SetActive(true);
            }
            else
            {
                // Show the wrong indication
                trueOrFalse.GetComponent<Text>().color = Color.red;
                trueOrFalse.GetComponent<Text>().text = "Yanlış!";
                trueOrFalse.GetComponent<Animator>().SetTrigger("setAnim");
                ClosePopup();
            }
        }
    }

    public void RenewQuestion()
    {
        if (renewQuestionCount>0)
        {
            renewQuestionCount--;

            AskQuestion(false);
        }
        else
        {
            popupPanel.SetActive(true);
            popupPanel.GetComponentInChildren<Text>().text = "Soru değiştirme\nhakkınız bitti!";
        }
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
        // Switch to question UI
        questionPanel.SetActive(true);

    }

    public void ToggleHelpPanel()
    {
        questionPanel.SetActive(!questionPanel.activeSelf);
        helpPanel.SetActive(!helpPanel.activeSelf);
    }

    private string GetQuestionText()
    {
        switch (currentQuestionType)
        {
            case QuestionType.Speed_AngleToMaxH:
                return "Hızı " + currentQuestion.speed + "m/s olan top " + currentQuestion.angle +
                    " derece açı ile atıldığına göre, çıkabileceği en yüksek nokta yerden kaç metredir?";
            case QuestionType.Speed_AngleToMaxX:
                return "Hızı " + currentQuestion.speed + "m/s olan top " + currentQuestion.angle +
                    " derece açı ile atıldığına göre, ulaşabileceği en uzak nokta yatay eksende kaç metredir?";
            case QuestionType.Speed_AngleToFlightTime:
                return "Hızı " + currentQuestion.speed + "m/s olan top " + currentQuestion.angle +
                    " derece açı ile atıldığına göre, topun yere düşmesi kaç saniye sürer?";
            case QuestionType.MaxH_AngleToMaxX:
                return "Çıkabileceği en yüksek nokta " + currentQuestion.maxH +
                    "m olan top " + currentQuestion.angle + " derece açı ile atıldığına göre, ulaşabileceği en uzak nokta yatay eksende kaç metredir?";
            case QuestionType.MaxH_AngleToSpeed:
                return "Çıkabileceği en yüksek nokta " + currentQuestion.maxH +
                    "m olan top " + currentQuestion.angle + " derece açı ile atıldığına göre, topun ilk hızı kaç m/s'dir?";
            case QuestionType.MaxHToFlightTime:
                return "Çıkabileceği en yüksek nokta " + currentQuestion.maxH +
                    "m olan topun yere düşmesi kaç saniye sürer?";
            case QuestionType.SpeedX_FlightTimeToSpeed:
                return "Yatay hızı " + currentQuestion.speedX + "m/s olan top " + currentQuestion.flightTime +
                    " saniyede yere düştüğüne göre, topun fırlatılma hızı kaç m/s'dir?";
            case QuestionType.Angle_Time_DistanceToSpeed:
                return currentQuestion.angle + " derece açı ile vurulan top " + currentQuestion.timePassed +
                    " saniyede yatay eksende " + currentQuestion.distance + "m yol aldığına göre, topun vurulma hızı kaç m/s'dir?";
            case QuestionType.Speed_AngleToTc:
                return "Hızı " + currentQuestion.speed + "m/s olan top " + currentQuestion.angle +
                    " derece açı ile atıldığına göre, çıkabileceği en yüksek noktaya çıkış süresi kaç saniyedir?";
            case QuestionType.Speed_Time_AngleToHeight:
                return "Hızı " + currentQuestion.speed + "m/s olan top " + currentQuestion.angle +
                    " derece açı ile atıldığına göre, topun " + currentQuestion.timePassed + "s sonra yerden yüksekliği kaç metredir?";
            case QuestionType.Speed_Time_AngleToDistance:
                return "Hızı " + currentQuestion.speed + "m/s olan top " + currentQuestion.angle +
                    " derece açı ile atıldığına göre, topun " + currentQuestion.timePassed + "s sonra yatay eksende katettiği yol kaç metredir?";
            case QuestionType.SpeedYToMaxH:
                return "Dikey hızı " + currentQuestion.speedY + "m/s olan topun yerden çıkabileceği en yüksek nokta kaç metredir?";
            case QuestionType.SpeedX_FlightTimeToDistance:
                return "Yatay hızı " + currentQuestion.speedX + "m/s olan top " + currentQuestion.flightTime +
                    " saniyede yere düştüğüne göre, topun menzili kaç metredir?";
            case QuestionType.SpeedYToFlightTime:
                return "Dikey hızı " + currentQuestion.speedY + "m/s olan topun havada kalma süresi kaç saniyedir?";

        }
        return "An error occured.";
    }

    public void CheckAnswer()
    {

        // This prevents user from spamming the "Vur" button.
        if ((Time.time-lastshotTimestamp)<2f)
        {
            return;
        }

        lastshotTimestamp = Time.time;

        // TryParse prevents throwing an exception when user's answer is blank
        int.TryParse(answerInput.text, out int answer);

        // Get angle, if it doesn't exist in question, set 60 deg as default
        int angle = (currentQuestion.angle == 0) ? 60 : currentQuestion.angle;
        // Clear answer
        answerInput.text = "";

        // Switch the active UI and return control to LevelManager
        gamePanel.SetActive(true);
        questionPanel.SetActive(false);

        if (answer == currentQuestion.result)
        {
            setGoalsToday(false);

            levelManager.isShootPressed(angle,1f,1f);
        }
        else
        {
            
            incorrectAnswerCount++;


            // If the answer is incorrect, change the route of the ball slightly to reflect the wrong answer
            // ex: shoot over the target if the answer is greater than real result
            float distanceMultiplier = (answer * 1f) / (currentQuestion.result * 1f);

            if (distanceMultiplier > 5f)
            {
                distanceMultiplier = 5f;
            }
            else if (distanceMultiplier > 1f && distanceMultiplier < 1.1f)
            {
                distanceMultiplier = 1.1f;
            }
            else if (distanceMultiplier < 1f && distanceMultiplier > 0.85f)
            {
                distanceMultiplier = 0.85f;
            }
            else if (distanceMultiplier < 0.2)
            {
                distanceMultiplier = 0.2f;
            }

            // Prevent angle from passing 80 deg
            float angleMultiplier = (distanceMultiplier > 80f / angle) ? 80f / angle : distanceMultiplier;

            levelManager.isShootPressed(angle, angleMultiplier, distanceMultiplier);

            setGoalsToday(true);
        }
    }


    public void setGoalsToday(bool fail)
    {
        string lastStatistic = PlayerPrefs.GetString("playerStatistic");
        // Get last record
        string[] getWhole = lastStatistic.Split(',');

        string[] getLastDay = getWhole[getWhole.Length - 1].Split('-');
        string[] getAnswers = getLastDay[1].Split(';');

        int trueAnswer = int.Parse(getAnswers[0]);
        int falseAnswer = int.Parse(getAnswers[1]);

        if (fail)
            falseAnswer++;
        else if (!fail && incorrectAnswerCount <= 3)
        {
            trueAnswer++;
        }
            

        PlayerPrefs.SetString("playerStatistic", "");
        for (int i = 0; i < getWhole.Length - 1; i++)
        {
            PlayerPrefs.SetString("playerStatistic", PlayerPrefs.GetString("playerStatistic") + getWhole[i] + ",");
        }

        PlayerPrefs.SetString("playerStatistic", PlayerPrefs.GetString("playerStatistic") + getLastDay[0] + "-" + trueAnswer + ";" + falseAnswer);
    }
}
