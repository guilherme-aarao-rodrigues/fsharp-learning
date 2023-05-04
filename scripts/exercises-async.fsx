//Exercises from Kip Eason's 'Stylish F#' book

//This section contains exercises to help you get used to translating code into an
//asynchronous world.

//EXERCISE 10-1 – MAKING SOME CODE ASYNCHRONOUS
//In the following code, the Server module contains a simulated server endpoint that returns
//a random string, taking half a second to do so. In the Consumer module, we call the server
//multiple times to build up an array of strings, which we then sort to produce a final result

open System

module Random =
    let private random = System.Random()

    let string () =
        let length = random.Next(0, 10)

        Array.init length (fun _ -> random.Next(0, 255) |> char)
        |> String

module Server =
    let AsyncGetString (id: int) =
        // id is unused
        async {
            do! Async.Sleep(500)
            return Random.string ()
        }

module Consumer =
    let GetData (count: int) =
        let strings =
            Array.init count (fun i -> Server.AsyncGetString i |> Async.RunSynchronously)

        strings |> Array.sort

    let GetDataAsync (count: int) =
        async {
            let! strings =
                (Array.init count (fun i -> Server.AsyncGetString i))
                |> Async.Parallel

            return (strings |> Array.sort)
        }

let logUsageError() =
    printfn
        @"This exercise takes 1 argument. Usage: 
    - dotnet fsi exercises-async.fsx S: Run synchronous solution
    - dotnet fsi exercises-async.fsx AP: Run asynchronous parallel solution
    - dotnet fsi exercises-async.fsx AS: Run asynchronous sequential solution"

let args = fsi.CommandLineArgs |> Array.tail
async {
if (args.Length <> 1) then
    logUsageError()
    return 1
else
    let sw = System.Diagnostics.Stopwatch()
    sw.Start()
    match args.[0] with
    | "S" -> 
        let results = Consumer.GetData 10
        printfn "Printing results (Synchronous case)"
        results |> Array.iter (fun s -> printfn "%s" s)
        printfn "That took %ums" sw.ElapsedMilliseconds
        return 0
    | "AP" -> 
        let! results = Consumer.GetDataAsync 10
        printfn "Printing results (Asynchronous case - Parallel)"
        results |> Array.iter (fun s -> printfn "%s" s)
        printfn "\nThat took %ums" sw.ElapsedMilliseconds
        return 0
    | _ -> 
        logUsageError()
        return 1

}|> Async.RunSynchronously //since its the only task in main, it makes no difference
      
        

//If you run the code, you’ll notice that this operation takes over 5 seconds to get ten results.
//Change the Consumer.GetData() function so that it is asynchronous and so that it runs all
//its calls to Server.AsyncGetString() in parallel.
//You don’t need to throttle the parallel computation. The changed function should be an F# style
//async function; that is, it should return Async<String[]>.
//Hint: You’ll also need to change the calling code so that the result of the changed function is
//passed into Async.RunSynchronously
