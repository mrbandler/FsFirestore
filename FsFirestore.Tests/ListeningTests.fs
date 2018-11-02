namespace FsFirestore.Tests

module ListeningTests =
    
    open System
    open Xunit
    open Google.Cloud.Firestore
    open FsFirestore.Firestore
    open FsFirestore.Listening
    open FsFirestore.Query
    open Config
    open Data
    open Utils

    [<Literal>]
    let DocId = "test-id"

    /// Listen for listener successfully added on Firestore DB test.
    [<Fact>]
    let ``Listener mounted`` () =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Test.
        let callback (snap: DocumentSnapshot) =
            Assert.Equal(snap.Id, DocId)
            Assert.Equal(snap.Reference.Parent.Id, ListenCollection)
            Assert.False(snap.Exists)

        let listener = listenOnDocument ListenCollection DocId callback

        // Tear down.
        wait 2
        stopListening listener

    /// Listen for document creation on Firestore DB test.
    [<Fact>]
    let ``Listen for document creation`` () =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        let testData = new Test()
        testData.Fill
        let mutable intialCallbackCalled = false

        // Test.
        let callback (snap: DocumentSnapshot) =
            if intialCallbackCalled = true then
                let data = snap |> convertSnapshotTo<Test>
                Assert.Equal<obj[]>(testData.AllFields, data.AllFields)
            
            intialCallbackCalled <- true

        let listener = listenOnDocument ListenCollection DocId callback
        addDocument ListenCollection (Some DocId) testData |> ignore

        // Tear down.
        wait 2
        stopListening listener
        deleteDocument None ListenCollection DocId

    /// Listen for document update on Firestore DB test.
    [<Fact>]
    let ``Listen for document update`` () =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        
        let testData = new Test()
        testData.Fill
        let docRef = addDocument ListenCollection None testData
        
        let newStr = "The half answer..."
        let newNum = 21
        let newArr = [| for i in 1 .. newNum -> i |]
        let mutable intialCallbackCalled = false

        // Test.
        let callback (snap: DocumentSnapshot) =
            Assert.True(snap.Exists)
            if intialCallbackCalled = true then
                let snapData = snap |> convertSnapshotTo<Test>
                Assert.Equal(snapData.Str, newStr)
                Assert.Equal(snapData.Num, newNum)
                Assert.Equal<int[]>(snapData.Arr, newArr)

            intialCallbackCalled <- true 

        let listener = listenOnDocument docRef.Parent.Id docRef.Id callback
        
        let data = document<Test> docRef.Parent.Id docRef.Id
        data.Str <- newStr
        data.Num <- newNum
        data.Arr <- newArr
        updateDocument data.CollectionId data.Id data |> ignore 

        // Tear down.
        wait 2
        stopListening listener
        deleteDocument None docRef.Parent.Id docRef.Id

    /// Listen for document deletion on Firestore DB test.
    [<Fact>]
    let ``Listen for document deletion`` () =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore
        
        let testData = new Test()
        testData.Fill
        let docRef = addDocument ListenCollection None testData
        let mutable intialCallbackCalled = false

        // Test.
        let callback (snap: DocumentSnapshot) =
            if intialCallbackCalled = true then
                Assert.False(snap.Exists)

            intialCallbackCalled <- true 

        let listener = listenOnDocument docRef.Parent.Id docRef.Id callback
        deleteDocument None docRef.Parent.Id docRef.Id
  
        // Tear down.
        wait 2
        stopListening listener

    /// Listen for changes on a query on Firestore DB test.
    [<Theory>]
    [<InlineData(15, 4, 13)>]
    let ``Listen for query changes (all documents)`` (numOfDocs, greaterThenNum, lessThenNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument ListenCollection None data).Id)
        let mutable intialCallbackCalled = false

        // Test.
        let callback (querySnap: QuerySnapshot) =
            if intialCallbackCalled = true then
                let data = convertSnapshotsTo<Test> querySnap.Documents |> List.ofSeq
                Assert.Equal((lessThenNum - greaterThenNum), data.Length)

            intialCallbackCalled <- true 
        
        let listener =
            collection ListenCollection
            |> whereGreaterThen "Num" greaterThenNum
            |> whereLessThen "Num" lessThenNum
            |> orderBy "Num"
            |> listenOnQuery callback

        let newDocData = new Test()
        newDocData.Num <- Random().Next(greaterThenNum, lessThenNum)
        let docIds = List.append docIds [(addDocument ListenCollection None newDocData).Id]

        // Tear down.
        wait 2
        stopListening listener
        deleteDocuments None ListenCollection docIds

    /// Listen for changes on a query on Firestore DB test.
    [<Theory>]
    [<InlineData(15, 4, 13)>]
    let ``Listen for query changes (only changed documents)`` (numOfDocs, greaterThenNum, lessThenNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument ListenCollection None data).Id)
        let mutable intialCallbackCalled = false
        let newNum = Random().Next(greaterThenNum, lessThenNum)

        // Test.
        let callback (querySnap: QuerySnapshot) =
            if intialCallbackCalled = true then
                let changes = convertQueryChanges<Test> querySnap.Changes |> List.ofSeq

                changes
                |> List.map (fun change -> Assert.NotNull(change); change)
                |> List.map (fun change -> Assert.Equal(change.document.Num, newNum); change)
                |> List.iter (fun change -> Assert.Equal(change.changeType, DocumentChange.Type.Added))

            intialCallbackCalled <- true 
        
        let listener =
            collection ListenCollection
            |> whereGreaterThen "Num" greaterThenNum
            |> whereLessThen "Num" lessThenNum
            |> orderBy "Num"
            |> listenOnQuery callback

        let newDocData = new Test()
        newDocData.Num <- newNum
        let docIds = List.append docIds [(addDocument ListenCollection None newDocData).Id]

        // Tear down.
        wait 2
        stopListening listener
        deleteDocuments None ListenCollection docIds