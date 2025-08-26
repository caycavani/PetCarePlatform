using System;

namespace PetCare.Review.Domain.ValueObjects
{
    public class Comment
    {
        public string Value { get; }

        public Comment(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Comment cannot be empty.");

            if (value.Length > 500)
                throw new ArgumentException("Comment cannot exceed 500 characters.");

            Value = value;
        }

        public override string ToString() => Value;
    }
}
