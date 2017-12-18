namespace SAF.Data.Providers.Tests.FieldsModel
{
    /// <summary>
    /// The department.
    /// </summary>
    public class Department
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Department"/> class.
        /// </summary>
        public Department()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Department"/> class.
        /// </summary>
        /// <param name="departmentId">
        /// The department ID.
        /// </param>
        public Department(int? departmentId)
        {
            this.DepartmentId = departmentId;
        }

        /// <summary>
        /// Gets the department ID.
        /// </summary>
        public int? DepartmentId { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}