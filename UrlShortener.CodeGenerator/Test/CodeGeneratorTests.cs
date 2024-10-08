using Xunit;

namespace UrlShortener.CodeGenerator.Tests
{
    public class CodeGeneratorTests
    {
        private const int MinimumLength = 5;
        private const int TestLength = 10;

        [Fact]
        public void CodeGenerator_LengthLessThanMinimum_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlphanumericalGenerator(MinimumLength - 1));
        }

        [Fact]
        public void CodeGenerator_GenerateCode_NotNullOrEmpty()
        {
            ICodeGenerator codeGenerator = new AlphanumericalGenerator(MinimumLength);

            string code = codeGenerator.GenerateCode();

            Assert.NotEmpty(code);
            Assert.NotNull(code);
        }

        [Fact]
        public void CodeGenerator_GenerateCode_ReturnsCodeOfCorrectLength()
        {
            ICodeGenerator codeGenerator = new AlphanumericalGenerator(TestLength);

            Assert.Equal(TestLength, codeGenerator.GenerateCode().Length);
        }

        [Fact]
        public void CodeGenerator_GenerateCode_ReturnsAlphanumericalCode()
        {
            ICodeGenerator codeGenerator = new AlphanumericalGenerator(TestLength);

            Assert.Matches("^[A-Za-z0-9]+$", codeGenerator.GenerateCode());
        }
    }
}