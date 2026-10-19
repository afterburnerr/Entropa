namespace Entropa.Core.StateMachine
{
    public interface IState<TContext>
    {
        void Enter(TContext context);

        void Tick(TContext context, float deltaTime);

        void Exit(TContext context);
    }
}
