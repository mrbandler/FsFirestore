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
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let endAtData = dataList |> List.find (fun data -> data.num = endAtNum)
        let queryResult = 
            collection QueryCollection
            |> orderBy "num"
            |> endAt (endAtData.fields("num"))
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, endAtNum)
        queryResult
        |> List.iter (fun doc -> Assert.NotNull(doc))

        // Tear down.
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'end before'`` (numOfDocs) =        
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let endBeforeNum = Random().Next(2, numOfDocs - 2)
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let endAtData = dataList |> List.find (fun data -> data.num = endBeforeNum)
        let queryResult = 
            collection QueryCollection
            |> orderBy "num"
            |> endBefore (endAtData.fields("num"))
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, endBeforeNum - 1)
        queryResult
        |> List.iter (fun doc -> Assert.NotNull(doc))

        // Tear down.
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'start at'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let startAtNum = Random().Next(2, numOfDocs - 2)
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let endAtData = dataList |> List.find (fun data -> data.num = startAtNum)
        let queryResult = 
            collection QueryCollection
            |> orderBy "num"
            |> startAt (endAtData.fields("num"))
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, numOfDocs - (startAtNum - 1))
        queryResult
        |> List.iter (fun doc -> Assert.NotNull(doc))

        // Tear down.
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'start after'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let startAfterNum = Random().Next(2, numOfDocs - 2)
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let endAtData = dataList |> List.find (fun data -> data.num = startAfterNum)
        let queryResult = 
            collection QueryCollection
            |> orderBy "num"
            |> startAfter (endAtData.fields("num"))
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, numOfDocs - startAfterNum)
        queryResult
        |> List.iter (fun doc -> Assert.NotNull(doc))

        // Tear down.
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(5, 3)>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'limit'`` (numOfDocs, limitCount) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

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
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(5, 3)>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'offset'`` (numOfDocs, offsetCount) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

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
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(5)>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'order by asc'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> orderBy "num"
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, numOfDocs)
        
        let isAscOrdered = queryResult |> List.pairwise |> List.forall (fun (a, b) -> a.num <= b.num)
        Assert.Equal(isAscOrdered, true)

        // Tear down.
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(5)>]
    [<InlineData(10)>]
    [<InlineData(15)>]
    let `` Query documents with 'oder by desc'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> orderByDescending "num"
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, numOfDocs)
        
        let isDescOrdered = queryResult |> List.pairwise |> List.forall (fun (a, b) -> a.num >= b.num)
        Assert.Equal(isDescOrdered, true)

        // Tear down.
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(5)>]
    let `` Query documents with 'select'`` (numOfDocs) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> select [|"num"|]
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, numOfDocs)
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc.num))
        queryResult |> List.iter (fun doc -> Assert.Equal("", doc.str))

        // Tear down.
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(5, 3)>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'where equal to'`` (numOfDocs, selectNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereEqualTo "num" selectNum
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        Assert.Equal(queryResult.Length, 1)

        let doc = queryResult |> List.find (fun doc -> doc.num = selectNum)
        Assert.NotNull(doc)

        // Tear down.
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(5, 3)>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'where greater then'`` (numOfDocs, greaterThenNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereGreaterThen "num" greaterThenNum
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        let filteredDocs = queryResult |> List.filter (fun doc -> doc.num > greaterThenNum)
        Assert.Equal(queryResult.Length, filteredDocs.Length)
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))
 
        // Tear down.
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(5, 3)>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'where greater then or equal to'`` (numOfDocs, greaterThenOrEqualNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereGreaterThenOrEqualTo "num" greaterThenOrEqualNum
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        let filteredDocs = queryResult |> List.filter (fun doc -> doc.num >= greaterThenOrEqualNum)
        Assert.Equal(queryResult.Length, filteredDocs.Length)
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))
 
        // Tear down.
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(5, 3)>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'where less then'`` (numOfDocs, lessThenNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereLessThen "num" lessThenNum
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        let filteredDocs = queryResult |> List.filter (fun doc -> doc.num < lessThenNum)
        Assert.Equal(queryResult.Length, filteredDocs.Length)
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))
 
        // Tear down.
        deleteDocuments QueryCollection docIds

    [<Theory>]
    [<InlineData(5, 3)>]
    [<InlineData(10, 5)>]
    [<InlineData(15, 10)>]
    let `` Query documents with 'where less then or equal to'`` (numOfDocs, lessThenOrEqualNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereLessThenOrEqualTo "num" lessThenOrEqualNum
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        let filteredDocs = queryResult |> List.filter (fun doc -> doc.num <= lessThenOrEqualNum)
        Assert.Equal(queryResult.Length, filteredDocs.Length)
        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))
 
        // Tear down.
        deleteDocuments QueryCollection docIds
     
    [<Theory>]
    [<InlineData(5, 2, 4)>]
    [<InlineData(10, 2, 7)>]
    [<InlineData(15, 4, 13)>]
    let `` Query documents with multiple conditions`` (numOfDocs, greaterThenNum, lessThenNum) =
        // Build up.
        connectToFirestore findGCPAuthentication |> ignore

        // Create test and add to DB.
        let dataList = createShuffledTestData numOfDocs
        let docIds = dataList |> List.map (fun data -> (addDocument QueryCollection data).Id)

        // Test.
        let queryResult =
            collection QueryCollection
            |> whereGreaterThen "num" greaterThenNum
            |> whereLessThen "num" lessThenNum
            |> orderBy "num"
            |> execQuery<Test>
            |> List.ofSeq

        Assert.NotEmpty(queryResult)
        
        let filteredDocs = queryResult |> List.filter (fun doc -> doc.num > greaterThenNum && doc.num < lessThenNum)
        Assert.Equal(queryResult.Length, filteredDocs.Length)
        
        let isAscOrdered = queryResult |> List.pairwise |> List.forall (fun (a, b) -> a.num <= b.num)
        Assert.Equal(isAscOrdered, true)

        queryResult |> List.iter (fun doc -> Assert.NotNull(doc))
 
        // Tear down.
        deleteDocuments QueryCollection docIds

