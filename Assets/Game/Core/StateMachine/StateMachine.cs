using System;

namespace Entropa.Core.StateMachine
{
    public sealed class StateMachine<TContext>
    {
        private readonly TContext _context;
        private IState<TContext> _current;

        public StateMachine(TContext context)
        {
            _context = context;
        }

        public IState<TContext> Current => _current;

        public void ChangeState(IState<TContext> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (ReferenceEquals(_current, next))
            {
                return;
            }

            _current?.Exit(_context);
            _current = next;
            _current.Enter(_context);
        }

        public void Tick(float deltaTime)
        {
            _current?.Tick(_context, deltaTime);
        }
    }
}
