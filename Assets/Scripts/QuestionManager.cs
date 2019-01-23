using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public Dictionary<QuestionType, Question[]> questions;

    private Question currentQuestion;

    private static string binFileName = "questions.bytes";

    public GameObject gameCanvas;
    public GameObject questionCanvas;

    public TMP_Text questionText;
    public TMP_InputField answerInput;

    private bool is3D = true;

    public LevelManager levelManager;

    private QuestionType[] questionTypes = (QuestionType[])Enum.GetValues(typeof(QuestionType));



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
    public void AskQuestion()
    {
        // Switch to question UI
        gameCanvas.SetActive(false);
        questionCanvas.SetActive(true);

        // Get a random question type
        currentQuestionType = questionTypes[random.Next(questionTypes.Length)];

        // Get a random question of that type
        currentQuestion = questions[currentQuestionType][random.Next(questions[currentQuestionType].Length)];
        
        levelManager.DisplayInfoSprites(currentQuestion.angle,currentQuestion.distance);

        questionText.text = GetQuestionText();

        Debug.Log("Answer: " + currentQuestion.result);
        
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
        // TryParse prevents trhrowing an exception when user's answer is blank
        int.TryParse(answerInput.text, out int answer);

        // Clear answer
        answerInput.text = "";

        if (answer == currentQuestion.result)
        {           
            // Switch the active UI and return control to LevelManager
            gameCanvas.SetActive(true);
            questionCanvas.SetActive(false);

            // reset view variable since it switches to 3d by default
            is3D = true;

            // Give the angle a default value if it doesn't exist

            levelManager.isShootPressed((currentQuestion.angle == 0) ? 60 : currentQuestion.angle);
        }
        else
        {
            // If the answer is incorrect, change the route of the ball slightly to reflect the wrong answer
            // ex: shoot over the target if the answer is greater than real result
            float multiplier = (answer > currentQuestion.result) ? 1.4f : 0.4f;

            // 2 different types of change in route: speed or angle
            levelManager.isShootPressed((currentQuestion.angle == 0) ? 60 : currentQuestion.angle, Question.angleDominantQuestionTypes.Contains(currentQuestionType), multiplier);
        }
    }

    public void SwitchView()
    {
        levelManager.changeTo2D(is3D);
        is3D = !is3D;
    }
}
