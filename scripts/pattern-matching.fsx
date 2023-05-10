open System
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Reflection

type Area = float * float
type Radius = float

type Shape =
    | Rectangle of area : Area
    | Circle of radius : Radius
    | Other
    | SomethingElse    

// Exhaustive matching and guard
module Evaluate =
    let area (shape : Shape) =
        match shape with
        | Rectangle (w, v) -> w * v
        | Circle r -> ((355/113) |> float) * (r ** 2) 
        | _ -> 0

    let form (shape : Shape) =
        match shape with
        | Rectangle(h,w) -> printfn "Shape is a rectangle with height %f, width %f and area %f" h w (shape |> area)
        | Circle r -> printfn "Shape is a circle with height %f and area %f" r (shape |> area)
        | _ -> Console.WriteLine("Shape is neither a circle nor a rectangle")
    
    let areaBiggerThan (value: float) (shape:Shape) =
        if (area shape > value) then printfn "The shape has an area bigger than %f" value
        else printfn "This shape has an area smaller than %f" value

        match (shape |> area) with
        | area when area > value -> printfn "The shape has an area bigger than %f" value
        | _ -> printfn "This shape has an area smaller than %f" value

let rectangle1 = Rectangle (3, 4)
let rectangle2 = Rectangle (4, 5)
let rectangle3 = Rectangle (5, 6)
let rectangle4 = Rectangle (8, 9)
let circle1 = Circle 6
let circle2 = Circle 7
let circle3 = Circle 9
let circle4 = Circle 10

circle1 |> Evaluate.form
rectangle2 |> Evaluate.form

rectangle1 |> Evaluate.areaBiggerThan 21
rectangle2 |> Evaluate.areaBiggerThan 21
rectangle3 |> Evaluate.areaBiggerThan 21
rectangle4 |> Evaluate.areaBiggerThan 21
circle1 |> Evaluate.areaBiggerThan 21
circle2 |> Evaluate.areaBiggerThan 21
circle3 |> Evaluate.areaBiggerThan 21
circle4 |> Evaluate.areaBiggerThan 21











