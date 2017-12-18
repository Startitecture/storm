namespace SAF.EntityServices
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents faults that occur due to invalid data.
    /// </summary>
    /// <typeparam name="T">The type of data contract that is invalid.</typeparam>
    [DataContract]
    public class DataContractFault<T> : FaultBase<T>
    {
    }
}