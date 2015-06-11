using System;

namespace Dictionary_Blender
{
    class BlenderFunction
    {
        public delegate void FunctionImplementation(string[] commands);
        public FunctionImplementation Implementation;

        public int MinArguments;
        public int MaxArguments;

        public BlenderFunction(FunctionImplementation implementation,int minArguments,int maxArguments = Int32.MinValue)
        {
            Implementation = implementation;
            MinArguments = minArguments;
            MaxArguments = ( maxArguments == Int32.MinValue ) ? MinArguments : maxArguments;
        }
    }
}
