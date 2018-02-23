using System;
using System.ComponentModel;

namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <inheritdoc />
    /// <summary>
    /// Class that that represents an asynchronous validation operation.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ValidationOperation<TResult> : INotifyPropertyChanged
        where TResult : class
    {
        private TResult _result;

        /// <summary>
        /// Gets or sets the valiation result for this ValidationOperation.
        /// </summary>
        public TResult Result
        {
            get => _result;
            set
            {
                if (_result == value) return;
                _result = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
                Completed?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Event raised when the validation operation completes.
        /// </summary>
        public event EventHandler Completed;

        /// <inheritdoc />
        /// <summary>
        /// Event raised whenever a ValidationOperation property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
