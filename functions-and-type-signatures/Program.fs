open System
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Reflection

//In this code snippet the following concepts will be tested/exercised:
// - type signatures
// - type inference
// - functions (partial application)
// - functions (composition)
// - functions (tupled)

//Exercises from https://fsharpforfunandprofit.com/posts/function-signatures/ :

//Test your understanding of function signatures
//How well do you understand function signatures? See if you can create simple functions that have each of these signatures.
//Avoid using explicit type annotations!

//val testA = int -> int
//val testB = int -> int -> int
//val testC = int -> (int -> int)
//val testD = (int -> int) -> int
//val testE = int -> int -> int -> int
//val testF = (int -> int) -> (int -> int)
//val testG = int -> (int -> int) -> int
//val testH = (int -> int -> int) -> int

module Function =
    let toString o =
        let rec loop nested t =
            if FSharpType.IsTuple t then
                FSharpType.GetTupleElements t
                |> Array.map (loop true)
                |> String.concat " * "
            elif FSharpType.IsFunction t then
                let fs =
                    if nested then
                        sprintf "(%s -> %s)"
                    else
                        sprintf "%s -> %s"

                let domain, range = FSharpType.GetFunctionElements t
                fs (loop true domain) (loop false range)
            else
                t.FullName

        loop false (o.GetType())

    let printTypeSignature name typeAsString =
        Console.WriteLine($"{name} : {typeAsString}")
//Due to the use of the + operator associated with a numeric value, the types were correctly inferred for every argument as 'int'
//Functions written in curried form (space between arguments)
let testA x = x + 1
let testB x y = x + y
let testC x = testB (x + 1) //Here, partial application is employed
let testC_2 x = (fun (y) -> y + x) //Here, another variant of testC type signature. Comment: I dont really understand how type inference worked here. y could be a string for all I know!
let testD fx = (1 |> fx) + 2
let testE x y z = x + y + z
let testF fx = (fun y -> (1 |> fx) + y) //Comment: I dont really understand how type inference worked here. Same as in testC_2 (see above)
let testG x fx = (fx (1 + x)) + 1
let testH fx = (fx 1 2) + 4

//Tupled versions of relevant functions above:
let testBTupled (x, y) = x + y
let testETupled (x, y, z) = x + y + z
let testGTupled (x, fx) = (fx (1 + x)) + 1

//Tupled functions are usually used in methods (functions associated with a type). Their advantage is to represent a format similar
//to the way arguments are usually passed in .NET methods.
//However, tupled functions cannot be decomposed in partial applications nor forward-piped (composed)

[<EntryPoint>]
let main args =
    //Here the concepts of function composition and partial application are tested / exercised
    Console.WriteLine("Exercise results:")

    testA
    |> Function.toString
    |> Function.printTypeSignature (nameof testA)

    testB
    |> Function.toString
    |> Function.printTypeSignature (nameof testB)

    testC
    |> Function.toString
    |> Function.printTypeSignature (nameof testC)

    testD
    |> Function.toString
    |> Function.printTypeSignature (nameof testD)

    testE
    |> Function.toString
    |> Function.printTypeSignature (nameof testE)

    testF
    |> Function.toString
    |> Function.printTypeSignature (nameof testF)

    testG
    |> Function.toString
    |> Function.printTypeSignature (nameof testG)

    testH
    |> Function.toString
    |> Function.printTypeSignature (nameof testH)

    //Demonstrating the concept of partial application
    let partialTestB = testB 1
    //PartialTestB should be of type int -> int
    Console.WriteLine(
        $"Signature of partialTestB: {partialTestB
                                      |> Function.toString
                                      |> Function.printTypeSignature (nameof partialTestB)}"
    )

    Console.WriteLine("Demonstrating the concept of partial application:")

    Console.WriteLine(
        $"(partialTestB 1) = (testB 1 1) <-> {testB 1 1} = {partialTestB 1} ({partialTestB 1 = testB 1 1})"
    )
    //Tupled versions of the functions:
    Console.WriteLine(
        "Demonstrating and comparing the type signatures of tupled versions of the relevant functions to their usual curried form:"
    )

    testB
    |> Function.toString
    |> Function.printTypeSignature (nameof testB)

    testBTupled
    |> Function.toString
    |> Function.printTypeSignature (nameof testBTupled)

    testE
    |> Function.toString
    |> Function.printTypeSignature (nameof testB)

    testETupled
    |> Function.toString
    |> Function.printTypeSignature (nameof testETupled)

    testG
    |> Function.toString
    |> Function.printTypeSignature (nameof testG)

    testGTupled
    |> Function.toString
    |> Function.printTypeSignature (nameof testGTupled)

    0
