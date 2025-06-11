namespace MappingService.Models
{
    public class LocalizedTitle
    {
        public required string Culture { get; init; }
        public required string Value { get; init; }

        public LocalizedTitle(string culture, string value)
        {
            Culture = culture;
            Value = value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Culture, Value);
        }

        public override bool Equals(object? obj)
        {
         
            if (obj is not LocalizedTitle other)
            {
                return false;
            }

            return Culture == other.Culture &&
                   Value == other.Value;
        }

        public bool Equals(LocalizedTitle? other)
        {
            if (other == null)
            {
                return false;
            }

            return Culture == other.Culture &&
                   Value == other.Value;
        }
    }
}