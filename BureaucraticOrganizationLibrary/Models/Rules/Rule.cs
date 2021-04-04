namespace BureaucraticOrganization
{
    public abstract class Rule
    {
        internal abstract void Execute(BypassSheet sheet);
    }
}
