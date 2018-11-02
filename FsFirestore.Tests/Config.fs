namespace FsFirestore.Tests

module Config =
    
    open System.IO
    open System

    /// Collection ID used for the CRUD tests.
    [<Literal>]    
    let CRUDCollection = "crud-tests"

    /// Collection ID used for the query tests.
    [<Literal>]    
    let QueryCollection = "query-tests"

    /// Collection ID used for the transaction tests.
    [<Literal>]
    let TransCollection = "trans-tests"

    /// Collection ID used for the transaction tests.
    [<Literal>]
    let ListenCollection = "listen-tests"
            
    /// Finds GCP authentication path.
    let findGCPAuthentication =
        let solutionPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../.."))
        Path.Combine(solutionPath, "GCP.json")

