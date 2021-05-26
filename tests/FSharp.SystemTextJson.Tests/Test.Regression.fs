module Tests.Regression

open Xunit
open System.Text.Json.Serialization
open System.Text.Json

type Color = Red | Blue | Green

[<Fact>]
let ``regression #33`` () =
    let serializerOptions = JsonSerializerOptions()
    serializerOptions.Converters.Add(JsonFSharpConverter(JsonUnionEncoding.UnwrapFieldlessTags))
    let actual = JsonSerializer.Deserialize<Color list>("""[ "Red", "Blue"] """, serializerOptions)
    Assert.Equal<Color>([Red; Blue], actual)
    let actual = JsonSerializer.Deserialize("""{"a":"Red","b":"Blue"}""", serializerOptions)
    Assert.Equal({| a = Red; b = Blue |}, actual)

type A = { A: int; B: string }

[<Fact>]
let ``regression #87`` () =
    let options = JsonSerializerOptions()
    options.Converters.Add(JsonFSharpConverter())
    let ex1 = Assert.Throws<JsonException>(fun () -> JsonSerializer.Deserialize<A>("""{ "A": 2, "B": null }""", options) |> ignore)
    Assert.Equal("A.B was expected to be of type String, but was null.", ex1.Message)
    let ex2 = Assert.Throws<JsonException>(fun () -> JsonSerializer.Deserialize<A>("""{ "A": null, "B": "a" }""", options) |> ignore)
    Assert.Equal("A.A was expected to be of type Int32, but was null.", ex2.Message)
