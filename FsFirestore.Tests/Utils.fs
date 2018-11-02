namespace FsFirestore.Tests

module Utils =

    open System.Threading.Tasks

    let wait seconds =  
        Task.Delay(seconds * 1000)
        |> Async.AwaitTask
        |> Async.RunSynchronously
