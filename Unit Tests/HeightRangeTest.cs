using NUnit.Framework;

[TestFixture]
public class HeightRangeTest
{
    [Test]
    public void WhenValuesAreSetInConstructorThenTheyCanBeRetrieved()
    {
        HeightRange heightRange = new HeightRange(2, 5, 3);

        Assert.That(heightRange.Min, Is.EqualTo(2));
        Assert.That(heightRange.Max, Is.EqualTo(5));
        Assert.That(heightRange.GroundHeight, Is.EqualTo(3));
    }

    [Test]
    public void WhenMinAndMaxAreReversedThenExceptionIsThrown()
    {
        Assert.That(() => new HeightRange(5, 2, 3), Throws.ArgumentException);
    }

    [Test]
    public void WhenGroundHeightIsOutsideOfMinAndMaxThenExceptionIsThrown()
    {
        Assert.That(() => new HeightRange(5, 2, 6), Throws.ArgumentException);
    }

    [Test]
    public void WhenMinIsChangedToBeGreaterThanMaxThenExceptionIsThrown()
    {
        HeightRange heightRange = new HeightRange(2, 5, 3);
        Assert.That(() => heightRange.Min = 6, Throws.ArgumentException);
    }

    [Test]
    public void WhenMaxIsChangedToBeSmallerThanMinThenExceptionIsThrown()
    {
        HeightRange heightRange = new HeightRange(2, 5, 3);
        Assert.That(() => heightRange.Max = 1, Throws.ArgumentException);
    }

    [Test]
    public void WhenMinIsChangedToExcludeGroundHeightThenExceptionIsThrown()
    {
        HeightRange heightRange = new HeightRange(2, 5, 3);
        Assert.That(() => heightRange.Min = 4, Throws.ArgumentException);
    }

    [Test]
    public void WhenMaxIsChangedToExcludeGroundHeightThenExceptionIsThrown()
    {
        HeightRange heightRange = new HeightRange(2, 5, 3);
        Assert.That(() => heightRange.Max = 2, Throws.ArgumentException);
    }

    [Test]
    public void WhenGroundHeightIsChangedToBeLowerThanMinThenExceptionIsThrown()
    {
        HeightRange heightRange = new HeightRange(2, 5, 3);
        Assert.That(() => heightRange.GroundHeight = 1, Throws.ArgumentException);
    }

    [Test]
    public void WhenGroundHeightIsChangedToBeHigherThanMaxThenExceptionIsThrown()
    {
        HeightRange heightRange = new HeightRange(2, 5, 3);
        Assert.That(() => heightRange.GroundHeight = 6, Throws.ArgumentException);
    }

    [Test]
    public void HeightRangeCanBeOneUnitLong()
    {
        HeightRange heightRange = new HeightRange(2, 2, 2);

        Assert.That(heightRange.Min, Is.EqualTo(2));
        Assert.That(heightRange.Max, Is.EqualTo(2));
        Assert.That(heightRange.GroundHeight, Is.EqualTo(2));
    }

    [Test]
    public void WhenHeightIsLessThanMinThenItIsNotInRange()
    {
        HeightRange heightRange = new HeightRange(2, 8, 5);

        Assert.That(heightRange.IsHeightInRange(1), Is.False);
    }

    [Test]
    public void WhenHeightIsGreaterThanMaxThenItIsNotInRange()
    {
        HeightRange heightRange = new HeightRange(2, 8, 5);

        Assert.That(heightRange.IsHeightInRange(10), Is.False);
    }

    [Test]
    public void WhenHeightIsEqualToMinThenItIsInRange()
    {
        HeightRange heightRange = new HeightRange(2, 8, 5);

        Assert.That(heightRange.IsHeightInRange(2), Is.True);
    }

    [Test]
    public void WhenHeightIsEqualToMaxThenItIsInRange()
    {
        HeightRange heightRange = new HeightRange(2, 8, 5);

        Assert.That(heightRange.IsHeightInRange(8), Is.True);
    }

    [Test]
    public void WhenHeightIsInBetweenMinAndMaxThenItIsInRange()
    {
        HeightRange heightRange = new HeightRange(2, 8, 5);

        Assert.That(heightRange.IsHeightInRange(4), Is.True);
    }

    [Test]
    public void WhenHeightRangeHasMinInBetweenMinAndMaxOfOtherHeightRangeThenTheyAreOverlapping()
    {
        HeightRange heightRangeOne = new HeightRange(6, 20, 15);
        HeightRange heightRangeTwo = new HeightRange(2, 8, 5);

        Assert.That(heightRangeOne.IsOverlappingWith(heightRangeTwo), Is.True);
    }

    [Test]
    public void WhenHeightRangeHasMaxInBetweenMinAndMaxOfOtherHeightRangeThenTheyAreOverlapping()
    {
        HeightRange heightRangeOne = new HeightRange(6, 20, 15);
        HeightRange heightRangeTwo = new HeightRange(2, 8, 5);

        Assert.That(heightRangeTwo.IsOverlappingWith(heightRangeOne), Is.True);
    }

    [Test]
    public void WhenHeightRangeHasMinEqualToMaxOfOtherHeightRangeThenTheyAreOverlapping()
    {
        HeightRange heightRangeOne = new HeightRange(6, 20, 15);
        HeightRange heightRangeTwo = new HeightRange(2, 6, 5);

        Assert.That(heightRangeOne.IsOverlappingWith(heightRangeTwo), Is.True);
    }

    [Test]
    public void WhenHeightRangeHasMaxEqualToMinOfOtherHeightRangeThenTheyAreOverlapping()
    {
        HeightRange heightRangeOne = new HeightRange(6, 20, 15);
        HeightRange heightRangeTwo = new HeightRange(2, 6, 5);

        Assert.That(heightRangeTwo.IsOverlappingWith(heightRangeOne), Is.True);
    }

    [Test]
    public void WhenHeightRangeEncompasesAnotherHeightRangeThenTheyAreOverlapping()
    {
        HeightRange heightRangeOne = new HeightRange(1, 20, 15);
        HeightRange heightRangeTwo = new HeightRange(2, 6, 5);

        Assert.That(heightRangeOne.IsOverlappingWith(heightRangeTwo), Is.True);
    }

    [Test]
    public void WhenHeightRangeIsEncompassedByAnotherHeightRangeThenTheyAreOverlapping()
    {
        HeightRange heightRangeOne = new HeightRange(1, 20, 15);
        HeightRange heightRangeTwo = new HeightRange(2, 6, 5);

        Assert.That(heightRangeTwo.IsOverlappingWith(heightRangeOne), Is.True);
    }

    [Test]
    public void WhenHeightRangeIsHigherThanAnotherHeightRangeThenTheyAreNotOverlapping()
    {
        HeightRange heightRangeOne = new HeightRange(10, 20, 15);
        HeightRange heightRangeTwo = new HeightRange(2, 6, 5);

        Assert.That(heightRangeOne.IsOverlappingWith(heightRangeTwo), Is.False);
    }

    [Test]
    public void WhenHeightRangeIsLowerThanAnotherHeightRangeThenTheyAreNotOverlapping()
    {
        HeightRange heightRangeOne = new HeightRange(10, 20, 15);
        HeightRange heightRangeTwo = new HeightRange(2, 6, 5);

        Assert.That(heightRangeTwo.IsOverlappingWith(heightRangeOne), Is.False);
    }
}
