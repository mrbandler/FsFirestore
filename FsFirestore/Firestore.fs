namespace FsFirestore

module Firestore =    

    open Google.Cloud.Firestore
    open FsFirestore.Env
    open FsFirestore.Utils

    /// Firestore Database.
    let mutable private db: FirestoreDb = null

    /// Initializes the firestore connection.
    let initFirestore path =
        match setupGCPEnvironment path with
            | Some projectId ->
                db <- FirestoreDb.Create(projectId)
                true

            | None -> 
                false

    /// Deserializes a given document ('T).
    let deserializeDocument<'T when 'T : not struct> (doc: DocumentReference) =
        getDocSnapshot doc
        |> deserializeSnapshot<'T>

    /// Deserializes a given documents ('T).
    let deserializeDocuments<'T when 'T : not struct> docs =
        docs
        |> Seq.map (fun doc -> (deserializeDocument<'T> doc))

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

    /// Returns a document from a collection.
    let document<'T when 'T : not struct> col id =
        documentRef col id
        |> deserializeDocument<'T>
    
    /// Returns list of document references from a collection.
    let documentRefs col ids =
        collection col
        |> getDocs ids

    /// Returns a list of documents from a collection.
    let documents<'T when 'T : not struct> col ids =
        documentRefs col ids
        |> deserializeDocuments<'T>

    /// Returrns all documents from a collection.
    let allDocuments<'T when 'T : not struct> col =
        (collection col |> getCollectionSnapshot).Documents 
        |> deserializeSnapshots<'T>

    /// Adds a document to a collection; the ID of the document is generated automatically.
    let addDocument col doc =
        collection col
        |> addDoc doc

    /// Adds a document to a collection.
    let addDocumentWithId col id doc =
        collection col
        |> createDoc id doc

    /// Updates a document in a collection.
    let updateDocument col id doc =
        collection col 
        |> setDoc id doc

    /// Deletes a document in a collection.
    let deleteDocument col id =
        collection col 
        |> deleteDoc id