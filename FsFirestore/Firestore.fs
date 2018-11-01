namespace FsFirestore

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
                db <- FirestoreDb.Create(projectId)
                true

            | None -> 
                false
            
    /// Deserializes a given document snapshot ('T).
    let convertSnapshotTo<'T when 'T : not struct> (snap: DocumentSnapshot) =
        snap
        |> deserializeSnapshot<'T>

    /// Deserializes a given document ('T).
    let convertTo<'T when 'T : not struct> (doc: DocumentReference) =
        getDocSnapshot doc
        |> convertSnapshotTo<'T>

    /// Deserializes a given documents ('T).
    let convertToMulti<'T when 'T : not struct> docs =
        docs
        |> Seq.map (fun doc -> (convertTo<'T> doc))

    /// Returns a collection from the DB with a given name.
    let collection name =
        getCollection db name
    
    /// Executes a given query.
    let execQuery<'T when 'T : not struct> (query: Query) =
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
    let deleteDocument col id =
        collection col 
        |> deleteDoc id

    /// Deletes multiple documents in a collection.
    let deleteDocuments col (ids: string seq) =
        ids
        |>Seq.iter (deleteDocument col)

