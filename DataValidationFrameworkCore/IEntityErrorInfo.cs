using System.Collections.Generic;
using System.Linq;

namespace OpenSoftware.DataValidationFrameworkCore
{
    public interface IEntityErrorInfo<TResult>
    {
        bool HasErrors { get; }
        IList<TResult> Errors { get; }
    }

    public class EntityErrorInfo<TResult> : IEntityErrorInfo<TResult>
    {
        public bool HasErrors => Errors.Any();
        public IList<TResult> Errors { get; } = new List<TResult>();
    }
}