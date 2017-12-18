// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the NativeMethods type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Core
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The native methods.
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int memcmp(byte[] b1, byte[] b2, long count);
    }
}
