//Skeleton Program code for the AQA A Level Paper 1 Summer 2025 examination
//this code should be used in conjunction with the Preliminary Material
//written by the AQA Programmer Team
//developed in the Visual Studio Community Edition programming environment

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TargetClearCS
{
    internal class Program
    {
        static Random RGen = new Random();

        static void Main(string[] args)
        {
            List<int> NumbersAllowed = new List<int>();
            List<int> LargeNumbers = new List<int> { 25, 50, 75, 100 };
            List<int> Targets;
            int MaxNumberOfTargets = 20;
            int MaxTarget;
            int MaxNumber;
            bool TrainingGame;
            string difficultyInput = "";

            Console.Write("Enter y to play the training game, anything else to play a random game: ");
            string Choice = Console.ReadLine().ToLower();
            Console.WriteLine();
            if (Choice == "y")
            {
                MaxNumber = 1000;
                MaxTarget = 1000;
                TrainingGame = true;
                Targets = new List<int> { -1, -1, -1, -1, -1, 23, 9, 140, 82, 121, 34, 45, 68, 75, 34, 23, 119, 43, 23, 119 };
            }
            else
            {
                Console.WriteLine("Select difficulty: \nStandard\nEasy\nMedium\nHard");
                difficultyInput = Console.ReadLine().ToLower();

                MaxNumber = 10;
                MaxTarget = 50;
                TrainingGame = false;
                Targets = CreateTargets(MaxNumberOfTargets, MaxTarget);
            }

            NumbersAllowed = FillNumbers(LargeNumbers, NumbersAllowed, TrainingGame, MaxNumber, difficultyInput);
            PlayGame(LargeNumbers, Targets, NumbersAllowed, TrainingGame, MaxTarget, MaxNumber, difficultyInput);
            Console.ReadLine();
        }

        static void PlayGame(List<int> LargeNumbers, List<int> Targets, List<int> NumbersAllowed, bool TrainingGame, int MaxTarget, int MaxNumber, string UserDifficulty)
        {
            int Score = 0;
            bool GameOver = false;
            string UserInput;
            List<string> UserInputInRPN;
            while (!GameOver)
            {
                DisplayState(Targets, NumbersAllowed, Score);
                Console.Write("Enter an expression: ");
                UserInput = Console.ReadLine();

                bool validNonNum = false;

                switch (UserInput.ToUpper())
                {
                    case "QUIT":
                        GameOver = true;
                        validNonNum = true;
                        break;

                    case "MOVE":
                        MoveTargetsBack(Targets, ref Score);
                        validNonNum = true;
                        break;
                }

                Console.WriteLine();
                if (validNonNum != true)
                {
                    if (CheckIfUserInputValid(UserInput))
                    {
                        UserInputInRPN = ConvertToRPN(UserInput);
                        if (CheckNumbersUsedAreAllInNumbersAllowed(NumbersAllowed, UserInputInRPN, MaxNumber))
                        {
                            if (CheckIfUserInputEvaluationIsATarget(Targets, UserInputInRPN, ref Score))
                            {
                                RemoveNumbersUsed(UserInput, MaxNumber, NumbersAllowed);
                                NumbersAllowed = FillNumbers(LargeNumbers, NumbersAllowed, TrainingGame, MaxNumber, UserDifficulty);
                            }
                        }
                    }
                }

                if (validNonNum == true)
                {
                    Score++;
                }

                Score--;
                if (Targets[0] != -1)
                {
                    GameOver = true;
                }
                else
                {
                    UpdateTargets(Targets, TrainingGame, MaxTarget);
                }
            }
            Console.WriteLine("Game over!");
            DisplayScore(Score);
        }

        static void MoveTargetsBack(List<int> Targets, ref int Score)
        {
            for (int index = Targets.Count - 1; index > 0; index--)
            {
                Targets[index] = Targets[index - 1];
            }
            Targets[0] = -1;
            Score -= 2;
        }

        static bool CheckIfUserInputEvaluationIsATarget(List<int> Targets, List<string> UserInputInRPN, ref int Score)
        {
            int OperatorCount = 0;
            int UserInputEvaluation = EvaluateRPN(UserInputInRPN, ref OperatorCount);
            Console.WriteLine($"{OperatorCount}");
            bool UserInputEvaluationIsATarget = false;
            if (UserInputEvaluation != -1)
            {
                for (int Count = 0; Count < Targets.Count; Count++)
                {
                    if (Targets[Count] == UserInputEvaluation)
                    {
                        Score += 2;
                        Console.WriteLine($"Score {Score} + {2 * OperatorCount}");
                        Score = Score + (2 * OperatorCount); //Bonus points
                        Targets[Count] = -1;
                        UserInputEvaluationIsATarget = true;
                    }
                }
            }

            if (UserInputEvaluationIsATarget == false)
            {
                Console.WriteLine("Input does not evaluate to a target.");
            }
            return UserInputEvaluationIsATarget;
        }

        static void RemoveNumbersUsed(string UserInput, int MaxNumber, List<int> NumbersAllowed)
        {
            List<string> UserInputInRPN = ConvertToRPN(UserInput);
            foreach (string Item in UserInputInRPN)
            {
                if (CheckValidNumber(Item, MaxNumber))
                {
                    if (NumbersAllowed.Contains(Convert.ToInt32(Item)))
                    {
                        NumbersAllowed.Remove(Convert.ToInt32(Item));
                    }
                }
            }
        }

        static void UpdateTargets(List<int> Targets, bool TrainingGame, int MaxTarget)
        {
            Targets.RemoveAt(0);
            
            if (TrainingGame)
            {
                Targets.Add(Targets[Targets.Count - 1]);
            }
            else
            {
                Targets.Add(GetTarget(MaxTarget));
            }
        }

        static bool CheckNumbersUsedAreAllInNumbersAllowed(List<int> NumbersAllowed, List<string> UserInputInRPN, int MaxNumber)
        {
            List<int> Temp = new List<int>();
            foreach (int Item in NumbersAllowed)
            {
                Temp.Add(Item);
            }
            foreach (string Item in UserInputInRPN)
            {
                if (CheckValidNumber(Item, MaxNumber))
                {
                    if (Temp.Contains(Convert.ToInt32(Item)))
                    {
                        Temp.Remove(Convert.ToInt32(Item));
                    }
                    else
                    {
                        Console.WriteLine("Use only the valid numbers!");
                        return false;
                    }
                }
            }
            return true;
        }

        static bool CheckValidNumber(string Item, int MaxNumber)
        {
            if (Regex.IsMatch(Item, "^[0-9]+$"))
            {
                int ItemAsInteger = Convert.ToInt32(Item);
                if (ItemAsInteger > 0 && ItemAsInteger <= MaxNumber)
                {
                    return true;
                }
            }
            return false;
        }

        static void DisplayState(List<int> Targets, List<int> NumbersAllowed, int Score)
        {
            DisplayTargets(Targets);
            DisplayNumbersAllowed(NumbersAllowed);
            DisplayScore(Score);
        }

        static void DisplayScore(int Score)
        {
            Console.WriteLine($"Current score: {Score}");
            Console.WriteLine();
            Console.WriteLine();
        }

        static void DisplayNumbersAllowed(List<int> NumbersAllowed)
        {
            Console.Write("Numbers available: ");
            foreach (int Number in NumbersAllowed)
            {
                Console.Write($"{Number}  ");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        static void DisplayTargets(List<int> Targets)
        {
            Console.Write("|");
            foreach (int T in Targets)
            {
                if (T == -1)
                {
                    Console.Write(" ");
                }
                else
                {
                    Console.Write(T);
                }
                Console.Write("|");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        static List<string> ConvertToRPN(string UserInput)
        {
            int Position = 0;
            Dictionary<string, int> Precedence = new Dictionary<string, int>
            {
                { "+", 2 }, { "-", 2 }, { "*", 4 }, { "/", 4 }
            };
            List<string> Operators = new List<string>();
            int Operand = GetNumberFromUserInput(UserInput, ref Position);
            List<string> UserInputInRPN = new List<string> { Operand.ToString() };
            Operators.Add(UserInput[Position - 1].ToString());
            while (Position < UserInput.Length)
            {
                Operand = GetNumberFromUserInput(UserInput, ref Position);
                UserInputInRPN.Add(Operand.ToString());
                if (Position < UserInput.Length)
                {
                    string CurrentOperator = UserInput[Position - 1].ToString();
                    while (Operators.Count > 0 && Precedence[Operators[Operators.Count - 1]] > Precedence[CurrentOperator])
                    {
                        UserInputInRPN.Add(Operators[Operators.Count - 1]);
                        Operators.RemoveAt(Operators.Count - 1);
                    }
                    if (Operators.Count > 0 && Precedence[Operators[Operators.Count - 1]] == Precedence[CurrentOperator])
                    {
                        UserInputInRPN.Add(Operators[Operators.Count - 1]);
                        Operators.RemoveAt(Operators.Count - 1);
                    }
                    Operators.Add(CurrentOperator);
                }
                else
                {
                    while (Operators.Count > 0)
                    {
                        UserInputInRPN.Add(Operators[Operators.Count - 1]);
                        Operators.RemoveAt(Operators.Count - 1);
                    }
                }
            }

            for (int i = 0; i < UserInputInRPN.Count; i++)
            {
                Console.Write($"{UserInputInRPN[i]}");
            }
            Console.WriteLine();
            
            return UserInputInRPN;
        }

        static int EvaluateRPN(List<string> UserInputInRPN, ref int OperatorCount)
        {
            List<string> S = new List<string>();
            while (UserInputInRPN.Count > 0)
            {
                while (!"+-*/".Contains(UserInputInRPN[0]))
                {
                    S.Add(UserInputInRPN[0]);
                    OperatorCount++;
                    UserInputInRPN.RemoveAt(0);
                }
                double Num2 = Convert.ToDouble(S[S.Count - 1]);
                S.RemoveAt(S.Count - 1);
                double Num1 = Convert.ToDouble(S[S.Count - 1]);
                S.RemoveAt(S.Count - 1);
                double Result = 0;
                switch (UserInputInRPN[0])
                {
                    case "+":
                        Result = Num1 + Num2;
                        break;

                    case "-":
                        Result = Num1 - Num2;
                        break;

                    case "*":
                        Result = Num1 * Num2;
                        break;

                    case "/":
                        Result = Num1 / Num2;
                        break;
                }
                UserInputInRPN.RemoveAt(0);
                S.Add(Convert.ToString(Result));

            }
            if (Convert.ToDouble(S[0]) - Math.Truncate(Convert.ToDouble(S[0])) == 0.0)
            {
                return (int)Math.Truncate(Convert.ToDouble(S[0]));
            }
            else
            {
                return -1;
            }
        }

        static int GetNumberFromUserInput(string UserInput, ref int Position)
        {
            string Number = "";
            bool MoreDigits = true;
            while (MoreDigits)
            {
                if (Regex.IsMatch(UserInput[Position].ToString(), "[0-9]"))
                {
                    Number += UserInput[Position];
                }
                else
                {
                    MoreDigits = false;
                }
                Position++;
                if (Position == UserInput.Length)
                {
                    MoreDigits = false;
                }
            }
            if (Number == "")
            {
                return -1;
            }
            else
            {
                return Convert.ToInt32(Number);
            }
        }

        static bool CheckIfUserInputValid(string UserInput)
        {
            if (Regex.IsMatch(UserInput, @"\/0+") == true)
            {
                Console.WriteLine("Do not divide by 0.");
                return false;
            }
            else if (Regex.IsMatch(UserInput, @"^([0-9]+[\+\-\*\/])+[0-9]+$") == true)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Infix notation only");
                return false;
            }
        }

        static int GetTarget(int MaxTarget)
        {
            return RGen.Next(MaxTarget) + 1;
        }

        static int GetNumber(int MaxNumber)
        {
            return RGen.Next(MaxNumber) + 1;
        }

        static List<int> CreateTargets(int SizeOfTargets, int MaxTarget)
        {
            List<int> Targets = new List<int>();
            for (int Count = 1; Count <= 5; Count++)
            {
                Targets.Add(-1);
            }
            for (int Count = 1; Count <= SizeOfTargets - 5; Count++)
            {
                Targets.Add(GetTarget(MaxTarget));
            }
            return Targets;
        }

        static List<int> FillNumbers(List<int> LargeNumbers, List<int> NumbersAllowed, bool TrainingGame, int MaxNumber, string UserDifficulty)
        {
            if (TrainingGame)
            {
                return new List<int> { 2, 3, 2, 8, 512 };
            }
            else
            {
                switch (UserDifficulty)
                {
                    case "standard":
                        while (NumbersAllowed.Count < 5)
                        {
                            NumbersAllowed.Add(GetNumber(MaxNumber));
                        }
                        break;
                    case "easy":
                        for (int LargeNumberCount = 0; LargeNumberCount < 1; LargeNumberCount++)
                        {
                            NumbersAllowed.Add(LargeNumbers[LargeNumberCount]);
                        }
                        while (NumbersAllowed.Count < 5)
                        {
                            NumbersAllowed.Add(GetNumber(MaxNumber));
                        }
                        break;
                    case "medium":
                        for (int LargeNumberCount = 0; LargeNumberCount < 2; LargeNumberCount++)
                        {
                            NumbersAllowed.Add(LargeNumbers[LargeNumberCount]);
                        }
                        while (NumbersAllowed.Count < 5)
                        {
                            NumbersAllowed.Add(GetNumber(MaxNumber));
                        }
                        break;
                    case "hard":
                        for (int LargeNumberCount = 0; LargeNumberCount < 4; LargeNumberCount++)
                        {
                            NumbersAllowed.Add(LargeNumbers[LargeNumberCount]);
                        }
                        while (NumbersAllowed.Count < 5)
                        {
                            NumbersAllowed.Add(GetNumber(MaxNumber));
                        }
                        break;
                    default:
                        break;
                }

                return NumbersAllowed;
            }
        }
    }
}