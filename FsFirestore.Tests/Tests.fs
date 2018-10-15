namespace FsFirestore.Tests

module Tests =

    open System.IO
    open System
    open Xunit
    open Google.Cloud.Firestore
    open FsFirestore.Firestore
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
    let private findGCPAuthentication =
        let solutionPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../.."))
        Path.Combine(solutionPath, "GCP.json")

    /// Init firestore connection test.
    [<Fact>]
    let ``Init Firestore connection`` () =
        let result = initFirestore findGCPAuthentication
        Assert.Equal(true, result)

    ///// Add document with generated ID to the Firestore DB test.
    //[<Fact>]
    //let ``Add document with generated ID`` () =
    //    // Build up.
    //    initFirestore findGCPAuthentication |> ignore
    //    let testData = new Test()

    //    // Test.
    //    let doc = addDocument testCollection testData
    //    let docData = deserializeDocument<Test> doc

    //    Assert.NotNull(doc.Id)
    //    Assert.Equal(testCollection, doc.Parent.Id)
    //    Assert.Equal<obj[]>(testData.fields, docData.fields)

    //    // Tear down.
    //    deleteDocument testCollection doc.Id

    ///// Add document with ID to the Firestore DB test.
    //[<Fact>]
    //let ``Add document with given ID`` () =
    //    // Build up.
    //    initFirestore findGCPAuthentication |> ignore
    //    let testData = new Test()

    //    // Test.
    //    let doc = addDocumentWithId testCollection testDocumentId testData
    //    let docData = deserializeDocument<Test> doc

    //    Assert.NotNull(doc.Id)
    //    Assert.Equal(testDocumentId, doc.Id)
    //    Assert.Equal(testCollection, doc.Parent.Id)
    //    Assert.Equal<obj[]>(testData.fields, docData.fields)

    //    // Tear down.
    //    deleteDocument testCollection testDocumentId

    ///// Update a document in the Firestore DB test.
    //[<Fact>]
    //let ``Update document`` () =
    //    // Build up.
    //    initFirestore findGCPAuthentication |> ignore
    //    let doc = addDocument testCollection (new Test())
    //    let docData = deserializeDocument<Test> doc

    //    // Test.
    //    let newStr = "Hello Test!"
    //    docData.str <- newStr
    //    updateDocument doc.Parent.Id doc.Id docData |> ignore
    //    let docUpdatedData = deserializeDocument<Test> doc
        
    //    Assert.Equal(newStr, docUpdatedData.str)
    //    Assert.Equal(docData.str, docUpdatedData.str)

    //    // Tear down.
    //    deleteDocument testCollection doc.Id

    ///// Retrieve a document from the Firestore DB test.
    //[<Fact>]
    //let ``Retrieve document`` () =
    //    // Build up.
    //    initFirestore findGCPAuthentication |> ignore
    //    let testData = new Test() 
    //    let docRef = addDocument testCollection testData

    //    // Test.
    //    let docData = document<Test> testCollection docRef.Id

    //    Assert.NotNull(docData)
    //    Assert.Equal<obj[]>(testData.fields, docData.fields)

    //    // Tear down.
    //    deleteDocument testCollection docRef.Id


    ///// Retrieve multiple documents from the Firestore DB test.
    //[<Fact>]
    //let ``Retrieve multiple documents`` () =
    //    // Build up.
    //    initFirestore findGCPAuthentication |> ignore

    //    // Little rec function to create data.
    //    let rec createData (list: List<Test>) =
    //        let length = list.Length
    //        if length = 3 then
    //            list
    //        else
    //            let counter = length + 1
    //            let data = new Test()
    //            data.str <- counter.ToString()
    //            data.num <- counter
    //            let newList = list @ [data]

    //            createData newList
                
    //    // Create test data.
    //    let dataList = createData []

    //    // Add the data to the DB.
    //    let docIds = dataList |> List.map (fun data -> (addDocument testCollection data).Id)

    //    // Test.
    //    let docs = Seq.toList (documents<Test> testCollection docIds)
    //    List.iter2 (fun (createdData: Test) (docData: Test) -> Assert.Equal<obj[]>(createdData.fields, docData.fields)) dataList docs
 
    //    // Tear down.
    //    docIds
    //    |> Seq.iter (fun id -> deleteDocument testCollection id)

    ///// Delete a document from the Firestore DB test.
    //[<Fact>]
    //let ``Delete document`` () =
    //    // Build up.
    //    initFirestore findGCPAuthentication |> ignore
    //    let doc = addDocument testCollection (new Test())

    //    // Test.
    //    deleteDocument testCollection doc.Id
    //    let docAfterDel = document<Test> testCollection doc.Id

    //    Assert.Null(docAfterDel)

    //    // Tear down.
    //    // Nothing to do here.       
        
    //[<Fact>]
    //let `` Query documents with 'end at'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'end before'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'start at'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'start after'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'limit'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'offset'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'order by asc'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'oder by desc'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'select'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'where equal to'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'where greater then'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'where greater then or equal to'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'where less then'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()

    //[<Fact>]
    //let `` Query documents with 'where less then or equal to'`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()
     
    //[<Fact>]
    //let `` Query documents with multiple conditions`` () =
    //    // Build up.

    //    // Test.

    //    // Tear down.
    //    ()