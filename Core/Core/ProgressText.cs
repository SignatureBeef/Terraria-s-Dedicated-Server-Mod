using System;
using OTA.Plugin;
using Terraria;

namespace TDSM.Core
{
    public partial class Entry
    {
        static readonly System.Text.RegularExpressions.Regex _fmtGeneration = new System.Text.RegularExpressions.Regex(".* - (.*) - (.*)%");
        static readonly System.Text.RegularExpressions.Regex _fmtSemi = new System.Text.RegularExpressions.Regex("(.*): (.*)%");
        static readonly System.Text.RegularExpressions.Regex _fmtDefault = new System.Text.RegularExpressions.Regex("(.*) (.*)%");

        string GetProgressKey(string input, out string progress)
        {
            progress = String.Empty;

            var gen = _fmtGeneration.Matches(input);
            if (gen != null && gen.Count == 1 && gen[0].Groups.Count == 3)
            {
                progress = gen[0].Groups[2].Value + '%';
                return gen[0].Groups[1].Value;
            }
            gen = _fmtSemi.Matches(input);
            if (gen != null && gen.Count == 1 && gen[0].Groups.Count == 3)
            {
                progress = gen[0].Groups[2].Value + '%';
                return gen[0].Groups[1].Value;
            }
            gen = _fmtDefault.Matches(input);
            if (gen != null && gen.Count == 1 && gen[0].Groups.Count == 3)
            {
                progress = gen[0].Groups[2].Value + '%';
                return gen[0].Groups[1].Value;
            }

            return input;
        }

        private int lastWritten = 0;

        [Hook(HookOrder.NORMAL)]
        void OnStatusTextChanged(ref HookContext ctx, ref HookArgs.StatusTextChange args)
        {
            ctx.SetResult(HookResult.IGNORE);
            //There's no locking and two seperate threads, so we must use local variables incase of changes
            var statusText = Terraria.Main.statusText;
            var oldStatusText = Terraria.Main.oldStatusText;

            if (oldStatusText != statusText)
            {
                if (!String.IsNullOrEmpty(statusText))
                {
                    string previousProgress, currentProgress;

                    string keyA = GetProgressKey(oldStatusText, out previousProgress);
                    string keyB = GetProgressKey(statusText, out currentProgress);

                    if (keyA != null && keyB != null)
                    {
                        keyA = keyA.Trim();
                        keyB = keyB.Trim();
                        if (keyA.Length > 0 && keyB.Length > 0)
                        {
                            if (keyA == keyB)
                            {
                                if (lastWritten > 0)
                                {
                                    for (var x = 0; x < lastWritten; x++)
                                        Console.Write("\b");
                                }

                                Console.Write(currentProgress);
                                lastWritten += currentProgress.Length - lastWritten;
                            }
                            else
                            {
                                Console.WriteLine();
                                lastWritten = 0;
                                Console.Write(statusText);

                                lastWritten += currentProgress.Length;

                                if (currentProgress.Length == 0)
                                    Console.WriteLine();
                            }
                        }
                        else
                        {
                            if (lastWritten > 0)//!String.IsNullOrEmpty(oldStatusText)) //There was existing text
                            {
                                Console.WriteLine();
                                lastWritten = 0;
                            }

                            Console.Write(statusText);
                            lastWritten += currentProgress.Length;
                        }
                    }
                    else if (keyA == null && keyB != null)
                    {
                        Console.Write(statusText);
                        lastWritten += currentProgress.Length;
                    }
                }
                else
                {
                    if (lastWritten > 0)//!String.IsNullOrEmpty(oldStatusText)) //There was existing text
                    {
                        Console.WriteLine();
                        lastWritten = 0;
                    }
                }
            }
            else if (statusText == String.Empty)
            {
                if (lastWritten > 0)//!String.IsNullOrEmpty(Terraria.Main.oldStatusText)) //There was existing text
                {
                    Console.WriteLine();
                    lastWritten = 0;
                }
            }
            Terraria.Main.oldStatusText = statusText;
        }
    }
}

