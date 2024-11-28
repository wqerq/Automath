using System;
using System.Collections.Generic;
using System.Linq;
using Automath.Semantic;
using Automath.Lexical;

namespace Automath.Syntax
{
    internal class Syntax
    {
        List<Lexeme> lexemes = new List<Lexeme>();
        private int currentIndex = 0;
        public PostfixProcessor postfixProcessor;
        Dictionary<string, int> variables = new Dictionary<string, int>();

        private Lexeme CurrentLexeme => currentIndex < lexemes.Count ? lexemes[currentIndex] : null;

        public Syntax(List<string> list)
        {
            foreach (var item in list)
            {
                var parts = item.Split();
                lexemes.Add(new Lexeme(parts[1], parts[2], int.Parse(parts[0])));
            }
            postfixProcessor = new PostfixProcessor();
        }

        public void Parse()
        {
            Console.WriteLine("Начало синтаксического анализа...");
            ParseIfStatement();
            Console.WriteLine("\nСинтаксический анализ завершен без ошибок.");
        }

        private void ParseIfStatement()
        {
            ExpectAndConsume("IF", "Анализ конструкции if...");
            ParseLogicalExpression();
            ExpectAndConsume("THEN", "Переход к анализу блока then...");

            postfixProcessor.WriteCmdPtr(-1);
            postfixProcessor.WriteCmd(Elements.ECmd.JZ);

            ParseStatementList();

            while (Match("ELSEIF"))
            {
                Consume("Анализ блока elseif...");
                ParseLogicalExpression();
                ExpectAndConsume("THEN", "Переход к анализу следующего блока...");

                postfixProcessor.WriteCmdPtr(-1);
                postfixProcessor.WriteCmd(Elements.ECmd.JZ);
                ParseStatementList();
            }

            if (Match("ELSE"))
            {
                Consume("Анализ блока else...");
                ParseStatementList();
            }

            ExpectAndConsume("END", "Конструкция if успешно завершена.");
        }

        private void ParseStatementList()
        {
            do
            {
                ParseStatement();
            } while (!MatchAndConsume("DLM", "Обнаружен разделитель."));
        }

        private void ParseStatement()
        {
            if (Match("ID"))
            {
                Console.WriteLine($"Обнаружен идентификатор '{CurrentLexeme.Value}'");
                Consume();
                ExpectAndConsume("ASGN", "Операция присваивания.");
                ParseArithmeticExpression();
                postfixProcessor.WriteCmd(Elements.ECmd.SET);
            }
            else if (Match("OUTPUT"))
            {
                Consume("Обнаружен оператор вывода.");
                ParseOperand();
            }
            else if (Match("IF"))
            {
                ParseIfStatement();
            }
            else
            {
                throw new Exception($"Ожидался оператор, но обнаружено '{CurrentLexeme?.Value}'");
            }
        }

        private void ParseLogicalExpression()
        {
            if (Match("NOT"))
            {
                Consume("Обнаружена унарная операция 'not'.");
                ParseComparisonExpression();
                postfixProcessor.WriteCmd(Elements.ECmd.NOT);
            }
            else
            {
                ParseComparisonExpression();

                while (Match("AND") || Match("OR"))
                {
                    string logicOp = CurrentLexeme.Value.ToUpper();
                    Consume($"Обнаружена бинарная операция '{logicOp}'.");
                    ParseComparisonExpression();
                    postfixProcessor.WriteCmd(logicOp == "AND" ? Elements.ECmd.AND : Elements.ECmd.OR);
                }
            }
        }

        private void ParseComparisonExpression()
        {
            ParseOperand();

            if (Match("REL"))
            {
                string comparisonOp = CurrentLexeme.Value;
                Consume($"Обнаружена операция сравнения '{comparisonOp}'");
                ParseOperand();

                switch (comparisonOp)
                {
                    case ">": postfixProcessor.WriteCmd(Elements.ECmd.CMPL); break;
                    case "<": postfixProcessor.WriteCmd(Elements.ECmd.CMPM); break;
                    case ">=": postfixProcessor.WriteCmd(Elements.ECmd.CMPLE); break;
                    case "<=": postfixProcessor.WriteCmd(Elements.ECmd.CMPME); break;
                    case "=": postfixProcessor.WriteCmd(Elements.ECmd.CMPE); break;
                    case "<>": postfixProcessor.WriteCmd(Elements.ECmd.CMPNE); break;
                }
            }
        }

        private void ParseArithmeticExpression()
        {
            ParseTerm();

            while (Match("MATH") && (CurrentLexeme.Value == "+" || CurrentLexeme.Value == "-"))
            {
                string op = CurrentLexeme.Value;
                Consume($"Обнаружена арифметическая операция '{op}'");
                ParseTerm();
                postfixProcessor.WriteCmd(op == "+" ? Elements.ECmd.ADD : Elements.ECmd.SUB);
            }
        }

        private void ParseTerm()
        {
            ParseFactor();

            while (Match("MATH") && (CurrentLexeme.Value == "*" || CurrentLexeme.Value == "/"))
            {
                string op = CurrentLexeme.Value;
                Consume($"Обнаружена арифметическая операция '{op}'");
                ParseFactor();
                postfixProcessor.WriteCmd(op == "*" ? Elements.ECmd.MUL : Elements.ECmd.DIV);
            }
        }

        private void ParseFactor()
        {
            if (Match("ID"))
            {
                Console.WriteLine($"Обнаружен идентификатор '{CurrentLexeme.Value}'");
                postfixProcessor.WriteVar(CurrentLexeme.Value);
                Consume();
            }
            else if (Match("NUM"))
            {
                Console.WriteLine($"Обнаружена константа '{CurrentLexeme.Value}'");
                postfixProcessor.WriteConst(int.Parse(CurrentLexeme.Value));
                Consume();
            }
            else if (Match("LPAREN"))
            {
                Consume("Обнаружены скобки, анализ вложенного выражения...");
                ParseArithmeticExpression();
                ExpectAndConsume("RPAREN", "Закрывающая скобка.");
            }
            else
            {
                throw new Exception("Ожидалось выражение, идентификатор или константа.");
            }
        }

        private void ExpectAndConsume(string type, string message)
        {
            Expect(type);
            Consume(message);
        }
        private void Expect(string type)
        {
            if (!Match(type))
            {
                throw new Exception($"Ожидалось '{type}', но обнаружено '{CurrentLexeme?.Type}'");
            }
        }

        private bool Match(string type)
        {
            return CurrentLexeme?.Type == type;
        }

        private void Consume(string message = null)
        {
            if (message != null)
                Console.WriteLine(message);
            currentIndex++;
        }
        private bool MatchAndConsume(string type, string message = null)
        {
            if (Match(type))
            {
                Consume(message);
                return true;
            }
            return false;
        }

        private void ParseOperand()
        {
            if (Match("ID"))
            {
                Console.WriteLine($"Обнаружен операнд (идентификатор) '{CurrentLexeme.Value}'");
                postfixProcessor.WriteVar(CurrentLexeme.Value);
                Consume();
            }
            else if (Match("NUM"))
            {
                Console.WriteLine($"Обнаружен операнд (константа) '{CurrentLexeme.Value}'");
                postfixProcessor.WriteConst(int.Parse(CurrentLexeme.Value));
                Consume();
            }
            else
            {
                throw new Exception($"Ожидался операнд, но обнаружено '{CurrentLexeme?.Value}'");
            }
        }
    }
}
