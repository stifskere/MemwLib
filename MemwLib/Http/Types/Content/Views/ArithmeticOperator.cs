using System.Diagnostics;
using MemwLib.Http.Types.Content.Views.Exceptions;

namespace MemwLib.Http.Types.Content.Views;

public readonly struct ArithmeticOperator
{
    private static readonly (string Symbol, int Precedence)[] UnaryOperators = [
        ("++", 8),
        ("--", 8),
        ("!", 8),
        ("~", 8)
    ];

    private static readonly (string Symbol, int Precedence)[] BinaryOperators = [
        ("<<=", 4),
        (">>=", 4),
        ("<<", 4),
        (">>", 4),
        ("+=", 5),
        ("-=", 5),
        ("|=", 3),
        ("^=", 3),
        ("&=", 3),
        ("*=", 6),
        ("/=", 6),
        ("%=", 6),
        ("==", 2),
        ("!=", 2),
        ("<=", 2),
        (">=", 2),
        ("<", 2),
        (">", 2),
        ("|", 3),
        ("^", 3),
        ("&", 3),
        ("*", 6),
        ("/", 6),
        ("%", 6),
        ("+", 5),
        ("-", 5)
    ];
    
    public string Symbol { get; }
    public bool IsUnary { get; }
    public string MethodName { get; }
    public bool IsAssignment { get; }
    
    public OperatorPosition OperatorPosition
    {
        get
        {
            if (!IsUnary)
                throw new InvalidOperationException("Binary operators don't have a specific position.");

            return Symbol switch
            {
                "++" => OperatorPosition.Left | OperatorPosition.Right,
                "--" => OperatorPosition.Left | OperatorPosition.Right,
                "!" => OperatorPosition.Left,
                "~" => OperatorPosition.Left,
                _ => throw new UnreachableException("Validation failed for operator.")
            };
        }
    }

    public int Precedence
    {
        get
        {
            ArithmeticOperator self = this;

            return (IsUnary ? UnaryOperators : BinaryOperators)
                .First(o => o.Symbol == self.Symbol)
                .Precedence;
        }
    }

    public ArithmeticOperator(string op, bool isUnary)
    {
        Symbol = op;
        IsUnary = isUnary;
        
        string operatorName = !"==!=>=<=".Contains(op) && op.EndsWith('=') ? op[..^1] : op;

        string methodName = isUnary
            ? operatorName switch
            {
                "+" => "UnaryPlus",
                "-" => "UnaryNegation",
                "++" => "Increment",
                "--" => "Decrement",
                "!" => "LogicalNot",
                "~" => "OnesComplement",
                _ => throw new ArgumentException($"Unsupported unary operator: {op}", nameof(op))
            }
            : operatorName switch
            {
                "+" => "Addition",
                "-" => "Subtraction",
                "&" => "BitwiseAnd",
                "|" => "BitwiseOr",
                "^" => "ExclusiveOr",
                "==" => "Equality",
                "!=" => "Inequality",
                "<" => "LessThan",
                ">" => "GreaterThan",
                "<=" => "LessThanOrEqual",
                ">=" => "GreaterThanOrEqual",
                "<<" => "LeftShift",
                ">>" => "RightShift",
                "%" => "Modulus",
                "/" => "Division",
                "*" => "Multiply",
                _ => throw new ArgumentException($"Unsupported binary operator: {op}", nameof(op))
            };

        IsAssignment = operatorName != op;
        MethodName =  $"op_{methodName}";
    }
    
    public static List<ArithmeticOperator> SplitOperators(string expression)
    {
        List<ArithmeticOperator> operators = [];
        expression = expression.Replace(" ", "");
        string sortCopy = expression;
        
        foreach (((string, int)[] opList, bool isUnary) in new[] { (UnaryOperators, true), (BinaryOperators, false) })
        {
            foreach ((string op, _) in opList)
            {
                for (int i = 0; i < expression.Length - op.Length + 1; i++)
                {
                    if (expression.Substring(i, op.Length) != op) 
                        continue;
                    
                    operators.Add(new ArithmeticOperator(op, isUnary));
                    expression = expression.Remove(i, op.Length);
                    break;
                }
            }
        }

        if (!string.IsNullOrEmpty(expression))
            throw new InvalidArithmeticOperatorException(expression);

        operators.Sort((op1, op2) 
            => sortCopy.IndexOf(op1.Symbol, StringComparison.Ordinal) 
               - sortCopy.IndexOf(op2.Symbol, StringComparison.Ordinal));
        
        return operators;
    }
}

[Flags]
public enum OperatorPosition
{
    Left = 1,
    Right = 1 << 1
}