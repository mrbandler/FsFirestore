namespace FsFirestore.Tests

module CRUDTests =

    open Xunit
    open FsFirestore.Firestore
    open Config

    /// Init firestore connection test.
    [<Fact>]
    let ``Init Firestore connection`` () =
        let result = initFirestore findGCPAuthentication
        Assert.Equal(true, result)

    /// Add document with generated ID to the Firestore DB test.
    [<Fact>]
    let ``Add document with generated ID`` () =
        // Build up.
        initFirestore findGCPAuthentication |> ignore
        let testData = new Test()

        // Test.
        let doc = addDocument testCollection testData
        let docData = convertTo<Test> doc

        Assert.NotNull(doc.Id)
        Assert.Equal(testCollection, doc.Parent.Id)
        Assert.Equal<obj[]>(testData.fields, docData.fields)

        // Tear down.
        deleteDocument testCollection doc.Id

    /// Add document with ID to the Firestore DB test.
    [<Fact>]
    let ``Add document with given ID`` () =
        // Build up.
        initFirestore findGCPAuthentication |> ignore
        let testData = new Test()

        // Test.
        let doc = addDocumentWithId testCollection testDocumentId testData
        let docData = convertTo<Test> doc

        Assert.NotNull(doc.Id)
        Assert.Equal(testDocumentId, doc.Id)
        Assert.Equal(testCollection, doc.Parent.Id)
        Assert.Equal<obj[]>(testData.fields, docData.fields)

        // Tear down.
        deleteDocument testCollection testDocumentId

    /// Update a document in the Firestore DB test.
    [<Fact>]
    let ``Update document`` () =
        // Build up.
        initFirestore findGCPAuthentication |> ignore
        let doc = addDocument testCollection (new Test())
        let docData = convertTo<Test> doc

        // Test.
        let newStr = "Hello Test!"
        docData.str <- newStr
        updateDocument doc.Parent.Id doc.Id docData |> ignore
        let docUpdatedData = convertTo<Test> doc
        
        Assert.Equal(newStr, docUpdatedData.str)
        Assert.Equal(docData.str, docUpdatedData.str)

        // Tear down.
        deleteDocument testCollection doc.Id

    /// Retrieve a document from the Firestore DB test.
    [<Fact>]
    let ``Retrieve document`` () =
        // Build up.
        initFirestore findGCPAuthentication |> ignore
        let testData = new Test() 
        let docRef = addDocument testCollection testData

        // Test.
        let docData = document<Test> testCollection docRef.Id

        Assert.NotNull(docData)
        Assert.Equal<obj[]>(testData.fields, docData.fields)

        // Tear down.
        deleteDocument testCollection docRef.Id


    /// Retrieve multiple documents from the Firestore DB test.
    [<Fact>]
    let ``Retrieve multiple documents`` () =
        // Build up.
        initFirestore findGCPAuthentication |> ignore

        // Little rec function to create data.
        let rec createData (list: List<Test>) =
            let length = list.Length
            if length = 3 then
                list
            else
                let counter = length + 1
                let data = new Test()
                data.str <- counter.ToString()
                data.num <- counter
                let newList = list @ [data]

                createData newList
                
        // Create test data.
        let dataList = createData []

        // Add the data to the DB.
        let docIds = dataList |> List.map (fun data -> (addDocument testCollection data).Id)

        // Test.
        let docs = Seq.toList (documents<Test> testCollection docIds)
        List.iter2 (fun (createdData: Test) (docData: Test) -> Assert.Equal<obj[]>(createdData.fields, docData.fields)) dataList docs
 
        // Tear down.
        docIds
        |> Seq.iter (fun id -> deleteDocument testCollection id)

    /// Delete a document from the Firestore DB test.
    [<Fact>]
    let ``Delete document`` () =
        // Build up.
        initFirestore findGCPAuthentication |> ignore
        let doc = addDocument testCollection (new Test())

        // Test.
        deleteDocument testCollection doc.Id
        let docAfterDel = document<Test> testCollection doc.Id

        Assert.Null(docAfterDel)

        // Tear down.
        // Nothing to do here.