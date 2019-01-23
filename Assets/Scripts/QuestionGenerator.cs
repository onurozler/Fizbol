using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestionGenerator
{
    private static Dictionary<int, float> sinValues = new Dictionary<int, float>()
    {
        {30, 0.5f},
        {37, 0.6f },
        {45, 0.7f},
        {53, 0.8f},
        {60, 0.85f}
    };

    private static System.Random r = new System.Random();

    public static int[] sinAnglesArray = sinValues.Keys.ToArray();

    public static float gravity = 10;

    public static List<Question> GenerateSpeed_AngleToMaxH()
    {
        List<Question> questions = new List<Question>();

        for (int i = 10; i <= 100; i += 5)
        {
            for (int j = 0; j < sinAnglesArray.Length; j++)
            {
                float maxH = GetMaxH(i, sinAnglesArray[j]);

                if (maxH % 1 <= (System.Double.Epsilon * 5))
                {
                    Question q = new Question()
                    {
                        speed = i,
                        angle = sinAnglesArray[j],
                        maxH = (int)maxH,
                        result = (int)maxH

                    };
                    questions.Add(q);
                }
            }
        }


        return questions;
    }
    public static List<Question> GenerateSpeed_AngleToMaxX()
    {
        List<Question> questions = new List<Question>();

        for (int i = 10; i <= 100; i += 5)
        {
            for (int j = 0; j < sinAnglesArray.Length; j++)
            {
                float maxX = GetMaxX(i, sinAnglesArray[j]);

                if (maxX % 1 <= (System.Double.Epsilon * 5))
                {
                    Question q = new Question()
                    {
                        speed = i,
                        angle = sinAnglesArray[j],
                        maxX = (int)maxX,
                        result = (int)maxX
                    };
                    questions.Add(q);
                }
            }
        }

        return questions;
    }
    public static List<Question> GenerateSpeed_AngleToFlightTime()
    {
        List<Question> questions = new List<Question>();

        for (int i = 10; i <= 100; i += 5)
        {
            for (int j = 0; j < sinAnglesArray.Length; j++)
            {
                float flightTime = GetFlightTime(i, sinAnglesArray[j]);

                if (flightTime % 1 <= (double.Epsilon * 5))
                {
                    Question q = new Question()
                    {
                        speed = i,
                        angle = sinAnglesArray[j],
                        flightTime = (int)flightTime,
                        result = (int)flightTime
                    };
                    questions.Add(q);
                }
            }
        }

        return questions;
    }
    public static List<Question> GenerateMaxH_AngleToMaxX(List<Question> maxHQuestions)
    {
        List<Question> questions = new List<Question>();

        foreach (var question in maxHQuestions)
        {
            float maxX = GetMaxXFromMaxH_Angle(question.maxH, question.angle);


            Question q = new Question()
            {
                maxH = question.maxH,
                angle = question.angle,
                maxX = (int)maxX,
                result = (int)maxX
            };
            questions.Add(q);

        }

        return questions;
    }
    public static List<Question> GenerateMaxH_AngleToSpeed(List<Question> maxHQuestions)
    {
        // The result of this function gives the same list as the maxHQuestions
        List<Question> questions = new List<Question>();

        foreach (var question in maxHQuestions)
        {
            Question q = new Question()
            {
                maxH = question.maxH,
                angle = question.angle,
                speed = question.speed,
                result = question.speed
            };
            questions.Add(q);

        }

        return questions;
    }
    public static List<Question> GenerateMaxHToFlightTime()
    {
        List<Question> questions = new List<Question>();

        for (int i = 1; i <= 10; i++)
        {
            Question q = new Question()
            {
                flightTime = i,
                maxH = i * i * 5,
                result = i

            };
            questions.Add(q);
        }

        return questions;
    }
    public static List<Question> GenerateSpeedX_FlightTimeToSpeed()
    {
        List<Question> questions = new List<Question>();

        for (int i = 1; i <= 5; i++)
        {
            //8,15,17
            questions.Add(new Question() { speedX = 8 * i, flightTime = 3 * i, speed = 17 * i, result = 17 * i});
            //12,5,13
            questions.Add(new Question() { speedX = 12 * i, flightTime = 1 * i, speed = 13 * i, result = 13 * i});
            //3,4,5
            questions.Add(new Question() { speedX = 3 * 5 * i, flightTime = 4 * i, speed = 5 * 5 * i, result = 5 * 5 * i});
            //4,3,5
            questions.Add(new Question() { speedX = 4 * 5 * i, flightTime = 3 * i, speed = 5 * 5 * i, result = 5 * 5 * i});
        }

        return questions;
    }
    public static List<Question> GenerateAngle_Time_DistanceToSpeed()
    {
        List<Question> questions = new List<Question>();

        for (int i = 5; i <= 50; i++)
        {
            for (int j = 2; j <= 10; j++)
            {
                for (int t = 0; t < sinAnglesArray.Length; t++)
                {
                    float speed = GetSpeedFromAngle_Time_Distance(sinAnglesArray[t], j, i);

                    if (speed % 1 <= (System.Double.Epsilon * 5))
                    {
                        Question q = new Question()
                        {
                            speed = (int)speed,
                            angle = sinAnglesArray[t],
                            timePassed = j,
                            distance = i,
                            result = (int)speed

                        };
                        questions.Add(q);
                    }
                }
            }
        }


        return questions;
    }
    public static List<Question> GenerateSpeed_AngleToTc()
    {
        List<Question> questions = new List<Question>();

        for (int i = 20; i <= 200; i += 5)
        {
            for (int j = 0; j < sinAnglesArray.Length; j++)
            {
                float flightTime = GetTC(i, sinAnglesArray[j]) * 2;

                if (flightTime % 1 <= (double.Epsilon * 5))
                {
                    Question q = new Question()
                    {
                        speed = i,
                        angle = sinAnglesArray[j],
                        flightTime = (int)flightTime,
                        result = ((int)flightTime)/2

                    };
                    questions.Add(q);
                }
            }
        }

        return questions;
    }
    public static List<Question> GenerateSpeed_Time_AngleToHeight(List<Question> flightTimeQuestions)
    {
        List<Question> questions = new List<Question>();

        foreach (var ftQuestion in flightTimeQuestions)
        {
            for (int i = 1; i < ftQuestion.flightTime; i++)
            {
                float height = GetHeightAtT(ftQuestion.speed, ftQuestion.angle, i);

                Question q = new Question()
                {
                    speed = ftQuestion.speed,
                    angle = ftQuestion.angle,
                    timePassed = i,
                    height = (int)height,
                    result = (int)height
                };
                questions.Add(q);
            }
        }

        return questions;
    }
    public static List<Question> GenerateSpeed_Time_AngleToDistance(List<Question> flightTimeQuestions)
    {
        List<Question> questions = new List<Question>();

        for (int i = 10; i <= 100; i += 5)
        {
            for (int j = 0; j < sinAnglesArray.Length; j++)
            {
                for (int t = 1; t <= 7; t++)
                {
                    float distance = GetXAtT(i, sinAnglesArray[j], t);

                    if (distance % 1 <= (System.Double.Epsilon * 5))
                    {
                        Question q = new Question()
                        {
                            speed = i,
                            angle = sinAnglesArray[j],
                            timePassed = t,
                            distance = (int)distance,
                            result = (int)distance
                        };
                        questions.Add(q);
                    }
                }
            }
        }

        return questions;
    }
    public static List<Question> GenerateSpeedYToMaxH()
    {
        List<Question> questions = new List<Question>();

        for (int i = 10; i <= 100; i += 10)
        {
            Question q = new Question()
            {
                speedY = i,
                maxH = (int)GetMaxH(i),
                result = (int)GetMaxH(i)
            };
            questions.Add(q);
        }

        return questions;
    }
    public static List<Question> GenerateSpeedX_FlightTimeToDistance()
    {
        List<Question> questions = new List<Question>();

        for (int i = 0; i < 30; i++)
        {
            Question q = new Question()
            {
                speedX = r.Next(10, 100),
                flightTime = r.Next(1, 20)
            };
            q.distance = q.speedX * q.flightTime;
            q.result = q.distance;
            questions.Add(q);
        }

        return questions;
    }
    public static List<Question> GenerateSpeedYToFlightTime()
    {
        List<Question> questions = new List<Question>();

        for (int i = 0; i < 20; i++)
        {
            Question q = new Question()
            {
                speedY = r.Next(10, 50) * 5
            };

            q.flightTime = (int)GetFlightTime(q.speedY);
            q.result = q.flightTime;
            questions.Add(q);
        }

        return questions;
    }


    public static float GetMaxH(float speed, int angle)
    {
        return (speed * speed * sinValues[angle] * sinValues[angle]) / (2f * gravity);
    }
    public static float GetMaxH(float speedY)
    {
        return (speedY * speedY) / (2f * gravity);
    }
    public static float GetSpeedFromMaxH_Angle(float maxH, int angle)
    {
        return Mathf.Sqrt(2 * gravity * maxH) * (1 / sinValues[angle]);
    }
    public static float GetMaxX(float speed, int angle)
    {
        return (2f * speed * speed * sinValues[angle] * sinValues[90 - angle]) / gravity;
    }
    public static float GetMaxX(float speedX, float flightTime)
    {
        return speedX * flightTime;
    }
    public static float GetMaxXFromMaxH_Angle(float maxH, int angle)
    {
        float speed = Mathf.Sqrt(2 * gravity * maxH) * (1 / sinValues[angle]);
        return GetMaxX(speed, angle);
    }
    public static float GetFlightTime(float speed, int angle)
    {
        return (2f * speed * sinValues[angle]) / gravity;
    }
    public static float GetFlightTime(float speedY)
    {
        return (2f * speedY) / gravity;
    }
    public static float GetHeightAtT(float speed, int angle, float time)
    {
        return speed * sinValues[angle] * time - 0.5f * gravity * time * time;
    }
    public static float GetXAtT(float speed, int angle, float time)
    {
        return speed * sinValues[90 - angle] * time;
    }
    public static float GetTC(float speed, int angle)
    {
        return (speed * sinValues[angle]) / gravity;
    }
    public static float GetSpeedFromAngle_Time_Distance(int angle, float timePassed, float distance)
    {
        float horizontalSpeed = distance / timePassed;
        return horizontalSpeed * (1f / sinValues[90 - angle]);
    }



}
