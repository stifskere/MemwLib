using MemwLib.Data.Json;
using MemwLib.Data.Json.Attributes;
using MemwLib.Data.Json.Enums;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class Json
{
    private const string PayloadWithFormat = """
                                    {
                                        "name": "John Doe",
                                        "age": 35,
                                        "is_sane": true,
                                        "children": [
                                            {
                                                "name": "Uganda Doe",
                                                "age": 12,
                                                "is_sane": true,
                                                "children": null
                                            }
                                        ]
                                    }
                                    """;

    private const string PayloadWithoutFormat = """{"name":"John Doe","age":35,"is_sane":true,"children":[{"name":"Uganda Doe","age":12,"is_sane":true,"children":null}]}""";
    
    private const string InvalidJson = """{"hello": bye, 1: "yes"}""";
    
    [Test]
    public void Serialize()
    {
        Person person = new()
        {
            Name = "John Doe",
            Age = 35,
            IsSane = true,
            Children = new Person[]
            {
                new()
                {
                    Name = "Uganda Doe",
                    Age = 12,
                    IsSane = true
                }
            }
        };

        string p = JsonParser.Serialize(person, 4);
        string pi = JsonParser.Serialize(person);
        
        Assert.Multiple(() =>
        {
            Assert.That(p, Is.EqualTo(PayloadWithFormat));
            Assert.That(pi, Is.EqualTo(PayloadWithoutFormat));
        });
    }

    [Test]
    public void Deserialize()
    {
        Assertion(JsonParser.Deserialize<Person>(PayloadWithFormat)!);
        Assertion(JsonParser.Deserialize<Person>(PayloadWithoutFormat)!);
        
        return;
        
        static void Assertion(Person person)
        {
            Assert.Multiple(() =>
            {
                Assert.That(person.Name, Is.EqualTo("John Doe"));
                Assert.That(person.Age, Is.EqualTo(35));
                Assert.That(person.Legal, Is.True);
                Assert.That(person.IsSane, Is.True);
                Assert.That(person.Children, Is.Not.Null.And.Not.Empty);
                Assert.That(person.Children![0], Is.InstanceOf<Person>());
                Assert.That(person.Children[0].Name, Is.EqualTo("Uganda Doe"));
                Assert.That(person.Children[0].Age, Is.EqualTo(12));
                Assert.That(person.Children[0].Legal, Is.False);
                Assert.That(person.Children[0].IsSane, Is.True);
            });
        }
    }

    [Test]
    public void CheckValidity()
    {
        Assert.Multiple(() =>
        {
            Assert.That(JsonParser.IsValidJson(InvalidJson), Is.False);
            Assert.That(JsonParser.IsValidJson(PayloadWithoutFormat, true), Is.True);
        });
    }
}

[JsonObject(JsonObjectBehavior.OnlyPropertiesWithAttribute)]
file class Person
{
    [JsonProperty("name")] public string Name { get; set; } = default!;
    
    [JsonProperty("age")] public int Age { get; set; }

    public bool Legal => Age > 18;
    
    [JsonProperty("is_sane")] public bool IsSane { get; set; }

    [JsonProperty("children")] public Person[]? Children { get; set; }
}