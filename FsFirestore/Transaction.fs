namespace FsFirestore

module Transaction =
    
    open Google.Cloud.Firestore
    open FsFirestore.Firestore
    open FsFirestore.Utils
    open FsFirestore.TransUtils

    /// Runs a given transaction function in a Firestore transaction.
    let runTransaction<'T when 'T : not struct> (transactionFunc: Transaction -> 'T) =
        db.RunTransactionAsync<'T>((fun trans -> async { return (transactionFunc trans) } |> Async.StartAsTask))
        |> Async.AwaitTask
        |> Async.RunSynchronously

    /// Executes a given query with respect to the given transaction.
    let execQueryInTrans<'T when 'T : not struct> transaction query =
        (getQuerySnapshotInTrans transaction query).Documents
        |> deserializeSnapshots<'T>

    /// Returns a document from a collection with respect to the given transaction.
    let documentInTrans<'T when 'T : not struct> transaction col id =
        documentRef col id
        |> getDocSnapshotInTrans transaction
        |> convertSnapshotTo<'T>

    /// Returns a list of documents from a collection with respect to a given transaction
    let documentsInTrans<'T when 'T : not struct> transaction col ids =
       documentRefs col ids
       |> getDocSnapshotsInTrans transaction
       |> deserializeSnapshots<'T>

    /// Adds a document to a collection with respect to the given transaction.
    let addDocumentInTrans transaction col id data =
        collection col
        |> createDocInTrans transaction id data

    /// Updates a document in a collection with respect to the given transaction.
    let updateDocumentInTrans transaction col id data =
        collection col 
        |> setDocInTrans transaction id data

    /// Deletes a document in a collection with respect to the given transaction.
    let deleteDocumentInTrans transaction precondition col id =
        collection col
        |> deleteDocTrans transaction precondition id
        
    /// Deletes multiple documents in a collection with respect to the given transaction.
    let deleteDocumentsInTrans transaction precondition col (ids: string seq)  =
        ids
        |>Seq.iter (deleteDocumentInTrans transaction precondition col)
