using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CourseManagement.Infrastructure.Common.Persistence
{
    public class ListOfStringsConverter(ConverterMappingHints? mappingHints = null)
        : ValueConverter<List<string>, string>(
            v => string.Join('|', v),
            v => v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList(),
            mappingHints)
    {
    }


    public class ListOfStringsComparer : ValueComparer<List<string>>
    {
        public ListOfStringsComparer() : base(
          (t1, t2) => t1!.SequenceEqual(t2!),
          t => t.Select(x => x!.GetHashCode()).Aggregate((x, y) => x ^ y),
          t => t)
        {
        }
    }
}
