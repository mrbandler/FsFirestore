namespace FsFirestore

module internal TransUtils =
    
    open Google.Cloud.Firestore
    open FsFirestore.Utils
        
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