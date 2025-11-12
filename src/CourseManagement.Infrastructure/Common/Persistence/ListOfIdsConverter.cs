using System.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CourseManagement.Infrastructure.Common.Persistence;

public class ListOfIdsConverter(ConverterMappingHints? mappingHints = null)
    : ValueConverter<List<int>, string>(
        v => string.Join(',', v),
        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(),
        mappingHints)
{
}

public class ListOfIdsComparer : ValueComparer<List<int>>
{
    public ListOfIdsComparer() : base(
      (t1, t2) => t1!.SequenceEqual(t2!),
      t => t.Select(x => x!.GetHashCode()).Aggregate((x, y) => x ^ y),
      t => t)
    {
    }
}