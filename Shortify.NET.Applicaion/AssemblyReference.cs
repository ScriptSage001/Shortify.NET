using System.Reflection;

namespace Shortify.NET.Applicaion
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
