using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public static Dictionary<QuestionType, Question[]> questions;
    public static BinaryFormatter binaryFormatter = new BinaryFormatter();

    public static string binFileName = "questions";


    // Start is called before the first frame update
    void Start()
    {

       /* GenerateQuestions();

        WriteQuestionsToBin();

        // Clean up the genereated version 
        questions.Clear();
        questions = null;*/

        ReadQuestionsFromBin();
    }

    public static void GenerateQuestions()
    {
        questions = new Dictionary<QuestionType, Question[]>();

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
    public static void ReadQuestionsFromBin()
    {

        var fi = new System.IO.FileInfo(@binFileName);

        if (fi.Exists)
        {
            try
            {
                using (var binaryFile = fi.OpenRead())
                {
                    questions = (Dictionary<QuestionType, Question[]>)binaryFormatter.Deserialize(binaryFile);
                }

                Debug.Log("Read bin successful!");
            }
            catch (System.Exception e)
            {
                throw e;
            }

        }
        else
        {
            Debug.Log("Bin file doesn't exist");
        }


    }
    public static void WriteQuestionsToBin()
    {


        var fi = new System.IO.FileInfo(@binFileName);

        try
        {
            using (var binaryFile = fi.Create())
            {
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



}
