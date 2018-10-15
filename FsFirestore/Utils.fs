namespace FsFirestore

module Utils =
    
    open Google.Cloud.Firestore
    open FsFirestore.Types

    /// Returns a collection from the DB with a given name.
    let internal getCollection (db: FirestoreDb) name =
        db.Collection(name)

    /// Returns a collection snapshot.
    let internal getCollectionSnapshot (collection: CollectionReference) =
        collection.GetSnapshotAsync()
        |> Async.AwaitTask
        |> Async.RunSynchronously

    /// Returns a document snapshot.
    let internal getDocSnapshot (doc: DocumentReference) =
        doc.GetSnapshotAsync()
        |> Async.AwaitTask 
        |> Async.RunSynchronously

    /// Returns a query snapshot.
    let internal getQuerySnapshot (query: Query) =
        query.GetSnapshotAsync()
        |> Async.AwaitTask
        |> Async.RunSynchronously

    /// Returns a document from a given collection with a given ID.
    let internal getDoc id (collection: CollectionReference) = 
        collection.Document(id)
    
    /// Returns a sequence of documents from a given collection with given IDs.
    let internal getDocs ids (collection: CollectionReference) = 
        ids |> Seq.map (fun id -> (getDoc id collection))
        
    /// Creates a document with a given ID in a given collection.
    let internal createDoc id doc (collection: CollectionReference) =
        let docRef = collection.Document(id)

        docRef.CreateAsync(doc)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore

        docRef
    
    /// Adds a document with a automatically generated ID to a given collection.
    let internal addDoc doc (collection: CollectionReference) =
        collection.AddAsync(doc)
        |> Async.AwaitTask
        |> Async.RunSynchronously

    /// Sets a document content with a given ID in a given collection.
    let internal setDoc id doc (collection: CollectionReference) =
        let docRef = getDoc id collection

        docRef.SetAsync(doc, SetOptions.MergeAll)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore

        docRef

    /// Deletes a document with a given ID in a given collection.
    let internal deleteDoc id (collection: CollectionReference) =
        let docRef = getDoc id collection

        docRef.DeleteAsync()
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore

    /// Deserializes a given snapshot ('T).
    let internal deserializeSnapshot<'T when 'T : not struct> (snapshot: DocumentSnapshot) = 
        let doc = snapshot.ConvertTo<'T>()

        match box doc with
        | :? FirestoreDocument as fireDoc ->
            fireDoc.id           <- snapshot.Reference.Id
            fireDoc.collectionId <- snapshot.Reference.Parent.Id

            doc

        | _ -> 
            doc

    /// Deserializes a given snapshots ('T).
    let internal deserializeSnapshots<'T when 'T : not struct> snapshots = 
        snapshots
        |> Seq.map (fun snap -> (deserializeSnapshot<'T> snap))

