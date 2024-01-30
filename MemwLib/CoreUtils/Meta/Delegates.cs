using System.Reflection;

namespace MemwLib.CoreUtils.Meta;

internal delegate IEnumerable<T> ExcludeMetaMembers<T>(IEnumerable<T> members) where T : MemberInfo;

internal delegate bool ProcessMemberIf<in T>(T member) where T : MemberInfo;

internal delegate void DoWithMetaMember<in T>(T member) where T : MemberInfo;