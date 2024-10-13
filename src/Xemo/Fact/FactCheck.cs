namespace Xemo.Fact;

public sealed class FactCheck<TBasis>(IFact<TBasis> fact) : IMorph<TBasis, bool>
{
    public ValueTask<bool> Shaped(TBasis content) => ValueTask.FromResult(fact.IsTrue(content));
}

public static class FactCheck
{
    public static FactCheck<TBasis> Of<TBasis>(IFact<TBasis> fact) => new(fact);
}