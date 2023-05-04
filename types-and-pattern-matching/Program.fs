open System

type Area = float * float
type Radius = float

type Shape =
    | Rectangle of area : Area
    | Circle of radius : Radius

type DiscriminatedUnion2 =
    | Option1 of int
    | Option2 of string
    | Option3

type BasicProfile =
    { FirstName: string
      LastName: string
      Age: int }

type CompactProfile = string * int

[<EntryPoint>]
let main args =
    //Here the concept of various types are demonstrated
    let guilherme: BasicProfile =
        { FirstName = "Guilherme"
          LastName = "Rodrigues"
          Age = 18 }

    Console.WriteLine(
        $"Hello {guilherme.FirstName} {guilherme.LastName}. In {21 - guilherme.Age} years you will be allowed to drink in the USA!"
    )
    //Records: A tuple where each element is labeled
    let guilhermeCompact: CompactProfile =
        ((guilherme.FirstName + " " + guilherme.LastName), guilherme.Age)
        
    Console.WriteLine(
        $"""Hello {guilhermeCompact |> fst}. {match guilhermeCompact |> snd with
                                              | x when x >= 18 -> "You can already drink in Brazil!"
                                              | _ -> "You have to drink milk!"}"""
    )
    //Discriminated Unions and pattern matching:ss




    0
