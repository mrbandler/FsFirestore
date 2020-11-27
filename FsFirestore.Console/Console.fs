namespace FsFirestore.Console

module DebuggingConsole =
    
    open FsFirestore.Firestore
    open FsFirestore.Tests.Config

    let run =
        connectToFirestore findGCPAuthentication |> ignore
