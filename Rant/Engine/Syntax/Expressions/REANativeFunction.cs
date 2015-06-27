﻿using Rant.Engine.ObjectModel;
using Rant.Stringes;
using System;
using System.Collections.Generic;

namespace Rant.Engine.Syntax.Expressions
{
    internal class REANativeFunction : RantExpressionAction
    {
        private int _argCount;
        public RantObject That;
        private RantFunctionInfo _function;

        public int ArgCount => _argCount;

        public REANativeFunction(Stringe token, int argCount, RantFunctionInfo info)
            : base(token)
        {
            _argCount = argCount;
            _function = info;
        }

        public override object GetValue(Sandbox sb)
        {
            return this;
        }

        public IEnumerator<RantExpressionAction> Execute(Sandbox sb)
        {
            List<object> args = new List<object>();
            for (var i = 0; i < _argCount; i++)
                args.Add(new RantObject(sb.ScriptObjectStack.Pop()));
            args.Add(That);
            args.Reverse();
            IEnumerator<RantAction> iterator = null;
            while (true)
            {
                try
                {
                    if(iterator == null)
                        iterator = _function.Invoke(sb, args.ToArray());
                    if (!iterator.MoveNext())
                        break;
                }
                // attach token to it and throw it up
                catch (RantRuntimeException e)
                {
                    e.SetToken(Range);
                    throw e;
                }
                yield return iterator.Current as RantExpressionAction;
            }
        }

        public override IEnumerator<RantAction> Run(Sandbox sb)
        {
            yield break;
        }
    }
}
