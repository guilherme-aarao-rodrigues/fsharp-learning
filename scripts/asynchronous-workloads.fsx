//Suggested coding practice for asynchronous workflows: Write the code first synchronously, then translate relevant portions to async
//attention to the use of match! to pattern match on the results of an async computation
//use of lock to make function thread safe when using shared resources (I/O, etc)
//Batching Seq.chunkBySize
//throttling : Async.Parallel maxDegreeOfParallelism parameter (optional)
//-Hot tasks: Tasks that are started/executed as soon as they are created. Closely aligned with C# Tasks.
//  Expressions inside the task {} code block in F# are already executed as soon as the code block is called.
//-Cold tasks: Tasks that are created but not yet executed.
//  They represent a workload constructor that will only get started once the caller explicitly does so.

#r "nuget: FSharp.Data"

module Log =
    open System
    open System.Threading

    /// Print a colored log message.
    let message (color: ConsoleColor) (message: string) =
        Console.ForegroundColor <- color
        printfn "%s (thread ID: %i)" message Thread.CurrentThread.ManagedThreadId
        Console.ResetColor()

    /// Print a red log message.
    let red = message ConsoleColor.Red
    /// Print a green log message.
    let green = message ConsoleColor.Green
    /// Print a yellow log message.
    let yellow = message ConsoleColor.Yellow
    /// Print a cyan log message.
    let cyan = message ConsoleColor.Cyan


module DownloadSynchronous =
    open FSharp.Data
    open System
    open System.IO
    open System.Net
    open System.Text.RegularExpressions

    /// If a download link starts with http: or https: return a Uri of it
    /// unchanged, otherwise return a uri of it relative to its page.
    let private absoluteUri (pageUri: Uri) (filePath: string) =
        if
            filePath.StartsWith("http:")
            || filePath.StartsWith("https:")
        then
            Uri(filePath)
        else
            let separator = '/'

            filePath.TrimStart(separator)
            |> (sprintf "%O%c%s" pageUri separator)
            |> Uri

    /// Get the URLs of all links in a specified page matching a
    /// specified regex pattern.
    let private getLinks (pageUri: Uri) (filePattern: string) =
        Log.cyan "Getting names..."
        let re = Regex(filePattern)
        let html = HtmlDocument.Load(pageUri.AbsoluteUri)

        let links =
            html.Descendants [ "a" ]
            |> Seq.choose (fun node ->
                node.TryGetAttribute("href")
                |> Option.map (fun att -> att.Value()))
            |> Seq.filter (re.IsMatch)
            |> Seq.map (absoluteUri pageUri)
            |> Seq.distinct
            |> Array.ofSeq

        links

    /// Download a file to the specified local path.
    let private tryDownload (localPath: string) (fileUri: Uri) =
        let fileName = fileUri.Segments |> Array.last
        Log.yellow (sprintf "%s - starting download" fileName)
        let filePath = Path.Combine(localPath, fileName)
        use client = new WebClient()

        try
            client.DownloadFile(fileUri, filePath)
            Log.green (sprintf "%s - download complete" fileName)
            Result.Ok fileName
        with
        | e ->
            let message =
                e.InnerException
                |> Option.ofObj
                |> Option.map (fun ie -> ie.Message)
                |> Option.defaultValue e.Message

            Log.red (sprintf "%s - error: %s" fileName message)
            Result.Error e.Message

    /// Download all the files linked to in the specified webpage, whose link path matches the specified regular expression,
    /// to the specified local path.

    let GetFiles (pageUri: Uri) (filePattern: string) (localPath: string) =
        let links = getLinks pageUri filePattern
        let downloadResults = links |> Array.map (tryDownload localPath)

        let isOk =
            function
            | Ok _ -> true
            | Error _ -> false

        let successCount = downloadResults |> Seq.filter isOk |> Seq.length

        let errorCount =
            downloadResults
            |> Seq.filter (isOk >> not)
            |> Seq.length

        {| SuccessCount = successCount
           ErrorCount = errorCount |}



open System
open System.Diagnostics

//  Usage: dotnet fsi asynchronous-workloads.fsx [uri] [pattern] [localPath] [mode]
let logUsageError =
    Log.red
        @"Usage: massdownload url nameregex downloadpath mode - e.g. 
    massdownload https://minorplanetcenter.net/data neam.*\.json\.gz$ c:\temp\downloads synchronous
    modes: synchronous .. (more to come)"

let args = fsi.CommandLineArgs |> Array.tail

type Mode =
    | Synchronous
    | Asynchronous
    | AsynchronousBatched
    | AsynchronousThrottled
    | Invalid
// A program to get multiple files from download links provided on a website.
// E.g. dotnet fsi asynchronous-workloads.fsx https://minorplanetcenter.net/data "neam.*\.json\.gz$" "C:\Users\GuilhermeRodrigues\source\fsharp-coaching\learning-experiments\fsharp-learning\scripts\asynchronous-workloads-filedump" "Synchronous"
// dotnet run http://compling.hss.ntu.edu.sg/omw "\.zip$" "c:\temp\downloads"
// Large!
// dotnet run http://storage.googleapis.com/books/ngrams/books/datasetsv2.html "eng\-1M\-2gram.*\.zip$" "c:\temp\downloads"
if args.Length = 4 then
    let uri = Uri args.[0]
    let pattern = args.[1]
    let localPath = args.[2]

    let mode =
        match args.[3].ToLower() with
        | "synchronous" -> Mode.Synchronous
        | _ -> Mode.Invalid

    let sw = Stopwatch()
    sw.Start()

    let result =
        match mode with
        | Synchronous -> DownloadSynchronous.GetFiles uri pattern localPath
        | _ -> {| SuccessCount = 0; ErrorCount = 0 |}

    Log.cyan (
        sprintf
            "%i files downloaded in %0.1fs, %i failed."
            result.SuccessCount
            sw.Elapsed.TotalSeconds
            result.ErrorCount
    )

    0
else
    logUsageError
    1
