using System;
using System.Collections.Generic;

namespace SALT.Console
{
    public abstract class DynamicCommand : ConsoleCommand
    {
        public virtual Func<string[], bool> ExecuteFunc { get; }

        public virtual Func<int, string, List<string>> AutoComplete { get; }

        public override bool Execute(string[] args)
        {
            Func<string[], bool> executeFunc = this.ExecuteFunc;
            return executeFunc != null && executeFunc(args);
        }

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            Func<int, string, List<string>> autoComplete = this.AutoComplete;
            return autoComplete == null ? base.GetAutoComplete(argIndex, argText) : autoComplete(argIndex, argText);
        }

        protected override bool ArgsOutOfBounds(int argCount, int min = -1, int max = -1)
        {
            return false;
        }
    }
}
