using System;
using System.Collections.Generic;
using System.Text;

namespace Startitecture.Orm.Testing.Model
{
    public class GenericSubmission
    {
        private List<FieldValue> submissionValues;

        public GenericSubmission(string subject)
        {
            this.Subject = subject;
        }

        private GenericSubmission()
        {
        }

        public int? GenericSubmissionId { get; private set; }

        public string Subject { get; private set; }

        public DomainIdentity SubmittedBy { get; private set; }

        public DateTimeOffset SubmissionTime { get; private set; }

        public IEnumerable<FieldValue> SubmissionValues => this.submissionValues;

        public void SetValue(Field field, object value)
        {
            this.submissionValues.Add(new FieldValue());
        }

        public void Submit(DomainIdentity submitter)
        {
            this.SubmittedBy = submitter;
            this.SubmissionTime = DateTimeOffset.Now;
        }
    }

    public class FieldValue
    {
        public long? FieldValueId { get; private set; }

        public Field Field { get; private set; }

        public DomainIdentity LastModifiiedBy { get; private set; }

        public DateTimeOffset LastModifiedTime { get; private set; }

        public object Value { get; set; }
    }

    public class Field
    {
        public int? FieldId { get; private set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class DomainIdentity
    {
        public int? DomainIdentityId { get; private set; }

        public string UniqueIdentifier { get; private set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
    }
}
