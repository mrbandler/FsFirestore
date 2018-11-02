namespace FsFirestore

/// Contains all relevant functions for Firestore change listener.
module Listening =
    
    open Google.Cloud.Firestore
    open FsFirestore.Firestore
    open FsFirestore.Utils

    /// Creates a listener for a document in a collection.
    let listenOnDocument col id (callback: DocumentSnapshot -> unit) =
        let doc = documentRef col id
        doc.Listen(callback)

    /// Creates a listener for a given document reference.
    let listenOnDocumentRef (callback: DocumentSnapshot -> unit) (doc: DocumentReference) =
        doc.Listen(callback)

    /// Creates a listener for a given query.
    let listenOnQuery (callback: QuerySnapshot -> unit) (query: Query) =
        query.Listen(callback)

    /// Stops a given listener.
    let stopListening (listener: FirestoreChangeListener) =
        listener.StopAsync()
        |> Async.AwaitTask
        |> Async.RunSynchronously