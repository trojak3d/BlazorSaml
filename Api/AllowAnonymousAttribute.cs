using System;

namespace Api
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AllowAnonymousAttribute : Attribute {
    }
}
