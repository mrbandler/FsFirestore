namespace FsFirestore

/// Contains all relevant functions for basic CRUD operations in Firestore.
module Firestore =    

    open Google.Cloud.Firestore
    open FsFirestore.Env
    open FsFirestore.Utils

    /// Firestore Database.
    let mutable internal db: FirestoreDb = null

    /// Initializes the firestore connection.
    let connectToFirestore path =
        match setupGCPEnvironment path with
            | Some projectId ->
                db <- FirestoreDb.Create projectId
                true

            | None -> 
                false
                
    /// Initializes the firestore connection using a project ID.
    let connectToFirestoreProject projectId =
         db <- FirestoreDb.Create projectId
         
    /// Initializes the firestore connection with a custom builder.    
    let connectToFirestoreWithBuilder path (builder: FirestoreDbBuilder) =        
        match setupGCPEnvironment path with
            | Some projectId ->
                builder.ProjectId <- projectId
                db <- builder.Build ()
                true

            | None -> 
                false
        
    /// Initializes the firestore connection with a custom builder.    
    let connectToFirestoreWithBuilderOnly (builder: FirestoreDbBuilder) =        
        db <- builder.Build ()

    /// Converts a given document snapshot to a given type.
    let convertSnapshotTo<'T when 'T : not struct> (snap: DocumentSnapshot) =
        snap
        |> deserializeSnapshot<'T>

    /// Converts given document snapshots to a given type.
    let convertSnapshotsTo<'T when 'T : not struct> (snaps: DocumentSnapshot seq) =
        snaps
        |> deserializeSnapshots<'T>

    /// Converts a given document to a type.
    let convertTo<'T when 'T : not struct> doc =
        getDocSnapshot doc
        |> convertSnapshotTo<'T>

    /// Converts given documents to a type.
    let convertToMulti<'T when 'T : not struct> docs =
        docs
        |> Seq.map (fun doc -> (convertTo<'T> doc))

    /// Converts query changes.
    let convertQueryChanges<'T when 'T : not struct> (changes: DocumentChange seq) =
        changes
        |> Seq.map convertDocumentChange<'T>

    /// Returns a collection from the DB with a given name.
    let collection name =
        getCollection db name

    /// Executes a given query.
    let execQuery<'T when 'T : not struct> query =
        (getQuerySnapshot query).Documents
        |> deserializeSnapshots<'T>

    /// Returns a document reference from a collection.
    let documentRef col id =
        collection col
        |> getDoc id
    
    /// Returns list of document references from a collection.
    let documentRefs col ids =
        collection col
        |> getDocs ids

    /// Returns a document snapshot from a collection.
    let documentSnapshot col id =
        collection col
        |> getDoc id
        |> getDocSnapshot

    /// Returns a list of document snapshots from a collection.
    let documentSnapshots col ids =
        collection col
        |> getDocs ids
        |> Seq.map (fun doc -> doc |> getDocSnapshot)

    /// Returns a document snapshot from a given documen reference.
    let documentSnapshotFromRef doc =
        getDocSnapshot doc

    /// Returns a document from a collection.
    let document<'T when 'T : not struct> col id =
        documentRef col id
        |> convertTo<'T>

    /// Returns a list of documents from a collection.
    let documents<'T when 'T : not struct> col ids =
        documentRefs col ids
        |> convertToMulti<'T>

    /// Returrns all documents from a collection.
    let allDocuments<'T when 'T : not struct> col =
        (collection col |> getCollectionSnapshot).Documents 
        |> deserializeSnapshots<'T>

    /// Adds a document to a collection.
    let addDocument col (id: string option) data =
        let colRef = collection col

        match id with
        | Some id -> colRef |> createDoc id data
        | None -> colRef |> addDoc data

    /// Updates a document in a collection.
    let updateDocument col id data =
        collection col 
        |> setDoc id data

    /// Deletes a document in a collection.
    let deleteDocument precondition col id =
        collection col 
        |> deleteDoc precondition id

    /// Deletes multiple documents in a collection.
    let deleteDocuments precondition col (ids: string seq) =
        ids
        |>Seq.iter (deleteDocument precondition col)

