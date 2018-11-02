namespace FsFirestore

module internal Utils =
    
    open Google.Cloud.Firestore
    open FsFirestore.Types

    /// Deserializes a given snapshot ('T).
    let internal deserializeSnapshot<'T when 'T : not struct> (snapshot: DocumentSnapshot) = 
        let doc = snapshot.ConvertTo<'T>()

        match box doc with
        | :? FirestoreDocument as fireDoc ->
            fireDoc.Id           <- snapshot.Reference.Id
            fireDoc.CollectionId <- snapshot.Reference.Parent.Id
            doc

        | _ -> 
            doc

    /// Deserializes a given snapshots ('T).
    let internal deserializeSnapshots<'T when 'T : not struct> snapshots = 
        snapshots
        |> Seq.map (fun snap -> (deserializeSnapshot<'T> snap))
    
    /// Converts a given document change
    let internal convertDocumentChange<'T when 'T : not struct> (change: DocumentChange) =
        {
            document = change.Document |> deserializeSnapshot<'T>
            changeType = change.ChangeType
            newIndex = Option.ofNullable change.NewIndex
            oldIndex = Option.ofNullable change.OldIndex
        }

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
    let internal createDoc id data (collection: CollectionReference) =
        let docRef = collection.Document(id)

        docRef.CreateAsync(data)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore

        docRef
    
    /// Adds a document with a automatically generated ID to a given collection.
    let internal addDoc data (collection: CollectionReference) =
        collection.AddAsync(data)
        |> Async.AwaitTask
        |> Async.RunSynchronously

    /// Sets a document content with a given ID in a given collection.
    let internal setDoc id data (collection: CollectionReference) =
        let docRef = getDoc id collection

        docRef.SetAsync(data, SetOptions.MergeAll)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore

        docRef

    /// Deletes a document with a given ID in a given collection.
    let internal deleteDoc (precondition: Precondition option) id (collection: CollectionReference) =
        let docRef = getDoc id collection

        match precondition with
        | Some precond ->         
            docRef.DeleteAsync(precond)
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> ignore
        | None ->
            docRef.DeleteAsync()
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> ignore