using System.Reflection;

namespace MemwLib.CoreUtils.Meta;

internal abstract class MetaProcessor<TMember> where TMember : MemberInfo
{
    protected HashSet<TMember> Members;

    protected MetaProcessor(HashSet<TMember> members)
    {
        Members = members;
    }

    public MetaProcessor<TMember> Exclude(ExcludeMetaMembers<TMember> predicate)
    {
        Members = predicate(Members).ToHashSet();
        return this;
    }

    public MetaProcessor<TMember> ExcludeIf(bool condition, ExcludeMetaMembers<TMember> predicate) 
        => condition ? Exclude(predicate) : this;
    
    public void Do(DoWithMetaMember<TMember> predicate)
    {
        foreach (TMember member in Members)
            predicate(member);
    }

    public void DoIf(ProcessMemberIf<TMember> condition, DoWithMetaMember<TMember> predicate)
    {
        foreach (TMember member in Members)
            if (condition(member))
                predicate(member);
    }
}