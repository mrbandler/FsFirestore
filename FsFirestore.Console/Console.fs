namespace FsFirestore.Console

module DebuggingConsole =
    
    open Google.Cloud.Firestore
    open FsFirestore.Firestore
    open FsFirestore.Query
    open FsFirestore.Listening
    open FsFirestore.Tests.Data
    open FsFirestore.Tests.Config
    open System.Threading.Tasks

    let run =
        connectToFirestore findGCPAuthentication |> ignore
