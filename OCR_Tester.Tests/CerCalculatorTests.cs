using OCR_Tester.Evaluation;

namespace OCR_Tester.Tests
{
    public class CerCalculatorTests
    {
        [Fact]
        public void CalculateCer_IdenticalTexts_ReturnsZero()
        {
            // Arrange
            string expectedText = "Hello World";
            string ocrResult = "Hello World";

            // Act
            int result = CerCalculator.CalculateCer(expectedText, ocrResult);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void CalculateCer_SingleError_ReturnsOne()
        {
            // Arrange
            string expectedText = "Hello";
            string ocrResult = "Hallo";

            // Act
            int result = CerCalculator.CalculateCer(expectedText, ocrResult);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void CalculateCer_MissingCharacter_ReturnsOne()
        {
            // Arrange
            string expectedText = "Hello";
            string ocrResult = "Helo";

            // Act
            int result = CerCalculator.CalculateCer(expectedText, ocrResult);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void CalculateCer_AdditionalCharacter_ReturnsOne()
        {
            // Arrange
            string expectedText = "Hello";
            string ocrResult = "Helloo";

            // Act
            int result = CerCalculator.CalculateCer(expectedText, ocrResult);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void CalculateCer_BothStringsEmpty_ReturnsZero()
        {
            // Arrange
            string expectedText = "";
            string ocrResult = "";

            // Act
            int result = CerCalculator.CalculateCer(expectedText, ocrResult);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void CalculateCer_ExpectedTextEmpty_ReturnsLengthOfOcrResult()
        {
            // Arrange
            string expectedText = "";
            string ocrResult = "Hello";

            // Act
            int result = CerCalculator.CalculateCer(expectedText, ocrResult);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public void CalculateCer_OcrResultEmpty_ReturnsLengthOfExpectedText()
        {
            // Arrange
            string expectedText = "Hello";
            string ocrResult = "";

            // Act
            int result = CerCalculator.CalculateCer(expectedText, ocrResult);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public void CalculateCerInPercent_IdenticalTexts_ReturnsZeroPercent()
        {
            // Arrange
            string expectedText = "Hello";
            string ocrResult = "Hello";

            // Act
            float result = CerCalculator.CalculateCerInPercent(expectedText, ocrResult);

            // Assert
            Assert.Equal(0.0f, result);
        }

        [Fact]
        public void CalculateCerInPercent_SingleError_ReturnsTwentyPercent()
        {
            // Arrange
            string expectedText = "Hello";
            string ocrResult = "Hallo";

            // Act
            float result = CerCalculator.CalculateCerInPercent(expectedText, ocrResult);

            // Assert
            Assert.Equal(20.0f, result);
        }

        [Fact]
        public void CalculateCerInPercent_MissingCharacter_ReturnsTwentyPercent()
        {
            // Arrange
            string expectedText = "Hello";
            string ocrResult = "Helo";

            // Act
            float result = CerCalculator.CalculateCerInPercent(expectedText, ocrResult);

            // Assert
            Assert.Equal(20.0f, result);
        }

        [Fact]
        public void CalculateCerInPercent_AdditionalCharacter_ReturnsTwentyPercent()
        {
            // Arrange
            string expectedText = "Hello";
            string ocrResult = "Helloo";

            // Act
            float result = CerCalculator.CalculateCerInPercent(expectedText, ocrResult);

            // Assert
            Assert.Equal(20.0f, result);
        }

        [Fact]
        public void CalculateCerInPercent_EmptyExpectedText_ReturnsZero()
        {
            // Arrange
            string expectedText = "";
            string ocrResult = "Hello";

            // Act
            float result = CerCalculator.CalculateCerInPercent(expectedText, ocrResult);

            // Assert
            Assert.Equal(0.0f, result);
        }
    }
}
