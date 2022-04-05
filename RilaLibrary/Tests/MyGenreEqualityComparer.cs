using Common.Models.OutputDtos;
using System.Collections.Generic;

namespace Tests
{
    public class MyGenreEqualityComparer : IEqualityComparer<GenreOutput>
    {
        public bool Equals(GenreOutput? x, GenreOutput? y)
        {
            if (object.ReferenceEquals(x, y)) return true;

            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return false;

            return x.Name == y.Name && x.Id == y.Id;
        }

        public int GetHashCode(GenreOutput obj)
        {
            if (object.ReferenceEquals(obj, null)) return 0;

            int hashCodeName = obj.Name == null ? 0 : obj.Name.GetHashCode();
            int hasCodeId = obj.Id.GetHashCode();

            return hashCodeName ^ hasCodeId;
        }
    }
}
