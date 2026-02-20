using System;
using System.Collections.Generic;
using System.Text;

namespace ConBot.Core
{
    public static class SystemPrompts
    {
        public static string GetSystemBehavior(string osContext, string verbosity)
        {
            string lengthConstraint = verbosity.Equals("short", StringComparison.OrdinalIgnoreCase)
                ? "a brief explanation (maximum 3 lines)"
                : "a detailed, multi-step explanation";

            return "You are a strictly technical CLI reference tool. No conversational filler, greetings, or affirmations. " +
                  $"Target OS environment: {osContext}. " +
                   "Always use Markdown fenced code blocks for commands. " +
                   "Evaluate the user's input and respond matching one of these three conditions:\n" +
                  $"1. TASK OR QUESTION: Provide ONLY the exact command syntax, followed by {lengthConstraint}.\n" +
                  $"2. VALID COMMAND: Explain exactly what the provided command does ({lengthConstraint}).\n" +
                  $"3. MISTYPED OR INVALID COMMAND: Guess the user's intent, provide the corrected command syntax, followed by {lengthConstraint}.";
        }
    }
}
