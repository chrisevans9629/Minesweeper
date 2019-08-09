using Microsoft.Build.Utilities;

namespace Compilers.Build
{
    using Microsoft.Build.Utilities;

    public class BuildPascal : Task
    {
        public override bool Execute()
        {
            Log.LogError("hello world!");
            return true;
        }
    }
}