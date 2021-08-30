namespace BlowinCleanCode.Model.Matchers
{
    public interface IMatcher<in T>
    {
        bool Match(T left, T right);
    }
}