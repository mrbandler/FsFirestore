namespace FsFirestore.Tests

module Config =
    
    open System.IO
    open System
    open Google.Cloud.Firestore
    open FsFirestore.Types

    /// Collection name used by the CRUD tests.
    [<Literal>]    
    let CRUDCollection = "crud-tests"

    /// Collection name used by the query tests.
    [<Literal>]    
    let QueryCollection = "query-tests"

    /// Test class to be used as a model for the  tests.
    [<FirestoreData>]
    type Test() =
        inherit FirestoreDocument()

        [<FirestoreProperty>]
        member val str: string = "" with get, set 

        [<FirestoreProperty>]
        member val num: int = 0 with get, set

        [<FirestoreProperty>]
        member val arr: int[] = [| |] with get, set
            
    /// Finds GCP authentication path.
    let findGCPAuthentication =
        let solutionPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../.."))
        Path.Combine(solutionPath, "GCP.json")

