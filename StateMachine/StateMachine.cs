using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMachine
{
    public abstract class StateMachine<T>
    {
        public List<When_> Whens;

        protected StateMachine()
        {
            Whens = new List<When_>();
        }

        public When_ When(Rule rule)
        {
            When_ when = new When_(rule);
            when.Rules.Add(rule);
            Whens.Add(when);
            return when;
        }

        public abstract bool Next();

        public void Run(T t)
        {
            while (Next())
            {
                Whens.ForEach(x => x.Eval(t));

                When_ when = Whens.FirstOrDefault(x => x.IsTrue);

                if (when != null)
                {
                    when.Actions.ForEach(x => x(t));
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public class When_
        {
            public When_(Rule rule)
            {
                Rules = new List<Rule>
                {
                    rule
                };
                Actions = new List<Action<T>>();
            }

            public List<Rule> Rules { get; set; }

            public List<Action<T>> Actions { get; set; }

            public bool IsTrue { get; set; }

            public When_ And(Rule rule)
            {
                Rules.Add(rule);
                return this;
            }

            public Then_ Then(Action<T> action)
            {
                Then_ then = new Then_(this);
                Actions.Add(action);
                return then;
            }

            public bool Eval(T t)
            {
                return IsTrue = Rules.All(x => x.Eval(t));
            }
        }

        // ReSharper disable once InconsistentNaming
        public class Then_
        {
            private readonly When_ _when;

            public Then_(When_ when)
            {
                _when = when;
            }

            public Then_ And(Action<T> action)
            {
                _when.Actions.Add(action);
                return this;
            }
        }

        public class Rule
        {
            private readonly Predicate<T> _predicate;

            public Rule(Predicate<T> predicate)
            {
                _predicate = predicate;
            }

            public bool Eval(T t)
            {
                return IsTrue = _predicate(t);
            }

            public bool IsTrue { get; set; }
        }
    }
}
