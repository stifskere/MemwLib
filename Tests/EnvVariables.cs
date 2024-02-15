using System.Reflection;
using MemwLib.Data.EnvironmentVariables;
using MemwLib.Data.EnvironmentVariables.Attributes;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class EnvVariables
{
    private readonly EnvContext _context = new();

    [OneTimeSetUp]
    public void SetupOnce()
    {
        _context.AddVariablesFrom(File.Open("./.env", FileMode.Open), true);
    }

    [Test]
    public void TestToType()
    {
        Context context = _context.ToType<Context>(true);
        
        Assert.Multiple(() =>
        {
            Assert.That(context.TestVariable1, Is.EqualTo("AAA"));
            Assert.That(context.TestVariable2, Is.EqualTo("AAA"));
        });
    }

    [Test]
    public void TestUsingIndexer()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_context["TEST_VAR1"], Is.EqualTo("AAA"));
            Assert.That(_context["TEST_VAR2"], Is.EqualTo("AAA"));
        });
    }

    public class Context
    {
        [EnvironmentVariable("TEST_VAR1")] public string TestVariable1 { get; set; } = default!;
        [EnvironmentVariable("TEST_VAR2")] public string TestVariable2 { get; set; } = default!;
    }
}