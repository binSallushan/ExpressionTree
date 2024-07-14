using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTree_Calculator
{
    public static class StringParser
    {
        public static Queue<string> Parse(string equation)
        {
            equation = equation.Replace(" ", string.Empty);
            //equation = "0" + equation;
            ValidateEquation(equation);

            Queue<string> result = new();

            bool containsDivision = false;
            bool containsMultiplication = false;
            bool containsAddition = false;
            bool containsSubtraction = false;

            //checking what the equation contains
            for (int i = 1; i < equation.Length; i++)
            {
                var c = equation[i];
                if (c == '*')
                    containsMultiplication = true;
                else if (c == '/')
                    containsDivision = true;
                else if (c == '+' && (equation[i - 1] != '*' && equation[i - 1] != '/'))
                    containsAddition = true;
                else if (c == '-' && (equation[i - 1] != '*' && equation[i - 1] != '/'))
                    containsSubtraction = true;
            }

            bool onlyContainsDivisionAndMultiplication = (containsDivision || containsMultiplication) && (!containsAddition && !containsSubtraction);
            bool onlyContainsSubtractionAndAddition = (containsSubtraction || containsAddition) && (!containsDivision && !containsMultiplication);
            if (!(containsAddition || containsSubtraction || containsDivision || containsMultiplication))
                result.Enqueue(equation);
            else if (onlyContainsDivisionAndMultiplication || onlyContainsSubtractionAndAddition)
            {
                var j = equation.Length - 1;
                //simple binary tree from right to left
                for (int i = equation.Length - 1; i > -1; i--)
                {
                    if (i == 0)
                        result.Enqueue(equation.Substring(i, j + 1));
                    else if(Expression.validSigns.Contains(equation[i].ToString()) && !Expression.validSigns.Contains(equation[i - 1].ToString()))
                    {
                        result.Enqueue(equation[i].ToString());
                        result.Enqueue(equation.Substring(i + 1, j - i));
                        j = i - 1;
                    }

                }
            }
            else            
                result = ParseComplex(equation);
                        
            return result;
        }

        private static Queue<string> ParseComplex(string equation)
        {
            var result = new Queue<string>();
            if (equation.Contains('+'))
            {

                var queueToAdd = (GetQueueFromOperator('+', equation));
                foreach(var item in queueToAdd)
                    result.Enqueue(item.ToString());                       
            }
            else if (equation.Contains('-')) 
            {
                var queueToAdd = (GetQueueFromOperator('-', equation));
                foreach (var item in queueToAdd)
                    result.Enqueue(item.ToString());                
            }
            else if (equation.Contains('*'))
            {
                var queueToAdd = (GetQueueFromOperator('*', equation));
                foreach (var item in queueToAdd)
                    result.Enqueue(item.ToString());                
            }
            else if (equation.Contains('/'))
            {
                var queueToAdd = (GetQueueFromOperator('/', equation));
                foreach (var item in queueToAdd)
                    result.Enqueue(item.ToString());                
            }
            else { result.Enqueue(equation); }
            return result;
        }
        private static Queue<string> GetQueueFromOperator(char operatorChar, string equation) 
        {
            var result = new Queue<string>();
            var index = equation.IndexOf(operatorChar);
            result.Enqueue(operatorChar.ToString());

            var rightSide = equation.Substring(index + 1);
            var leftSide = equation.Substring(0, index);

            var rightSideQueue = ParseComplex(rightSide);
            foreach (var item in rightSideQueue)
                result.Enqueue(item);

            var leftSideQueue = ParseComplex(leftSide);
            foreach(var item in leftSideQueue)
                result.Enqueue(item);

            return result;
        }

        private static void ValidateEquation(string equation)
        {
            if (equation.StartsWith("/") || equation.StartsWith("*") || equation.EndsWith("/") || equation.EndsWith("*"))
                throw new FormatException(nameof(equation));
            else
            {
                List<int> multiplicationIndexes = new();
                List<int> divisionIndexes = new();
                List<int> additionIndexes = new();
                List<int> subtractionIndexes = new();

                //store all operator indexes and check if illegal characters present
                for (int i = 0; i < equation.Length; i++)
                {
                    switch (equation[i])
                    {
                        case '+':
                            additionIndexes.Add(i);
                            break;
                        case '-':
                            subtractionIndexes.Add(i);
                            break;
                        case '*':
                            multiplicationIndexes.Add(i);
                            break;
                        case '/':
                            divisionIndexes.Add(i);
                            break;
                        default:
                            int.Parse(equation[i].ToString());
                            break;
                    }
                }

                containsConsecutiveIllegalOperator(multiplicationIndexes, multiplicationIndexes);

                containsConsecutiveIllegalOperator(multiplicationIndexes, divisionIndexes);

                containsConsecutiveIllegalOperator(divisionIndexes, divisionIndexes);

                containsConsecutiveIllegalOperator(divisionIndexes, multiplicationIndexes);

                containsConsecutiveIllegalOperator(additionIndexes, divisionIndexes);

                containsConsecutiveIllegalOperator(additionIndexes, multiplicationIndexes);

                containsConsecutiveIllegalOperator(subtractionIndexes, divisionIndexes);

                containsConsecutiveIllegalOperator(subtractionIndexes, multiplicationIndexes);


            }

            bool containsConsecutiveIllegalOperator(List<int> indexesOperatorToCheck, List<int> indexesIllegalOperator)
            {
                var containsConsecutiveIllegalOperator = false;
                indexesOperatorToCheck.ForEach(x => { if (indexesIllegalOperator.Contains(x + 1)) throw new FormatException($"{nameof(equation)} contains consecutive illegal characters"); });
                return containsConsecutiveIllegalOperator;
            }

        }
    }
}
