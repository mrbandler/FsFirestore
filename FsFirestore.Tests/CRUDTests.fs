namespace FsFirestore.Tests

module CRUDTests =

    open System.Threading
    open Xunit
    open FsFirestore.Firestore
    open Config
    open Data

    /// Init firestore connection test.
    [<Fact>]
    let ``Establish Firestore connection`` () =
        let result = connectToFirestore findGCPAuthentication
        Assert.Equal(true, result)

    /// Add document with generated ID to the Firestore DB test.
    [<Fact>]
    let ``Add document with generated ID`` () =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        let testData = new Test()

        // Test.
        let doc = addDocument CRUDCollection testData
        let docData = convertTo<Test> doc

        Assert.NotNull(doc.Id)
        Assert.Equal(CRUDCollection, doc.Parent.Id)
        Assert.Equal<obj[]>(testData.fields, docData.fields)

        // Tear down.
        deleteDocument CRUDCollection doc.Id

    /// Add document with ID to the Firestore DB test.
    [<Theory>]
    [<InlineData("test-1")>]
    [<InlineData("test-2")>]
    [<InlineData("1234")>]
    let ``Add document with given ID`` (docId) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        let testData = new Test()

        // Test.
        let doc = addDocumentWithId CRUDCollection docId testData
        let docData = convertTo<Test> doc

        Assert.NotNull(doc.Id)
        Assert.Equal(docId, doc.Id)
        Assert.Equal(CRUDCollection, doc.Parent.Id)
        Assert.Equal<obj[]>(testData.fields, docData.fields)

        // Tear down.
        deleteDocument CRUDCollection docId

    /// Update a document in the Firestore DB test.
    [<Theory>]
    [<InlineData("Hello Test #1", 3)>]
    [<InlineData("Hello Test #2", 2)>]
    [<InlineData("Hello Test #3", 1)>]
    let ``Update document`` (updateStr, updateNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        let doc = addDocument CRUDCollection (new Test())
        let docData = convertTo<Test> doc

        // Test.
        docData.str <- updateStr
        docData.num <- updateNum
        updateDocument doc.Parent.Id doc.Id docData |> ignore
        let docUpdatedData = convertTo<Test> doc
        
        Assert.Equal(updateStr, docUpdatedData.str)
        Assert.Equal(docData.str, docUpdatedData.str)   
        Assert.Equal(updateNum, docUpdatedData.num)
        Assert.Equal(docData.num, docUpdatedData.num)

        // Tear down.
        deleteDocument CRUDCollection doc.Id

    /// Retrieve a document from the Firestore DB test.
    [<Fact>]
    let ``Read document`` () =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        let testData = new Test() 
        let docRef = addDocument CRUDCollection testData

        // Test.
        let docData = document<Test> CRUDCollection docRef.Id

        Assert.NotNull(docData)
        Assert.Equal<obj[]>(testData.fields, docData.fields)

        // Tear down.
        deleteDocument CRUDCollection docRef.Id


    /// Retrieve multiple documents from the Firestore DB test.
    [<Theory>]
    [<InlineData(2)>]
    [<InlineData(5)>]
    [<InlineData(10)>]
    let ``Read multiple documents`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
                
        // Create test data.
        let dataList = createTestData numOfDocs

        // Add the data to the DB.
        let docIds = dataList |> List.map (fun data -> (addDocument CRUDCollection data).Id)

        // Test.
        let docs = documents<Test> CRUDCollection docIds |> List.ofSeq
        Assert.NotEmpty(docs)
        
        let sortedDocs = docs |> List.sortBy (fun doc -> doc.num)
        let sortedDataList = dataList |> List.sortBy (fun doc -> doc.num)
        List.iter2 (fun (createdData: Test) (docData: Test) -> Assert.Equal<obj[]>(createdData.fields, docData.fields)) sortedDataList sortedDocs

        // Tear down.
        deleteDocuments CRUDCollection docIds

    [<Fact>]
    let ``Read all documents from collection`` () =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
    
        // Create test data and add it to the collection.
        let dataList = createTestData 5
        let docIds = dataList |> List.map (fun data -> (addDocument CRUDCollection data).Id)
        
        // Test.
        let docs = allDocuments<Test> CRUDCollection |> List.ofSeq
        Assert.NotEmpty(docs)

        let sortedDocs = docs |> List.sortBy (fun doc -> doc.num)
        let sortedDataList = dataList |> List.sortBy (fun doc -> doc.num)
        List.iter2 (fun (createdData: Test) (docData: Test) -> Assert.Equal<obj[]>(createdData.fields, docData.fields)) sortedDataList sortedDocs

        // Tear down.
        deleteDocuments CRUDCollection docIds

    /// Delete a document from the Firestore DB test.
    [<Fact>]
    let ``Delete document`` () =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        let doc = addDocument CRUDCollection (new Test())

        // Test.
        deleteDocument CRUDCollection doc.Id
        let docAfterDel = document<Test> CRUDCollection doc.Id

        Assert.Null(docAfterDel)

        // Tear down.
        // Nothing to do here.
    
    /// Delete multiple documents from the Firestore DB test.
    [<Theory>]
    [<InlineData(2)>]
    [<InlineData(5)>]
    [<InlineData(10)>]
    let ``Delete multiple document`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test data.
        let dataList = createTestData numOfDocs

        // Add the data to the DB.
        let docIds = dataList |> List.map(fun data -> (addDocument CRUDCollection data).Id)

        // Test.
        deleteDocuments CRUDCollection docIds
        let docsAfterDel = documents<Test> CRUDCollection docIds

        docsAfterDel
        |> List.ofSeq
        |> List.iter (fun doc -> Assert.Null(doc))

        // Tear down.
        // Nothing to do here.