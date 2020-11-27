namespace FsFirestore.Tests

module ConverterTests =
       
    open Xunit
    open Google.Cloud.Firestore
    open FsFirestore.Firestore
    open FsFirestore.Types
    open Config
    
    type PlayerId (id: string) =
            let id = id
            member this.Id with get () = id

    type PlayerIdConverter () =
           interface IFirestoreConverter<PlayerId> with
           
           member this.ToFirestore value = value.Id :> obj
           
           member this.FromFirestore value =
                   match value with
                   | :? string as id -> PlayerId id
                   | _ -> PlayerId ""
                   
    [<FirestoreData>]
    type Game () =
        inherit FirestoreDocument()

        [<FirestoreProperty>]
        member val PlayerA = PlayerId "0"

        [<FirestoreProperty>]
        member val PlayerB = PlayerId "1"
        
    /// Add document with generated ID to the Firestore DB test.
    [<Fact>]
    let ``Add document with generated ID`` () =
        // Build up.
        let converter = PlayerIdConverter ()
        let registry = ConverterRegistry ()
        registry.Add converter
        
        let builder = FirestoreDbBuilder ()
        builder.ConverterRegistry <- registry
        
        connectToFirestoreWithBuilder findGCPAuthentication builder |> ignore
        let game = Game ()

        // Test.
        let doc = addDocument ConverterCollection None game
        let docData = convertTo<Game> doc

        Assert.NotNull(doc.Id)
        Assert.Equal(ConverterCollection, doc.Parent.Id)
        Assert.Equal(doc.Id, docData.Id)
        Assert.Equal(ConverterCollection, docData.CollectionId)
        Assert.Equal<string>(game.PlayerA.Id, docData.PlayerA.Id)
        Assert.Equal<string>(game.PlayerB.Id, docData.PlayerB.Id)

        // Tear down.
        deleteDocument None ConverterCollection doc.Id