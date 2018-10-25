namespace FsFirestore.Tests

module Data =

    open FsFirestore.Tests.Config
    open System

    /// Fills the array in the given test data set.
    let private fillArray (dataList: Test list) =
        let length = dataList.Length
        
        dataList
        |> List.iter (fun data -> data.arr <- [| for i in 1 .. data.num -> i |])
        
        dataList


    /// Shuffles an array in place.
    let private shuffleInPlace (array : 'a[]) =
        let swap i j =
            let temp = array.[i]
            array.[i] <- array.[j]
            array.[j] <- temp

        let random = new Random()
        let len = array.Length
        [0..len-2] |> Seq.iter(fun i -> swap i (random.Next(i, len)))
        array

    /// Creates a list of test data entries.
    let rec private createData max counter (list: Test list) =
        if counter = max then
            list
        else
            let internalCounter = counter + 1
            let data = new Test()
            data.str <- internalCounter.ToString()
            data.num <- internalCounter

            createData max internalCounter (List.append list [data])

    /// Creates test data for test cases.
    let createTestData numOfDocs =
        (createData numOfDocs 0 [])
        |> fillArray

    /// Creates shuffled test data for test cases.
    let createShuffledTestData numOfDocs =
        let dataList = createData numOfDocs 0 []

        dataList    
        |> Array.ofList
        |> shuffleInPlace
        |> List.ofArray
        |> fillArray