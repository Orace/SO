using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Common.WPF.ViewModels;

namespace SO_72434408;

public class ViewModel : NotifyPropertyChangedBase, IDataErrorInfo
{
    private double _value;

    [Range(0, 3.14159, ErrorMessage = "{0} should be in [{1}; {2}]")]
    public double Value
    {
        get => _value;
        set => Update(ref _value, value);
    }

    string IDataErrorInfo.Error => throw new NotSupportedException("IDataErrorInfo.Error is not supported.");

    string IDataErrorInfo.this[string propertyName]
    {
        get
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }

            var error = string.Empty;
            var value = GetValue(propertyName);
            var results = new List<ValidationResult>(1);
            var result =
                Validator.TryValidateProperty(value,
                                              new ValidationContext(this, null, null) { MemberName = propertyName },
                                              results);

            return result ? error : results.First().ErrorMessage ?? string.Empty;
        }
    }

    private object? GetValue(string propertyName)
    {
        var propInfo = GetType().GetProperty(propertyName);
        return propInfo?.GetValue(this);
    }
}