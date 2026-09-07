using System.Text.Json;
using OCR_Tester.Domain.Models;
using OCR_Tester.Input;

namespace OCR_Tester.Tests;

public class GroundTruthReaderTests : IDisposable
{
    private readonly string _testDirectory;

    public GroundTruthReaderTests()
    {
        _testDirectory = Path.Combine(
            Path.GetTempPath(),
            "OCR_Tester_Tests",
            Guid.NewGuid().ToString()
        );

        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public void LoadGroundTruthFromJson_ValidJson_ReturnsGroundTruth()
    {
        // Arrange
        string json = """
            {
                "Image1": {
                    "ImageName": "test.png",
                    "ExpectedText": "Hello World"
                }
            }
            """;

        File.WriteAllText(Path.Combine(_testDirectory, "groundtruth.json"), json);

        var dataLoader = new DataLoader();

        // Act
        var result = dataLoader.LoadGroundTruthFromJson(_testDirectory);

        // Assert
        Assert.Single(result);
        Assert.Equal("test.png", result[0].ImageName);
        Assert.Equal("Hello World", result[0].ExpectedText);
    }

    [Fact]
    public void LoadGroundTruthFromJson_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        string invalidJson = """
            {
                "Image1": {
                    "Image": {
                        "ImageName": "test.png",
                        "ImagePath": "images/test.png"
                    },
                    "ExpectedText": "Hello World"
            """;

        File.WriteAllText(Path.Combine(_testDirectory, "invalid.json"), invalidJson);

        var dataLoader = new DataLoader();

        // Act & Assert
        Assert.Throws<JsonException>(() => dataLoader.LoadGroundTruthFromJson(_testDirectory));
    }

    [Fact]
    public void LoadGroundTruthFromJson_MissingImageReference_ReturnsGroundTruthWithEmptyImage()
    {
        // Arrange
        string json = """
            {
                "image1": {
                    "Image": {
                        "ImageName": "",
                        "ImagePath": ""
                    },
                    "ExpectedText": "Hello World"
                }
            }
            """;

        File.WriteAllText(Path.Combine(_testDirectory, "missing-image.json"), json);

        var dataLoader = new DataLoader();

        // Act
        var result = dataLoader.LoadGroundTruthFromJson(_testDirectory);

        // Assert
        Assert.Single(result);
        Assert.Empty(result[0].ImageName);
        Assert.Equal("Hello World", result[0].ExpectedText);
    }

    [Fact]
    public void LoadGroundTruthFromJson_EmptyGroundTruth_ReturnsEmptyExpectedText()
    {
        // Arrange
        string json = """
            {
                "Image1": {
                    "ImageName": "test.png",
                    "ExpectedText": ""
                }
            }
            """;

        File.WriteAllText(Path.Combine(_testDirectory, "empty-groundtruth.json"), json);

        var dataLoader = new DataLoader();

        // Act
        var result = dataLoader.LoadGroundTruthFromJson(_testDirectory);

        // Assert
        Assert.Single(result);
        Assert.Equal("test.png", result[0].ImageName);
        Assert.Empty(result[0].ExpectedText);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }
}
