using System;
using System.Collections.Generic;
using Automath.Elements;
using Automath.Lexical;

namespace Automath.Semantic
{
    internal class PostfixProcessor
    {
        private List<PostfixEntry> postfixEntries;

        public PostfixProcessor()
        {
            postfixEntries = new List<PostfixEntry>();
        }

        public int WriteCmd(ECmd cmd)
        {
            postfixEntries.Add(new PostfixEntry(EEntryType.etCmd, (int)cmd));
            return postfixEntries.Count - 1;
        }

        public int WriteVar(string varName)
        {
            postfixEntries.Add(new PostfixEntry(EEntryType.etVar, varName.GetHashCode()));
            return postfixEntries.Count - 1;
        }

        public int WriteConst(int value)
        {
            postfixEntries.Add(new PostfixEntry(EEntryType.etConst, value));
            return postfixEntries.Count - 1;
        }

        public int WriteCmdPtr(int ptr)
        {
            postfixEntries.Add(new PostfixEntry(EEntryType.etCmdPtr, ptr));
            return postfixEntries.Count - 1;
        }

        public void SetCmdPtr(int ind, int ptr)
        {
            if (postfixEntries[ind].Type == EEntryType.etCmdPtr)
            {
                postfixEntries[ind] = new PostfixEntry(EEntryType.etCmdPtr, ptr);
            }
            else
            {
                throw new Exception("Неверный тип записи для обновления указателя команды");
            }
        }

        public Dictionary<int, string> GetPostfixString(List<Lexeme> lexemes, int elsePtr)
        {
            int i = 0;
            Dictionary<int, string> orderedPostfixes = new Dictionary<int, string>();

            foreach (var entry in postfixEntries)
            {
                string entryDescription = "";

                switch (entry.Type)
                {
                    case EEntryType.etCmd:
                        entryDescription = ((ECmd)entry.Index).ToString();
                        break;

                    case EEntryType.etVar:                     
                        var varLexeme = lexemes.FirstOrDefault(l => l.Value.GetHashCode() == entry.Index);
                        entryDescription = varLexeme != null ? varLexeme.Value : $"Var#{entry.Index}";
                        break;

                    case EEntryType.etConst:
                        entryDescription = entry.Index.ToString();
                        break;

                    case EEntryType.etCmdPtr:
                        if (entry.Index == -1) entry.Index = elsePtr;
                        if (entry.Index == -2) entry.Index = postfixEntries.Count + 1;
                        entryDescription = entry.Index.ToString();
                        break;

                    default:
                        entryDescription = "Unknown";
                        break;
                }

                i++;
                orderedPostfixes.Add(i, entryDescription);
            }

            return orderedPostfixes;
        }

        public Dictionary<int, PostfixEntry> GetPostfixes(List<Lexeme> lexemes, int elsePtr)
        {
            Dictionary<int, PostfixEntry> pairs = new Dictionary<int, PostfixEntry>();
            int i = 1;

            foreach (var entry in postfixEntries)
            {
                if (entry.Type == EEntryType.etCmdPtr)
                {
                    if (entry.Index == -1) entry.Index = elsePtr;
                    if (entry.Index == -2) entry.Index = postfixEntries.Count + 1;
                }

                pairs.Add(i, entry);
                i++;
            }

            return pairs;
        }

        public void PrintPostfix()
        {
            Console.WriteLine("ПОЛИЗ:");
            for (int i = 0; i < postfixEntries.Count; i++)
            {
                var entry = postfixEntries[i];
                Console.WriteLine($"{i + 1}: {entry.Type} - {entry.Index}");
            }
        }
    }
}
