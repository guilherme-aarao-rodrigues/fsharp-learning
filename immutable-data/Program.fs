open System
open Microsoft.FSharp.Collections

let printLists (lists: (string * (int list)) list) : unit =
    lists
    |> List.iter (fun (n, l) -> printfn "%s = %A" n l)

[<EntryPoint>]
let main args =   

    //Data in F# is immutable by default. 
    //Being immutable implies, operations performed over data merely transforms it and copies it somewhere else in memory, instead of changing it in its original location.
    //Variables are mere pointers to data in a given location in memory. 
    //This can be evaluated e.g. by the function System.Object.ReferenceEquals(objA:obj, objB:obj) : bool
    //Operations with these pointers that do not involve the word 'mutable' merely move these pointers around or create 
    //new pointers referencing new locations in memory. 
    //In this process, some data can partially be reused by the newly created variables. 
    //Since data is by default immutable in F#, this is safe.
    let listA = [ 1; 2; 3; 4 ]
    let listB = [ 1; 2; 3; 4 ]
    let listC = 0 :: listA
    let listD = listC.Tail

    printLists [ ((nameof listA), listA)
                 ((nameof listB), listB)
                 ((nameof listC), listC)
                 ((nameof listD), listD) ]
    //Here we can see that listA is not equal to listB despite being identical in content.
    Console.WriteLine($"listA = listB ? {Object.ReferenceEquals(listA, listB).ToString()}")
    //Here we can see that listD points to the same memory location as listA. Data was in this case reused.
    Console.WriteLine($"listA = listD ? {Object.ReferenceEquals(listA, listD).ToString()}")
    //Now we redeclare the variable listA as listB. This moves the pointer listA to the same memory location listB is pointing to
    let listA = listB
    //And now listA really is equal to listB
    Console.WriteLine(
        $"listA = listB after assignment listA = listB? {Object.ReferenceEquals(listA, listB).ToString()}"
    )
    //This statement isnt true anymore, since listD is pointing to the original location listA used to point before being assigned "= listB"
    Console.WriteLine(
        $"listggA = List D after assignment ListA = ListB? {Object.ReferenceEquals(listA, listD).ToString()}"
    )
    let listA = listD //Resetting the location of the pointer listA to its original position
    let mutable x = listA//Creating mutable location containing elements pointed by listA  
    printLists [(nameof x), x]
    Console.WriteLine(
        $"listA = x after creating mutable location x and assigning it to listA? {Object.ReferenceEquals(listA, x).ToString()}"
    )
    x <- [5; 6; 7]//Mutating x
    Console.WriteLine("Mutating x...")    
    printLists [(nameof x), x]//Printing x
    Console.WriteLine(
        $"listA = x after mutating x? {Object.ReferenceEquals(listA, x).ToString()}"
    )
    printLists [(nameof listA), listA]//Printing A
    //Conclusion: The results show that x references A only before actually mutating a value. 
    //If x is assigned another value, it will use another part of the memory to do so, preserving the contents pointed by listA.

    //References: https://fsharpforfunandprofit.com/posts/correctness-immutability/

    //Exercises: 
    //1. How would we add up ten numbers ( 1 .. 10 ) without using Linq.SumBy / Seq.sumBy - and you can't use the 
    //mutable keyword. what do you do?
    //let myArray = [ 1 .. 10 ]
    //let sum = myArray |> Seq.
    //2. Understand the difference between expressions + statements; how does it relate to mutability? how does it relate to the ignore function? what are the costs + benefits of expressions vs statements?
    0