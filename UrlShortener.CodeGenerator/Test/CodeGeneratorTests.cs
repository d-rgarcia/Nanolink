using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace UrlShortener.CodeGenerator.Tests
{
    public class CodeGeneratorTests
    {
        private const int TestLength = 7;

        private readonly IOptionsMonitor<CodeGeneratorOptions> options;
        private CodeGeneratorOptions _defaultOptions;

        public CodeGeneratorTests()
        {
            _defaultOptions = new CodeGeneratorOptions();
            
            var optionsMock = new Mock<IOptionsMonitor<CodeGeneratorOptions>>();
            optionsMock.Setup(o => o.CurrentValue).Returns(_defaultOptions);

            options = optionsMock.Object;
        }

        [Fact]
        public void CodeGenerator_LengthLessThanMinimum_ThrowsArgumentOutOfRangeException()
        {
            _defaultOptions.CodeLength = 4;

            Assert.Throws<ArgumentOutOfRangeException>(() => new AlphanumericalGenerator(options).GenerateCode());
        }

        [Fact]
        public void CodeGenerator_GenerateCode_NotNullOrEmpty()
        {
            ICodeGenerator codeGenerator = new AlphanumericalGenerator(options);

            string code = codeGenerator.GenerateCode();

            Assert.NotEmpty(code);
            Assert.NotNull(code);
        }

        [Fact]
        public void CodeGenerator_GenerateCode_ReturnsCodeOfCorrectLength()
        {
            ICodeGenerator codeGenerator = new AlphanumericalGenerator(options);

            Assert.Equal(TestLength, codeGenerator.GenerateCode().Length);
        }

        [Fact]
        public void CodeGenerator_GenerateCode_ReturnsAlphanumericalCode()
        {
            ICodeGenerator codeGenerator = new AlphanumericalGenerator(options);

            Assert.Matches("^[A-Za-z0-9]+$", codeGenerator.GenerateCode());
        }
    }
}