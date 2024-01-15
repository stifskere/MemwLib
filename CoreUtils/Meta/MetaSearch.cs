using System.Reflection;

namespace MemwLib.CoreUtils.Meta;

file class MetaProcessorImpl<TMember> : MetaProcessor<TMember> where TMember : MemberInfo
{
    public MetaProcessorImpl(IEnumerable<TMember> members) : base(members.ToHashSet()) {}
}

internal static class MetaSearch
{
    public static MetaProcessor<PropertyInfo> ProcessProperties<TType>(BindingFlags flags = BindingFlags.Default) 
        => new MetaProcessorImpl<PropertyInfo>(typeof(TType).GetProperties(flags));

    public static MetaProcessor<FieldInfo> ProcessFields<TType>(BindingFlags flags = BindingFlags.Default)
        => new MetaProcessorImpl<FieldInfo>(typeof(TType).GetFields(flags));

    public static MetaProcessor<MethodInfo> ProcessMethods<TType>(BindingFlags flags = BindingFlags.Default)
        => new MetaProcessorImpl<MethodInfo>(typeof(TType).GetMethods(flags));

    public static MetaProcessor<EventInfo> ProcessEvents<TType>(BindingFlags flags = BindingFlags.Default)
        => new MetaProcessorImpl<EventInfo>(typeof(TType).GetEvents(flags));
}