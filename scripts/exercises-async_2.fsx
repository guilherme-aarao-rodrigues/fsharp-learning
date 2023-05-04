//Exercises from Kip Eason's 'Stylish F#' book, Chapter 10

//This section contains exercises to help you get used to translating code into an
//asynchronous world.

//EXERCISE 10-2 – RETURNING TASKS
//How would your solution to Exercise 10-1 change if Consumer.GetData() needed to return
//a C# style Task? The dummy API should be unchanged; in other words, it should still return an
//Async<string>.
//There is more than one way to solve this exercise.

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
    let GetDataAsTask (count: int) =
        task {
            let strings =
                Array.init count (fun i -> Server.AsyncGetString i |> Async.RunSynchronously)

            return strings |> Array.sort
        }


    let AsyncGetDataParallel (count: int) =
        async {
            let! strings =
                (Array.init count (fun i -> Server.AsyncGetString i)) //Fork
                |> Async.Parallel //Join

            return (strings |> Array.sort)
        }

let main () =
    task {
        let sw = System.Diagnostics.Stopwatch()
        sw.Start()
        let! results = Consumer.GetDataAsTask 10
        printfn "Printing results"
        results |> Array.iter (fun s -> printfn "%s" s)
        printfn "\nThat took %ums" sw.ElapsedMilliseconds
        return 0
    }

main () |> ignore
