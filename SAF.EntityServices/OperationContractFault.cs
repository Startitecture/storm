namespace SAF.EntityServices
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents faults that occur during operations.
    /// </summary>
    /// <typeparam name="T">The type of data contract associated with the operation.</typeparam>
    [DataContract]
    public class OperationContractFault<T> : FaultBase<T>
    {
    }
}