using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using Automath.Lexer;
using Automath.Semantic;

namespace Automath.Syntax
{
    internal class Syntax
    {
        List<Lexeme> lexemes = new List<Lexeme>();
        private int currentIndex = 0;
        private PostfixProcessor postfixProcessor;
        Dictionary<string, int> variables = new Dictionary<string, int>();

        private Lexeme CurrentLexeme => currentIndex < lexemes.Count ? lexemes[currentIndex] : null;

        public Syntax(List<string> list)
        {
            string line;
            foreach (var item in list)
            {
                var parts = item.Split();
                lexemes.Add(new Lexeme(parts[0], parts[1]));

            }         
            postfixProcessor = new PostfixProcessor();
        }

        public void Parse()
        {
            Console.WriteLine("Начало синтаксического анализа...");
            ParseIfStatement();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nСинтаксический анализ завершен без ошибок.");
            Console.ResetColor();
        }

        private void ParseIfStatement()
        {
            Expect("IF");
            Console.WriteLine("Анализ конструкции if...");
            ParseLogicalExpression();
            Expect("THEN");

            postfixProcessor.WriteCmdPtr(-1);
            postfixProcessor.WriteCmd(Elements.ECmd.JZ);

            Console.WriteLine("Анализ блока then...");
            ParseStatementList();

            while (Match("ELSEIF"))
            {
                Console.WriteLine("Анализ блока elseif...");
                ParseLogicalExpression();
                Expect("THEN");

                postfixProcessor.WriteCmdPtr(-1);
                postfixProcessor.WriteCmd(Elements.ECmd.JZ);
                ParseStatementList();
            }

            if (Match("ELSE"))
            {
                Console.WriteLine("Анализ блока else...");
                ParseStatementList();
            }

            Expect("END");
            Console.WriteLine("Конструкция if успешно проанализирована.");
        }

        private void ParseStatementList()
        {
            do
            {
                ParseStatement();
            } while (Match("DLM"));
        }

        private void ParseStatement()
        {
            if (Match("ID"))
            {
                Console.WriteLine($"Обнаружен идентификатор '{CurrentLexeme.Value}'");
                Expect("ASGN");
                ParseArithmeticExpression();
                postfixProcessor.WriteCmd(Elements.ECmd.SET);
            }
            else if (Match("OUTPUT"))
            {
                Console.WriteLine("Обнаружен оператор вывода.");
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
                Console.WriteLine("Обнаружена унарная операция 'not'.");
                ParseComparisonExpression();
                postfixProcessor.WriteCmd(Elements.ECmd.NOT);
            }
            else
            {
                ParseComparisonExpression();

                while (Match("AND") || Match("OR"))
                {
                    var logicOp = CurrentLexeme.Value.ToUpper();
                    Console.WriteLine($"Обнаружена бинарная операция '{logicOp}'.");
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
                Console.WriteLine($"Обнаружена операция сравнения '{CurrentLexeme.Value}'");
                var comparisonOp = CurrentLexeme.Value;
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
                var op = CurrentLexeme.Value;
                Console.WriteLine($"Обнаружена арифметическая операция '{op}'");
                currentIndex++;
                ParseTerm();
                postfixProcessor.WriteCmd(op == "+" ? Elements.ECmd.ADD : Elements.ECmd.SUB);
            }
        }

        private void ParseTerm()
        {
            ParseFactor();

            while (Match("MATH") && (CurrentLexeme.Value == "*" || CurrentLexeme.Value == "/"))
            {
                var op = CurrentLexeme.Value;
                Console.WriteLine($"Обнаружена арифметическая операция '{op}'");
                currentIndex++;
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
            }
            else if (Match("NUM"))
            {
                Console.WriteLine($"Обнаружена константа '{CurrentLexeme.Value}'");
                postfixProcessor.WriteConst(int.Parse(CurrentLexeme.Value));
            }
            else if (Match("LPAREN"))
            {
                Console.WriteLine("Обнаружены скобки, анализ вложенного выражения...");
                ParseArithmeticExpression();
                Expect("RPAREN");
            }
            else
            {
                throw new Exception("Ожидалось выражение, идентификатор или константа.");
            }
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
            if (CurrentLexeme?.Type == type)
            {
                currentIndex++;
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
            }
            else if (Match("NUM"))
            {
                Console.WriteLine($"Обнаружен операнд (константа) '{CurrentLexeme.Value}'");
                postfixProcessor.WriteConst(int.Parse(CurrentLexeme.Value));
            }
            else
            {
                throw new Exception($"Ожидался операнд, но обнаружено '{CurrentLexeme?.Value}'");
            }
        }
    }
}
