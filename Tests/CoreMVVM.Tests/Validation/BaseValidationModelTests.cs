using CoreMVVM.Validation;
using Xunit;

namespace CoreMVVM.Tests.Validation
{
    public sealed class BaseValidationModelTests
    {
        private readonly ValidationModel _validationModel;
        private int _errorsChangedCount;

        public BaseValidationModelTests()
        {
            _validationModel = new ValidationModel();
            _validationModel.ErrorsChanged += Model_ErrorsChanged;
        }

        private void Model_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            _errorsChangedCount++;
        }

        #region ValidationModel_Raises_ErrorsChanged

        [Fact]
        public void ValidationModel_Raises_ErrorsChanged()
        {
            Assert.Equal(0, _errorsChangedCount);
            _validationModel.SimpleProperty = new object();
            Assert.Equal(1, _errorsChangedCount);
        }

        private sealed class AlwaysFalseValidationAttribute : ValidationAttribute
        {
            protected override bool IsValid(object? value)
            {
                return false;
            }
        }

        #endregion ValidationModel_Raises_ErrorsChanged

        #region ValidationModel_Raises_ErrorsChanged_OnErrosModified

        [Fact]
        public void ValidationModel_Raises_ErrorsChanged_OnErrosModified()
        {
            Assert.Equal(0, _errorsChangedCount);
            _validationModel.Integer = 1;
            Assert.Equal(1, _errorsChangedCount);
            _validationModel.Integer = 4;
            Assert.Equal(1, _errorsChangedCount);
        }

        private sealed class TrueWhenZeroValidationAttribute : ValidationAttribute
        {
            protected override bool IsValid(object? value)
            {
                return value is int intVal && intVal == 0;
            }
        }

        #endregion ValidationModel_Raises_ErrorsChanged_OnErrosModified

        private sealed class ValidationModel : BaseValidationModel
        {
            private object? _simpleProperty;

            [AlwaysFalseValidation]
            public object? SimpleProperty
            {
                get => _simpleProperty;
                set => SetProperty(ref _simpleProperty, value);
            }

            private int _integer;

            [TrueWhenZeroValidation]
            public int Integer
            {
                get => _integer;
                set => SetProperty(ref _integer, value);
            }
        }
    }
}