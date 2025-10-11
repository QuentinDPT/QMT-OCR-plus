namespace QMTGroup.Image2.Test;

public class MatrixTests
{

    [Fact]
    public void OnDispose_WhenDisposeACopy_ShouldIDK()
    {
        // Arrange
        Matrix mat = new Matrix(16, 9, ChannelType.Y8);
        Matrix mat_copy = mat;

        // Act
        mat_copy.Dispose();

        // Assert
        Assert.NotNull(mat);
    }


    [Fact]
    public void OnNewMatrix_WhenInstantiationIsGood_ShouldReturnSameValues()
    {
        Matrix matrix = new Matrix(1600, 900, ChannelType.Y8);

        Assert.NotNull(matrix);
        Assert.Equal(900, matrix.Height);
        Assert.Equal(1600, matrix.Width);
        Assert.Equal(ChannelType.Y8, matrix.ChannelType);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(-1, 10)]
    [InlineData(10, -1)]
    public void OnNewMatrix_WhenNegativeSpaces_ShouldThrowArgumentOutOfRangeException(int a, int b)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Matrix(a, b, ChannelType.Y8));
    }

    [Fact]
    public void OnNewMatrix_WhenSpacesAreZero_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Matrix(0, 0, ChannelType.Y8));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(10, 0)]
    public void OnNewMatrix_WhenOneSpaceIsZero_ShouldThrowArgumentOutOfRangeException(int a, int b)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Matrix(a, b, ChannelType.Y8));
    }

    [Theory]
    [InlineData(int.MaxValue, 1)]
    [InlineData(1, int.MaxValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MaxValue/2, int.MaxValue/2+1)]
    public void OnNewMatrix_WhenLargeValues_ShouldThrowArgumentOutOfRangeException(int a, int b)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Matrix(a, b, ChannelType.Y8));
    }


    [Fact]
    public void OnClone_WhenOriginalWasFreed_ShouldAccessClonedValues()
    {
        // Arrange
        var original = new Matrix(1600, 900, ChannelType.Y8);
        var clone = original.Clone();

        // Act
        original.Dispose();

        // Assert
        Assert.NotNull(clone);
        Assert.False(clone.IsInvalid);
        Assert.Equal(900, clone.Height);
        Assert.Equal(1600, clone.Width);
        Assert.Equal(ChannelType.Y8, clone.ChannelType);
    }
}