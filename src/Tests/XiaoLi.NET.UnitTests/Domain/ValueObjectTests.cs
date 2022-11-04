using XiaoLi.NET.Domain;

namespace XiaoLi.NET.UnitTests.Domain;

public class ValueObjectTests
{
    [Theory]
    [MemberData(nameof(EqualValueObjects))]
    public void Equals_EqualValueObjects_ReturnsTrue(ValueObject instanceA, ValueObject instanceB, string reason)
    {
        // Act
        var result = instanceA.Equals(instanceB);

        // Assert
        Assert.True(result, reason);
    }

    [Fact]
    public void Equals_NullObject_ThrowsError()
    {
        Assert.Throws<NullReferenceException>(() =>
        {
            object objA = null;
            var objB = new object();
            objA.Equals(objB);
        });
    }
    
    
    private static readonly ValueObject APrettyValueObject = new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3"));
    
    public static readonly TheoryData<ValueObject, ValueObject, string> EqualValueObjects = new TheoryData<ValueObject, ValueObject, string>
    {
        {
            null,
            null,
            "they should be equal because they are both null"
        },
        {
            APrettyValueObject,
            APrettyValueObject,
            "they should be equal because they are the same object"
        },
        {
            new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3")),
            new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3")),
            "they should be equal because they have equal members"
        },
        {
            new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3"), notAnEqualityComponent: "xpto"),
            new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3"), notAnEqualityComponent: "xpto2"),
            "they should be equal because all equality components are equal, even though an additional member was set"
        },
        {
            new ValueObjectB(1, "2",  1, 2, 3 ),
            new ValueObjectB(1, "2",  1, 2, 3 ),
            "they should be equal because all equality components are equal, including the 'C' list"
        }
    };
}