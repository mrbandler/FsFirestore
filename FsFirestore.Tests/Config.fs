namespace FsFirestore.Tests

module Config =
    
    open System.IO
    open System
    open Google.Cloud.Firestore
    open FsFirestore.Types

    /// Collection name used by the tests.
    [<Literal>]    
    let testCollection = "tests"

    /// Document IDs used by the tests.
    [<Literal>]
    let testDocumentId = "test-id"

    /// Test class to be used as a model for the  tests.
    [<FirestoreData>]
    type Test() =
        inherit FirestoreDocument()

        [<FirestoreProperty>]
        member val str = "Hello World!" with get, set 

        [<FirestoreProperty>]
        member val num = 42 with get, set
            
    /// Finds GCP authentication path.
    let findGCPAuthentication =
        let solutionPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../.."))
        Path.Combine(solutionPath, "GCP.json")

