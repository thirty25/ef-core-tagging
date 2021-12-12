using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EfCoreTagging.Data
{
    public static class IQueryableTaggingExtensions
    {
        public static IQueryable<T> TagWithSource<T>(this IQueryable<T> queryable,
            [NotParameterized] string tag = default,
            [NotParameterized] [CallerLineNumber] int lineNumber = 0,
            [NotParameterized] [CallerFilePath] string filePath = default,
            [NotParameterized] [CallerMemberName] string memberName = default,
            [NotParameterized] [CallerArgumentExpression("queryable")]
            string argument = "")
        {
            return queryable.TagWith(GetTagContent(tag, lineNumber, filePath, memberName, argument));
        }

        public static async Task<List<T>> ToListWithSourceAsync<T>(this IQueryable<T> queryable,
            [NotParameterized] string tag = default,
            [NotParameterized] [CallerLineNumber] int lineNumber = 0,
            [NotParameterized] [CallerFilePath] string filePath = "",
            [NotParameterized] [CallerMemberName] string memberName = default,
            [NotParameterized] [CallerArgumentExpression("queryable")]
            string argument = default,
            CancellationToken cancellationToken = default)
        {
            return await queryable
                .TagWith(GetTagContentWithMethodName(tag, lineNumber, filePath, memberName, argument))
                .ToListAsync(cancellationToken);
        }

        private static string GetTagContentWithMethodName(string tag, int lineNumber, string filePath, string memberName,
            string argument, [CallerMemberName] string additionalMethodInfo = default)
        {
            return GetTagContent(tag, lineNumber, filePath, memberName, argument + $".{additionalMethodInfo}()");
        }

        private static string GetTagContent(string tag, int lineNumber, string filePath, string memberName,
            string argument)
        {
            // argument could be multiple lines with whitespace so let's normalize it down to one line
            var trimmedLines = string.Join(
                string.Empty,
                argument.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim())
            );

            var tagContent = string.IsNullOrWhiteSpace(tag)
                ? default
                : tag + Environment.NewLine;

            tagContent += trimmedLines + Environment.NewLine + $" at {memberName}  - {filePath}:{lineNumber}";

            return tagContent;
        }
    }
}
