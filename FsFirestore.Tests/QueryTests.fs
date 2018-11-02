namespace FsFirestore.Tests

module QueryTests =
    
    open System
    open Xunit
    open FsFirestore.Firestore
    open FsFirestore.Query
    open Config
    open Data

    [<Theory>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'end at'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let endAtNum = Random().Next(2, numOfDocs - 2)
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let endAtData = dataList |> List.find (fun data -> data.Num = endAtNum)
        let queryResult = 
            collection QueryCollection
            |> orderBy "Num"
            |> endAt (endAtData.Fields("Num"))
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, endAtNum)
        queryResult
        |> List.iter (fun doc -> Assert.NotNull(doc))

        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'end before'`` (numOfDocs) =        
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let endBeforeNum = Random().Next(2, numOfDocs - 2)
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let endAtData = dataList |> List.find (fun data -> data.Num = endBeforeNum)
        let queryResult = 
            collection QueryCollection
            |> orderBy "Num"
            |> endBefore (endAtData.Fields("Num"))
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, endBeforeNum - 1)
        queryResult
        |> List.iter (fun doc -> Assert.NotNull(doc))

        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'start at'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let startAtNum = Random().Next(2, numOfDocs - 2)
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let endAtData = dataList |> List.find (fun data -> data.Num = startAtNum)
        let queryResult = 
            collection QueryCollection
            |> orderBy "Num"
            |> startAt (endAtData.Fields("Num"))
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, numOfDocs - (startAtNum - 1))
        queryResult
        |> List.iter (fun doc -> Assert.NotNull(doc))

        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'start after'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let startAfterNum = Random().Next(2, numOfDocs - 2)
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let endAtData = dataList |> List.find (fun data -> data.Num = startAfterNum)
        let queryResult = 
            collection QueryCollection
            |> orderBy "Num"
            |> startAfter (endAtData.Fields("Num"))
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, numOfDocs - startAfterNum)
        queryResult
        |> List.iter (fun doc -> Assert.NotNull(doc))

        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'limit'`` (numOfDocs, limitCount) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test
        let queryResult = 
            collection QueryCollection
            |> limit limitCount
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, limitCount)
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))

        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'offset'`` (numOfDocs, offsetCount) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test
        let queryResult = 
            collection QueryCollection
            |> offset offsetCount
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, (numOfDocs - offsetCount))
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))

        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'order by asc'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> orderBy "Num"
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, numOfDocs)
        
        let isAscOrdered = queryResult |> List.pairwise |> List.forall (fun (a, b) -> a.Num <= b.Num)
        Assert.Equal(isAscOrdered, true)

        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'oder by desc'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> orderByDescending "Num"
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, numOfDocs)
        
        let isDescOrdered = queryResult |> List.pairwise |> List.forall (fun (a, b) -> a.Num >= b.Num)
        Assert.Equal(isDescOrdered, true)

        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(5)>]
    let `` Query documents with 'select'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> select [|"Num"|]
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, numOfDocs)
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc.Num))
        queryResult |> List.iter (fun doc -> Assert.Equal("", doc.Str))

        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'array contains'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let arrayContainsValue = Random().Next(2, numOfDocs - 2)
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereArrayContains "Arr" arrayContainsValue
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, numOfDocs - (arrayContainsValue - 1))
        queryResult |> List.iter (fun doc ->  Assert.True(doc.Arr |> Array.contains arrayContainsValue))
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))

        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'where equal to'`` (numOfDocs, selectNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereEqualTo "Num" selectNum
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, 1)

        let doc = queryResult |> List.find (fun doc -> doc.Num = selectNum)
        Assert.NotNull(doc)

        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'where greater then'`` (numOfDocs, greaterThenNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereGreaterThen "Num" greaterThenNum
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        let filteredDocs = queryResult |> List.filter (fun doc -> doc.Num > greaterThenNum)
        Assert.Equal(queryResult.Length, filteredDocs.Length)
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))
 
        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'where greater then or equal to'`` (numOfDocs, greaterThenOrEqualNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereGreaterThenOrEqualTo "Num" greaterThenOrEqualNum
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        let filteredDocs = queryResult |> List.filter (fun doc -> doc.Num >= greaterThenOrEqualNum)
        Assert.Equal(queryResult.Length, filteredDocs.Length)
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))
 
        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'where less then'`` (numOfDocs, lessThenNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)
         
        // Test.
        let queryResult =
            collection QueryCollection
            |> whereLessThen "Num" lessThenNum
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        let filteredDocs = queryResult |> List.filter (fun doc -> doc.Num < lessThenNum)
        Assert.Equal(queryResult.Length, filteredDocs.Length)
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))
 
        // Tear down.
        deleteDocuments None QueryCollection docIds

    [<Theory>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'where less then or equal to'`` (numOfDocs, lessThenOrEqualNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereLessThenOrEqualTo "Num" lessThenOrEqualNum
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        let filteredDocs = queryResult |> List.filter (fun doc -> doc.Num <= lessThenOrEqualNum)
        Assert.Equal(queryResult.Length, filteredDocs.Length)
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))
 
        // Tear down.
        deleteDocuments None QueryCollection docIds
     
    [<Theory>]
    [<InlineData(10, 2, 7)>]
    [<InlineData(15, 4, 13)>]
    let `` Query documents with multiple conditions`` (numOfDocs, greaterThenNum, lessThenNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection None data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereGreaterThen "Num" greaterThenNum
            |> whereLessThen "Num" lessThenNum
            |> orderBy "Num"
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        
        let filteredDocs = queryResult |> List.filter (fun doc -> doc.Num > greaterThenNum && doc.Num < lessThenNum)
        Assert.Equal(queryResult.Length, filteredDocs.Length)
        
        let isAscOrdered = queryResult |> List.pairwise |> List.forall (fun (a, b) -> a.Num <= b.Num)
        Assert.Equal(isAscOrdered, true)

        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))
 
        // Tear down.
        deleteDocuments None QueryCollection docIds

