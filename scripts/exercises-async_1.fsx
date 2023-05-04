//Exercises from Kip Eason's 'Stylish F#' book, Chapter 10

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

    let GetStringAsync (id: int) =
        // id is unused
        task {
            do! Async.Sleep(500)
            return Random.string ()
        }

module Async =
    let ParallelThrottled maxDegreeOfParallelism tasks =
        Async.Parallel(tasks, maxDegreeOfParallelism)

module Consumer =
    let GetData (count: int) =
        let strings =
            Array.init count (fun i -> Server.AsyncGetString i |> Async.RunSynchronously)

        strings |> Array.sort

    let AsyncGetDataParallel (count: int) =
        async {
            let! strings =
                (Array.init count (fun i -> Server.AsyncGetString i)) //Fork
                |> Async.Parallel //Join

            return (strings |> Array.sort)
        }

    let AsyncGetDataParallelThrottled (count: int) (maxDegreeOfParallelism: int) =
        async {
            let! strings =
                (Array.init count (fun i -> Server.AsyncGetString i)) //Fork
                |> Async.ParallelThrottled maxDegreeOfParallelism //Join

            return (strings |> Array.sort)
        }

    let GetDataAsyncSequential (count: int) =
        async {
            let! strings =
                (Array.init count (fun i -> Server.AsyncGetString i))
                |> Async.Sequential

            return (strings |> Array.sort)
        }

let logUsage () =
    printfn
        @"This exercise takes 1 argument. Usage: 
    - dotnet fsi exercises-async_1.fsx S: Run synchronous solution
    - dotnet fsi exercises-async_1.fsx AP: Run asynchronous parallel solution
    - dotnet fsi exercises-async_1.fsx APT: Run asynchronous parallel throttled (2) solution
    - dotnet fsi exercises-async_1.fsx AS: Run asynchronous sequential solution"

let args = fsi.CommandLineArgs |> Array.tail

async {
    if (args.Length <> 1) then
        logUsage ()
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
            let! results = Consumer.AsyncGetDataParallel 10
            printfn "Printing results (Asynchronous case - Parallel)"
            results |> Array.iter (fun s -> printfn "%s" s)
            printfn "\nThat took %ums" sw.ElapsedMilliseconds
            return 0
        | "APT" ->
            let! results = Consumer.AsyncGetDataParallelThrottled 10 2
            printfn "Printing results (Asynchronous case - Parallel)"
            results |> Array.iter (fun s -> printfn "%s" s)
            printfn "\nThat took %ums" sw.ElapsedMilliseconds
            return 0

        | "AS" ->
            let! results = Consumer.GetDataAsyncSequential 10
            printfn "Printing results (Asynchronous case - Parallel)"
            results |> Array.iter (fun s -> printfn "%s" s)
            printfn "\nThat took %ums" sw.ElapsedMilliseconds
            return 0
        | _ ->
            logUsage ()
            return 1

}
|> Async.RunSynchronously //since its the only task in main, it makes no difference



//If you run the code, you’ll notice that this operation takes over 5 seconds to get ten results.
//Change the Consumer.GetData() function so that it is asynchronous and so that it runs all
//its calls to Server.AsyncGetString() in parallel.
//You don’t need to throttle the parallel computation. The changed function should be an F# style
//async function; that is, it should return Async<String[]>.
//Hint: You’ll also need to change the calling code so that the result of the changed function is
//passed into Async.RunSynchronously
