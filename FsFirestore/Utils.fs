namespace FsFirestore

module internal Utils =
    
    open Google.Cloud.Firestore
    open FsFirestore.Types

    /// Sets Firestore properties on a given data object if it inherits from FirestoreDocument.
    let internal setFirestoreProperties id col data =
        match box data with
            | :? FirestoreDocument as fireDoc ->
                fireDoc.Id <- id
                fireDoc.CollectionId <- col
                data

            | _ -> 
                data

    /// Deserializes a given snapshot ('T).
    let internal deserializeSnapshot<'T when 'T : not struct> (snapshot: DocumentSnapshot) = 
        snapshot.ConvertTo<'T>()
        |> setFirestoreProperties snapshot.Reference.Id snapshot.Reference.Parent.Id

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

        data |> setFirestoreProperties docRef.Id docRef.Parent.Id |> ignore
        docRef
    
    /// Adds a document with a automatically generated ID to a given collection.
    let internal addDoc data (collection: CollectionReference) =
        let docRef = 
            collection.AddAsync(data)
            |> Async.AwaitTask
            |> Async.RunSynchronously

        data |> setFirestoreProperties docRef.Id docRef.Parent.Id |> ignore
        docRef

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

    /// Returns a query snapshot with respect to the given transaction
    let internal getQuerySnapshotInTrans (trans: Transaction) (query: Query) =
        trans.GetSnapshotAsync(query)
        |> Async.AwaitTask
        |> Async.RunSynchronously

    /// Returns a document snapshot with respect to the given transaction.
    let internal getDocSnapshotInTrans (trans: Transaction) (doc: DocumentReference) =
        trans.GetSnapshotAsync(doc)
        |> Async.AwaitTask
        |> Async.RunSynchronously

    /// Returns all document snapshot with respect to the given transaction.
    let internal getDocSnapshotsInTrans (trans: Transaction) (docs: DocumentReference seq) =
        trans.GetAllSnapshotsAsync(docs)
        |> Async.AwaitTask
        |> Async.RunSynchronously

    /// Creates a document with a given ID in a given collection with respect to the given transaction.
    let internal createDocInTrans (trans: Transaction) id data (collection: CollectionReference) =
        let docRef = collection.Document(id)
        
        trans.Create(docRef, data)

        docRef

    /// Sets a document content with a given ID in a given collection.
    let internal setDocInTrans (trans: Transaction) id data (collection: CollectionReference) =
        let docRef = getDoc id collection

        trans.Set(docRef, data, SetOptions.MergeAll)

        docRef

    /// Deletes a document with a given ID in a given collection with respect to the given transaction.
    let internal deleteDocTrans (trans: Transaction) (precondition: Precondition option) id (collection: CollectionReference) =
        match precondition with
        | Some precon -> trans.Delete((getDoc id collection), precon)
        | None -> trans.Delete(getDoc id collection)