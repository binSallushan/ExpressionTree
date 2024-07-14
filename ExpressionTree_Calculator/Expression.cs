using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTree_Calculator
{
    public abstract class Expression
    {
        public static string[] validSigns = new string[] { "+", "-", "/", "*" };
        public abstract double Evaluate();

        public static Expression GenerateExpressionTree(Queue<string> stack)
        {
            Expression expression;
            if (Expression.validSigns.Contains(stack.Peek()))
            {
                var sign = stack.Dequeue();
                expression = new BinaryExpression() { Sign = sign, RightExpression = GenerateExpressionTree(stack) };
                (expression as BinaryExpression).LeftExpression = GenerateExpressionTree(stack);
            }
            else
                expression = new ConstantExpression(double.Parse(stack.Dequeue()));

            return expression;
        }
    }

    internal class BinaryExpression : Expression
    {
        public Expression LeftExpression { get; set; }
        public Expression RightExpression { get; set; }
        private string sign;
        public string Sign
        {
            get
            { 
                return sign;
            }
            set
            {
                if (!validSigns.Contains(value)) throw new ArgumentException($"{nameof(sign)} should be \"+\", \"-\", \"/\", \"*\"");
                sign = value;
            }
        }        
        public BinaryExpression(Expression left, Expression right, string sign)
        {
            this.LeftExpression = left ?? throw new ArgumentNullException(nameof(left));
            this.RightExpression = right ?? throw new ArgumentNullException(nameof(right));
            this.Sign = validSigns.Contains(sign) ? sign : throw new ArgumentException($"{nameof(sign)} should be \"+\", \"-\", \"/\", \"*\"");
        }

        public BinaryExpression()
        {

        }

        public override double Evaluate()
        {
            if (LeftExpression is null)
                throw new ArgumentNullException(nameof(LeftExpression));
            if (RightExpression is null) throw new ArgumentNullException( nameof(RightExpression));
            return this.Sign switch
            {
                "+" => LeftExpression.Evaluate() + RightExpression.Evaluate(),
                "-" => LeftExpression.Evaluate() - RightExpression.Evaluate(),
                "/" => LeftExpression.Evaluate() / RightExpression.Evaluate(),
                _ => LeftExpression.Evaluate() * RightExpression.Evaluate(),
            };
        }
    }

    internal class ConstantExpression : Expression
    {
        public double Value { get; set; }
        public ConstantExpression(double value) { this.Value = value; }
        public ConstantExpression() { }
        public override double Evaluate() { return this.Value; }
    }
}
