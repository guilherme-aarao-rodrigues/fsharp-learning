open System
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Reflection

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

let testFunctionCurried x y = x + y
let partialApplicationOfTestFunction = testFunctionCurried 1
let partialApplicationOfTestFunctionWrittenAlternatively = 1 |> testFunctionCurried

//Tupled version
let testFunctionTupled (x, y) = x + y

testFunctionCurried |> Function.toString |> Function.printTypeSignature (nameof testFunctionCurried)
partialApplicationOfTestFunction |> Function.toString |> Function.printTypeSignature (nameof partialApplicationOfTestFunction)
partialApplicationOfTestFunctionWrittenAlternatively |> Function.toString |> Function.printTypeSignature (nameof partialApplicationOfTestFunctionWrittenAlternatively)

//Tupled functions are usually used in methods (functions associated with a type). Their advantage is to represent a format similar
//to the way arguments are usually passed in .NET methods.
//However, tupled functions cannot be decomposed in partial applications nor forward-piped (composed)

testFunctionTupled |> Function.toString |> Function.printTypeSignature (nameof testFunctionTupled)

