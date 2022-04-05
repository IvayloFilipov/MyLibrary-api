using Common.Models.OutputDtos;
using System.Collections.Generic;

namespace Tests
{
    public class MyBookEqualityComparer : IEqualityComparer<BookOutput>
    {
        public bool Equals(BookOutput? x, BookOutput? y)
        {
            if (object.ReferenceEquals(x, y)) return true;

            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return false;

            return x.Title == y.Title && x.Id == y.Id;
        }

        public int GetHashCode(BookOutput obj)
        {
            if (object.ReferenceEquals(obj, null)) return 0;

            int hashCodeName = obj.Title == null ? 0 : obj.Title.GetHashCode();
            int hasCodeId = obj.Id.GetHashCode();

            return hashCodeName ^ hasCodeId;
        }
    }
}
