namespace FsFirestore.Tests

module TransactionTests =
    
    open Xunit
    open Google.Cloud.Firestore
    open FsFirestore.Firestore
    open FsFirestore.Transaction
    open Config
    open Data

    /// Add document with ID in a transaction to the Firestore DB test.
    [<Theory>]
    [<InlineData("test-1")>]
    [<InlineData("test-2")>]
    [<InlineData("1234")>]
    let ``Add document with given ID in transaction`` (docId: string) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        let testData = new Test()

        // Test.
        let transFunc (trans: Transaction) =
            addDocumentInTrans trans TransCollection docId testData

        let doc = runTransaction transFunc None
        let docData = convertTo<Test> doc

        Assert.NotNull(docData)
        Assert.Equal(docId, doc.Id)
        Assert.Equal(TransCollection, doc.Parent.Id)
        Assert.Equal<obj[]>(testData.AllFields, docData.AllFields)

        // Tear down.
        deleteDocument TransCollection docData.Id

    /// Update a document in a transaction in the Firestore DB test.
    [<Theory>]
    [<InlineData("Hello Test #1", 3)>]
    [<InlineData("Hello Test #2", 2)>]
    [<InlineData("Hello Test #3", 1)>]
    let ``Update document in transaction`` (updateStr, updateNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        let doc = addDocument TransCollection None (new Test())
        let docData = convertTo<Test> doc

        // Test.
        let transFunc (trans: Transaction) =
            docData.str <- updateStr
            docData.num <- updateNum
            updateDocumentInTrans trans doc.Parent.Id doc.Id docData

        let docUpdated = runTransaction transFunc None
        let docUpdatedData = convertTo<Test> docUpdated
        
        Assert.NotNull(docUpdated)
        Assert.Equal(updateStr, docUpdatedData.str)
        Assert.Equal(docData.str, docUpdatedData.str)   
        Assert.Equal(updateNum, docUpdatedData.num)
        Assert.Equal(docData.num, docUpdatedData.num)

        // Tear down.
        deleteDocument TransCollection doc.Id

    /// Retrieve a document in transaction from the Firestore DB test.
    [<Fact>]
    let ``Read document in transaction`` () =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        let testData = new Test() 
        let docRef = addDocument TransCollection None testData

        // Test.
        let transFunc (trans: Transaction) =
            documentInTrans<Test> trans TransCollection docRef.Id

        let docData = runTransaction transFunc None

        Assert.NotNull(docData)
        Assert.Equal<obj[]>(testData.AllFields, docData.AllFields)

        // Tear down.
        deleteDocument TransCollection docRef.Id

    /// Retrieve multiple documents in transaction from the Firestore DB test.
    [<Theory>]
    [<InlineData(2)>]
    [<InlineData(5)>]
    [<InlineData(10)>]
    let ``Read multiple documents in transaction`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
                
        // Create test data.
        let dataList = createTestData numOfDocs

        // Add the data to the DB.
        let docIds = dataList |> List.map (fun data -> (addDocument TransCollection None data).Id)

        // Test.
        let transFunc (trans: Transaction) =
            documentsInTrans<Test> trans TransCollection docIds |> List.ofSeq
        
        let docs = runTransaction transFunc None
        
        Assert.NotEmpty(docs)

        let sortedDocs = docs |> List.sortBy (fun doc -> doc.num)
        let sortedDataList = dataList |> List.sortBy (fun doc -> doc.num)
        List.iter2 (fun (createdData: Test) (docData: Test) -> Assert.Equal<obj[]>(createdData.AllFields, docData.AllFields)) sortedDataList sortedDocs

        // Tear down.
        deleteDocuments TransCollection docIds

    /// Delete a document in transaction from the Firestore DB test.
    [<Fact>]
    let ``Delete document in transaction`` () =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        let doc = addDocument TransCollection None (new Test())

        // Test.
        let transFunc (trans: Transaction) =
            deleteDocumentInTrans trans None TransCollection doc.Id

        runTransaction transFunc None
        let docAfterDel = document<Test> TransCollection doc.Id

        Assert.Null(docAfterDel)

        // Tear down.
        // Nothing to do here.
    
    /// Delete multiple documents in transaction from the Firestore DB test.
    [<Theory>]
    [<InlineData(2)>]
    [<InlineData(5)>]
    [<InlineData(10)>]
    let ``Delete multiple document in transaction`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test data.
        let dataList = createTestData numOfDocs

        // Add the data to the DB.
        let docIds = dataList |> List.map(fun data -> (addDocument TransCollection None data).Id)

        // Test.
        let transFunc (trans: Transaction) =
            deleteDocumentsInTrans trans None TransCollection docIds

        runTransaction transFunc None 
        let docsAfterDel = documents<Test> TransCollection docIds

        docsAfterDel
        |> List.ofSeq
        |> List.iter (fun doc -> Assert.Null(doc))

        // Tear down.
        // Nothing to do here.