using FiniteStateMachine;

namespace RegularExpressionStateMachineBuilder;

public interface IRegexStateMachineBuilder
{
    IStateMachine BuildStateMachineFromRegularExpression(string regularExpression);
}