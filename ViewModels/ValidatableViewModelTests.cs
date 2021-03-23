using FluentValidation;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Energetic.Clients.ViewModels.Tests
{
    [TestClass()]
    public class ValidatableViewModelTests
    {
        [TestMethod()]
        public void ValidatableViewModel_GivenNullValidator_ThrowsArgumentNull()
        {
            // Arrange
            var fakeCommandFactory = CreateFakeCommandFactory();
            var action = new Action(() =>
            {
                var sut = new TestViewModel(fakeCommandFactory, null!);
            });

            // Act and Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod()]
        public void GetErrors_GivenOnePropertyInError_ReturnsOneError()
        {
            // Arrange
            var sut = CreateFakeViewModel();
            sut.TestProperty = string.Empty;

            // Act
            var errors = (IEnumerable<string>)sut.GetErrors(nameof(sut.TestProperty));
            int countErrors = errors.Count();

            // Assert
            Assert.IsTrue(countErrors == 1);
        }

        private ICommandFactory CreateFakeCommandFactory()
        {
            return new Mock<ICommandFactory>().Object;
        }

        private IValidator<TestViewModel> CreateFakeValidator()
        {
            var fakeValidationResult = CreateFakeValidationResult();

            var fake = new Mock<IValidator<TestViewModel>>();
            
            fake.Setup(validator => validator.Validate(It.IsAny<TestViewModel>()))
                .Returns(fakeValidationResult);

            return fake.Object;
        }

        private TestViewModel CreateFakeViewModel()
        {
            var fakeCommandFactory = CreateFakeCommandFactory();
            var fakeValidator = CreateFakeValidator();
            return new TestViewModel(fakeCommandFactory, fakeValidator);
        }

        private ValidationResult CreateFakeValidationResult()
        {
            var list = new List<ValidationFailure>();
            list.Add(CreateFakeValidationFailure());
            return new ValidationResult(list);
        }

        private ValidationFailure CreateFakeValidationFailure()
        {
            return new ValidationFailure(nameof(TestViewModel.TestProperty), "Nope");
        }
    }

    public class TestViewModel : ValidatableViewModel<TestViewModel>
    {
        public TestViewModel(ICommandFactory fakeCommandFactory, IValidator<TestViewModel> fakeValidator) :
            base(fakeCommandFactory, fakeValidator)
        {
        }

        public string TestProperty { get; set; } = string.Empty;
    }
}